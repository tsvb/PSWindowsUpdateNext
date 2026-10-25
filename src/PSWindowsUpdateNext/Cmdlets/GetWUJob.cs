using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using TaskScheduler;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get invoked WUJob.</para>
/// <para type="description">Use Get-WUJob cmdlet to get invoked WUJob in Task Scheduler.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Get invoked WUJob on remote machine.</para>
///
///  Get-WUJob -ComputerName MG-PC
///
///  </code>
///  </example>
[Cmdlet("Get", "WUJob", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
[OutputType(new Type[] { typeof(WUJob) })]
public class GetWUJob : PSCmdlet
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
	private SwitchParameter SendReport { get; set; }

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
	/// <para type="description">Specify custom name for Task Scheduler job. Default is 'PSWindowsUpdateNext'.</para>
	/// </summary>
	[Parameter]
	public string TaskName { get; set; }

	/// <summary>
	/// <para type="description">Clear expired WUJob.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter ClearExpired { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private string UserName { get; set; }

	private string Domain { get; set; }

	private string Password { get; set; }

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
		if (string.Equals(invocationName.ToString(), "Clear-WUJob", StringComparison.OrdinalIgnoreCase))
		{
			ClearExpired = true;
		}
		global::TaskScheduler.TaskScheduler taskScheduler = (global::TaskScheduler.TaskScheduler)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
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
			for (int j = 1; j <= 3; j++)
			{
				try
				{
					if (Credential != null)
					{
						WriteVerbose("Try to connect " + text + ". Test " + j);
						taskScheduler.Connect(text, UserName, Domain, Password);
					}
					else
					{
						WriteVerbose("Try to connect " + text + ". Test " + j);
						taskScheduler.Connect(text, Type.Missing, Type.Missing, Type.Missing);
					}
				}
				catch (COMException ex)
				{
					if (j >= 3)
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
							ErrorRecord errorRecord = new ErrorRecord(ex, "Debug", ErrorCategory.CloseError, null);
							ThrowTerminatingError(errorRecord);
						}
						else
						{
							WriteError(new ErrorRecord(new Exception(text + ": Unknown failure. Try debug."), "Unknown", ErrorCategory.CloseError, null));
						}
					}
					else
					{
						Thread.Sleep(500);
					}
				}
			}
			try
			{
				int num2 = 0;
				ITaskFolder folder = taskScheduler.GetFolder("\\");
				if (!MyInvocation.BoundParameters.ContainsKey("TaskName"))
				{
					IRegisteredTaskCollection tasks = folder.GetTasks(1);
					foreach (IRegisteredTask item in tasks)
					{
						if (item.Definition.RegistrationInfo.Source == "PSWindowsUpdateNext")
						{
							PSObject pSObject = new PSObject(item);
							pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
							switch (item.State)
							{
							case _TASK_STATE.TASK_STATE_DISABLED:
								pSObject.Properties.Add(new PSNoteProperty("StateName", "Disabled"));
								break;
							case _TASK_STATE.TASK_STATE_QUEUED:
								pSObject.Properties.Add(new PSNoteProperty("StateName", "Queued"));
								break;
							case _TASK_STATE.TASK_STATE_READY:
								pSObject.Properties.Add(new PSNoteProperty("StateName", "Ready"));
								break;
							case _TASK_STATE.TASK_STATE_RUNNING:
								pSObject.Properties.Add(new PSNoteProperty("StateName", "Running"));
								break;
							case _TASK_STATE.TASK_STATE_UNKNOWN:
								pSObject.Properties.Add(new PSNoteProperty("StateName", "Unknown"));
								break;
							}
							pSObject.TypeNames.Clear();
							pSObject.TypeNames.Add("PSWindowsUpdateNext.WUJob");
							WriteObject(pSObject);
							num2++;
						}
					}
				}
				else
				{
					IRegisteredTask task = folder.GetTask(TaskName);
					PSObject pSObject2 = new PSObject(task);
					pSObject2.Properties.Add(new PSNoteProperty("ComputerName", text));
					switch (task.State)
					{
					case _TASK_STATE.TASK_STATE_DISABLED:
						pSObject2.Properties.Add(new PSNoteProperty("StateName", "Disabled"));
						break;
					case _TASK_STATE.TASK_STATE_QUEUED:
						pSObject2.Properties.Add(new PSNoteProperty("StateName", "Queued"));
						break;
					case _TASK_STATE.TASK_STATE_READY:
						pSObject2.Properties.Add(new PSNoteProperty("StateName", "Ready"));
						break;
					case _TASK_STATE.TASK_STATE_RUNNING:
						pSObject2.Properties.Add(new PSNoteProperty("StateName", "Running"));
						break;
					case _TASK_STATE.TASK_STATE_UNKNOWN:
						pSObject2.Properties.Add(new PSNoteProperty("StateName", "Unknown"));
						break;
					}
					pSObject2.TypeNames.Clear();
					pSObject2.TypeNames.Add("PSWindowsUpdateNext.WUJob");
					WriteObject(pSObject2);
					num2++;
				}
				WriteVerbose("Found " + num2 + " tasks");
				if (MyInvocation.BoundParameters.ContainsKey("ClearExpired"))
				{
					continue;
				}
				IRegisteredTaskCollection tasks2 = folder.GetTasks(1);
				foreach (IRegisteredTask item2 in tasks2)
				{
					if (item2.Definition.RegistrationInfo.Source == "PSWindowsUpdateNext" && DateTime.Parse(item2.Definition.Triggers[1].EndBoundary) < DateTime.Now && item2.State != _TASK_STATE.TASK_STATE_RUNNING && ShouldProcess(text, "Clear expired task: " + item2.Name))
					{
						folder.DeleteTask(item2.Name, 0);
					}
				}
			}
			catch
			{
				ErrorRecord errorRecord2 = new ErrorRecord(new Exception("Cant find task " + TaskName + " on " + text), "WUJob", ErrorCategory.ResourceUnavailable, null);
				WriteError(errorRecord2);
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
