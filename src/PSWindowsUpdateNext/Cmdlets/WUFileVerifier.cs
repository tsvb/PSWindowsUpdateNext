using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
	/// Verifies the Authenticode signature of a file with WinVerifyTrust.
	/// Returns 0 when the signature chains to a trusted root, otherwise the HRESULT.
	/// </summary>
	public static int VerifyAuthenticode(string path)
	{
		Guid action = new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE"); // WINTRUST_ACTION_GENERIC_VERIFY_V2
		WINTRUST_FILE_INFO fileInfo = new WINTRUST_FILE_INFO
		{
			cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)),
			pcwszFilePath = path,
			hFile = IntPtr.Zero,
			pgKnownSubject = IntPtr.Zero
		};
		IntPtr fileInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)));
		IntPtr dataPtr = IntPtr.Zero;
		try
		{
			Marshal.StructureToPtr(fileInfo, fileInfoPtr, fDeleteOld: false);
			WINTRUST_DATA data = new WINTRUST_DATA
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_DATA)),
				pPolicyCallbackData = IntPtr.Zero,
				pSIPClientData = IntPtr.Zero,
				dwUIChoice = 2, // WTD_UI_NONE
				fdwRevocationChecks = 0, // WTD_REVOKE_NONE
				dwUnionChoice = 1, // WTD_CHOICE_FILE
				pFile = fileInfoPtr,
				dwStateAction = 0, // WTD_STATEACTION_IGNORE
				hWVTStateData = IntPtr.Zero,
				pwszURLReference = IntPtr.Zero,
				dwProvFlags = 0x00000010 | 0x00000100, // WTD_REVOCATION_CHECK_NONE | WTD_CACHE_ONLY_URL_RETRIEVAL
				dwUIContext = 0
			};
			dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_DATA)));
			Marshal.StructureToPtr(data, dataPtr, fDeleteOld: false);
			return WinVerifyTrust(IntPtr.Zero, ref action, dataPtr);
		}
		finally
		{
			if (dataPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(dataPtr);
			}
			Marshal.FreeHGlobal(fileInfoPtr);
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private struct WINTRUST_FILE_INFO
	{
		public uint cbStruct;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pcwszFilePath;
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
	}

	[DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false)]
	private static extern int WinVerifyTrust(IntPtr hwnd, ref Guid pgActionID, IntPtr pWVTData);
}
