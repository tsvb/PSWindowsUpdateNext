using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Security.Principal;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Reset Windows Update components.</para>
/// <para type="description">Use Reset-WUComponents cmdlet to reset all Windows Update components to default.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Reset Windows Update components to default.</para>
///
///  Reset-WUComponents -Verbose
///  Step 1: Stop Windows Update services
/// VERBOSE: Background Intelligent Transfer Service(BITS)
/// VERBOSE: Windows Update(wuauserv)
/// VERBOSE: Application Identity(appidsvc)
/// VERBOSE: Cryptographic Services(cryptsvc)
/// Step 2: Delete the qmgr*.dat files
/// Step 3: Backup softare distribution folders
/// VERBOSE: Renaming Software Distribution folder to C:\WINDOWS\SoftwareDistribution.bak5
/// VERBOSE: Renaming CatRoot  folder to C:\WINDOWS\System32\Catroot2.bak1
/// Step 4: Remove old Windows Update logs
/// VERBOSE: Deleting the C:\WINDOWS\WindowsUpdate.log files.
/// Step 5: Reset Windows Update services
/// VERBOSE: Reset BITS service
/// VERBOSE: Reset Windows Update service
/// Step 6: Reregister dll's
/// VERBOSE: regsvr32.exe / s atl.dll
/// VERBOSE: regsvr32.exe / s urlmon.dll
/// VERBOSE: regsvr32.exe / s mshtml.dll
/// VERBOSE: regsvr32.exe / s shdocvw.dll
/// VERBOSE: regsvr32.exe / s browseui.dll
/// VERBOSE: regsvr32.exe / s jscript.dll
/// VERBOSE: regsvr32.exe / s vbscript.dll
/// VERBOSE: regsvr32.exe / s scrrun.dll
/// VERBOSE: regsvr32.exe / s msxml.dll
/// VERBOSE: regsvr32.exe / s msxml3.dll
/// VERBOSE: regsvr32.exe / s msxml6.dll
/// VERBOSE: regsvr32.exe / s actxprxy.dll
/// VERBOSE: regsvr32.exe / s softpub.dll
/// VERBOSE: regsvr32.exe / s wintrust.dll
/// VERBOSE: regsvr32.exe / s dssenh.dll
/// VERBOSE: regsvr32.exe / s rsaenh.dll
/// VERBOSE: regsvr32.exe / s gpkcsp.dll
/// VERBOSE: regsvr32.exe / s sccbase.dll
/// VERBOSE: regsvr32.exe / s slbcsp.dll
/// VERBOSE: regsvr32.exe / s cryptdlg.dll
/// VERBOSE: regsvr32.exe / s oleaut32.dll
/// VERBOSE: regsvr32.exe / s ole32.dll
/// VERBOSE: regsvr32.exe / s shell32.dll
/// VERBOSE: regsvr32.exe / s initpki.dll
/// VERBOSE: regsvr32.exe / s wuapi.dll
/// VERBOSE: regsvr32.exe / s wuaueng.dll
/// VERBOSE: regsvr32.exe / s wuaueng1.dll
/// VERBOSE: regsvr32.exe / s wucltui.dll
/// VERBOSE: regsvr32.exe / s wups.dll
/// VERBOSE: regsvr32.exe / s wups2.dll
/// VERBOSE: regsvr32.exe / s wuweb.dll
/// VERBOSE: regsvr32.exe / s qmgr.dll
/// VERBOSE: regsvr32.exe / s qmgrprxy.dll
/// VERBOSE: regsvr32.exe / s wucltux.dll
/// VERBOSE: regsvr32.exe / s muweb.dll
/// VERBOSE: regsvr32.exe / s wuwebv.dll
/// Step 7: Reset WinSock
/// VERBOSE: netsh winsock reset
/// Step 8: Reset Proxy
/// VERBOSE: netsh winhttp reset proxy
/// Step 9: Start Windows Update services
/// VERBOSE: Cryptographic Services (cryptsvc)
/// VERBOSE: Application Identity (appidsvc)
/// VERBOSE: Windows Update (wuauserv)
/// VERBOSE: Background Intelligent Transfer Service (BITS)
/// Step 10: Start Windows Update services
/// VERBOSE: wuauclt /resetauthorization /detectnow
///  </code>
///  </example>
[Cmdlet("Reset", "WUComponents", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
public class ResetWUComponents : PSCmdlet
{
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
			ErrorRecord errorRecord = new ErrorRecord(new Exception("To perform operations you must run an elevated Windows PowerShell console."), "AccessDenied", ErrorCategory.PermissionDenied, null);
			ThrowTerminatingError(errorRecord);
		}
		WUToolsObj = new WUTools();
		// WUTools is a cmdlet that is never invoked, so its WriteWarning and WriteDebug do nothing
		// unless they borrow this cmdlet's runtime.
		WUToolsObj.CommandRuntime = CommandRuntime;
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
		string[] computerName = ComputerName;
		foreach (string computer in computerName)
		{
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 1: Stop Windows Update services");
			WUToolsObj.StopService(computer, "BITS");
			WriteVerbose("Background Intelligent Transfer Service (BITS)");
			WUToolsObj.StopService(computer, "wuauserv");
			WriteVerbose("Windows Update (wuauserv)");
			WUToolsObj.StopService(computer, "appidsvc");
			WriteVerbose("Application Identity (appidsvc)");
			WUToolsObj.StopService(computer, "cryptsvc");
			WriteVerbose("Cryptographic Services (cryptsvc)");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 2: Delete the qmgr*.dat files");
			string environmentVariable = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
			try
			{
				string[] files = Directory.GetFiles(environmentVariable + "\\Microsoft\\Network\\Downloader", "qmgr*.dat");
				string[] array = files;
				foreach (string text in array)
				{
					WriteVerbose("Deleting the " + text + " files.");
					File.Delete(text);
				}
			}
			catch
			{
			}
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 3: Backup softare distribution folders");
			string environmentVariable2 = Environment.GetEnvironmentVariable("SYSTEMROOT");
			string text2 = environmentVariable2 + "\\SoftwareDistribution";
			string text3 = environmentVariable2 + "\\SoftwareDistribution.bak";
			if (Directory.Exists(text2))
			{
				string text4 = text3;
				int num = 1;
				while (Directory.Exists(text4))
				{
					text4 = text3 + num;
					num++;
				}
				text3 = text4;
				Directory.Move(text2, text3);
				if (Directory.Exists(text3))
				{
					WriteVerbose("Renaming Software Distribution folder to " + text3);
				}
			}
			string text5 = environmentVariable2 + "\\System32\\Catroot2";
			string text6 = environmentVariable2 + "\\System32\\Catroot2.bak";
			if (Directory.Exists(text5))
			{
				string text7 = text6;
				int num2 = 1;
				while (Directory.Exists(text7))
				{
					text7 = text6 + num2;
					num2++;
				}
				text6 = text7;
				Directory.Move(text5, text6);
				if (Directory.Exists(text6))
				{
					WriteVerbose("Renaming CatRoot  folder to " + text6);
				}
			}
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 4: Remove old Windows Update logs");
			try
			{
				string[] files2 = Directory.GetFiles(environmentVariable2, "WindowsUpdate.log");
				string[] array2 = files2;
				foreach (string text8 in array2)
				{
					WriteVerbose("Deleting the " + text8 + " files.");
					File.Delete(text8);
				}
			}
			catch
			{
			}
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 5: Reset Windows Update services");
			WriteVerbose("Reset BITS service");
			WUToolsObj.RunProcess("sc.exe", "sdset bits D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;AU)(A;;CCLCSWRPWPDTLOCRRC;;;PU)");
			WriteVerbose("Reset Windows Update service");
			WUToolsObj.RunProcess("sc.exe", "sdset wuauserv D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;AU)(A;;CCLCSWRPWPDTLOCRRC;;;PU)");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 6: Reregister dll's");
			WriteVerbose("regsvr32.exe / s atl.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\atl.dll");
			WriteVerbose("regsvr32.exe / s urlmon.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\urlmon.dll");
			WriteVerbose("regsvr32.exe / s mshtml.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\mshtml.dll");
			WriteVerbose("regsvr32.exe / s shdocvw.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\shdocvw.dll");
			WriteVerbose("regsvr32.exe / s browseui.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\browseui.dll");
			WriteVerbose("regsvr32.exe / s jscript.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\jscript.dll");
			WriteVerbose("regsvr32.exe / s vbscript.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\vbscript.dll");
			WriteVerbose("regsvr32.exe / s scrrun.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\scrrun.dll");
			WriteVerbose("regsvr32.exe / s msxml.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\msxml.dll");
			WriteVerbose("regsvr32.exe / s msxml3.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\msxml3.dll");
			WriteVerbose("regsvr32.exe / s msxml6.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\msxml6.dll");
			WriteVerbose("regsvr32.exe / s actxprxy.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\actxprxy.dll");
			WriteVerbose("regsvr32.exe / s softpub.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\softpub.dll");
			WriteVerbose("regsvr32.exe / s wintrust.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wintrust.dll");
			WriteVerbose("regsvr32.exe / s dssenh.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\dssenh.dll");
			WriteVerbose("regsvr32.exe / s rsaenh.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\rsaenh.dll");
			WriteVerbose("regsvr32.exe / s gpkcsp.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\gpkcsp.dll");
			WriteVerbose("regsvr32.exe / s sccbase.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\sccbase.dll");
			WriteVerbose("regsvr32.exe / s slbcsp.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\slbcsp.dll");
			WriteVerbose("regsvr32.exe / s cryptdlg.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\cryptdlg.dll");
			WriteVerbose("regsvr32.exe / s oleaut32.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\oleaut32.dll");
			WriteVerbose("regsvr32.exe / s ole32.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\ole32.dll");
			WriteVerbose("regsvr32.exe / s shell32.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\shell32.dll");
			WriteVerbose("regsvr32.exe / s initpki.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\initpki.dll");
			WriteVerbose("regsvr32.exe / s wuapi.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wuapi.dll");
			WriteVerbose("regsvr32.exe / s wuaueng.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wuaueng.dll");
			WriteVerbose("regsvr32.exe / s wuaueng1.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wuaueng1.dll");
			WriteVerbose("regsvr32.exe / s wucltui.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wucltui.dll");
			WriteVerbose("regsvr32.exe / s wups.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wups.dll");
			WriteVerbose("regsvr32.exe / s wups2.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wups2.dll");
			WriteVerbose("regsvr32.exe / s wuweb.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wuweb.dll");
			WriteVerbose("regsvr32.exe / s qmgr.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\qmgr.dll");
			WriteVerbose("regsvr32.exe / s qmgrprxy.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\qmgrprxy.dll");
			WriteVerbose("regsvr32.exe / s wucltux.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wucltux.dll");
			WriteVerbose("regsvr32.exe / s muweb.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\muweb.dll");
			WriteVerbose("regsvr32.exe / s wuwebv.dll");
			WUToolsObj.RunProcess("regsvr32.exe", "/s " + environmentVariable2 + "\\wuwebv.dll");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 7: Reset WinSock");
			WriteVerbose("netsh winsock reset");
			WUToolsObj.RunProcess("netsh.exe", "winsock reset");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 8: Reset Proxy");
			WriteVerbose("netsh winhttp reset proxy");
			WUToolsObj.RunProcess("netsh.exe", "winhttp reset proxy");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 9: Start Windows Update services");
			WUToolsObj.StartService(computer, "cryptsvc");
			WriteVerbose("Cryptographic Services (cryptsvc)");
			WUToolsObj.StartService(computer, "appidsvc");
			WriteVerbose("Application Identity (appidsvc)");
			WUToolsObj.StartService(computer, "wuauserv");
			WriteVerbose("Windows Update (wuauserv)");
			WUToolsObj.StartService(computer, "BITS");
			WriteVerbose("Background Intelligent Transfer Service (BITS)");
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "Step 10: Start Windows Update services");
			WriteVerbose("wuauclt /resetauthorization /detectnow");
			WUToolsObj.RunProcess("wuauclt.exe", "/resetauthorization /detectnow");
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
