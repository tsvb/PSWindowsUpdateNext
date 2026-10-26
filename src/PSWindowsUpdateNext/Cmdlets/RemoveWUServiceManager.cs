using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Remove windows update service manager.</para>
/// <para type="description">Use Remove-WUServiceManager cmdlet to unregister Windows Update Service Manager.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Try unregister Microsoft Update Service.</para>
///
///  Remove-WUServiceManager -ServiceID "7971f918-a847-4430-9279-4a52d1efe18d"
///
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Unregister Windows Update Service Manager: 7971f918-a847-4430-9279-4a52d1efe18d" on target "MG-PC".</para>
///  <para></para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  </code>
///  </example>
[Cmdlet("Remove", "WUServiceManager", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
public class RemoveWUServiceManager : PSCmdlet
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
	/// <para type="description">Specify ServiceID of Service Manager that you want to remove.</para>
	/// </summary>
	[Parameter(Mandatory = true)]
	[ValidateNotNullOrEmpty]
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
			ErrorRecord errorRecord = new ErrorRecord(new Exception("To perform operations you must run an elevated Windows PowerShell console."), "AccessDenied", ErrorCategory.PermissionDenied, null);
			ThrowTerminatingError(errorRecord);
		}
		WUToolsObj = new WUTools();
		// WUTools is a cmdlet that is never invoked. Lending it this cmdlet's runtime makes its
		// warnings and debug messages appear in this cmdlet's streams; without it they are dropped.
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
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Unregister Windows Update Service Manager: " + ServiceID))
			{
				continue;
			}
			if (WUToolsObj.IsLocalHost(text) && ScheduleJob == DateTime.MinValue)
			{
				WriteVerbose("Run WUApi");
				WUAPIObject wUApiServiceManagerObj = WUToolsObj.GetWUApiServiceManagerObj(text);
				WriteDebug(DateTime.Now.ToString() + " ServiceManagerObj mode: " + wUApiServiceManagerObj.Mode);
				if (wUApiServiceManagerObj.Status)
				{
					ServiceManagerObj = (UpdateServiceManager)wUApiServiceManagerObj.Object;
					try
					{
						WUToolsObj.RestartService(text);
						ServiceManagerObj.RemoveService(ServiceID);
						WUToolsObj.RestartService(text);
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
							WriteError(new ErrorRecord(new Exception(text + ": Unknown failure. Try debug."), "Unknown", ErrorCategory.CloseError, null));
						}
					}
				}
				else if ((bool)Debuger)
				{
					ErrorRecord errorRecord2 = new ErrorRecord(wUApiServiceManagerObj.Exception, "Debug", ErrorCategory.CloseError, null);
					WriteError(errorRecord2);
				}
				else
				{
					ErrorRecord error = wUApiServiceManagerObj.Error;
					WriteError(error);
				}
				continue;
			}
			ErrorRecord errorRecord3 = WUToolsObj.CheckPSWUModule(text);
			if (errorRecord3 != null)
			{
				WriteError(errorRecord3);
				continue;
			}
			string text2 = "Remove-WUServiceManager -ServiceID " + ServiceID + " -Verbose -Confirm:$false *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log";
			InvokeWUJob invokeWUJob = new InvokeWUJob();
			invokeWUJob.ComputerName = new string[1] { text };
			if (Credential != null)
			{
				invokeWUJob.Credential = Credential;
			}
			invokeWUJob.Script = text2;
			if (ScheduleJob != DateTime.MinValue)
			{
				invokeWUJob.TriggerDate = ScheduleJob;
				WriteVerbose("Invoke-WUJob: " + text + " (" + ScheduleJob.ToString() + "): ");
			}
			else
			{
				invokeWUJob.RunNow = true;
				WriteVerbose("Invoke-WUJob: " + text + " (Now): ");
			}
			WriteVerbose("powershell.exe -Command \"" + text2 + "\"");
			IEnumerable enumerable = invokeWUJob.Invoke();
			foreach (object item in enumerable)
			{
				WriteObject(item);
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
