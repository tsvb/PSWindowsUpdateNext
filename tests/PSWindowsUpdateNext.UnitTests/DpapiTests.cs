using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

/// <summary>
/// WUTools.EncryptString / DecryptString wrap machine-scope DPAPI, so every test here needs Windows.
/// On other platforms they are reported as skipped, with the reason, instead of failing.
/// </summary>
public class DpapiTests
{
	[WindowsFact]
	public void Round_trips_ascii_text()
	{
		string protectedText = WUTools.EncryptString("P@ssw0rd!", "ignored");

		Assert.Equal("P@ssw0rd!", WUTools.DecryptString(protectedText, "ignored"));
	}

	[WindowsFact]
	public void Round_trips_unicode_quotes_and_newlines()
	{
		string secret = "café 日本語 \U0001F600 \"quoted\" 'single' $var `tick\r\nsecond line\ttab";

		Assert.Equal(secret, WUTools.DecryptString(WUTools.EncryptString(secret, null), null));
	}

	[WindowsFact]
	public void The_pass_phrase_is_ignored()
	{
		string protectedText = WUTools.EncryptString("secret", "one");

		Assert.Equal("secret", WUTools.DecryptString(protectedText, "something else entirely"));
	}

	[WindowsFact]
	public void The_output_is_base64_and_does_not_contain_the_plain_text()
	{
		string protectedText = WUTools.EncryptString("a-distinctive-secret-value", null);

		byte[] blob = Convert.FromBase64String(protectedText);
		Assert.NotEmpty(blob);
		Assert.DoesNotContain("a-distinctive-secret-value", protectedText);
		Assert.DoesNotContain("a-distinctive-secret-value", Encoding.UTF8.GetString(blob));
		Assert.DoesNotContain("a-distinctive-secret-value", Encoding.Unicode.GetString(blob));
	}

	[WindowsFact]
	public void Encrypting_twice_gives_different_blobs_that_both_decrypt()
	{
		string first = WUTools.EncryptString("same input", null);
		string second = WUTools.EncryptString("same input", null);

		Assert.NotEqual(first, second);
		Assert.Equal("same input", WUTools.DecryptString(first, null));
		Assert.Equal("same input", WUTools.DecryptString(second, null));
	}

	[WindowsFact]
	public void A_tampered_blob_fails_to_decrypt()
	{
		byte[] blob = Convert.FromBase64String(WUTools.EncryptString("do not alter", null));
		blob[blob.Length / 2] ^= 0x01;

		Assert.ThrowsAny<CryptographicException>(() => WUTools.DecryptString(Convert.ToBase64String(blob), null));
	}

	[WindowsFact]
	public void A_truncated_blob_fails_to_decrypt()
	{
		byte[] blob = Convert.FromBase64String(WUTools.EncryptString("do not truncate", null));
		byte[] truncated = new byte[blob.Length - 4];
		Array.Copy(blob, truncated, truncated.Length);

		Assert.ThrowsAny<CryptographicException>(() => WUTools.DecryptString(Convert.ToBase64String(truncated), null));
	}

	[WindowsFact]
	public void Input_that_was_not_produced_by_this_module_fails_to_decrypt()
	{
		Assert.ThrowsAny<CryptographicException>(() => WUTools.DecryptString(Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }), null));
	}

	[WindowsFact]
	public void A_blob_protected_without_this_modules_entropy_is_rejected()
	{
		// Same machine, same scope, but protected by "other software" with no (or other) entropy.
		byte[] plain = Encoding.UTF8.GetBytes("not from this module");
		string noEntropy = Convert.ToBase64String(ProtectedData.Protect(plain, null, DataProtectionScope.LocalMachine));
		string otherEntropy = Convert.ToBase64String(ProtectedData.Protect(plain, Encoding.UTF8.GetBytes("other"), DataProtectionScope.LocalMachine));

		Assert.ThrowsAny<CryptographicException>(() => WUTools.DecryptString(noEntropy, null));
		Assert.ThrowsAny<CryptographicException>(() => WUTools.DecryptString(otherEntropy, null));
	}

	[WindowsFact]
	public void Text_that_is_not_base64_is_rejected()
	{
		Assert.Throws<FormatException>(() => WUTools.DecryptString("this is not base64!", null));
	}

	[WindowsFact]
	public void Null_input_is_rejected()
	{
		Assert.ThrowsAny<ArgumentException>(() => WUTools.EncryptString(null, null));
		Assert.ThrowsAny<ArgumentException>(() => WUTools.DecryptString(null, null));
	}
}
