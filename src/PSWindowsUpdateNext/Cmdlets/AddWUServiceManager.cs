using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Register a new Windows Update API Service Manager.</para>
/// <para type="description">Use Add-WUServiceManager cmdlet to register new Windows Update Service Manager.</para>
/// <para type="description">It's combination old 'Add-WUServiceManager' and 'Add-WUOfflineSync' functions to register online  and offline ServiceManager </para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Try register Microsoft Update Service by custom ServiceID.</para>
///
///  Add-WUServiceManager -ServiceID "7971f918-a847-4430-9279-4a52d1efe18d"
///
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Register Windows Update Service Manager: 7971f918-a847-4430-9279-4a52d1efe18d" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///
///  <para>ServiceID                            IsManaged IsDefault Name</para>
///  <para>---------                            --------- --------- ----</para>
///  <para>7971f918-a847-4430-9279-4a52d1efe18d False     False     Microsoft Update</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Try register Microsoft Update service as Service Manager.</para>
///
///  Add-WUServiceManager -MicrosoftUpdate
///
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Register Windows Update Service Manager: 7971f918-a847-4430-9279-4a52d1efe18d" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///
///  <para>ServiceID                            IsManaged IsDefault Name</para>
///  <para>---------                            --------- --------- ----</para>
///  <para>7971f918-a847-4430-9279-4a52d1efe18d False     False     Microsoft Update</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Try register Offline Sync Service from file C:\wsusscn2.cab.</para>
///
///  Add-WUServiceManager -ScanFileLocation C:\wsusscn2.cab
///
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Register Offline Windows Update Service Manager: C:\wsusscn2.cab" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///
///  <para>ServiceID                            IsManaged IsDefault Name</para>
///  <para>---------                            --------- --------- ----</para>
///  <para>7e1364ef-e30e-4f4e-9c66-84194eebcbbe False     False     Offline Sync Service</para>
///  </code>
///  </example>
[Cmdlet("Add", "WUServiceManager", SupportsShouldProcess = true, DefaultParameterSetName = "LocalServiceID", ConfirmImpact = ConfirmImpact.High)]
[OutputType(new Type[] { typeof(ServiceManager) })]
public class AddWUServiceManager : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	private int _AddServiceFlag = 2;

	private string _ServiceName = "Offline Sync Service";

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "RemoteServiceID")]
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "MicrosoftUpdate")]
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
	/// <para type="description">An identifier for the service to be registered. </para>
	/// <para type="description">Examples Of ServiceID: 
	/// \r\n
	/// \r\n -- Windows Update 				    9482f4b4-e343-43b6-b170-9a65bc822c77
	/// \r\n -- Microsoft Update 				7971f918-a847-4430-9279-4a52d1efe18d
	/// \r\n -- Windows Store 					117cab2d-82b1-4b5a-a08c-4d62dbee7782
	/// \r\n -- Windows Server Update Service 	3da21691-e39d-4da6-8a4b-b43877bcb1b7</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "LocalServiceID")]
	[Parameter(Mandatory = true, ParameterSetName = "RemoteServiceID")]
	[ValidateNotNullOrEmpty]
	public string ServiceID { get; set; }

	/// <summary>
	/// <para type="description">A combination of AddServiceFlag values:
	/// \r\n0x1 - asfAllowPendingRegistration
	/// \r\n0x2 - asfAllowOnlineRegistration
	/// \r\n0x4 - asfRegisterServiceWithAU</para>
	/// </summary>
	[Parameter(ParameterSetName = "LocalServiceID")]
	public int AddServiceFlag
	{
		get
		{
			return _AddServiceFlag;
		}
		set
		{
			_AddServiceFlag = value;
		}
	}

	/// <summary>
	/// <para type="description">The path of the Microsoft signed local cabinet file (.cab) that has the information that is required for a service registration. If empty, the update agent searches for the authorization cabinet file (.cab) during service registration when a network connection is available.</para>
	/// </summary>
	[Parameter(ParameterSetName = "LocalServiceID")]
	public string AuthorizationCabPath { get; set; }

	/// <summary>
	/// <para type="description">Register Microsoft Update Service Manager - '7971f918-a847-4430-9279-4a52d1efe18d'</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "MicrosoftUpdate")]
	public SwitchParameter MicrosoftUpdate { get; set; }

	/// <summary>
	/// <para type="description">Name under which it will be registered Windows Update offline service. Default name is 'Offline Sync Service'.</para>
	/// </summary>
	[Parameter(ParameterSetName = "OfflineSync")]
	public string ServiceName
	{
		get
		{
			return _ServiceName;
		}
		set
		{
			_ServiceName = value;
		}
	}

	/// <summary>
	/// <para type="description">Path to Windows Update offline scan file (wsusscan.cab or wsusscn2.cab).</para>
	/// </summary>
	[Parameter(Mandatory = true, ParameterSetName = "OfflineSync")]
	public string ScanFileLocation { get; set; }

	/// <summary>
	/// <para type="description">Don't return output.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Silent { get; set; }

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
		if (MyInvocation.BoundParameters.ContainsKey("MicrosoftUpdate"))
		{
			ServiceID = "7971f918-a847-4430-9279-4a52d1efe18d";
		}
	}

	private void CoreProcessing()
	{
		string invocationName = MyInvocation.InvocationName;
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text + ": Connecting...");
			PSObject pSWUModule = WUToolsObj.GetPSWUModule(text);
			WriteDebug(DateTime.Now.ToString() + " Module version: " + pSWUModule.Properties["Version"].Value);
			WriteDebug(DateTime.Now.ToString() + " Dll version: " + pSWUModule.Properties["PSWUDllVersion"].Value);
			WUAPIObject wUApiServiceManagerObj = WUToolsObj.GetWUApiServiceManagerObj(text);
			WriteDebug(DateTime.Now.ToString() + " ServiceManagerObj mode: " + wUApiServiceManagerObj.Mode);
			if (wUApiServiceManagerObj.Status)
			{
				ServiceManagerObj = (UpdateServiceManager)wUApiServiceManagerObj.Object;
				IUpdateService obj = null;
				string value = "";
				WUToolsObj.RestartService(text);
				if (ParameterSetName == "OfflineSync")
				{
					if (ShouldProcess(text, "(" + DateTime.Now.ToString() + ") Register Offline Windows Update Service Manager: " + ScanFileLocation))
					{
						try
						{
							obj = ServiceManagerObj.AddScanPackageService(ServiceName, ScanFileLocation, 1);
							value = "Registered";
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
								ErrorRecord errorRecord = new ErrorRecord(ex, "Debug", ErrorCategory.CloseError, null);
								ThrowTerminatingError(errorRecord);
							}
							continue;
						}
					}
				}
				else if (ShouldProcess(text, "(" + CmdletStart.ToString() + ") Register Windows Update Service Manager: " + ServiceID))
				{
					try
					{
						IUpdateServiceRegistration updateServiceRegistration = ServiceManagerObj.AddService2(ServiceID, AddServiceFlag, AuthorizationCabPath);
						obj = updateServiceRegistration.Service;
						switch ((int)updateServiceRegistration.RegistrationState)
						{
						case 1:
							value = "Not Registered";
							break;
						case 2:
							value = "Registration Pending";
							break;
						case 3:
							value = "Registered";
							break;
						}
					}
					catch (COMException ex2)
					{
						WUApiCode wUApiCodeDetails2 = WUToolsObj.GetWUApiCodeDetails(ex2.ErrorCode);
						if (wUApiCodeDetails2 != null)
						{
							int codeType2 = wUApiCodeDetails2.CodeType;
							int num2 = codeType2;
							if (num2 == 2)
							{
								WriteError(new ErrorRecord(new Exception(wUApiCodeDetails2.Description), wUApiCodeDetails2.HResult, ErrorCategory.CloseError, null));
							}
						}
						else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
						{
							ErrorRecord errorRecord2 = new ErrorRecord(ex2, "Debug", ErrorCategory.CloseError, null);
							ThrowTerminatingError(errorRecord2);
						}
						continue;
					}
				}
				WUToolsObj.RestartService(text);
				PSObject pSObject = new PSObject(obj);
				pSObject.Properties.Add(new PSNoteProperty("RegistrationStateName", value));
				pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
				pSObject.TypeNames.Clear();
				pSObject.TypeNames.Add("PSWindowsUpdateNext.ServiceManager");
				OutputObj.Add(pSObject);
				if (!Silent)
				{
					WriteObject(pSObject, enumerateCollection: true);
				}
			}
			else if ((bool)Debuger)
			{
				ErrorRecord errorRecord3 = new ErrorRecord(wUApiServiceManagerObj.Exception, "Debug", ErrorCategory.CloseError, null);
				WriteError(errorRecord3);
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
