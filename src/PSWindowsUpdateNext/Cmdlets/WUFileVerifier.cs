using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PSWindowsUpdateNext;

/// <summary>
/// Integrity checks for files downloaded from the Microsoft Update Catalog.
/// </summary>
public static class WUFileVerifier
{
	/// <summary>
	/// Computes the SHA-1 digest of a file and returns it Base64-encoded, matching the
	/// "digest" value the Catalog download dialog publishes for each file.
	/// </summary>
	public static string Sha1Base64(string path)
	{
		using FileStream stream = File.OpenRead(path);
		using SHA1 sha1 = SHA1.Create();
		return Convert.ToBase64String(sha1.ComputeHash(stream));
	}

	/// <summary>
	/// Computes the SHA-256 digest of a file as a lowercase hex string.
	/// </summary>
	public static string Sha256Hex(string path)
	{
		using FileStream stream = File.OpenRead(path);
		using SHA256 sha256 = SHA256.Create();
		byte[] hash = sha256.ComputeHash(stream);
		char[] hex = new char[hash.Length * 2];
		for (int i = 0; i < hash.Length; i++)
		{
			hash[i].ToString("x2").CopyTo(0, hex, i * 2, 2);
		}
		return new string(hex);
	}

	/// <summary>
	/// Microsoft root CAs that Catalog content may legitimately chain to, keyed by SHA-1 thumbprint.
	/// Each thumbprint was computed from the certificate file downloaded from Microsoft's PKI repository
	/// (https://www.microsoft.com/pkiops/docs/repository.htm and the www.microsoft.com/pki/certs/ files
	/// noted below); the 2010, 2011 and ECC 2018 values also match the thumbprints printed on that page.
	/// "Microsoft Identity Verification Root Certificate Authority 2020" is deliberately absent: it is
	/// operated under Microsoft's Third Party CPS and issues code-signing certificates to non-Microsoft
	/// publishers (Trusted Signing), so it does not prove that Microsoft signed a file.
	/// </summary>
	private static readonly Dictionary<string, string> MicrosoftRoots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		// Microsoft Root Certificate Authority 2010 (RSA 4096, valid 2010-06-23 to 2035-06-23).
		// Windows and Windows Server updates: Microsoft Windows Production PCA 2011 -> this root.
		// Source: https://www.microsoft.com/pki/certs/MicRooCerAut_2010-06-23.crt
		{ "3B1EFD3A66EA28B16697394703A72CA340A05BD5", "Microsoft Root Certificate Authority 2010" },
		// Microsoft Root Certificate Authority 2011 (RSA 4096, valid 2011-03-22 to 2036-03-22).
		// Office, .NET and other product updates: Microsoft Code Signing PCA 2011 -> this root.
		// Source: https://www.microsoft.com/pki/certs/MicRooCerAut2011_2011_03_22.crt
		{ "8F43288AD272F3103B6FB1428485EA3014C0BCFE", "Microsoft Root Certificate Authority 2011" },
		// Microsoft ECC Product Root Certificate Authority 2018 (ECC P-384, valid 2018-02-27 to 2043-02-27).
		// Corporate CPS product root; listed so ECC-signed packages are not rejected if Microsoft moves to them.
		// Source: https://www.microsoft.com/pkiops/certs/Microsoft%20ECC%20Product%20Root%20Certificate%20Authority%202018.crt
		{ "06F1AA330B927B753A40E68CDF22E34BCBEF3352", "Microsoft ECC Product Root Certificate Authority 2018" },
		// Microsoft Root Certificate Authority (DC=com, DC=microsoft; RSA 4096, valid 2001-05-09 to 2021-05-09, expired).
		// Older SHA-1 era Catalog packages chain here; WinVerifyTrust still accepts them when the signature
		// was timestamped while the chain was valid.
		// Source: https://www.microsoft.com/pki/certs/MicrosoftRootCert.crt
		{ "CDD4EEAE6000AC7F40C3802C171E30148030C072", "Microsoft Root Certificate Authority" },
		// Microsoft Root Authority (OU=Copyright (c) 1997 Microsoft Corp.; RSA 2048, valid 1997-01-10 to 2020-12-31, expired).
		// The oldest Catalog packages chain here.
		// Source: https://www.microsoft.com/pki/certs/MicrosoftRootAuthority.crt
		{ "A43489159A520F0D93D032CCAF37E7FE20A8B419", "Microsoft Root Authority" }
	};

	private static readonly Guid WintrustActionGenericVerifyV2 = new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");

	private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

	private const uint WTD_UI_NONE = 2;

	private const uint WTD_REVOKE_NONE = 0;

	private const uint WTD_REVOKE_WHOLECHAIN = 1;

	private const uint WTD_CHOICE_FILE = 1;

	private const uint WTD_STATEACTION_VERIFY = 1;

	private const uint WTD_STATEACTION_CLOSE = 2;

	private const uint WTD_REVOCATION_CHECK_NONE = 0x00000010;

	private const uint WTD_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT = 0x00000080;

	private const uint WTD_CACHE_ONLY_URL_RETRIEVAL = 0x00001000;

	private const uint WTD_UICONTEXT_EXECUTE = 0;

	private const int TRUST_E_SUBJECT_FORM_UNKNOWN = unchecked((int)0x800B0003);

	private const int TRUST_E_NOSIGNATURE = unchecked((int)0x800B0100);

	private const int CERT_E_EXPIRED = unchecked((int)0x800B0101);

	private const int CERT_E_UNTRUSTEDROOT = unchecked((int)0x800B0109);

	private const int CERT_E_CHAINING = unchecked((int)0x800B010A);

	private const int CERT_E_REVOKED = unchecked((int)0x800B010C);

	private const int CERT_E_REVOCATION_FAILURE = unchecked((int)0x800B010E);

	private const int TRUST_E_EXPLICIT_DISTRUST = unchecked((int)0x800B0111);

	private const int TRUST_E_CERT_SIGNATURE = unchecked((int)0x80096004);

	private const int TRUST_E_BAD_DIGEST = unchecked((int)0x80096010);

	private const int CRYPT_E_REVOKED = unchecked((int)0x80092010);

	private const int CRYPT_E_NO_REVOCATION_CHECK = unchecked((int)0x80092012);

	private const int CRYPT_E_REVOCATION_OFFLINE = unchecked((int)0x80092013);

	private const uint CERT_TRUST_REVOCATION_STATUS_UNKNOWN = 0x00000040;

	private const uint CERT_TRUST_IS_OFFLINE_REVOCATION = 0x01000000;

	/// <summary>
	/// Verifies that a file carries a valid Authenticode signature from Microsoft.
	/// </summary>
	/// <remarks>
	/// WinVerifyTrust (WINTRUST_ACTION_GENERIC_VERIFY_V2) runs with revocation checking on every
	/// certificate in the chain except the self-signed root, whose trust is governed by the Trusted Root
	/// and Disallowed stores rather than by a CRL. The chain WinVerifyTrust built for the primary signer
	/// is read from its state data, and the root's SHA-1 thumbprint must be one of the Microsoft roots
	/// listed in this class. When the only problem is that revocation could not be checked, the file is
	/// verified a second time with revocation off, so a revocation-offline result can never hide another
	/// chain error; if that pass succeeds and the root is Microsoft's, the result is
	/// <see cref="WUSignatureStatus.RevocationUnknown"/>.
	/// </remarks>
	public static WUSignatureResult VerifyAuthenticode(string path)
	{
		TrustCheck online = RunWinVerifyTrust(path, checkRevocation: true);
		int hr = online.HResult;
		if (hr == 0)
		{
			// The default Software Publishing policy may accept an offline revocation check and still
			// return S_OK. The chain's own trust status says whether revocation was really confirmed.
			bool revocationConfirmed = (online.ChainErrorStatus & (CERT_TRUST_REVOCATION_STATUS_UNKNOWN | CERT_TRUST_IS_OFFLINE_REVOCATION)) == 0;
			return FromTrustedChain(online, revocationConfirmed ? WUSignatureStatus.Valid : WUSignatureStatus.RevocationUnknown, revocationConfirmed ? hr : CRYPT_E_REVOCATION_OFFLINE);
		}
		if (IsRevocationUnknown(hr))
		{
			TrustCheck offline = RunWinVerifyTrust(path, checkRevocation: false);
			if (offline.HResult != 0)
			{
				return Classify(offline.HResult, offline);
			}
			return FromTrustedChain(offline, WUSignatureStatus.RevocationUnknown, hr);
		}
		return Classify(hr, online);
	}

	private static bool IsRevocationUnknown(int hr)
	{
		return hr == CERT_E_REVOCATION_FAILURE || hr == CRYPT_E_REVOCATION_OFFLINE || hr == CRYPT_E_NO_REVOCATION_CHECK;
	}

	/// <summary>
	/// Applies the Microsoft-root rule to a chain that WinVerifyTrust accepted.
	/// </summary>
	private static WUSignatureResult FromTrustedChain(TrustCheck check, WUSignatureStatus status, int hr)
	{
		if (string.IsNullOrEmpty(check.RootThumbprint))
		{
			return Build(WUSignatureStatus.Untrusted, hr, "the signature verified, but the signer's certificate chain could not be read, so its root CA cannot be confirmed as Microsoft's.", check);
		}
		if (!MicrosoftRoots.TryGetValue(check.RootThumbprint, out string rootName))
		{
			return Build(WUSignatureStatus.NotMicrosoftRoot, hr, "the signature is valid but chains to root CA '" + check.RootSubject + "' (thumbprint " + check.RootThumbprint + "), which is not a Microsoft root CA." + SignerSuffix(check, includeRoot: false), check);
		}
		if (status == WUSignatureStatus.RevocationUnknown)
		{
			return Build(status, hr, "signed by '" + check.SignerSubject + "' under " + rootName + ", but certificate revocation could not be checked (" + DescribeHResult(hr) + "). The computer may be offline or unable to reach the CRL/OCSP servers through its proxy.", check);
		}
		return Build(status, hr, "signed by '" + check.SignerSubject + "' under " + rootName + " (" + check.RootThumbprint + ").", check);
	}

	/// <summary>
	/// Maps a failing WinVerifyTrust result (other than revocation-unknown) to a status.
	/// </summary>
	private static WUSignatureResult Classify(int hr, TrustCheck check)
	{
		if (hr == TRUST_E_NOSIGNATURE || hr == TRUST_E_SUBJECT_FORM_UNKNOWN)
		{
			return Build(WUSignatureStatus.NotSigned, hr, "the file has no Authenticode signature (" + DescribeHResult(hr) + ").", check);
		}
		if (hr == CERT_E_REVOKED || hr == CRYPT_E_REVOKED)
		{
			return Build(WUSignatureStatus.Revoked, hr, "a certificate in the signing chain has been revoked (" + DescribeHResult(hr) + ")." + SignerSuffix(check, includeRoot: true), check);
		}
		return Build(WUSignatureStatus.Untrusted, hr, "the signature or its certificate chain is not trusted (" + DescribeHResult(hr) + ")." + SignerSuffix(check, includeRoot: true), check);
	}

	private static WUSignatureResult Build(WUSignatureStatus status, int hr, string message, TrustCheck check)
	{
		return new WUSignatureResult(status, hr, message, check.SignerSubject, check.SignerThumbprint, check.RootSubject, check.RootThumbprint);
	}

	private static string SignerSuffix(TrustCheck check, bool includeRoot)
	{
		if (string.IsNullOrEmpty(check.SignerSubject))
		{
			return string.Empty;
		}
		string text = " Signer: '" + check.SignerSubject + "'";
		if (includeRoot && !string.IsNullOrEmpty(check.RootSubject))
		{
			text += ", root: '" + check.RootSubject + "' (" + check.RootThumbprint + ")";
		}
		return text + ".";
	}

	private static string DescribeHResult(int hr)
	{
		string name;
		switch (hr)
		{
			case TRUST_E_SUBJECT_FORM_UNKNOWN: name = "TRUST_E_SUBJECT_FORM_UNKNOWN"; break;
			case TRUST_E_NOSIGNATURE: name = "TRUST_E_NOSIGNATURE"; break;
			case CERT_E_EXPIRED: name = "CERT_E_EXPIRED"; break;
			case CERT_E_UNTRUSTEDROOT: name = "CERT_E_UNTRUSTEDROOT"; break;
			case CERT_E_CHAINING: name = "CERT_E_CHAINING"; break;
			case CERT_E_REVOKED: name = "CERT_E_REVOKED"; break;
			case CERT_E_REVOCATION_FAILURE: name = "CERT_E_REVOCATION_FAILURE"; break;
			case TRUST_E_EXPLICIT_DISTRUST: name = "TRUST_E_EXPLICIT_DISTRUST"; break;
			case TRUST_E_CERT_SIGNATURE: name = "TRUST_E_CERT_SIGNATURE"; break;
			case TRUST_E_BAD_DIGEST: name = "TRUST_E_BAD_DIGEST"; break;
			case CRYPT_E_REVOKED: name = "CRYPT_E_REVOKED"; break;
			case CRYPT_E_NO_REVOCATION_CHECK: name = "CRYPT_E_NO_REVOCATION_CHECK"; break;
			case CRYPT_E_REVOCATION_OFFLINE: name = "CRYPT_E_REVOCATION_OFFLINE"; break;
			default: name = null; break;
		}
		string text = "0x" + hr.ToString("X8");
		if (name != null)
		{
			text += " " + name;
		}
		string systemMessage = null;
		try
		{
			// FormatMessage text for the HRESULT, e.g. "No signature was present in the subject."
			systemMessage = new Win32Exception(hr).Message;
		}
		catch (Exception)
		{
		}
		if (!string.IsNullOrWhiteSpace(systemMessage) && !systemMessage.StartsWith("Unknown error", StringComparison.OrdinalIgnoreCase))
		{
			text += ": " + systemMessage.Trim().TrimEnd('.');
		}
		return text;
	}

	/// <summary>
	/// Runs one WinVerifyTrust verification with its state kept open, reads the primary signer's chain,
	/// then always closes the state and frees every unmanaged buffer.
	/// </summary>
	private static TrustCheck RunWinVerifyTrust(string path, bool checkRevocation)
	{
		TrustCheck check = new TrustCheck();
		Guid action = WintrustActionGenericVerifyV2;
		int stateActionOffset = Marshal.OffsetOf(typeof(WINTRUST_DATA), nameof(WINTRUST_DATA.dwStateAction)).ToInt32();
		int stateDataOffset = Marshal.OffsetOf(typeof(WINTRUST_DATA), nameof(WINTRUST_DATA.hWVTStateData)).ToInt32();
		IntPtr pathPtr = IntPtr.Zero;
		IntPtr fileInfoPtr = IntPtr.Zero;
		IntPtr dataPtr = IntPtr.Zero;
		bool verifyCalled = false;
		try
		{
			pathPtr = Marshal.StringToHGlobalUni(path);
			WINTRUST_FILE_INFO fileInfo = new WINTRUST_FILE_INFO
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)),
				pcwszFilePath = pathPtr,
				hFile = IntPtr.Zero,
				pgKnownSubject = IntPtr.Zero
			};
			fileInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)));
			Marshal.StructureToPtr(fileInfo, fileInfoPtr, fDeleteOld: false);
			WINTRUST_DATA data = new WINTRUST_DATA
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_DATA)),
				pPolicyCallbackData = IntPtr.Zero,
				pSIPClientData = IntPtr.Zero,
				dwUIChoice = WTD_UI_NONE,
				// Online pass: revocation on the signer, intermediate and timestamp chains (root excluded, see
				// VerifyAuthenticode), with network retrieval allowed so CRLs and OCSP responses can be fetched.
				// Fallback pass: the original settings, no revocation and cached URLs only.
				fdwRevocationChecks = checkRevocation ? WTD_REVOKE_WHOLECHAIN : WTD_REVOKE_NONE,
				dwUnionChoice = WTD_CHOICE_FILE,
				pFile = fileInfoPtr,
				dwStateAction = WTD_STATEACTION_VERIFY,
				hWVTStateData = IntPtr.Zero,
				pwszURLReference = IntPtr.Zero,
				dwProvFlags = checkRevocation ? WTD_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT : (WTD_REVOCATION_CHECK_NONE | WTD_CACHE_ONLY_URL_RETRIEVAL),
				dwUIContext = WTD_UICONTEXT_EXECUTE,
				pSignatureSettings = IntPtr.Zero
			};
			dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_DATA)));
			Marshal.StructureToPtr(data, dataPtr, fDeleteOld: false);
			check.HResult = WinVerifyTrust(InvalidHandleValue, ref action, dataPtr);
			verifyCalled = true;
			ReadSignerChain(Marshal.ReadIntPtr(dataPtr, stateDataOffset), check);
		}
		finally
		{
			if (verifyCalled)
			{
				// WinVerifyTrust stored hWVTStateData in this block; the same block releases it.
				Marshal.WriteInt32(dataPtr, stateActionOffset, unchecked((int)WTD_STATEACTION_CLOSE));
				WinVerifyTrust(InvalidHandleValue, ref action, dataPtr);
			}
			if (dataPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(dataPtr);
			}
			if (fileInfoPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(fileInfoPtr);
			}
			if (pathPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pathPtr);
			}
		}
		return check;
	}

	/// <summary>
	/// Reads the signing certificate and the root of the primary signer's chain from WinVerifyTrust
	/// state data. Leaves the fields empty when any link is missing. Must run before the state is closed.
	/// </summary>
	private static void ReadSignerChain(IntPtr stateData, TrustCheck check)
	{
		if (stateData == IntPtr.Zero)
		{
			return;
		}
		IntPtr provData = WTHelperProvDataFromStateData(stateData);
		if (provData == IntPtr.Zero)
		{
			return;
		}
		IntPtr signer = WTHelperGetProvSignerFromChain(provData, 0, fCounterSigner: false, 0);
		if (signer == IntPtr.Zero)
		{
			return;
		}
		int chainContextOffset = Marshal.OffsetOf(typeof(CRYPT_PROVIDER_SGNR), nameof(CRYPT_PROVIDER_SGNR.pChainContext)).ToInt32();
		if (Marshal.ReadInt32(signer) < chainContextOffset + IntPtr.Size)
		{
			return;
		}
		IntPtr chainContextPtr = Marshal.ReadIntPtr(signer, chainContextOffset);
		if (chainContextPtr == IntPtr.Zero)
		{
			return;
		}
		CERT_CHAIN_CONTEXT chainContext = (CERT_CHAIN_CONTEXT)Marshal.PtrToStructure(chainContextPtr, typeof(CERT_CHAIN_CONTEXT));
		check.ChainErrorStatus = chainContext.TrustStatus.dwErrorStatus;
		if (chainContext.cChain == 0 || chainContext.rgpChain == IntPtr.Zero)
		{
			return;
		}
		// rgpChain[0] is the simple chain from the signing certificate up to its trust anchor.
		IntPtr simpleChainPtr = Marshal.ReadIntPtr(chainContext.rgpChain);
		if (simpleChainPtr == IntPtr.Zero)
		{
			return;
		}
		CERT_SIMPLE_CHAIN simpleChain = (CERT_SIMPLE_CHAIN)Marshal.PtrToStructure(simpleChainPtr, typeof(CERT_SIMPLE_CHAIN));
		if (simpleChain.cElement == 0 || simpleChain.rgpElement == IntPtr.Zero)
		{
			return;
		}
		IntPtr leafCert = CertContextOfElement(simpleChain.rgpElement, 0);
		IntPtr rootCert = CertContextOfElement(simpleChain.rgpElement, (int)simpleChain.cElement - 1);
		if (leafCert != IntPtr.Zero)
		{
			// X509Certificate2(IntPtr) duplicates the CERT_CONTEXT, so the copy is independent of the state data.
			using X509Certificate2 leaf = new X509Certificate2(leafCert);
			check.SignerSubject = leaf.Subject;
			check.SignerThumbprint = leaf.Thumbprint;
		}
		if (rootCert != IntPtr.Zero)
		{
			using X509Certificate2 root = new X509Certificate2(rootCert);
			check.RootSubject = root.Subject;
			check.RootThumbprint = root.Thumbprint;
		}
	}

	private static IntPtr CertContextOfElement(IntPtr rgpElement, int index)
	{
		IntPtr element = Marshal.ReadIntPtr(rgpElement, index * IntPtr.Size);
		if (element == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		return Marshal.ReadIntPtr(element, Marshal.OffsetOf(typeof(CERT_CHAIN_ELEMENT), nameof(CERT_CHAIN_ELEMENT.pCertContext)).ToInt32());
	}

	private sealed class TrustCheck
	{
		public int HResult;

		public string SignerSubject;

		public string SignerThumbprint;

		public string RootSubject;

		public string RootThumbprint;

		public uint ChainErrorStatus;
	}

	// The interop structs below are blittable (no managed strings), so StructureToPtr allocates nothing
	// that FreeHGlobal would miss. LayoutKind.Sequential with default packing reproduces the Windows SDK
	// layout on x86 (4-byte pointers) and x64 (8-byte pointers, with padding after leading DWORDs).
	// Structs that are only read are declared up to the last member used; FILETIME is two DWORDs.

	[StructLayout(LayoutKind.Sequential)]
	private struct WINTRUST_FILE_INFO
	{
		public uint cbStruct;
		public IntPtr pcwszFilePath;
		public IntPtr hFile;
		public IntPtr pgKnownSubject;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct WINTRUST_DATA
	{
		public uint cbStruct;
		public IntPtr pPolicyCallbackData;
		public IntPtr pSIPClientData;
		public uint dwUIChoice;
		public uint fdwRevocationChecks;
		public uint dwUnionChoice;
		public IntPtr pFile;
		public uint dwStateAction;
		public IntPtr hWVTStateData;
		public IntPtr pwszURLReference;
		public uint dwProvFlags;
		public uint dwUIContext;
		public IntPtr pSignatureSettings;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct CRYPT_PROVIDER_SGNR
	{
		public uint cbStruct;
		public uint sftVerifyAsOfLow;
		public uint sftVerifyAsOfHigh;
		public uint csCertChain;
		public IntPtr pasCertChain;
		public uint dwSignerType;
		public IntPtr psSigner;
		public uint dwError;
		public uint csCounterSigners;
		public IntPtr pasCounterSigners;
		public IntPtr pChainContext;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct CERT_TRUST_STATUS
	{
		public uint dwErrorStatus;
		public uint dwInfoStatus;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct CERT_CHAIN_CONTEXT
	{
		public uint cbSize;
		public CERT_TRUST_STATUS TrustStatus;
		public uint cChain;
		public IntPtr rgpChain;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct CERT_SIMPLE_CHAIN
	{
		public uint cbSize;
		public CERT_TRUST_STATUS TrustStatus;
		public uint cElement;
		public IntPtr rgpElement;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct CERT_CHAIN_ELEMENT
	{
		public uint cbSize;
		public IntPtr pCertContext;
	}

	[DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false)]
	private static extern int WinVerifyTrust(IntPtr hwnd, ref Guid pgActionID, IntPtr pWVTData);

	[DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false)]
	private static extern IntPtr WTHelperProvDataFromStateData(IntPtr hStateData);

	[DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false)]
	private static extern IntPtr WTHelperGetProvSignerFromChain(IntPtr pProvData, uint idxSigner, [MarshalAs(UnmanagedType.Bool)] bool fCounterSigner, uint idxCounterSigner);
}
