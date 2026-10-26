namespace PSWindowsUpdateNext;

/// <summary>
/// Outcome of the Authenticode check applied to a file downloaded from the Microsoft Update Catalog.
/// </summary>
public enum WUSignatureStatus
{
	/// <summary>Signed, the chain is trusted, ends at a Microsoft root CA, and no certificate is revoked.</summary>
	Valid,

	/// <summary>Signed and chained to a Microsoft root CA, but revocation status could not be determined (offline, proxy, unreachable CRL/OCSP).</summary>
	RevocationUnknown,

	/// <summary>The file carries no Authenticode signature, or its format cannot hold one.</summary>
	NotSigned,

	/// <summary>The signature or certificate chain is not trusted (bad digest, untrusted root, broken chain, expired, distrusted, and so on).</summary>
	Untrusted,

	/// <summary>The signature is valid but the chain ends at a root CA that is not one of Microsoft's code-signing roots.</summary>
	NotMicrosoftRoot,

	/// <summary>A certificate in the signing chain has been revoked.</summary>
	Revoked
}

/// <summary>
/// Result of <see cref="WUFileVerifier.VerifyAuthenticode(string)"/>.
/// </summary>
public sealed class WUSignatureResult
{
	internal WUSignatureResult(WUSignatureStatus status, int hResult, string message, string signerSubject, string signerThumbprint, string rootSubject, string rootThumbprint)
	{
		Status = status;
		HResult = hResult;
		Message = message;
		SignerSubject = signerSubject;
		SignerThumbprint = signerThumbprint;
		RootSubject = rootSubject;
		RootThumbprint = rootThumbprint;
	}

	/// <summary>Classification of the result.</summary>
	public WUSignatureStatus Status { get; }

	/// <summary>The HRESULT that WinVerifyTrust returned with revocation checking on (0 when trusted).</summary>
	public int HResult { get; }

	/// <summary>Administrator-facing explanation of the result.</summary>
	public string Message { get; }

	/// <summary>Subject of the signing certificate, when the chain could be read.</summary>
	public string SignerSubject { get; }

	/// <summary>SHA-1 thumbprint of the signing certificate, when the chain could be read.</summary>
	public string SignerThumbprint { get; }

	/// <summary>Subject of the root certificate the signer chain ends at, when the chain could be read.</summary>
	public string RootSubject { get; }

	/// <summary>SHA-1 thumbprint of the root certificate the signer chain ends at, when the chain could be read.</summary>
	public string RootThumbprint { get; }

	/// <summary>True for <see cref="WUSignatureStatus.Valid"/> only.</summary>
	public bool IsValid => Status == WUSignatureStatus.Valid;

	internal static WUSignatureResult Failed(string reason)
	{
		return new WUSignatureResult(WUSignatureStatus.Untrusted, 0, "signature verification could not be completed: " + reason, null, null, null, null);
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		return Status + ": " + Message;
	}
}
