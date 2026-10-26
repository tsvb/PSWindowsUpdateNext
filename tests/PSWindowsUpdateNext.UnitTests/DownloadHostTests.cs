using System;
using System.Collections.Generic;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

public class DownloadHostTests
{
	private static bool IsTrusted(string url)
	{
		return MsuCatalogParser.IsTrustedDownloadHost(new Uri(url));
	}

	/// <summary>
	/// What the Catalog parser lets through when the URL is the only download entry of a dialog.
	/// This is the check that actually gates downloads, so the tricky cases go through it too.
	/// </summary>
	private static bool PassesParser(string url)
	{
		string html = "downloadInformation[0].files[0].url = '" + url + "';";
		return MsuCatalogParser.ParseDownloadFiles(html, null).Count == 1;
	}

	[Theory]
	[InlineData("https://download.windowsupdate.com/c/msdownload/update/software/secu/2020/03/x.msu")]
	[InlineData("https://catalog.s.download.windowsupdate.com/d/msdownload/update/software/secu/2020/03/x.msu")]
	[InlineData("https://catalog.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/0a1b2c3d")]
	[InlineData("https://DOWNLOAD.WindowsUpdate.COM/c/x.msu")]
	[InlineData("https://Catalog.S.Download.WindowsUpdate.Com/d/x.msu")]
	[InlineData("https://download.windowsupdate.com:443/c/x.msu")]
	[InlineData("https://download.windowsupdate.com/c/x.msu?a=b#frag")]
	[InlineData("https://a.b.c.windowsupdate.com/x")]
	[InlineData("https://dl.delivery.mp.microsoft.com/x")]
	public void Trusted_hosts_over_https_are_accepted(string url)
	{
		Assert.True(IsTrusted(url));
		Assert.True(PassesParser(url));
	}

	[Theory]
	// Look-alike names that merely contain or end with the trusted name.
	[InlineData("https://evilmicrosoft.com/x.msu")]
	[InlineData("https://evilwindowsupdate.com/x.msu")]
	[InlineData("https://notdownload.windowsupdate.com.evil/x.msu")]
	[InlineData("https://microsoft.com.evil.net/x.msu")]
	[InlineData("https://windowsupdate.com.attacker.org/x.msu")]
	[InlineData("https://download.windowsupdate.com.attacker.org/x.msu")]
	[InlineData("https://download.windowsupdate.com.microsoft.com.evil.net/x.msu")]
	[InlineData("https://download-windowsupdate.com/x.msu")]
	[InlineData("https://download.windowsupdate.co/x.msu")]
	[InlineData("https://download.windowsupdate.com-evil.net/x.msu")]
	[InlineData("https://microsoft.com.cn/x.msu")]
	[InlineData("https://evil.net/download.windowsupdate.com/x.msu")]
	[InlineData("https://evil.net/?h=download.windowsupdate.com")]
	[InlineData("https://evil.net/#.microsoft.com")]
	// Userinfo: the part before @ is not the host.
	[InlineData("https://download.windowsupdate.com@evil.net/")]
	[InlineData("https://download.windowsupdate.com:password@evil.net/")]
	[InlineData("https://user:download.windowsupdate.com@evil.net/x.msu")]
	[InlineData("https://catalog.sf.dl.delivery.mp.microsoft.com%40evil.net/")]
	[InlineData("https://download.windowsupdate.com%2Fx@evil.net/")]
	// Wrong scheme.
	[InlineData("http://download.windowsupdate.com/x.msu")]
	[InlineData("ftp://download.windowsupdate.com/x.msu")]
	[InlineData("file://download.windowsupdate.com/x.msu")]
	[InlineData("file:///C:/Windows/x.msu")]
	[InlineData("//download.windowsupdate.com/x.msu")]
	// The bare registrable domains are not covered by the subdomain suffix checks.
	[InlineData("https://windowsupdate.com/x.msu")]
	[InlineData("https://microsoft.com/x.msu")]
	// Trailing dot (fully qualified form) is not recognised; rejecting is the safe direction.
	[InlineData("https://download.windowsupdate.com./x.msu")]
	// Raw IP addresses.
	[InlineData("https://127.0.0.1/x.msu")]
	[InlineData("https://[::1]/x.msu")]
	[InlineData("https://13.107.4.50/x.msu")]
	public void Untrusted_hosts_and_schemes_are_rejected(string url)
	{
		Uri uri;
		try
		{
			uri = new Uri(url);
		}
		catch (UriFormatException)
		{
			// Not even parseable as an absolute URI: ParseDownloadFiles drops it as malformed.
			Assert.False(PassesParser(url));
			return;
		}

		Assert.False(MsuCatalogParser.IsTrustedDownloadHost(uri));
		Assert.False(PassesParser(url));
	}

	[NetCoreTheory]
	// .NET reads a backslash as a slash in http(s) URLs, and a downloader might not. Every variant
	// that mixes a backslash with a second host-like part is refused outright.
	[InlineData("https://evil.net\\@download.windowsupdate.com/x.msu")]
	[InlineData("https://evil.net\\.download.windowsupdate.com/x.msu")]
	[InlineData("https://evil.net/\\@download.windowsupdate.com/x.msu")]
	[InlineData("https://download.windowsupdate.com\\@evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com\\.evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com\\evil.net/x.msu")]
	[InlineData("https:\\\\download.windowsupdate.com\\x.msu")]
	// Control characters and whitespace inside the authority.
	[InlineData("https://download.windowsupdate.com\t.evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com .evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com\r\n@evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com\u0000.evil.net/x.msu")]
	// Look-alike Unicode that must not normalise into a trusted ASCII host or end the host early.
	[InlineData("https://download.windowsupdate\u2024com.evil.net/x.msu")]
	[InlineData("https://download.windowsupdate\u3002com/x.msu")]
	[InlineData("https://download.windowsupdate.c\u043Em/x.msu")]
	[InlineData("https://download.w\u0131ndowsupdate.com/x.msu")]
	[InlineData("https://download.windowsupdate.com\uFF20evil.net/x.msu")]
	[InlineData("https://download.windowsupdate.com\uFF0Fevil.net/x.msu")]
	// Userinfo whose password part looks like a port in front of the real host.
	[InlineData("https://download.windowsupdate.com:evil@evil.net/")]
	[InlineData("https://download.windowsupdate.com:80@evil.net/")]
	// Scheme-only or scheme-relative forms.
	[InlineData("https:/download.windowsupdate.com/x.msu")]
	[InlineData("https:download.windowsupdate.com/x.msu")]
	public void Parser_differential_tricks_do_not_get_a_url_through(string url)
	{
		Assert.False(PassesParser(url));
	}

	/// <summary>
	/// Characterisation of forms that are accepted today. In every one the host System.Uri reads is a
	/// genuine windowsupdate.com or microsoft.com name, so none of them reaches a foreign host; they
	/// are pinned so that a change to the check is a conscious one.
	/// </summary>
	[Theory]
	// Userinfo in front of a trusted host: credentials would go to Microsoft, not to the other name.
	[InlineData("https://evil.net@download.windowsupdate.com/x.msu", "download.windowsupdate.com")]
	[InlineData("https://evil.net:443@download.windowsupdate.com/x.msu", "download.windowsupdate.com")]
	// '@' after the host ends it in a query or fragment, so it never becomes userinfo.
	[InlineData("https://download.windowsupdate.com?@evil.net/", "download.windowsupdate.com")]
	[InlineData("https://download.windowsupdate.com#@evil.net/", "download.windowsupdate.com")]
	// Any port, scheme in upper case, leading whitespace.
	[InlineData("https://download.windowsupdate.com:8443/x.msu", "download.windowsupdate.com")]
	[InlineData("HTTPS://download.windowsupdate.com/x.msu", "download.windowsupdate.com")]
	[InlineData("  https://download.windowsupdate.com/x.msu", "download.windowsupdate.com")]
	public void Accepted_forms_always_resolve_to_a_trusted_host(string url, string expectedIdnHost)
	{
		CatalogFile file = Assert.Single(MsuCatalogParser.ParseDownloadFiles("downloadInformation[0].files[0].url = '" + url + "';", null));

		Uri uri = new Uri(file.Url);
		Assert.Equal(expectedIdnHost, uri.IdnHost);
		Assert.True(uri.IdnHost.EndsWith(".windowsupdate.com", StringComparison.Ordinal) || uri.IdnHost.EndsWith(".microsoft.com", StringComparison.Ordinal));
	}

	[NetCoreTheory]
	// A non-ASCII label is a real subdomain of windowsupdate.com (punycode on the wire).
	[InlineData("https://d\u00F6wnload.windowsupdate.com/x.msu", "xn--dwnload-90a.windowsupdate.com")]
	public void Accepted_non_ascii_subdomain_resolves_to_a_trusted_host(string url, string expectedIdnHost)
	{
		Accepted_forms_always_resolve_to_a_trusted_host(url, expectedIdnHost);
	}
}
