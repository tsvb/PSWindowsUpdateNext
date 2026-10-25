using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Win32;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get Windows Update reboot status.</para>
/// <para type="description">Use Get-WURebootStatus cmdlet to check if reboot is needed.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Check if restart is necessary. If yes, ask to do this or don't.</para>
///
///  Get-WURebootStatus 
///
///  <para>Reboot is required. Do it now ? [Y/N] (default is 'N')</para>
///  <para>ComputerName RebootRequired RebootScheduled</para>
///  <para>------------ -------------- ---------------</para>
///  <para>MG-PC        True</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Check if restart is necessary. If yes, then shedule it.</para>
///
///  Get-WURebootStatus -ScheduleReboot (Get-Date -Hour 18 -Minute 0 -Second 0)
///
///  <para>ComputerName RebootRequired RebootScheduled</para>
///  <para>------------ -------------- ---------------</para>
///  <para>MG-PC        True           31.08.2017 18:00:00</para>
///  </code>
///  </example>
[Cmdlet("Get", "WURebootStatus", SupportsShouldProcess = true, DefaultParameterSetName = "ManualReboot", ConfirmImpact = ConfirmImpact.Medium)]
[OutputType(new Type[] { typeof(RebootStatus) })]
public class GetWURebootStatus : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter]
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
	/// \r\n@{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25];[Subject="Alternative Subject"];[Properties="Alternative object properties"];[Style="Table|List"]}</para>
	/// <para type="description">Send parameters can also be saved to a PSWUSettings.xml file in ModuleBase path: 
	/// \r\nExport-Clixml @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";[Port=25]}"</para>
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
	/// <para type="description">Return true/false only.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "AutoReboot")]
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ScheduleReboot")]
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ManualReboot")]
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "CancelReboot")]
	public SwitchParameter Silent { get; set; }

	/// <summary>
	/// <para type="description">Do not ask for reboot if it needed, but do it now.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "AutoReboot")]
	public SwitchParameter AutoReboot { get; set; }

	/// <summary>
	/// <para type="description">Specify schedule time for reboot.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "ScheduleReboot")]
	public DateTime ScheduleReboot { get; set; }

	/// <summary>
	/// <para type="description">Cancel scheduled reboot.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "CancelReboot")]
	public SwitchParameter CancelReboot { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static SystemInformation SystemInfoObj { get; set; }

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
				PSWUSettings.Add("Properties", new string[2] { "ComputerName", "RebootRequired" });
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
			WriteDebug(DateTime.Now.ToString() + " Connect to local machine: " + ComputerName[0]);
		}
	}

	private void CoreProcessing()
	{
		string invocationName = MyInvocation.InvocationName;
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
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Get Windows Update reboot status"))
			{
				continue;
			}
			bool flag = false;
			if (WUToolsObj.IsLocalHost(text))
			{
				WUAPIObject wUApiSystemInfoObj = WUToolsObj.GetWUApiSystemInfoObj(text);
				WriteDebug(DateTime.Now.ToString() + " SystemInfoObj mode: " + wUApiSystemInfoObj.Mode);
				if (!wUApiSystemInfoObj.Status)
				{
					if ((bool)Debuger)
					{
						ErrorRecord errorRecord = new ErrorRecord(wUApiSystemInfoObj.Exception, "Debug", ErrorCategory.CloseError, null);
						WriteError(errorRecord);
					}
					else
					{
						ErrorRecord error = wUApiSystemInfoObj.Error;
						WriteError(error);
					}
					continue;
				}
				SystemInfoObj = (SystemInformation)wUApiSystemInfoObj.Object;
				flag = SystemInfoObj.RebootRequired;
			}
			else
			{
				RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, text);
				RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\");
				string[] subKeyNames = registryKey2.GetSubKeyNames();
				flag = subKeyNames.Contains("RebootRequired");
			}
			PSObject pSObject = new PSObject();
			pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
			pSObject.Properties.Add(new PSNoteProperty("RebootRequired", flag));
			if (ScheduleReboot != DateTime.MinValue)
			{
				pSObject.Properties.Add(new PSNoteProperty("RebootScheduled", ScheduleReboot));
			}
			else
			{
				pSObject.Properties.Add(new PSNoteProperty("RebootScheduled", null));
			}
			if ((bool)Silent)
			{
				WriteObject(flag, enumerateCollection: true);
			}
			else
			{
				WriteObject(pSObject, enumerateCollection: true);
			}
			OutputObj.Add(pSObject);
			if (flag)
			{
				if (ScheduleReboot != DateTime.MinValue)
				{
					WriteDebug(DateTime.Now.ToString() + " " + text + ": Schedule Reboot " + ScheduleReboot.ToString());
					WUToolsObj.CancelReboot(text);
					string text2 = WUToolsObj.ScheduleReboot(text, ScheduleReboot);
					WriteVerbose(text2);
				}
				else if ((bool)AutoReboot)
				{
					WriteDebug(DateTime.Now.ToString() + " " + text + ": Auto Reboot");
					WUToolsObj.CancelReboot(text);
					string text3 = WUToolsObj.RunReboot(text);
					WriteVerbose(text3);
				}
				else if (!Silent)
				{
					Host.UI.WriteLine(ConsoleColor.White, Host.UI.RawUI.BackgroundColor, "Reboot is required. Do it now? [Y / N] (default is 'N')");
					string text4 = Host.UI.ReadLine();
					if (text4.ToUpper() == "Y")
					{
						WriteDebug(DateTime.Now.ToString() + " " + text + ": Manually Reboot");
						WUToolsObj.CancelReboot(text);
						string text5 = WUToolsObj.RunReboot(text);
						WriteVerbose(text5);
					}
				}
			}
			if ((bool)CancelReboot)
			{
				WriteDebug(DateTime.Now.ToString() + " " + text + ": Cancel Reboot");
				string text6 = WUToolsObj.CancelReboot(text);
				WriteVerbose(text6);
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
				PSWUSettings.Add("Properties", new string[2] { "ComputerName", "RebootRequired" });
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
