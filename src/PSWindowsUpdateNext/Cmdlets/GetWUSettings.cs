using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Win32;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get Windows Update Client settings.</para>
/// <para type="description">Use Get-WUSettings cmdlet to get current configuration of Windows Update Client.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Get current Windows Update Client configuration.</para>
///
///  Get-WUSettings 
///
///  <para>ComputerName                  : MG-PC</para>
///  <para>AcceptTrustedPublisherCerts   : 0</para>
///  <para>WUServer                      : https://wsus.commandlinegeeks.com</para>
///  <para>WUStatusServer                : https://wsus.commandlinegeeks.com</para>
///  <para>DetectionFrequencyEnabled     : 1</para>
///  <para>DetectionFrequency            : 2</para>
///  <para>NoAutoRebootWithLoggedOnUsers : 1</para>
///  <para>RebootRelaunchTimeoutEnabled  : 1</para>
///  <para>RebootRelaunchTimeout         : 240</para>
///  <para>IncludeRecommendedUpdates     : 0</para>
///  <para>NoAutoUpdate                  : 0</para>
///  <para>AUOptions                     : 2 - Notify before download</para>
///  <para>ScheduledInstallDay           : 0 - Every Day</para>
///  <para>ScheduledInstallTime          : 4</para>
///  <para>UseWUServer                   : 1</para>
///  </code>
///  </example>
[Cmdlet("Get", "WUSettings", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
[OutputType(new Type[] { typeof(WUSettings) })]
public class GetWUSettings : PSCmdlet
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
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

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
		Hashtable hashtable = new Hashtable();
		hashtable.Add(0, "0 - Not configured");
		hashtable.Add(1, "1 - Disabled");
		hashtable.Add(2, "2 - Notify before download");
		hashtable.Add(3, "3 - Notify before installation");
		hashtable.Add(4, "4 - Scheduled installation");
		hashtable.Add(5, "5 - Users configure");
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add(0, "0 - Every Day");
		hashtable2.Add(1, "1 - Every Sunday");
		hashtable2.Add(2, "2 - Every Monday");
		hashtable2.Add(3, "3 - Every Tuesday");
		hashtable2.Add(4, "4 - Every Wednesday");
		hashtable2.Add(5, "5 - Every Thursday");
		hashtable2.Add(6, "6 - Every Friday");
		hashtable2.Add(7, "7 - Every Saturday");
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
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Get Windows Update settings"))
			{
				continue;
			}
			RegistryKey registryKey = null;
			registryKey = ((!WUToolsObj.IsLocalHost(text)) ? RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, text) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default));
			RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\");
			RegistryKey registryKey3 = registryKey.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU\\");
			if (registryKey2 != null)
			{
				WriteVerbose("Some settings are managed by your system administrator.");
				PSObject pSObject = new PSObject();
				pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
				string[] valueNames = registryKey2.GetValueNames();
				foreach (string name in valueNames)
				{
					object value = registryKey2.GetValue(name);
					pSObject.Properties.Add(new PSNoteProperty(name, value));
				}
				string[] valueNames2 = registryKey3.GetValueNames();
				foreach (string text2 in valueNames2)
				{
					object obj2 = registryKey3.GetValue(text2);
					string text3 = text2;
					string text4 = text3;
					if (!(text4 == "AUOptions"))
					{
						if (text4 == "ScheduledInstallDay")
						{
							obj2 = hashtable2[obj2];
						}
					}
					else
					{
						obj2 = hashtable[obj2];
					}
					pSObject.Properties.Add(new PSNoteProperty(text2, obj2));
				}
				WriteObject(pSObject, enumerateCollection: true);
				OutputObj.Add(pSObject);
			}
			else
			{
				WriteVerbose("Settings not found.");
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
