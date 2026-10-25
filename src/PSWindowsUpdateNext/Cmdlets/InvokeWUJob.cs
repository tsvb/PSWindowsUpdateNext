using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using TaskScheduler;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Invoke WUJobs by Task Schduler.</para>
/// <para type="description">Use Invoke-WUJobs cmdlet to invoke PSWindowsUpdateNext actions remotly. It Based on TaskScheduler because CreateUpdateDownloader() and CreateUpdateInstaller() methods can't be called from a remote computer - E_ACCESSDENIED.</para>
/// <para type="description">Note: Because we do not have the ability to interact, is recommended use -AcceptAll for Install-WindowsUpdate.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Invoke Install-WindowsUpdate on remote machine today at 6:00 PM.</para>
///
///  Invoke-WUJob -ComputerName MG-PC -Script "ipmo PSWindowsUpdateNext; Install-WindowsUpdate -AcceptAll | Out-File C:\PSWindowsUpdateNext.log" -TriggerDate (Get-Date -Hour 18 -Minute 0 -Second 0)
///
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Invoke WU job: powershell.exe -Command "ipmo PSWindowsUpdateNext; Install-WindowsUpdate -AcceptAll | Out-File C:\PSWindowsUpdateNext.log"" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  </code>
///  </example>
[Cmdlet("Invoke", "WUJob", SupportsShouldProcess = true, DefaultParameterSetName = "RunNow", ConfirmImpact = ConfirmImpact.High)]
public class InvokeWUJob : Cmdlet
{
	private string _TaskName = "PSWindowsUpdateNext";

	private bool _Hidden = true;

	private string _Script = "ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\\PSWindowsUpdateNext.log";

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
	/// <para type="description">Specify custom name for Task Scheduler job. Default is 'PSWindowsUpdateNext'.</para>
	/// </summary>
	[Parameter]
	public string TaskName
	{
		get
		{
			return _TaskName;
		}
		set
		{
			_TaskName = value;
		}
	}

	/// <summary>
	/// <para type="description">Specify if task must be hidden. Default is true.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Hidden
	{
		get
		{
			return _Hidden;
		}
		set
		{
			_Hidden = value;
		}
	}

	/// <summary>
	/// <para type="description">Specify EndBoundary for Task Scheduler trigger.</para>
	/// </summary>
	[Parameter]
	[ValidateDateTime]
	public DateTime EndBoundary { get; set; }

	/// <summary>
	/// <para type="description">Specify Time trigger for Task Scheduler job.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "TriggerDate")]
	[ValidateDateTime]
	public DateTime TriggerDate { get; set; }

	/// <summary>
	/// <para type="description">Specify system startup trigger for Task Scheduler job.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "TriggerAtStart")]
	public SwitchParameter TriggerAtStart { get; set; }

	/// <summary>
	/// <para type="description">Run Task Scheduler job immediately.</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "RunNow")]
	public SwitchParameter RunNow { get; set; }

	/// <summary>
	/// <para type="description">Specify PowerShell script that you what to run. Default is {ipmo PSWindowsUpdateNext; Get-WindowsUpdate -AcceptAll -Install | Out-File $Env:TEMP\PSWindowsUpdateNext.log}</para>
	/// </summary>
	[Parameter]
	public string Script
	{
		get
		{
			return _Script;
		}
		set
		{
			_Script = value;
		}
	}

	/// <summary>
	/// <para type="description">Force update old Task Scheduler Job if it's available.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Force { get; set; }

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
		WriteDebug(DateTime.Now.ToString() + " CmdletStart: Invoke-WUJob");
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
		{
			ErrorRecord errorRecord = new ErrorRecord(new Exception("To perform operations you must run an elevated Windows PowerShell console."), "AccessDenied", ErrorCategory.PermissionDenied, null);
			ThrowTerminatingError(errorRecord);
		}
		WUToolsObj = new WUTools();
		OutputObj = new Collection<PSObject>();
		if (ComputerName == null)
		{
			ComputerName = new string[1] { Environment.MachineName };
		}
	}

	private void CoreProcessing()
	{
		global::TaskScheduler.TaskScheduler taskScheduler = (global::TaskScheduler.TaskScheduler)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")));
		ITaskDefinition taskDefinition = taskScheduler.NewTask(0u);
		IRegistrationInfo registrationInfo = taskDefinition.RegistrationInfo;
		registrationInfo.Description = TaskName + ": " + Script;
		registrationInfo.Author = WindowsIdentity.GetCurrent().Name;
		registrationInfo.Source = "PSWindowsUpdateNext";
		ITaskSettings settings = taskDefinition.Settings;
		settings.Enabled = true;
		settings.StartWhenAvailable = true;
		settings.Hidden = Hidden;
		settings.DeleteExpiredTaskAfter = "PT30M";
		IExecAction execAction = (IExecAction)taskDefinition.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
		execAction.Path = "powershell.exe";
		// -EncodedCommand carries the script verbatim: no quoting rules, no way for a quote inside
		// -Script to break out of the command line. The readable form is kept in the task description.
		execAction.Arguments = "-NoProfile -NonInteractive -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(Script));
		string displayCommand = execAction.Path + " -NoProfile -NonInteractive -Command \"" + Script + "\"";
		if (TriggerDate != DateTime.MinValue)
		{
			ITimeTrigger timeTrigger = (ITimeTrigger)taskDefinition.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
			timeTrigger.StartBoundary = TriggerDate.ToString("yyyy-MM-ddTHH:mm:ss");
			timeTrigger.EndBoundary = ((EndBoundary != DateTime.MinValue) ? EndBoundary.ToString("yyyy-MM-ddTHH:mm:ss") : TriggerDate.AddHours(1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
		}
		else if ((bool)TriggerAtStart)
		{
			IBootTrigger bootTrigger = (IBootTrigger)taskDefinition.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_BOOT);
			bootTrigger.Delay = "PT30S";
			bootTrigger.EndBoundary = ((EndBoundary != DateTime.MinValue) ? EndBoundary.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.AddDays(1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
		}
		else
		{
			ITimeTrigger timeTrigger2 = (ITimeTrigger)taskDefinition.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
			timeTrigger2.StartBoundary = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
			timeTrigger2.EndBoundary = ((EndBoundary != DateTime.MinValue) ? EndBoundary.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.AddHours(1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
		}
		taskDefinition.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text + ": Connecting...");
			PSObject pSObject = new PSObject();
			try
			{
				ErrorRecord errorRecord = WUToolsObj.CheckPSWUModule(text);
				if (errorRecord != null)
				{
					WriteError(errorRecord);
				}
			}
			catch
			{
				WriteWarning(text + ": Can't check PSWindowsUpdateNext on destination machine - check WinRM. Can't guarantee Invoke success.");
			}
			if (!ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Invoke WU job: " + displayCommand))
			{
				continue;
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
						else if ((bool)Debuger)
						{
							ErrorRecord errorRecord2 = new ErrorRecord(ex, "Debug", ErrorCategory.CloseError, null);
							ThrowTerminatingError(errorRecord2);
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
			ITaskFolder folder = taskScheduler.GetFolder("\\");
			int num2 = 1;
			IRunningTaskCollection runningTasks = taskScheduler.GetRunningTasks(0);
			IRunningTask runningTask = null;
			foreach (IRunningTask item in runningTasks)
			{
				if (item.Name.ToString() == TaskName)
				{
					runningTask = item;
					break;
				}
			}
			if (runningTask != null)
			{
				num2 = 0;
				if ((bool)Force)
				{
					WriteVerbose("Stopping task: " + TaskName);
					folder.GetTask(TaskName).Stop(0);
					num2 = 1;
				}
				else
				{
					IRegisteredTask task = folder.GetTask(TaskName);
					string path = ((IExecAction)task.Definition.Actions[0]).Path;
					string arguments = ((IExecAction)task.Definition.Actions[0]).Arguments;
					ErrorRecord errorRecord3 = new ErrorRecord(new Exception(text + ": Job " + TaskName + " (" + path + " " + arguments + ") is still running."), "RegisterTaskDefinition", ErrorCategory.CloseError, null);
					ThrowTerminatingError(errorRecord3);
				}
			}
			if (num2 == 1)
			{
				WriteVerbose("Registering task: " + TaskName);
				folder.RegisterTaskDefinition(TaskName, taskDefinition, 6, "SYSTEM", null, _TASK_LOGON_TYPE.TASK_LOGON_PASSWORD, Type.Missing);
				if ((bool)RunNow)
				{
					Thread.Sleep(5000);
					WriteVerbose("Starting task: " + TaskName);
					folder.GetTask(TaskName).Run(0);
				}
			}
			IRegisteredTaskCollection tasks = folder.GetTasks(0);
			foreach (IRegisteredTask item2 in tasks)
			{
				string text2 = "";
				try
				{
					text2 = item2.Definition.RegistrationInfo.Source;
				}
				catch
				{
					WriteVerbose("Task sourse error. Skipping clear");
				}
				if (text2 == "PSWindowsUpdateNext" && DateTime.Parse(item2.Definition.Triggers[1].EndBoundary) < DateTime.Now && item2.State != _TASK_STATE.TASK_STATE_RUNNING)
				{
					folder.DeleteTask(item2.Name, 0);
				}
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
		base.EndProcessing();
	}

	/// <summary>
	/// Stop
	/// </summary>
	protected override void StopProcessing()
	{
		base.StopProcessing();
	}
}
