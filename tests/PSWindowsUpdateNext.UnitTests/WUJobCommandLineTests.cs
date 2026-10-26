using System;
using System.Text;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

public class WUJobCommandLineTests
{
	private const string Prefix = "-NoProfile -NonInteractive -EncodedCommand ";

	/// <summary>
	/// Decodes the argument the way powershell.exe does: Base64 of the UTF-16LE script text.
	/// </summary>
	private static string Decode(string arguments)
	{
		Assert.StartsWith(Prefix, arguments, StringComparison.Ordinal);
		return Encoding.Unicode.GetString(Convert.FromBase64String(arguments.Substring(Prefix.Length)));
	}

	public static readonly object[][] Scripts =
	{
		new object[] { "ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\\PSWindowsUpdateNext.log" },
		new object[] { "Write-Output \"double\" 'single' \"nested 'single' in double\" 'it''s'" },
		new object[] { "$a = $env:TEMP; \"$a\\$($b.Name) ${c}\"; $$; $?; $^; $_" },
		new object[] { "`n `t `\" `$notAVariable `` ```" },
		new object[] { "line one\nline two\r\nline three\rline four" },
		new object[] { "Write-Output \"quoted \\\" backslash-quote\"; Write-Output 'C:\\dir\\'" },
		new object[] { "Write-Output 'caf\u00E9 \u00FC\u00DF \u65E5\u672C\u8A9E \u0420\u0443\u0441\u0441\u043A\u0438\u0439 \u05E2\u05D1\u05E8\u05D9\u05EA \U0001F600 \U0001D11E'" },
		new object[] { "& { param($x) $x } 'a' ; exit 0 # comment with \"quote" },
		new object[] { "a & b | c > d < e ^ f % g ! h ; i , j ( k ) l" },
		new object[] { "-EncodedCommand AAAA -Command \"evil\" ; ' ; calc.exe ; '" },
		new object[] { "  leading and trailing whitespace  \t " },
		new object[] { "" },
		new object[] { "x" },
		new object[] { "tab\there\0nul and \u2028 line separator \uFEFF bom" },
	};

	[Theory]
	[MemberData(nameof(Scripts))]
	public void EncodedCommand_round_trips_the_script_exactly(string script)
	{
		string arguments = WUJobCommandLine.BuildArguments(script);

		Assert.Equal(script, Decode(arguments));
	}

	[Fact]
	public void Lone_surrogates_become_the_replacement_character()
	{
		// Not valid Unicode text, so UTF-16 encoding substitutes U+FFFD. Every well-formed string,
		// supplementary-plane characters included, round-trips exactly (see the theory above).
		Assert.Equal("a\uFFFDb\uFFFDc", Decode(WUJobCommandLine.BuildArguments("a\uD800b\uDC00c")));
	}

	[Theory]
	[MemberData(nameof(Scripts))]
	public void Arguments_start_with_NoProfile_and_NonInteractive(string script)
	{
		string arguments = WUJobCommandLine.BuildArguments(script);

		Assert.StartsWith(Prefix, arguments, StringComparison.Ordinal);
		Assert.Contains("-NoProfile", arguments);
		Assert.Contains("-NonInteractive", arguments);
	}

	[Theory]
	[MemberData(nameof(Scripts))]
	public void The_script_cannot_add_arguments_or_quote_characters(string script)
	{
		string arguments = WUJobCommandLine.BuildArguments(script);

		// The script travels only as the Base64 tail: one token with no spaces, quotes or shell
		// metacharacters, however hostile the script text is.
		string[] tokens = arguments.Split(' ');
		Assert.Equal(new[] { "-NoProfile", "-NonInteractive", "-EncodedCommand" }, new[] { tokens[0], tokens[1], tokens[2] });
		Assert.Equal(4, tokens.Length);
		Assert.Matches("^[A-Za-z0-9+/]*={0,2}$", tokens[3]);
		Assert.DoesNotContain("\"", arguments);
		Assert.DoesNotContain("'", arguments);
	}

	[Fact]
	public void The_default_script_is_not_visible_in_the_arguments()
	{
		string arguments = WUJobCommandLine.BuildArguments("Get-WindowsUpdate -AcceptAll -Install");

		Assert.DoesNotContain("Get-WindowsUpdate", arguments);
	}

	[Fact]
	public void The_encoding_is_utf16le_without_a_byte_order_mark()
	{
		string arguments = WUJobCommandLine.BuildArguments("AB");

		// 'A' 'B' as UTF-16LE: 41 00 42 00.
		Assert.Equal(Prefix + Convert.ToBase64String(new byte[] { 0x41, 0x00, 0x42, 0x00 }), arguments);
	}

	[Fact]
	public void A_null_script_still_throws()
	{
		Assert.Throws<ArgumentNullException>(() => WUJobCommandLine.BuildArguments(null));
	}
}
