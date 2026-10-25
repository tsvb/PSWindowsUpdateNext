using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.Text.RegularExpressions;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get list of updates history.</para>
/// <para type="description">Use function Get-WUHistory to get list of installed updates on specific machine.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Get Windows Update history.</para>
///
///  Get-WUHistory
///
///  <para>ComputerName Operationname  Date                KB        Title</para>
///  <para>------------ -------------  ----                --        -----</para>
///  <para>MG-PC        Installation   30.08.2017 12:03:53 KB2267602 Definition Update for Windows Defender - KB2267602(Defini...</para>
///  <para>MG-PC        Installation   29.08.2017 11:49:50 KB3186568 Microsoft .NET Framework 4.7 for Windows 10 Version 1607 a...</para>
///  <para>MG-PC        Installation   29.08.2017 11:30:37 KB4035631 2017-08 Update for Windows Server 2016 for x64-based Syste...</para>
///  <para>MG-PC        Installation   29.08.2017 11:21:12 KB890830  Windows Malicious Software Removal Tool for Windows 8, 8.1...</para>
///  <para>MG-PC        Installation   29.08.2017 07:53:36 KB2267602 Definition Update for Windows Defender - KB2267602 (Defini...</para>
///  <para>MG-PC        Installation   27.08.2017 07:53:39 KB2267602 Definition Update for Windows Defender - KB2267602 (Defini...</para>
///  <para>MG-PC        Installation   25.08.2017 07:54:38 KB2267602 Definition Update for Windows Defender - KB2267602 (Defini...</para>
///  <para>MG-PC        Installation   23.08.2017 13:01:26 KB2267602 Definition Update for Windows Defender - KB2267602 (Defini...</para>
///  <para>MG-PC        Installation   23.08.2017 12:45:45 KB4023307 Security Update for Microsoft Silverlight (KB4023307)</para>
///  <para>MG-PC        Installation   23.08.2017 07:53:56 KB2267602 Definition Update for Windows Defender - KB2267602 (Defini...</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Get Windows Update Agent history for last 24h.</para>
///
///  Get-WUHistory -MaxDate (Get-Date).AddDays(-1)
///
///  <para>ComputerName Operationname  Date                KB        Title</para>
///  <para>------------ -------------  ----                --        -----</para>
///  <para>MG-PC        Installation   30.08.2017 12:03:53 KB2267602 Definition Update for Windows Defender - KB2267602(Defini...</para>
///  </code>
///  </example>
[Cmdlet("Get", "WUHistory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
[OutputType(new Type[] { typeof(History) })]
public class GetWUHistory : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
	public string[] ComputerName { get; set; }

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
	public SwitchParameter SendReport { get; set; }

	/// <summary>
	/// <para type="description">Required parameter for -SendReport.</para>
	/// <para type="description">Passes the parameters (as hashtable) necessary to send the report: 
	/// @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25];[Subject="Alternative Subject"];[Properties="Alternative object properties"];[Style="Table|List"]}</para>
	/// <para type="description">Send parameters can also be saved to a PSWUSettings.xml file in ModuleBase path: 
	/// Export-Clixml @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25]}"</para>
	/// </summary>
	[Parameter]
	public Hashtable PSWUSettings
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
	/// <para type="description">Filter results by date.</para>
	/// </summary>
	[Parameter]
	public DateTime MaxDate { get; set; }

	/// <summary>
	/// <para type="description">Last X history entry.</para>
	/// </summary>
	[Parameter]
	[ValidateIntGt0]
	public int Last { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static UpdateSession UpdateSessionObj { get; set; }

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
		OutputObj = new Collection<PSObject>();
		if ((bool)SendReport)
		{
			WriteDebug(DateTime.Now.ToString() + " Test smtp settings");
			if (!PSWUSettings.ContainsKey("Properties"))
			{
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "OperationName", "Result", "Date", "Title" });
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

	private void CoreProcessing()
	{
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text + ": Connecting...");
			try
			{
				PSObject pSWUModule = WUToolsObj.GetPSWUModule(text);
				WriteDebug(DateTime.Now.ToString() + " Module version: " + pSWUModule.Properties["Version"].Value);
				WriteDebug(DateTime.Now.ToString() + " Dll version: " + pSWUModule.Properties["PSWUDllVersion"].Value);
			}
			catch
			{
			}
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Get Windows Update History"))
			{
				continue;
			}
			WUAPIObject wUApiUpdateSessionObj = WUToolsObj.GetWUApiUpdateSessionObj(text);
			WriteDebug(DateTime.Now.ToString() + " UpdateSessionObj mode: " + wUApiUpdateSessionObj.Mode);
			if (wUApiUpdateSessionObj.Status)
			{
				UpdateSessionObj = (UpdateSession)wUApiUpdateSessionObj.Object;
				IUpdateSearcher updateSearcher = UpdateSessionObj.CreateUpdateSearcher();
				Collection<PSObject> collection = new Collection<PSObject>();
				int num = 0;
				bool flag = false;
				do
				{
					try
					{
						IUpdateHistoryEntryCollection updateHistoryEntryCollection = updateSearcher.QueryHistory(num, 1);
						foreach (IUpdateHistoryEntry item in updateHistoryEntryCollection)
						{
							if (item.Date.Year > 1900 && item.Date > MaxDate)
							{
								PSObject pSObject = new PSObject(item);
								string value = "";
								switch ((int)item.Operation)
								{
								case 1:
									value = "Installation";
									break;
								case 2:
									value = "Uninstallation";
									break;
								}
								string value2 = "";
								switch ((int)item.ResultCode)
								{
								case 0:
									value2 = "NotStarted";
									break;
								case 1:
									value2 = "InProgress";
									break;
								case 2:
									value2 = "Succeeded";
									break;
								case 3:
									value2 = "SucceededWithErrors";
									break;
								case 4:
									value2 = "Failed";
									break;
								case 5:
									value2 = "Aborted ";
									break;
								}
								string value3 = "";
								Match match = Regex.Match(item.Title, "KB(\\d+)");
								if (match.Success)
								{
									value3 = match.Value;
								}
								pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
								pSObject.Properties.Add(new PSNoteProperty("OperationName", value));
								pSObject.Properties.Add(new PSNoteProperty("Date", item.Date.ToLocalTime()));
								pSObject.Properties.Add(new PSNoteProperty("Title", item.Title));
								pSObject.Properties.Add(new PSNoteProperty("Result", value2));
								pSObject.Properties.Add(new PSNoteProperty("KB", value3));
								pSObject.TypeNames.Clear();
								pSObject.TypeNames.Add("PSWindowsUpdateNext.History");
								collection.Add(pSObject);
								WriteObject(pSObject, enumerateCollection: true);
								flag = true;
							}
							else
							{
								flag = false;
							}
						}
						if (updateHistoryEntryCollection.Count == 0)
						{
							flag = false;
						}
					}
					catch
					{
						flag = false;
						break;
					}
					num++;
					if (Last > 0 && num >= Last)
					{
						flag = false;
					}
				}
				while (flag);
				OutputObj = new Collection<PSObject>(OutputObj.Concat(collection).ToList());
			}
			else if ((bool)Debuger)
			{
				ErrorRecord errorRecord = new ErrorRecord(wUApiUpdateSessionObj.Exception, "Debug", ErrorCategory.CloseError, null);
				WriteError(errorRecord);
			}
			else
			{
				ErrorRecord error = wUApiUpdateSessionObj.Error;
				WriteError(error);
			}
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
			WriteDebug(DateTime.Now.ToString() + " UserName: " + userName + "; Domain: " + domain + "; Password: " + "*****");
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
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "OperationName", "Result", "Date", "Title" });
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
