using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get Service Manager configuration.</para>
/// <para type="description">Use Get-WUServiceManager cmdlet to get available configuration of update services.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Check currently available Windows Update Services on machine.</para>
///
///  Get-WUServiceManager 
///
///  <para>ServiceID                            IsManaged IsDefault Name</para>
///  <para>---------                            --------- --------- ----</para>
///  <para>9482f4b4-e343-43b6-b170-9a65bc822c77 False     False     Windows Update</para>
///  <para>7971f918-a847-4430-9279-4a52d1efe18d False     False     Microsoft Update</para>
///  <para>3da21691-e39d-4da6-8a4b-b43877bcb1b7 True      True      Windows Server Update Service</para>
///  <para>13df3d8f-78d7-4eb8-bb9c-2a101870d350 False     False     Offline Sync Service2</para>
///  <para>a8f3b5e6-fb1f-4814-a047-2257d39c2460 False     False     Offline Sync Service</para>
///  </code>
///  </example>
[Cmdlet("Get", "WUServiceManager", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
[OutputType(new Type[] { typeof(ServiceManager) })]
public class GetWUServiceManager : PSCmdlet
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
	public PSCredential Credential { get; set; }

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
	/// <para type="description">Get specific Service Manager if it's available.</para>
	/// </summary>
	[Parameter]
	public string ServiceID { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static UpdateServiceManager ServiceManagerObj { get; set; }

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
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "Name", "ServiceID", "IsManaged", "IsDefaultAUService" });
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
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Get Windows Update ServiceManager"))
			{
				continue;
			}
			WUAPIObject wUApiServiceManagerObj = WUToolsObj.GetWUApiServiceManagerObj(text);
			WriteDebug(DateTime.Now.ToString() + " ServiceManagerObj mode: " + wUApiServiceManagerObj.Mode);
			if (wUApiServiceManagerObj.Status)
			{
				ServiceManagerObj = (UpdateServiceManager)wUApiServiceManagerObj.Object;
				Collection<PSObject> collection = new Collection<PSObject>();
				foreach (IUpdateService service in ServiceManagerObj.Services)
				{
					WriteDebug(DateTime.Now.ToString() + " " + text + ": Found service " + service.Name);
					PSObject pSObject = new PSObject(service);
					pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
					pSObject.TypeNames.Clear();
					pSObject.TypeNames.Add("PSWindowsUpdateNext.ServiceManager");
					collection.Add(pSObject);
				}
				Collection<PSObject> collection2 = new Collection<PSObject>();
				collection2 = ((ServiceID == null) ? collection : new Collection<PSObject>(collection.Where((PSObject x) => x.Properties["ServiceID"].Value.ToString() == ServiceID).ToList()));
				WriteObject(collection2, enumerateCollection: true);
				OutputObj = new Collection<PSObject>(OutputObj.Concat(collection2).ToList());
			}
			else if ((bool)Debuger)
			{
				ErrorRecord errorRecord = new ErrorRecord(wUApiServiceManagerObj.Exception, "Debug", ErrorCategory.CloseError, null);
				WriteError(errorRecord);
			}
			else
			{
				ErrorRecord error = wUApiServiceManagerObj.Error;
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
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "Name", "ServiceID", "IsManaged", "IsDefaultAUService" });
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
