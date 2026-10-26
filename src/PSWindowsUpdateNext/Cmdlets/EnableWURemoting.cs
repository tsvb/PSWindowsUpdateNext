using System;
using System.Collections.ObjectModel;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;
using NetFwTypeLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Enable firewall rules for PSWindowsUpdateNext remoting.</para>
/// <para type="description">Use Enable-WURemoting cmdlet to enable nessesery firewall rules for PSWindowsUpdateNext remoting.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Enable firewall rules for PSWindowsUpdateNext remoting.</para>
///
///  Enable-WURemoting -Verbose
///  </code>
///  </example>
[Cmdlet("Enable", "WURemoting", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
public class EnableWURemoting : PSCmdlet
{
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
	/// <para type="description">Set LocalAccountTokenFilterPolicy registry entry to builds an elevated token on the target remote computer.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter LocalAccountTokenFilterPolicy { get; set; }

	/// <summary>
	/// <para type="description">Enable WinRM Public access from all subnets. Default access is only enabled from local subnet. Required for workgroup computers.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter WinRMPublic { get; set; }

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
			ErrorRecord errorRecord = new ErrorRecord(new Exception("To perform operations you must run an elevated Windows PowerShell console."), "AccessDenied", ErrorCategory.PermissionDenied, null);
			ThrowTerminatingError(errorRecord);
		}
		if (ComputerName == null)
		{
			ComputerName = new string[1] { Environment.MachineName };
		}
	}

	private void CoreProcessing()
	{
		WUTools wUTools = new WUTools();
		string invocationName = MyInvocation.InvocationName;
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			INetFwPolicy2 netFwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
			bool flag = false;
			foreach (INetFwRule rule in netFwPolicy.Rules)
			{
				if (rule.Name == "PSWindowsUpdateNext (RPC Dynamics Ports)")
				{
					flag = true;
				}
			}
			if (!flag)
			{
				WriteVerbose("Create firewall rule: PSWindowsUpdateNext (RPC Dynamics Ports)");
				INetFwRule2 netFwRule2 = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
				netFwRule2.Grouping = "PSWindowsUpdateNext";
				netFwRule2.Enabled = true;
				netFwRule2.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
				netFwRule2.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
				netFwRule2.Protocol = 6;
				netFwRule2.LocalPorts = "RPC";
				netFwRule2.ApplicationName = "%SystemRoot%\\System32\\dllhost.exe";
				netFwRule2.Name = "PSWindowsUpdateNext (RPC Dynamics Ports)";
				netFwPolicy.Rules.Add(netFwRule2);
			}
			else if (!netFwPolicy.get_IsRuleGroupCurrentlyEnabled("PSWindowsUpdateNext"))
			{
				WriteVerbose("Enable firewall rule: PSWindowsUpdateNext (RPC Dynamics Ports)");
				netFwPolicy.EnableRuleGroup(int.MaxValue, "PSWindowsUpdateNext", enable: true);
			}
			else
			{
				WriteDebug(DateTime.Now.ToString() + " PSWindowsUpdateNext (RPC Dynamics Ports) firewall rule is enabled");
			}
			if ((bool)WinRMPublic)
			{
				bool flag2 = false;
				foreach (INetFwRule rule2 in netFwPolicy.Rules)
				{
					if (rule2.Name == "PSWindowsUpdateNext (WinRM Public)")
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					WriteVerbose("Create firewall rule: PSWindowsUpdateNext (WinRM Public)");
					INetFwRule2 netFwRule4 = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
					netFwRule4.Grouping = "PSWindowsUpdateNext";
					netFwRule4.Enabled = true;
					netFwRule4.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
					netFwRule4.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
					netFwRule4.Protocol = 6;
					netFwRule4.LocalPorts = "5985";
					netFwRule4.ApplicationName = "System";
					netFwRule4.Name = "PSWindowsUpdateNext (WinRM Public)";
					netFwPolicy.Rules.Add(netFwRule4);
				}
				else if (!netFwPolicy.get_IsRuleGroupCurrentlyEnabled("PSWindowsUpdateNext"))
				{
					WriteVerbose("Enable firewall rule: PSWindowsUpdateNext (WinRM Public)");
					netFwPolicy.EnableRuleGroup(int.MaxValue, "PSWindowsUpdateNext", enable: true);
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " PSWindowsUpdateNext (WinRM Public) firewall rule is enabled");
				}
			}
			if (!netFwPolicy.get_IsRuleGroupCurrentlyEnabled("Remote Scheduled Tasks Management"))
			{
				WriteVerbose("Enable firewall rules: Remote Scheduled Tasks Management");
				netFwPolicy.EnableRuleGroup(int.MaxValue, "Remote Scheduled Tasks Management", enable: true);
			}
			else
			{
				WriteDebug(DateTime.Now.ToString() + " Remote Scheduled Tasks Management firewall rules are enabled");
			}
			if (!netFwPolicy.get_IsRuleGroupCurrentlyEnabled("Windows Management Instrumentation (WMI)"))
			{
				WriteVerbose("Enable firewall rules: Windows Management Instrumentation (WMI)");
				netFwPolicy.EnableRuleGroup(int.MaxValue, "Windows Management Instrumentation (WMI)", enable: true);
			}
			else
			{
				WriteDebug(DateTime.Now.ToString() + " Windows Management Instrumentation (WMI) firewall rules are enabled");
			}
			if ((bool)LocalAccountTokenFilterPolicy)
			{
				// Both settings only matter for remote access with local accounts, and both weaken it, so they
				// are applied only when the caller asks for them.
				WriteVerbose("Set LocalAccountTokenFilterPolicy=1 registry entry");
				RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
				RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\system\\", writable: true);
				registryKey2.SetValue("LocalAccountTokenFilterPolicy", Convert.ToInt32(1), RegistryValueKind.DWord);
				WriteVerbose("Set ForceGuest=0 registry entry (local accounts authenticate as themselves)");
				RegistryKey lsaKey = registryKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Lsa\\", writable: true);
				lsaKey.SetValue("ForceGuest", Convert.ToInt32(0), RegistryValueKind.DWord);
			}
			try
			{
				// Enable-PSRemoting is idempotent. TrustedHosts is deliberately left alone: it is a client-side
				// list of hosts this computer may send credentials to, and a computer being prepared for remote
				// management does not need it.
				WriteVerbose("Enable PSRemoting");
				using PowerShell powerShell = PowerShell.Create();
				powerShell.AddScript("Enable-PSRemoting -Force; Start-Service WinRM");
				powerShell.Invoke();
				foreach (ErrorRecord remotingError in powerShell.Streams.Error)
				{
					WriteError(remotingError);
				}
			}
			catch (Exception exception)
			{
				WriteError(new ErrorRecord(exception, "PSRemoting", ErrorCategory.ResourceUnavailable, null));
			}

			try
			{
				uint num = 1u;
				string queryString = "SELECT * FROM Win32_Service WHERE Name = 'RemoteRegistry'";
				ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString);
				ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				foreach (ManagementObject item in managementObjectCollection)
				{
					if (Convert.ToString(item.GetPropertyValue("StartMode")) == "Disabled")
					{
						WriteVerbose("Change startmode service: Remote Registry");
						ManagementBaseObject methodParameters = item.GetMethodParameters("ChangeStartMode");
						methodParameters["startmode"] = "Automatic";
						ManagementBaseObject managementBaseObject = item.InvokeMethod("ChangeStartMode", methodParameters, null);
						num = Convert.ToUInt16(managementBaseObject.Properties["ReturnValue"].Value);
					}
					else
					{
						WriteDebug(DateTime.Now.ToString() + " Remote Registry service start mode is ok");
					}
				}
			}
			catch (Exception exception2)
			{
				WriteError(new ErrorRecord(exception2, "RemoteRegistry", ErrorCategory.ResourceUnavailable, null));
			}
			try
			{
				uint num2 = 1u;
				string queryString2 = "SELECT * FROM Win32_Service WHERE Name = 'RemoteRegistry'";
				ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher(queryString2);
				ManagementObjectCollection managementObjectCollection2 = managementObjectSearcher2.Get();
				foreach (ManagementObject item2 in managementObjectCollection2)
				{
					if (Convert.ToString(item2.GetPropertyValue("State")) == "Stopped")
					{
						WriteVerbose("Start service: Remote Registry");
						ManagementBaseObject managementBaseObject2 = item2.InvokeMethod("StartService", null, null);
						num2 = Convert.ToUInt16(managementBaseObject2.Properties["ReturnValue"].Value);
					}
					else
					{
						WriteDebug(DateTime.Now.ToString() + " Remote Registry service is Running");
					}
				}
			}
			catch (COMException ex)
			{
				WUApiCode wUApiCodeDetails = wUTools.GetWUApiCodeDetails(ex.ErrorCode);
				if (wUApiCodeDetails != null)
				{
					int codeType = wUApiCodeDetails.CodeType;
					int num3 = codeType;
					if (num3 == 2)
					{
						WriteError(new ErrorRecord(new Exception(wUApiCodeDetails.Description), wUApiCodeDetails.HResult, ErrorCategory.CloseError, null));
					}
				}
				else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
				{
					ErrorRecord errorRecord = new ErrorRecord(ex, "Debug", ErrorCategory.CloseError, null);
					ThrowTerminatingError(errorRecord);
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
