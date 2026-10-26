using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

public class MsuCatalogParserSearchTests
{
	private const string IdLcu = "4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35";
	private const string IdRollup = "b81d70f3-0c42-4a6e-8e55-91c3a7d2e4f8";
	private const string IdSsu = "E7F0A1B2-3C4D-4E5F-8A9B-0C1D2E3F4A5B";

	private static List<CatalogRow> ParseFixture()
	{
		return MsuCatalogParser.ParseSearchResults(Fixture.Load("search-results.html"));
	}

	[Fact]
	public void Finds_one_row_per_update_and_skips_the_header_row()
	{
		List<CatalogRow> rows = ParseFixture();

		Assert.Equal(new[] { IdLcu, IdRollup, IdSsu }, rows.Select(r => r.Identity).ToArray());
	}

	[Fact]
	public void Extracts_every_field_of_a_plain_row()
	{
		CatalogRow row = ParseFixture()[0];

		Assert.Equal("2020-03 Cumulative Update for Windows Server, version 1909 for x64-based Systems (KB4551762)", row.Title);
		Assert.Equal("Windows Server 2019", row.Product);
		Assert.Equal("Security Updates", row.Classification);
		Assert.Equal("3/12/2020", row.LastUpdated);
		Assert.Equal("n/a", row.Version);
		Assert.Equal("354.4 MB", row.Size);
		Assert.Equal("371609600", row.OriginalSize);
	}

	[Fact]
	public void Decodes_html_entities_and_strips_nested_tags()
	{
		CatalogRow row = ParseFixture()[1];

		// &amp; &#8211; (en dash) &lt; &gt; &quot; all decoded; surrounding whitespace trimmed.
		Assert.Equal("2020-03 Security & Quality Rollup for .NET Framework 3.5, 4.8 on Windows 10 \u2013 x64 (KB4551762) <preview> \"Update\"", row.Title);
		Assert.Equal("Windows 10, version 1903 and later & Windows Server", row.Product);
		// Cells that wrap their text in <div> lose the tag.
		Assert.Equal("Update Rollups", row.Classification);
		Assert.Equal("3/10/2020", row.LastUpdated);
		// &nbsp; decodes to U+00A0, which is not trimmed.
		Assert.Equal("1.2\u00A0GB", row.Size);
		Assert.Equal("1288490188", row.OriginalSize);
	}

	[Fact]
	public void Missing_size_cells_give_empty_strings_and_keep_the_rest()
	{
		CatalogRow row = ParseFixture()[2];

		Assert.Equal("Servicing Stack Update for Windows 10 Version 1909 (KB4551762)", row.Title);
		Assert.Equal(string.Empty, row.Size);
		Assert.Equal(string.Empty, row.OriginalSize);
		Assert.Equal("Windows 10", row.Product);
		Assert.Equal("2/11/2020", row.LastUpdated);
	}

	[Fact]
	public void Row_ids_match_case_insensitively_and_keep_their_original_case()
	{
		Assert.Equal(IdSsu, ParseFixture()[2].Identity);
	}

	[Fact]
	public void Title_falls_back_to_the_identity_when_the_row_has_no_link()
	{
		string html = "<tr id=\"" + IdLcu + "_R0\"><td id=\"" + IdLcu + "_C2_R0\">Windows 11</td></tr>";

		CatalogRow row = Assert.Single(MsuCatalogParser.ParseSearchResults(html));

		Assert.Equal(IdLcu, row.Title);
		Assert.Equal("Windows 11", row.Product);
		Assert.Equal(string.Empty, row.Classification);
	}

	[Fact]
	public void Rows_are_not_confused_with_each_other()
	{
		// The second row has no product cell; it must not borrow the first row's.
		string html =
			"<tr id=\"" + IdLcu + "_R0\"><td id=\"" + IdLcu + "_C2_R0\">Product A</td></tr>" +
			"<tr id=\"" + IdRollup + "_R1\"><td id=\"" + IdRollup + "_C3_R1\">Class B</td></tr>";

		List<CatalogRow> rows = MsuCatalogParser.ParseSearchResults(html);

		Assert.Equal(2, rows.Count);
		Assert.Equal("Product A", rows[0].Product);
		Assert.Equal(string.Empty, rows[1].Product);
		Assert.Equal("Class B", rows[1].Classification);
	}

	[Fact]
	public void Clean_decodes_entities_after_removing_tags()
	{
		// Decoding first would turn &lt;b&gt; into a tag that the tag stripper then removes.
		Assert.Equal("<b>bold</b>", MsuCatalogParser.Clean("  &lt;b&gt;<i>bold</i>&lt;/b&gt;\r\n"));
		Assert.Equal("Tom & Jerry", MsuCatalogParser.Clean("<span>Tom &amp; Jerry</span>"));
		Assert.Equal("caf\u00E9", MsuCatalogParser.Clean("caf&eacute;"));
		Assert.Equal("\u00A9 2020", MsuCatalogParser.Clean("&#169; 2020"));
		Assert.Equal("\u00A9", MsuCatalogParser.Clean("&#xA9;"));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("<html></html>")]
	[InlineData("No updates were found")]
	[InlineData("<tr id=\"")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\"")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\">no closing tag")]
	[InlineData("<tr id=\"not-a-guid_R0\"><td>x</td></tr>")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R\"></tr>")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\"></tr>")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\"><td id=\"x_C2_R0\"")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\"><a id='x_link'>unterminated")]
	[InlineData("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R0\"><span id=\"x_size\">12 MB</tr>")]
	[InlineData("<<<<>>>> &&&& &#; &#x; &#99999999999; &unknown; </tr></tr></tr>")]
	[InlineData("\0\0\0\u0001\u001f\uFFFF\uD800")]
	public void Malformed_search_pages_do_not_throw(string html)
	{
		Exception error = Record.Exception(() => MsuCatalogParser.ParseSearchResults(html));

		Assert.Null(error);
	}

	[Fact]
	public void Truncated_fixture_never_throws_at_any_length()
	{
		string html = Fixture.Load("search-results.html");

		for (int length = 0; length <= html.Length; length += 7)
		{
			string truncated = html.Substring(0, length);
			Exception error = Record.Exception(() => MsuCatalogParser.ParseSearchResults(truncated));
			Assert.Null(error);
		}
	}

	[Fact]
	public void Many_unterminated_rows_parse_in_reasonable_time()
	{
		StringBuilder html = new StringBuilder();
		for (int i = 0; i < 300; i++)
		{
			html.Append("<tr id=\"4a2b9c6e-5d1f-4e3a-9b7c-2f8e1d0a6c35_R").Append(i).Append("\"><td>");
		}

		System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
		List<CatalogRow> rows = MsuCatalogParser.ParseSearchResults(html.ToString());
		watch.Stop();

		Assert.Empty(rows);
		Assert.True(watch.Elapsed < TimeSpan.FromSeconds(10), "took " + watch.Elapsed);
	}
}

public class MsuCatalogParserDownloadTests
{
	private static List<CatalogFile> ParseFixture(out List<string> warnings)
	{
		List<string> collected = new List<string>();
		List<CatalogFile> files = MsuCatalogParser.ParseDownloadFiles(Fixture.Load("download-dialog.html"), collected.Add);
		warnings = collected;
		return files;
	}

	[Fact]
	public void Returns_every_file_of_every_update_in_page_order()
	{
		List<CatalogFile> files = ParseFixture(out List<string> warnings);

		Assert.Empty(warnings);
		Assert.Equal(3, files.Count);
		Assert.Equal("windows10.0-kb4540673-x64_f1ab2c3d4e5f60718293a4b5c6d7e8f901234567.msu", files[0].FileName);
		Assert.Equal("windows10.0-kb4551762-x64_7d8e9f0a1b2c3d4e5f60718293a4b5c6d7e8f901.msu", files[1].FileName);
		Assert.Equal("windows10.0-kb4551762-x86_aabbccddeeff00112233445566778899aabbccdd.cab", files[2].FileName);
	}

	[Fact]
	public void Reads_url_digest_and_file_name_of_each_file()
	{
		List<CatalogFile> files = ParseFixture(out _);

		Assert.Equal("https://catalog.s.download.windowsupdate.com/d/msdownload/update/software/secu/2020/03/windows10.0-kb4540673-x64_f1ab2c3d4e5f60718293a4b5c6d7e8f901234567.msu", files[0].Url);
		Assert.Equal("qZk+NkcGgWq6PiVxeFDCbJzQ2J0=", files[0].Digest);
		Assert.Equal("https://catalog.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/0a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f9", files[1].Url);
		Assert.Equal("2jmj7l5rSw0yVb/vlWAYkK/YBwk=", files[1].Digest);
		Assert.Equal("https://download.windowsupdate.com/c/msdownload/update/software/updt/2020/03/windows10.0-kb4551762-x86_aabbccddeeff00112233445566778899aabbccdd.cab", files[2].Url);
		Assert.Equal("ungWv48Bz+pBQUDeXa4iI7ADYaM=", files[2].Digest);
	}

	[Fact]
	public void The_digestAlgorithm_property_is_not_mistaken_for_the_digest()
	{
		List<CatalogFile> files = ParseFixture(out _);

		Assert.All(files, f => Assert.NotEqual("SHA1", f.Digest));
	}

	[Fact]
	public void Property_names_and_whitespace_around_the_equals_sign_are_tolerated()
	{
		string html =
			"downloadInformation[0].files[0].URL = 'https://download.windowsupdate.com/a/x.msu';\n" +
			"DOWNLOADINFORMATION[0].FILES[0].DIGEST\t=\t'abc=';\n" +
			"downloadInformation[0].files[0].filename='x.msu';";

		CatalogFile file = Assert.Single(MsuCatalogParser.ParseDownloadFiles(html, null));

		Assert.Equal("https://download.windowsupdate.com/a/x.msu", file.Url);
		Assert.Equal("abc=", file.Digest);
		Assert.Equal("x.msu", file.FileName);
	}

	[Fact]
	public void Missing_file_name_is_taken_from_the_url_path()
	{
		string html = "downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/c/msdownload/windows10.0-kb1-x64.msu?x=1#frag';";

		CatalogFile file = Assert.Single(MsuCatalogParser.ParseDownloadFiles(html, null));

		Assert.Equal("windows10.0-kb1-x64.msu", file.FileName);
		Assert.Null(file.Digest);
	}

	[Fact]
	public void File_name_never_carries_a_directory()
	{
		string html =
			"downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/a/x.msu';\n" +
			"downloadInformation[0].files[0].fileName = '../../Windows/evil.msu';";

		CatalogFile file = Assert.Single(MsuCatalogParser.ParseDownloadFiles(html, null));

		Assert.Equal("evil.msu", file.FileName);
	}

	[Fact]
	public void Entries_without_a_url_are_dropped()
	{
		string html =
			"downloadInformation[0].files[0].digest = 'abc=';\n" +
			"downloadInformation[0].files[0].fileName = 'x.msu';\n" +
			"downloadInformation[0].files[1].url = '';\n" +
			"downloadInformation[0].files[2].url = 'https://download.windowsupdate.com/a/y.msu';";

		CatalogFile file = Assert.Single(MsuCatalogParser.ParseDownloadFiles(html, null));

		Assert.Equal("https://download.windowsupdate.com/a/y.msu", file.Url);
	}

	[Fact]
	public void Malformed_urls_are_skipped_with_a_warning()
	{
		string html =
			"downloadInformation[0].files[0].url = 'not a url';\n" +
			"downloadInformation[0].files[1].url = 'https://';\n" +
			"downloadInformation[0].files[2].url = 'https://download.windowsupdate.com/a/ok.msu';";
		List<string> warnings = new List<string>();

		List<CatalogFile> files = MsuCatalogParser.ParseDownloadFiles(html, warnings.Add);

		Assert.Equal("https://download.windowsupdate.com/a/ok.msu", Assert.Single(files).Url);
		Assert.Equal(2, warnings.Count);
		Assert.Contains("malformed", warnings[0]);
	}

	[Fact]
	public void Untrusted_hosts_and_schemes_are_skipped_with_a_warning()
	{
		string html =
			"downloadInformation[0].files[0].url = 'https://evil.example.net/x.msu';\n" +
			"downloadInformation[0].files[1].url = 'http://download.windowsupdate.com/x.msu';\n" +
			"downloadInformation[0].files[2].url = 'ftp://download.windowsupdate.com/x.msu';\n" +
			"downloadInformation[0].files[3].url = 'file:///C:/Windows/x.msu';\n" +
			"downloadInformation[0].files[4].url = 'https://download.windowsupdate.com/a/ok.msu';";
		List<string> warnings = new List<string>();

		List<CatalogFile> files = MsuCatalogParser.ParseDownloadFiles(html, warnings.Add);

		Assert.Equal("https://download.windowsupdate.com/a/ok.msu", Assert.Single(files).Url);
		Assert.Equal(4, warnings.Count);
		Assert.All(warnings, w => Assert.Contains("untrusted", w));
	}

	[Fact]
	public void The_same_file_index_in_different_updates_is_kept_apart()
	{
		string html =
			"downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/a/first.msu';\n" +
			"downloadInformation[1].files[0].url = 'https://download.windowsupdate.com/a/second.msu';";

		List<CatalogFile> files = MsuCatalogParser.ParseDownloadFiles(html, null);

		Assert.Equal(new[] { "first.msu", "second.msu" }, files.Select(f => f.FileName).ToArray());
	}

	[Theory]
	[InlineData("")]
	[InlineData("<html>Server error</html>")]
	[InlineData("downloadInformation")]
	[InlineData("downloadInformation[0].files[0].url")]
	[InlineData("downloadInformation[0].files[0].url = ")]
	[InlineData("downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/a/x.msu")]
	[InlineData("downloadInformation[x].files[y].url = 'https://download.windowsupdate.com/a/x.msu';")]
	[InlineData("downloadInformation[99999999999999999999].files[0].url = 'https://download.windowsupdate.com/a/x.msu';")]
	[InlineData("downloadInformation[0].files[0].url = \"https://download.windowsupdate.com/a/x.msu\";")]
	[InlineData("downloadInformation[0].files[0].url = '\0\u0001\uFFFF';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://%zz/';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://[::1/x';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://download.windowsupdate.com:99999999/x';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/a/x.msu';downloadInformation[0].files[0].fileName = '';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://download.windowsupdate.com/';")]
	[InlineData("downloadInformation[0].files[0].url = 'https://download.windowsupdate.com';downloadInformation[0].files[0].fileName = '..';")]
	public void Malformed_download_dialogs_do_not_throw(string html)
	{
		Exception error = Record.Exception(() => MsuCatalogParser.ParseDownloadFiles(html, null));

		Assert.Null(error);
	}

	[Fact]
	public void Truncated_fixture_never_throws_at_any_length()
	{
		string html = Fixture.Load("download-dialog.html");

		for (int length = 0; length <= html.Length; length += 5)
		{
			string truncated = html.Substring(0, length);
			Exception error = Record.Exception(() => MsuCatalogParser.ParseDownloadFiles(truncated, null));
			Assert.Null(error);
		}
	}
}
