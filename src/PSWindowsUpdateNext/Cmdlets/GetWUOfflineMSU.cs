using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get offline MSU package.</para>
/// <para type="description">Use Get-WUOfflineMSU cmdlet to download MSU package from Microsoft Update Catalog website.</para>
/// <para type="description">Every file is downloaded over HTTPS, checked against the SHA-1 digest the Catalog publishes, and its Authenticode signature is verified. Files that fail either check are deleted.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Download package for KB4551762.</para>
/// PS&gt; Get-WUOfflineMSU -KBArticleID 4551762 -Destination C:\Temp
///
///     Confirm
///     Are you sure you want to perform this action?
///     Performing the operation "(20.04.2020 14:27:17) 2020-03 Cumulative Update for Windows Server, version 1909 for
///     x64-based Systems (KB4551762)[354.4 MB]" on target "DESKTOP-GCQBCBS".
///     [Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): N
///
///     X Result     Title                                                                                 LastUpdated     Size
///     - ------     -----                                                                                 -----------     ----
///     1 Rejected   2020-03 Cumulative Update for Windows Server, version 1909 for x64-based Systems (... 3/12/2020   354.4 MB
///     1 Accepted   2020-03 Cumulative Update for Windows 10 Version 1909 for x64-based Systems (KB455... 3/12/2020   354.4 MB
///     2 Downloaded 2020-03 Cumulative Update for Windows 10 Version 1909 for x64-based Systems (KB455... 3/12/2020   354.4 MB
///  </code>
///  </example>
[Cmdlet("Get", "WUOfflineMSU", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
public class GetWUOfflineMSU : PSCmdlet
{
	private const string CatalogSearchUrl = "https://www.catalog.update.microsoft.com/Search.aspx?q=";

	private const string CatalogDownloadDialogUrl = "https://www.catalog.update.microsoft.com/DownloadDialog.aspx";

	private static readonly Regex RowRegex = new Regex("<tr id=\"(?<id>[0-9a-fA-F-]{36})_R\\d+\"(?<row>.*?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex TitleRegex = new Regex("_link'[^>]*>(?<v>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

	private static readonly Regex SizeRegex = new Regex("_size\">(?<v>[^<]*)</span>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex OriginalSizeRegex = new Regex("_originalSize\">(?<v>[^<]*)</span>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex TagRegex = new Regex("<[^>]+>", RegexOptions.Compiled);

	private static readonly Regex DownloadInfoRegex = new Regex("downloadInformation\\[(?<u>\\d+)\\]\\.files\\[(?<f>\\d+)\\]\\.(?<k>url|digest|fileName)\\s*=\\s*'(?<v>[^']*)'", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private Hashtable _PSWUSettings = new Hashtable();

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
	private string[] ComputerName { get; set; }

	/// <summary>
	/// <para type="description">Specify alternative credential.</para>
	/// </summary>
	[Parameter]
	private PSCredential Credential { get; set; }

	/// <summary>
	/// <para type="description">Send report email to specific recipients.</para>
	/// <para type="description">Requires the parameter -PSWUSettings or declare the PSWUSettings.xml file in ModuleBase path.</para>
	/// </summary>
	[Parameter]
	private SwitchParameter SendReport { get; set; }

	/// <summary>
	/// <para type="description">Required parameter for -SendReport.</para>
	/// <para type="description">Passes the parameters (as hashtable) necessary to send the report:
	/// \r\n@{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25];[Subject="Alternative Subject"];[Properties="Alternative object properties"];[Style="Table|List"]}</para>
	/// <para type="description">Send parameters can also be saved to a PSWUSettings.xml file in ModuleBase path:
	/// \r\nExport-Clixml @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25]}"</para>
	/// </summary>
	[Parameter]
	private Hashtable PSWUSettings
	{
		get
		{
			return _PSWUSettings;
		}
		set
		{
			_PSWUSettings = value;
		}
	}

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	private SwitchParameter Debuger { get; set; }

	/// <summary>
	/// <para type="description">Finds updates that contain a KBArticleID.</para>
	/// </summary>
	[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
	public string KBArticleID { get; set; }

	/// <summary>
	/// <para type="description">Destination for downloaded files.</para>
	/// </summary>
	[Parameter(Mandatory = true)]
	public string Destination { get; set; }

	/// <summary>
	/// <para type="description">Do not ask confirmation for download updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AcceptAll { get; set; }

	/// <summary>
	/// <para type="description">Keep files whose Authenticode signature cannot be verified (the SHA-1 digest is always enforced).</para>
	/// </summary>
	[Parameter]
	public SwitchParameter SkipSignatureCheck { get; set; }

	/// <summary>
	/// <para type="description">Only consider Catalog entries with these update IDs (the GUID shown as Identity). Combine with -AcceptAll to download a chosen subset without prompts.</para>
	/// </summary>
	[Parameter]
	public string[] UpdateID { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static WUBits WUBitsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static DateTime CmdletStart { get; set; }

	private static DateTime CmdletEnd { get; set; }

	/// <summary>
	/// Begin
	/// </summary>
	protected override void BeginProcessing()
	{
		CmdletStart = DateTime.Now;
		string invocationName = MyInvocation.InvocationName;
		WriteDebug(DateTime.Now.ToString() + " CmdletStart: " + invocationName);
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
		{
			WriteWarning("To perform some operations you must run an elevated Windows PowerShell console.");
		}
		WUToolsObj = new WUTools();
		WUBitsObj = new WUBits();
		OutputObj = new Collection<PSObject>();
		if ((bool)SendReport)
		{
			WriteDebug(DateTime.Now.ToString() + " Test smtp settings");
			if (!PSWUSettings.ContainsKey("Properties"))
			{
				PSWUSettings.Add("Properties", new string[1] { "*" });
			}
			PSObject pSObject = WUToolsObj.TestMail(PSWUSettings);
			if (pSObject.Properties.Match("ErrorRecord").Count == 1)
			{
				WriteError((ErrorRecord)pSObject.Properties["ErrorRecord"].Value);
				SendReport = false;
				WriteDebug(DateTime.Now.ToString() + " Disabling -SendReport");
			}
		}
		if (ComputerName == null)
		{
			ComputerName = new string[1] { Environment.MachineName };
		}
	}

	private static string Cell(string row, int column)
	{
		Match match = Regex.Match(row, "_C" + column + "_R\\d+\"[^>]*>(?<v>.*?)</td>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		return match.Success ? Clean(match.Groups["v"].Value) : string.Empty;
	}

	private static string Clean(string html)
	{
		return WebUtility.HtmlDecode(TagRegex.Replace(html, string.Empty)).Trim();
	}

	private static bool IsTrustedDownloadHost(Uri uri)
	{
		if (uri.Scheme != Uri.UriSchemeHttps)
		{
			return false;
		}
		string host = uri.Host.ToLowerInvariant();
		return host == "download.windowsupdate.com" || host.EndsWith(".windowsupdate.com", StringComparison.Ordinal) || host.EndsWith(".microsoft.com", StringComparison.Ordinal);
	}

	/// <summary>
	/// Parse the Catalog "DownloadDialog" script block into (url, fileName, digest) tuples.
	/// </summary>
	private List<PSObject> ParseDownloadFiles(string html)
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
		List<PSObject> result = new List<PSObject>();
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
				WriteWarning("Skipping malformed download URL: " + url);
				continue;
			}
			if (!IsTrustedDownloadHost(uri))
			{
				WriteWarning("Skipping download URL on untrusted host or scheme: " + url);
				continue;
			}
			entry.TryGetValue("filename", out string fileName);
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = Path.GetFileName(uri.LocalPath);
			}
			fileName = Path.GetFileName(fileName);
			entry.TryGetValue("digest", out string digest);
			PSObject file = new PSObject();
			file.Properties.Add(new PSNoteProperty("Url", url));
			file.Properties.Add(new PSNoteProperty("FileName", fileName));
			file.Properties.Add(new PSNoteProperty("Digest", digest));
			result.Add(file);
		}
		return result;
	}

	private void CoreProcessing()
	{
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			if (!Regex.IsMatch(KBArticleID, "^KB", RegexOptions.IgnoreCase))
			{
				KBArticleID = "KB" + KBArticleID;
			}
			string searchUrl = CatalogSearchUrl + Uri.EscapeDataString(KBArticleID);
			WriteDebug(DateTime.Now.ToString() + " " + searchUrl);
			string searchHtml = WUToolsObj.InvokeRestMethod(searchUrl, WUTools.HttpWebRequestMethod.GET, null, null);
			MatchCollection rows = RowRegex.Matches(searchHtml);
			int count = rows.Count;
			WriteVerbose("Found [" + count + "] Updates in Microsoft Update Catalog");
			int num = 0;
			ProgressRecord progressRecord = new ProgressRecord(1, "Choose updates for " + text, "[" + num + "/" + count + "]");
			Collection<PSObject> collection = new Collection<PSObject>();
			HashSet<string> wanted = UpdateID != null && UpdateID.Length > 0 ? new HashSet<string>(UpdateID, StringComparer.OrdinalIgnoreCase) : null;
			foreach (Match row in rows)
			{
				string rowHtml = row.Groups["row"].Value;
				string identity = row.Groups["id"].Value;
				if (wanted != null && !wanted.Contains(identity))
				{
					num++;
					continue;
				}
				Match titleMatch = TitleRegex.Match(rowHtml);
				string title = titleMatch.Success ? Clean(titleMatch.Groups["v"].Value) : identity;
				Match sizeMatch = SizeRegex.Match(rowHtml);
				string size = sizeMatch.Success ? Clean(sizeMatch.Groups["v"].Value) : string.Empty;
				Match originalSizeMatch = OriginalSizeRegex.Match(rowHtml);
				string originalSize = originalSizeMatch.Success ? Clean(originalSizeMatch.Groups["v"].Value) : string.Empty;
				PSObject pSObject = new PSObject();
				pSObject.Properties.Add(new PSNoteProperty("X", 1));
				pSObject.Properties.Add(new PSNoteProperty("Identity", identity));
				pSObject.Properties.Add(new PSNoteProperty("Title", title));
				pSObject.Properties.Add(new PSNoteProperty("Product", Cell(rowHtml, 2)));
				pSObject.Properties.Add(new PSNoteProperty("Classification", Cell(rowHtml, 3)));
				pSObject.Properties.Add(new PSNoteProperty("LastUpdated", Cell(rowHtml, 4)));
				pSObject.Properties.Add(new PSNoteProperty("Version", Cell(rowHtml, 5)));
				pSObject.Properties.Add(new PSNoteProperty("Size", size));
				pSObject.Properties.Add(new PSNoteProperty("OriginalSize", originalSize));
				progressRecord.StatusDescription = "[" + num + "/" + count + "] " + title + " " + size;
				progressRecord.PercentComplete = count == 0 ? 100 : num * 100 / count;
				WriteProgress(progressRecord);
				num++;
				WriteDebug(DateTime.Now.ToString() + " Show update to accept: " + title);
				bool accepted = (bool)AcceptAll || ShouldProcess(text, "(" + DateTime.Now.ToString() + ") " + title + " [" + size + "]");
				if (accepted)
				{
					pSObject.Properties.Add(new PSNoteProperty("ChooseResult", "Accepted"));
					pSObject.Properties.Add(new PSNoteProperty("Status", "A"));
					pSObject.Properties.Add(new PSNoteProperty("Result", "Accepted"));
					Dictionary<string, string> body = new Dictionary<string, string>
					{
						{ "updateIDs", "[{\"uidInfo\":\"" + identity + "\",\"updateID\":\"" + identity + "\",\"size\":0}]" }
					};
					WriteDebug(DateTime.Now.ToString() + " " + body["updateIDs"]);
					string dialogHtml = WUToolsObj.InvokeRestMethod(CatalogDownloadDialogUrl, WUTools.HttpWebRequestMethod.POST, body, null);
					List<PSObject> files = ParseDownloadFiles(dialogHtml);
					if (files.Count == 0)
					{
						WriteError(new ErrorRecord(new Exception(title + ": the Catalog returned no downloadable files."), "NoDownloadUrl", ErrorCategory.ObjectNotFound, identity));
						pSObject.Properties["Result"].Value = "Error";
					}
					pSObject.Properties.Add(new PSNoteProperty("Files", files.ToArray()));
					pSObject.Properties.Add(new PSNoteProperty("DownloadUrl", files.Count > 0 ? files[0].Properties["Url"].Value : null));
					pSObject.Properties.Add(new PSNoteProperty("FileName", files.Count > 0 ? files[0].Properties["FileName"].Value : null));
					foreach (PSObject file in files)
					{
						WriteDebug(DateTime.Now.ToString() + " " + file.Properties["Url"].Value);
					}
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Rejected");
					pSObject.Properties.Add(new PSNoteProperty("ChooseResult", "Rejected"));
					pSObject.Properties.Add(new PSNoteProperty("Status", "R"));
					pSObject.Properties.Add(new PSNoteProperty("Result", "Rejected"));
					pSObject.Properties.Add(new PSNoteProperty("Files", new PSObject[0]));
					pSObject.Properties.Add(new PSNoteProperty("DownloadUrl", null));
					pSObject.Properties.Add(new PSNoteProperty("FileName", null));
				}
				pSObject.TypeNames.Clear();
				pSObject.TypeNames.Add("PSWindowsUpdateNext.OfflineMSU");
				collection.Add(pSObject);
			}
			List<PSObject> acceptedUpdates = collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Accepted").ToList();
			WriteObject(collection, enumerateCollection: true);
			WriteVerbose("Accepted [" + acceptedUpdates.Count + "] Updates ready to Download");
			int downloadIndex = 0;
			ProgressRecord progressRecord2 = new ProgressRecord(1, "Download updates for " + text, "[" + downloadIndex + "/" + acceptedUpdates.Count + "]");
			foreach (PSObject item2 in acceptedUpdates)
			{
				item2.Properties["X"].Value = 2;
				string title = item2.Properties["Title"].Value.ToString();
				string size = item2.Properties["Size"].Value.ToString();
				progressRecord2.StatusDescription = "[" + downloadIndex + "/" + acceptedUpdates.Count + "] " + title + " " + size;
				progressRecord2.PercentComplete = downloadIndex * 100 / acceptedUpdates.Count;
				WriteProgress(progressRecord2);
				downloadIndex++;
				if (!Directory.Exists(Destination))
				{
					Directory.CreateDirectory(Destination);
					WriteDebug(DateTime.Now.ToString() + " Creating destination: " + Destination);
				}
				string status = item2.Properties["Status"].Value.ToString();
				string downloadResult = "Downloaded";
				foreach (PSObject file in (PSObject[])item2.Properties["Files"].Value)
				{
					string source = file.Properties["Url"].Value.ToString();
					string target = Path.Combine(Destination, file.Properties["FileName"].Value.ToString());
					WriteDebug(DateTime.Now.ToString() + " " + target);
					if (File.Exists(target))
					{
						WriteDebug(DateTime.Now.ToString() + " File exists and will be overwritten: " + target);
					}
					string bitsResult = WUBitsObj.StartBitsTransfer(source, target);
					WriteDebug(DateTime.Now.ToString() + " " + bitsResult);
					if (bitsResult == "CANCELED")
					{
						downloadResult = "Canceled";
						break;
					}
					if (bitsResult != "ACKNOWLEDGED")
					{
						downloadResult = "Error";
						break;
					}
					string expectedDigest = file.Properties["Digest"].Value as string;
					if (!string.IsNullOrEmpty(expectedDigest))
					{
						string actualDigest = WUFileVerifier.Sha1Base64(target);
						if (!string.Equals(actualDigest, expectedDigest, StringComparison.Ordinal))
						{
							WriteError(new ErrorRecord(new Exception(target + ": SHA-1 digest mismatch (expected " + expectedDigest + ", got " + actualDigest + "). File deleted."), "DigestMismatch", ErrorCategory.InvalidData, target));
							File.Delete(target);
							downloadResult = "Error";
							break;
						}
						WriteVerbose(Path.GetFileName(target) + ": digest OK");
					}
					int trust = WUFileVerifier.VerifyAuthenticode(target);
					if (trust != 0)
					{
						string message = target + ": Authenticode verification failed (0x" + trust.ToString("X8") + ").";
						if ((bool)SkipSignatureCheck)
						{
							WriteWarning(message + " Kept because -SkipSignatureCheck was given.");
						}
						else
						{
							WriteError(new ErrorRecord(new Exception(message + " File deleted."), "SignatureInvalid", ErrorCategory.SecurityError, target));
							File.Delete(target);
							downloadResult = "Error";
							break;
						}
					}
					else
					{
						WriteVerbose(Path.GetFileName(target) + ": Authenticode signature OK");
					}
					file.Properties.Add(new PSNoteProperty("Path", target));
					file.Properties.Add(new PSNoteProperty("Sha256", WUFileVerifier.Sha256Hex(target)));
				}
				string statusSuffix = downloadResult == "Downloaded" ? "D" : (downloadResult == "Error" ? "E" : "C");
				item2.Properties.Add(new PSNoteProperty("DownloadResult", downloadResult));
				item2.Properties["Status"].Value = status + statusSuffix;
				item2.Properties["Result"].Value = downloadResult;
			}
			List<PSObject> downloaded = collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Downloaded").ToList();
			WriteObject(downloaded, enumerateCollection: true);
			WriteVerbose("Downloaded [" + downloaded.Count + "] Updates");
		}
	}

	/// <summary>
	/// Process
	/// </summary>
	protected override void ProcessRecord()
	{
		bool flag = false;
		if (Credential != null)
		{
			string userName = Credential.GetNetworkCredential().UserName;
			string domain = Credential.GetNetworkCredential().Domain;
			string password = Credential.GetNetworkCredential().Password;
			WriteDebug(DateTime.Now.ToString() + " UserName: " + userName + "; Domain: " + domain + "; Password: *****");
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			string text = "";
			if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				text = "RunAs";
			}
			WriteDebug(DateTime.Now.ToString() + " Before User: " + WindowsIdentity.GetCurrent().Name + " " + text);
			WUImpersonator.LogonSessionType logonType = WUImpersonator.LogonSessionType.Interactive;
			WUImpersonator.LogonProvider logonProvider = WUImpersonator.LogonProvider.Default;
			if (!WUToolsObj.IsLocalHost(ComputerName[0]))
			{
				logonType = WUImpersonator.LogonSessionType.NewCredentials;
				logonProvider = WUImpersonator.LogonProvider.WinNT50;
			}
			WUImpersonator.Run(userName, domain, password, logonType, logonProvider, () =>
			{
				windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				text = "";
				if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					text = "RunAs";
					WriteDebug(DateTime.Now.ToString() + " Impersonated User (" + logonType.ToString() + "|" + logonProvider.ToString() + "): " + WindowsIdentity.GetCurrent().Name + " " + text);
					try
					{
						CoreProcessing();
						flag = false;
					}
					catch (Exception ex)
					{
						WriteDebug(DateTime.Now.ToString() + " Something goes wrong: " + ex.Message);
						flag = true;
					}
				}
				else
				{
					text = "Can't RunAs";
					WriteDebug(DateTime.Now.ToString() + " Impersonated User (" + logonType.ToString() + "|" + logonProvider.ToString() + "): " + WindowsIdentity.GetCurrent().Name + " " + text);
					flag = true;
				}
				WriteDebug(DateTime.Now.ToString() + " Leaving impersonated session");
			});
			windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			text = "";
			if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				text = "RunAs";
			}
			WriteDebug(DateTime.Now.ToString() + " After User: " + WindowsIdentity.GetCurrent().Name + " " + text);
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			CoreProcessing();
		}
	}

	/// <summary>
	/// End
	/// </summary>
	protected override void EndProcessing()
	{
		CmdletEnd = DateTime.Now;
		PSObject pSObject = new PSObject();
		pSObject.Properties.Add(new PSNoteProperty("CmdletStart", CmdletStart));
		pSObject.Properties.Add(new PSNoteProperty("CmdletEnd", CmdletEnd));
		pSObject.Properties.Add(new PSNoteProperty("CmdletLine", MyInvocation.Line));
		if ((bool)SendReport)
		{
			WriteDebug(DateTime.Now.ToString() + " Send report");
			if (!PSWUSettings.ContainsKey("Properties"))
			{
				PSWUSettings.Add("Properties", "*");
			}
			PSObject pSObject2 = WUToolsObj.SendMail(PSWUSettings, OutputObj, pSObject);
			if (pSObject2.Properties.Match("ErrorRecord").Count == 1)
			{
				WriteError((ErrorRecord)pSObject2.Properties["ErrorRecord"].Value);
			}
		}
		WriteDebug(DateTime.Now.ToString() + " CmdletEnd");
	}

	/// <summary>
	/// Stop
	/// </summary>
	protected override void StopProcessing()
	{
		base.StopProcessing();
	}
}
