using System;
using System.IO;
using System.Text;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

public class WUFileVerifierTests : IDisposable
{
	private readonly string _dir = Path.Combine(Path.GetTempPath(), "pswun-tests-" + Guid.NewGuid().ToString("N"));

	public WUFileVerifierTests()
	{
		Directory.CreateDirectory(_dir);
	}

	public void Dispose()
	{
		try
		{
			Directory.Delete(_dir, true);
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
	}

	private string WriteFile(byte[] content)
	{
		string path = Path.Combine(_dir, Guid.NewGuid().ToString("N") + ".bin");
		File.WriteAllBytes(path, content);
		return path;
	}

	private string WriteFile(string ascii)
	{
		return WriteFile(Encoding.ASCII.GetBytes(ascii));
	}

	// Vectors: FIPS 180-4 / NIST CAVS examples ("abc", empty message, one million 'a').

	[Fact]
	public void Sha1Base64_of_abc()
	{
		Assert.Equal("qZk+NkcGgWq6PiVxeFDCbJzQ2J0=", WUFileVerifier.Sha1Base64(WriteFile("abc")));
	}

	[Fact]
	public void Sha1Base64_of_empty_file()
	{
		Assert.Equal("2jmj7l5rSw0yVb/vlWAYkK/YBwk=", WUFileVerifier.Sha1Base64(WriteFile(new byte[0])));
	}

	[Fact]
	public void Sha1Base64_of_one_million_a()
	{
		Assert.Equal("NKqXPNTE2qT2Husr260nMWU0AW8=", WUFileVerifier.Sha1Base64(WriteFile(new string('a', 1000000))));
	}

	[Fact]
	public void Sha1Base64_is_the_base64_of_the_raw_digest_not_of_the_hex()
	{
		// a9993e364706816aba3e25717850c26c9cd0d89d is the hex form of SHA-1("abc").
		byte[] raw = new byte[]
		{
			0xa9, 0x99, 0x3e, 0x36, 0x47, 0x06, 0x81, 0x6a, 0xba, 0x3e,
			0x25, 0x71, 0x78, 0x50, 0xc2, 0x6c, 0x9c, 0xd0, 0xd8, 0x9d
		};

		Assert.Equal(Convert.ToBase64String(raw), WUFileVerifier.Sha1Base64(WriteFile("abc")));
	}

	[Fact]
	public void Sha256Hex_of_abc()
	{
		Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", WUFileVerifier.Sha256Hex(WriteFile("abc")));
	}

	[Fact]
	public void Sha256Hex_of_empty_file()
	{
		Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", WUFileVerifier.Sha256Hex(WriteFile(new byte[0])));
	}

	[Fact]
	public void Sha256Hex_of_one_million_a()
	{
		Assert.Equal("cdc76e5c9914fb9281a1c7e284d73e67f1809a48a497200e046d39ccc7112cd0", WUFileVerifier.Sha256Hex(WriteFile(new string('a', 1000000))));
	}

	[Fact]
	public void Sha256Hex_is_lowercase_zero_padded_and_64_characters()
	{
		// Bytes below 0x10 must keep their leading zero; every byte value is hashed over a few files.
		for (int i = 0; i < 16; i++)
		{
			string hex = WUFileVerifier.Sha256Hex(WriteFile(new byte[] { (byte)i }));

			Assert.Equal(64, hex.Length);
			Assert.Equal(hex.ToLowerInvariant(), hex);
			Assert.Matches("^[0-9a-f]{64}$", hex);
		}
		// SHA-256 of the single byte 0x00.
		Assert.Equal("6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d", WUFileVerifier.Sha256Hex(WriteFile(new byte[] { 0 })));
	}

	[Fact]
	public void Digests_cover_binary_content_larger_than_one_read_buffer()
	{
		byte[] content = new byte[300000];
		for (int i = 0; i < content.Length; i++)
		{
			content[i] = (byte)(i * 31 + 7);
		}
		string path = WriteFile(content);

		using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
		{
			Assert.Equal(BitConverter.ToString(sha.ComputeHash(content)).Replace("-", string.Empty).ToLowerInvariant(), WUFileVerifier.Sha256Hex(path));
		}
		using (System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create())
		{
			Assert.Equal(Convert.ToBase64String(sha.ComputeHash(content)), WUFileVerifier.Sha1Base64(path));
		}
	}

	[Fact]
	public void A_single_changed_byte_changes_both_digests()
	{
		string original = WriteFile("windows update package");
		string tampered = WriteFile("windows update packagf");

		Assert.NotEqual(WUFileVerifier.Sha1Base64(original), WUFileVerifier.Sha1Base64(tampered));
		Assert.NotEqual(WUFileVerifier.Sha256Hex(original), WUFileVerifier.Sha256Hex(tampered));
	}

	[Fact]
	public void A_missing_file_throws_instead_of_returning_a_digest()
	{
		string missing = Path.Combine(_dir, "does-not-exist.msu");

		Assert.Throws<FileNotFoundException>(() => WUFileVerifier.Sha1Base64(missing));
		Assert.Throws<FileNotFoundException>(() => WUFileVerifier.Sha256Hex(missing));
	}
}
