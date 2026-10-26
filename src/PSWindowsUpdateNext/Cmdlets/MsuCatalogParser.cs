using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PSWindowsUpdateNext;

/// <summary>
/// One row of a Microsoft Update Catalog search result page.
/// </summary>
internal sealed class CatalogRow
{
	public string Identity { get; set; }

	public string Title { get; set; }

	public string Product { get; set; }

	public string Classification { get; set; }

	public string LastUpdated { get; set; }

	public string Version { get; set; }

	public string Size { get; set; }

	public string OriginalSize { get; set; }
}

/// <summary>
/// One downloadable file listed by the Catalog "DownloadDialog" page.
/// </summary>
internal sealed class CatalogFile
{
	public string Url { get; set; }

	public string FileName { get; set; }

	public string Digest { get; set; }
}

/// <summary>
/// Pure parsing of Microsoft Update Catalog pages, plus the check that decides which download hosts
/// are trusted. Extracted from <see cref="GetWUOfflineMSU"/> so it can be tested without a network.
/// </summary>
internal static class MsuCatalogParser
{
	private static readonly Regex RowRegex = new Regex("<tr id=\"(?<id>[0-9a-fA-F-]{36})_R\\d+\"(?<row>.*?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex TitleRegex = new Regex("_link'[^>]*>(?<v>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex SizeRegex = new Regex("_size\">(?<v>[^<]*)</span>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex OriginalSizeRegex = new Regex("_originalSize\">(?<v>[^<]*)</span>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex TagRegex = new Regex("<[^>]+>", RegexOptions.Compiled);

	private static readonly Regex DownloadInfoRegex = new Regex("downloadInformation\\[(?<u>\\d+)\\]\\.files\\[(?<f>\\d+)\\]\\.(?<k>url|digest|fileName)\\s*=\\s*'(?<v>[^']*)'", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	/// <summary>
	/// Parse the rows of a Catalog search result page. A page without rows yields an empty list.
	/// </summary>
	public static List<CatalogRow> ParseSearchResults(string html)
	{
		List<CatalogRow> result = new List<CatalogRow>();
		foreach (Match row in RowRegex.Matches(html))
		{
			string rowHtml = row.Groups["row"].Value;
			string identity = row.Groups["id"].Value;
			Match titleMatch = TitleRegex.Match(rowHtml);
			string title = titleMatch.Success ? Clean(titleMatch.Groups["v"].Value) : identity;
			Match sizeMatch = SizeRegex.Match(rowHtml);
			string size = sizeMatch.Success ? Clean(sizeMatch.Groups["v"].Value) : string.Empty;
			Match originalSizeMatch = OriginalSizeRegex.Match(rowHtml);
			string originalSize = originalSizeMatch.Success ? Clean(originalSizeMatch.Groups["v"].Value) : string.Empty;
			result.Add(new CatalogRow
			{
				Identity = identity,
				Title = title,
				Product = Cell(rowHtml, 2),
				Classification = Cell(rowHtml, 3),
				LastUpdated = Cell(rowHtml, 4),
				Version = Cell(rowHtml, 5),
				Size = size,
				OriginalSize = originalSize
			});
		}
		return result;
	}

	/// <summary>
	/// Parse the Catalog "DownloadDialog" script block into (url, fileName, digest) entries.
	/// Entries with a malformed URL or a URL on an untrusted host or scheme are skipped, and
	/// <paramref name="warn"/> (when given) is told why.
	/// </summary>
	public static List<CatalogFile> ParseDownloadFiles(string html, Action<string> warn)
	{
		Dictionary<string, Dictionary<string, string>> files = new Dictionary<string, Dictionary<string, string>>();
		foreach (Match m in DownloadInfoRegex.Matches(html))
		{
			string key = m.Groups["u"].Value + "/" + m.Groups["f"].Value;
			if (!files.TryGetValue(key, out Dictionary<string, string> entry))
			{
				entry = new Dictionary<string, string>();
				files[key] = entry;
			}
			entry[m.Groups["k"].Value.ToLowerInvariant()] = m.Groups["v"].Value;
		}
		List<CatalogFile> result = new List<CatalogFile>();
		foreach (Dictionary<string, string> entry in files.Values)
		{
			if (!entry.TryGetValue("url", out string url) || string.IsNullOrEmpty(url))
			{
				continue;
			}
			Uri uri;
			try
			{
				uri = new Uri(url);
			}
			catch (UriFormatException)
			{
				warn?.Invoke("Skipping malformed download URL: " + url);
				continue;
			}
			if (!IsTrustedDownloadHost(uri))
			{
				warn?.Invoke("Skipping download URL on untrusted host or scheme: " + url);
				continue;
			}
			entry.TryGetValue("filename", out string fileName);
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = Path.GetFileName(uri.LocalPath);
			}
			fileName = Path.GetFileName(fileName);
			entry.TryGetValue("digest", out string digest);
			// Hand on the canonical form of the URI that was validated, never the raw string, so the host
			// BITS connects to cannot differ from the host the allowlist approved.
			result.Add(new CatalogFile { Url = uri.AbsoluteUri, FileName = fileName, Digest = digest });
		}
		return result;
	}

	/// <summary>
	/// True when the URI is HTTPS and its host is download.windowsupdate.com or a subdomain of
	/// windowsupdate.com or microsoft.com.
	/// </summary>
	public static bool IsTrustedDownloadHost(Uri uri)
	{
		if (uri.Scheme != Uri.UriSchemeHttps)
		{
			return false;
		}
		string host = uri.Host.ToLowerInvariant();
		return host == "download.windowsupdate.com" || host.EndsWith(".windowsupdate.com", StringComparison.Ordinal) || host.EndsWith(".microsoft.com", StringComparison.Ordinal);
	}

	/// <summary>
	/// Text of the n-th cell (<c>_C{n}_R{row}</c>) of a row, tags removed and entities decoded.
	/// </summary>
	public static string Cell(string row, int column)
	{
		Match match = Regex.Match(row, "_C" + column + "_R\\d+\"[^>]*>(?<v>.*?)</td>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		return match.Success ? Clean(match.Groups["v"].Value) : string.Empty;
	}

	/// <summary>
	/// Strip tags, decode HTML entities and trim.
	/// </summary>
	public static string Clean(string html)
	{
		return WebUtility.HtmlDecode(TagRegex.Replace(html, string.Empty)).Trim();
	}
}
