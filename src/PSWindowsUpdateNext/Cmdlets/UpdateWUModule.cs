using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Security.Principal;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Update PSWindowsUpdateNext module.</para>
/// <para type="description">Use Use Update-WUModule cmdlet to remote update PSWindowsUpdateNext module.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Update PSWindowsUpdateNext module from PSGallery</para>
///
///  Update-WUModule -ComputerName MG-PC -Online
///
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Update PSWindowsUpdateNext module from current serwer. Require SMB connection to destination machine.</para>
///
///  Update-WUModule -ComputerName MG-PC -Local
///
///  </code>
///  </example>
[Cmdlet("Update", "WUModule", SupportsShouldProcess = true, DefaultParameterSetName = "Online", ConfirmImpact = ConfirmImpact.High)]
public class UpdateWUModule : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Mandatory = true, ParameterSetName = "Local")]
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Online")]
	public string[] ComputerName { get; set; }

	/// <summary>
	/// <para type="description">Specify alternative credential.</para>
	/// </summary>
	[Parameter]
	public PSCredential Credential { get; set; }

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
	/// <para type="description">Update from PSGallery.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "Online")]
	public SwitchParameter Online { get; set; }

	/// <summary>
	/// <para type="description">Update from current module.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "Local")]
	public SwitchParameter Local { get; set; }

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
		if ((bool)Local)
		{
			PSObject pSWUModule = WUToolsObj.GetPSWUModule(Environment.MachineName);
			try
			{
				Version version = (Version)pSWUModule.Properties["Version"].Value;
				Version version2 = (Version)pSWUModule.Properties["PSWUDllVersion"].Value;
				WriteVerbose(Environment.MachineName + ": Local module: " + version.ToString() + " (" + version2.ToString() + ")");
			}
			catch
			{
			}
		}
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text + ": Connecting...");
			try
			{
				PSObject pSWUModule2 = WUToolsObj.GetPSWUModule(text);
				WriteDebug(DateTime.Now.ToString() + " Module version: " + pSWUModule2.Properties["Version"].Value);
				WriteDebug(DateTime.Now.ToString() + " Dll version: " + pSWUModule2.Properties["PSWUDllVersion"].Value);
			}
			catch
			{
			}
			PSObject pSWUModule3 = WUToolsObj.GetPSWUModule(text);
			try
			{
				Version version3 = (Version)pSWUModule3.Properties["Version"].Value;
				Version version4 = (Version)pSWUModule3.Properties["PSWUDllVersion"].Value;
				WriteVerbose(text + ": Remote module: " + version3.ToString() + " (" + version4.ToString() + ")");
			}
			catch
			{
				WriteDebug(DateTime.Now.ToString() + " Can't get PSWU module from " + text);
			}
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Update PSWindowsUpdateNext module"))
			{
				continue;
			}
			string text2 = "";
			if ((bool)Online)
			{
				WriteVerbose("Installing module...");
				text2 = "Invoke-Command -ComputerName $Computer -Command { Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force; Install-Module -Name PSWindowsUpdateNext -Force -Verbose } -Authentication Negotiate";
				text2 = text2.Replace("$Computer", text);
				WriteDebug(DateTime.Now.ToString() + " Online mode: " + text2);
			}
			else
			{
				WriteVerbose("Cloning module...");
				text2 = "$ModuleBase = (Get-Module PSWindowsUpdateNext -ListAvailable | Sort Version -Descending | Select -First 1).ModuleBase\r\n$Destination = '\\\\$Computer\\' + $($ModuleBase -replace ':','$') \r\n\r\nCopy-Item $ModuleBase -Destination $Destination -Force\r\nCopy-Item ($ModuleBase + '\\*') -Destination $Destination -Force\r\n\r\nInvoke-Command -ComputerName $Computer -Command { ipmo PSWindowsUpdateNext; Enable-WURemoting -Verbose } -Authentication Negotiate";
				text2 = text2.Replace("$Computer", text);
				WriteDebug(DateTime.Now.ToString() + " Local mode: " + text2);
			}
			ErrorRecord errorRecord = null;
			using (PowerShell powerShell = PowerShell.Create())
			{
				powerShell.AddScript(text2);
				Collection<PSObject> collection = powerShell.Invoke();
				WriteObject(powerShell.Streams.Verbose);
				WriteObject(powerShell.Streams.Error);
				if (powerShell.Streams.Error.Count > 0)
				{
					errorRecord = powerShell.Streams.Error[0];
				}
			}
			PSObject pSWUModule4 = WUToolsObj.GetPSWUModule(text);
			try
			{
				Version version5 = (Version)pSWUModule4.Properties["Version"].Value;
				Version version6 = (Version)pSWUModule4.Properties["PSWUDllVersion"].Value;
				WriteVerbose(text + ": Remote module: " + version5.ToString() + " (" + version6.ToString() + ")");
			}
			catch
			{
				WriteDebug(DateTime.Now.ToString() + " Can't get PSWU module from " + text);
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
