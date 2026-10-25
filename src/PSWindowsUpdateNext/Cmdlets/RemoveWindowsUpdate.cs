using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Uninstall update.</para>
/// <para type="description">Use Remove-WindowsUpdate to uninstall update.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Try to uninstall update with specific KBArticleID = KB958830.</para>
///
///  Get-WUUninstall -KBArticleID KB958830
///
///  </code>
///  </example>
[Cmdlet("Remove", "WindowsUpdate", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
public class RemoveWindowsUpdate : PSCmdlet
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
	/// <para type="description">Specify schedule time job.</para>
	/// </summary>
	[Parameter]
	public DateTime ScheduleJob { get; set; }

	/// <summary>
	/// <para type="description">KBArticleID that will be uninstalled.</para>
	/// </summary>
	[Parameter(ParameterSetName = "WUSAMode", Mandatory = true)]
	[Alias(new string[] { "HotFixID" })]
	public string KBArticleID { get; set; }

	/// <summary>
	/// <para type="description">Update ID that will be uninstalled.</para>
	/// </summary>
	[Parameter(ParameterSetName = "WUApiMode", Mandatory = true)]
	public string UpdateID { get; set; }

	/// <summary>
	/// <para type="description">Do not ask for reboot if it needed.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AutoReboot { get; set; }

	/// <summary>
	/// <para type="description">Do not ask for reboot if it needed, but do not reboot automaticaly.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IgnoreReboot { get; set; }

	/// <summary>
	/// <para type="description">Specify time when system will be rebooted.</para>
	/// </summary>
	[Parameter]
	[ValidateDateTime]
	public DateTime ScheduleReboot { get; set; }

	/// <summary>
	/// <para type="description">Don't use automatic reboot.</para>
	/// </summary>
	[Parameter]
	private SwitchParameter NoRestart { get; set; }

	/// <summary>
	/// <para type="description">Wse wusa.exe instead of WU Api.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter WUSAMode { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static UpdateSession UpdateSessionObj { get; set; }

	private static IUpdateSearcher SearcherObj { get; set; }

	private static UpdateInstaller InstallerObj { get; set; }

	private static bool NeedsReboot { get; set; }

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
			ErrorRecord errorRecord = new ErrorRecord(new Exception("To perform operations you must run an elevated Windows PowerShell console."), "AccessDenied", ErrorCategory.PermissionDenied, null);
			ThrowTerminatingError(errorRecord);
		}
		WUToolsObj = new WUTools();
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

	private void CoreProcessing()
	{
		string invocationName = MyInvocation.InvocationName;
		string text = string.Empty;
		if (MyInvocation.BoundParameters.ContainsKey("KBArticleID"))
		{
			text = KBArticleID.Replace("KB", "");
		}
		string[] computerName = ComputerName;
		foreach (string text2 in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text2 + ": Connecting...");
			try
			{
				PSObject pSWUModule = WUToolsObj.GetPSWUModule(text2);
				WriteDebug(DateTime.Now.ToString() + " Module version: " + pSWUModule.Properties["Version"].Value);
				WriteDebug(DateTime.Now.ToString() + " Dll version: " + pSWUModule.Properties["PSWUDllVersion"].Value);
			}
			catch
			{
			}
			WriteVerbose("AA");
			if (ScheduleJob == DateTime.MinValue && WUToolsObj.IsLocalHost(text2))
			{
				if ((bool)WUSAMode)
				{
					WriteVerbose(text2 + ": Try to uninstall KB" + KBArticleID);
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					processStartInfo.FileName = "cmd";
					if (WUToolsObj.IsLocalHost(text2))
					{
						processStartInfo.Arguments = "/C wusa.exe /uninstall /KB:" + text + " /quiet /norestart";
					}
					else
					{
						processStartInfo.Arguments = "/C winrs.exe -r:" + text2 + " wusa.exe /uninstall /KB:" + text + " /quiet /norestart";
					}
					WriteDebug("Process: " + processStartInfo.FileName + " " + processStartInfo.Arguments);
					if (ShouldProcess(text2, "(" + DateTime.Now.ToString() + ") Remove (wusa.exe) Windows Update: " + KBArticleID))
					{
						Process.Start(processStartInfo);
						WriteDebug("Process started.");
					}
					continue;
				}
				WUAPIObject wUApiUpdateSessionObj = WUToolsObj.GetWUApiUpdateSessionObj(text2);
				WriteDebug(DateTime.Now.ToString() + " UpdateSessionObj mode: " + wUApiUpdateSessionObj.Mode);
				if (wUApiUpdateSessionObj.Status)
				{
					UpdateSessionObj = (UpdateSession)wUApiUpdateSessionObj.Object;
					SearcherObj = UpdateSessionObj.CreateUpdateSearcher();
					string text3 = "IsInstalled = 1";
					if (UpdateID != null)
					{
						text3 = text3 + " and UpdateID='" + UpdateID + "'";
					}
					WriteDebug(DateTime.Now.ToString() + " Pre search criteria: " + text3);
					ISearchResult searchResult;
					try
					{
						searchResult = SearcherObj.Search(text3);
					}
					catch (COMException ex)
					{
						WUApiCode wUApiCodeDetails = WUToolsObj.GetWUApiCodeDetails(ex.ErrorCode);
						if (wUApiCodeDetails != null)
						{
							int codeType = wUApiCodeDetails.CodeType;
							int num = codeType;
							if (num == 2)
							{
								WriteError(new ErrorRecord(new Exception(wUApiCodeDetails.Description), wUApiCodeDetails.HResult, ErrorCategory.CloseError, null));
							}
						}
						else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
						{
							ErrorRecord errorRecord = new ErrorRecord(ex, "Debug: " + ex.ErrorCode, ErrorCategory.CloseError, null);
							ThrowTerminatingError(errorRecord);
						}
						else
						{
							WriteError(new ErrorRecord(new Exception(text2 + ": Unknown failure. Try debug."), "Unknown", ErrorCategory.CloseError, null));
						}
						continue;
					}
					int count = searchResult.Updates.Count;
					WriteDebug("Found [" + count + "] Updates in pre search criteria");
					if (count == 0)
					{
						continue;
					}
					UpdateCollection updateCollection = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
					UpdateCollection updateCollection2 = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
					updateCollection2 = searchResult.Updates;
					if (KBArticleID != null)
					{
						foreach (IUpdate item in updateCollection2)
						{
							bool flag = false;
							foreach (string kBArticleID in item.KBArticleIDs)
							{
								if (Regex.IsMatch(KBArticleID, kBArticleID.ToString(), RegexOptions.IgnoreCase))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								updateCollection.Add(item);
								break;
							}
						}
					}
					else
					{
						updateCollection = updateCollection2;
					}
					int count2 = updateCollection.Count;
					WriteDebug("Found [" + count2 + "] Updates in post search criteria");
					if (count2 == 0 || !ShouldProcess(text2, "(" + DateTime.Now.ToString() + ") Remove (WUApi) Windows Update: " + updateCollection[0].Title))
					{
						continue;
					}
					IUpdateInstaller updateInstaller = UpdateSessionObj.CreateUpdateInstaller();
					updateInstaller.Updates = updateCollection;
					IInstallationResult installationResult = null;
					try
					{
						installationResult = updateInstaller.Uninstall();
						WriteObject(installationResult);
						string text5 = "";
						switch ((int)installationResult.ResultCode)
						{
						case 0:
							text5 = "NotStarted";
							break;
						case 1:
							text5 = "InProgress";
							break;
						case 2:
							text5 = "Uninstalled";
							break;
						case 3:
							text5 = "UninstalledWithErrors";
							break;
						case 4:
							text5 = "Failed";
							break;
						case 5:
							text5 = "Aborted";
							break;
						}
						WriteDebug(DateTime.Now.ToString() + " Uninstallation status: " + text5);
						if (installationResult.RebootRequired)
						{
							NeedsReboot = installationResult.RebootRequired;
							WriteDebug(DateTime.Now.ToString() + " Reboot is required");
						}
					}
					catch (Exception ex2)
					{
						if (Regex.IsMatch(ex2.Message, "HRESULT: 0x80240028"))
						{
							WriteWarning(ex2.Message + ". The update could not be uninstalled because the request did not originate from a WSUS server.");
						}
						if (Regex.IsMatch(ex2.Message, "HRESULT: 0x80240044"))
						{
							WriteWarning(ex2.Message + ". Your security policy don't allow a non-administator identity to perform this task.");
						}
					}
				}
				else if ((bool)Debuger)
				{
					ErrorRecord errorRecord2 = new ErrorRecord(wUApiUpdateSessionObj.Exception, "Debug", ErrorCategory.CloseError, null);
					WriteError(errorRecord2);
				}
				else
				{
					ErrorRecord error = wUApiUpdateSessionObj.Error;
					WriteError(error);
				}
				continue;
			}
			ErrorRecord errorRecord3 = WUToolsObj.CheckPSWUModule(text2);
			if (errorRecord3 != null)
			{
				WriteError(errorRecord3);
				continue;
			}
			string text6 = "";
			text6 = ((!Debuger) ? "Remove-WindowsUpdate" : "$DebugPreference = 'Continue'; Remove-WindowsUpdate");
			if (KBArticleID != null)
			{
				text6 = text6 + " -KBArticleID " + KBArticleID;
			}
			if (UpdateID != null)
			{
				text6 = text6 + " -UpdateID '" + UpdateID + "'";
			}
			if ((bool)AutoReboot)
			{
				text6 += " -AutoReboot";
			}
			if (ScheduleReboot != DateTime.MinValue)
			{
				text6 = text6 + " -ScheduleReboot " + ScheduleReboot.ToString();
			}
			if ((bool)IgnoreReboot)
			{
				text6 += " -IgnoreReboot";
			}
			text6 += " -Verbose -Confirm:$false *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log";
			InvokeWUJob invokeWUJob = new InvokeWUJob();
			invokeWUJob.ComputerName = new string[1] { text2 };
			if (Credential != null)
			{
				invokeWUJob.Credential = Credential;
			}
			invokeWUJob.Script = text6;
			if (ScheduleJob != DateTime.MinValue)
			{
				invokeWUJob.TriggerDate = ScheduleJob;
				WriteVerbose("Invoke-WUJob: " + text2 + " (" + ScheduleJob.ToString() + ") " + text6);
			}
			else
			{
				invokeWUJob.RunNow = true;
				WriteVerbose("Invoke-WUJob: " + text2 + " (Now) " + text6);
			}
			WriteVerbose("powershell.exe -Command \"" + text6 + "\"");
			IEnumerable enumerable = invokeWUJob.Invoke();
			foreach (object item2 in enumerable)
			{
				WriteObject(item2);
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
				PSWUSettings.Add("Properties", "*");
			}
			PSObject pSObject2 = WUToolsObj.SendMail(PSWUSettings, OutputObj, pSObject);
			if (pSObject2.Properties.Match("ErrorRecord").Count == 1)
			{
				WriteError((ErrorRecord)pSObject2.Properties["ErrorRecord"].Value);
			}
		}
		if (NeedsReboot)
		{
			if (ScheduleReboot != DateTime.MinValue)
			{
				WriteDebug(DateTime.Now.ToString() + " Schedule Reboot " + ScheduleReboot.ToString());
				string text = WUToolsObj.ScheduleReboot("localhost", ScheduleReboot);
				WriteVerbose(text);
			}
			else if ((bool)AutoReboot)
			{
				WriteDebug(DateTime.Now.ToString() + " Auto Reboot");
				string text2 = WUToolsObj.RunReboot("localhost");
				WriteVerbose(text2);
			}
			else if ((bool)IgnoreReboot)
			{
				Host.UI.WriteLine(ConsoleColor.White, Host.UI.RawUI.BackgroundColor, "Reboot is required, but do it manually.");
			}
			else
			{
				Host.UI.WriteLine(ConsoleColor.White, Host.UI.RawUI.BackgroundColor, "Reboot is required. Do it now? [Y / N] (default is 'N')");
				string text3 = Host.UI.ReadLine();
				if (text3.ToUpper() == "Y")
				{
					WriteDebug(DateTime.Now.ToString() + " Manually Reboot");
					string text4 = WUToolsObj.RunReboot("localhost");
					WriteVerbose(text4);
				}
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
