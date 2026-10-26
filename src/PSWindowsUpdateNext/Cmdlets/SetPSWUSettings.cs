using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Security;
using System.Security.Principal;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Save PSWUSettings.</para>
/// <para type="description">Use Set-PSWUSettings save PSWindowsUpdateNext module settings to XML file.</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para>
///  <example>
///  <code>
///  <para>Set Office 365 as smtp server for PSWindowsUpdateNext module.</para>
///
///  Set-PSWUSettings -SmtpServer smtp.office365.com -SmtpPort 587 -SmtpEnableSsl $true -SmtpSubject "PSWindowsUpdateNext Report" -SmtpTo mgajda@psmvp.pl -SmtpFrom mgajda@psmvp.pl -SmtpCredential (Get-Credential mgajda@psmvp.pl)
///
///  </code>
///  </example>
[Cmdlet("Set", "PSWUSettings", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
public class SetPSWUSettings : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	private int _Port = 25;

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection.</para>
	/// </summary>
	[Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
	private string[] ComputerName { get; set; }

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
	/// <para type="description">Save smtp server to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public string SmtpServer { get; set; }

	/// <summary>
	/// <para type="description">Save smtp port to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public int SmtpPort
	{
		get
		{
			return _Port;
		}
		set
		{
			_Port = value;
		}
	}

	/// <summary>
	/// <para type="description">Save enable ssl to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public bool SmtpEnableSsl { get; set; }

	/// <summary>
	/// <para type="description">Save smtp credential to Credential Manager.</para>
	/// </summary>
	[Parameter]
	public PSCredential SmtpCredential { get; set; }

	/// <summary>
	/// <para type="description">Save smtp recipient to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public string SmtpTo { get; set; }

	/// <summary>
	/// <para type="description">Save smtp sernder to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public string SmtpFrom { get; set; }

	/// <summary>
	/// <para type="description">Save alternative message subject to PSWUSettings file.</para>
	/// </summary>
	[Parameter]
	public string SmtpSubject { get; set; }

	/// <summary>
	/// <para type="description">Alternative report message propertie.s</para>
	/// </summary>
	[Parameter]
	public string Properties { get; set; }

	/// <summary>
	/// <para type="description">Alternative report message format style.</para>
	/// </summary>
	[ValidateSet(new string[] { "Table", "List" }, IgnoreCase = true)]
	[Parameter]
	public string Style { get; set; }

	/// <summary>
	/// <para type="description">Invoke-WUJob to save credential as system user</para>
	/// </summary>
	[Parameter]
	public string SaveAsSystem { get; set; }

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
		// WUTools is a cmdlet that is never invoked. Lending it this cmdlet's runtime makes its
		// warnings and debug messages appear in this cmdlet's streams; without it they are dropped.
		WUToolsObj.CommandRuntime = CommandRuntime;
		OutputObj = new Collection<PSObject>();
		if ((bool)SendReport)
		{
			WriteDebug(DateTime.Now.ToString() + " Test smtp settings");
			if (!PSWUSettings.ContainsKey("Properties"))
			{
				PSWUSettings.Add("Properties", "*");
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
		Collection<PSObject> collection = new Collection<PSObject>();
		string[] computerName = ComputerName;
		foreach (string text in computerName)
		{
			PSObject pSObject = new PSObject();
			pSObject.Properties.Add(new PSNoteProperty("ComputerName", text));
			if (MyInvocation.BoundParameters.ContainsKey("SaveAsSystem"))
			{
				string text2 = "";
				WriteDebug(DateTime.Now.ToString() + " Catched encrypted string: " + SaveAsSystem);
				try
				{
					// DPAPI, machine scope (see WUTools.DecryptString): only this computer can read the value.
					text2 = WUTools.DecryptString(SaveAsSystem, null);
				}
				catch (Exception ex)
				{
					WriteError(new ErrorRecord(new Exception("Cannot decrypt the -SaveAsSystem value. It must be created by Set-PSWUSettings on this computer: " + ex.Message, ex), "Decrypt", ErrorCategory.InvalidData, null));
					return;
				}
				pSObject.Properties.Add(new PSNoteProperty("SaveAsSystem", SaveAsSystem));
				string[] array = text2.Split('|');
				string text3 = array[0];
				string text4 = array[1];
				WriteDebug(DateTime.Now.ToString() + " Decrypted UserName: " + text3);
				SecureString secureString = new SecureString();
				char[] array2 = text4.ToCharArray();
				char[] array3 = array2;
				foreach (char c in array3)
				{
					secureString.AppendChar(c);
				}
				SmtpCredential = new PSCredential(text3, secureString);
			}
			if (SmtpCredential != null)
			{
				WriteDebug(DateTime.Now.ToString() + " Save credential to Credential Manager");
				WUToolsObj.SaveCredential(SmtpCredential.GetNetworkCredential().UserName, SmtpCredential.GetNetworkCredential().Password);
				if (!MyInvocation.BoundParameters.ContainsKey("SaveAsSystem"))
				{
					string text5 = WUTools.EncryptString(SmtpCredential.GetNetworkCredential().UserName + "|" + SmtpCredential.GetNetworkCredential().Password, null);
					WriteDebug(DateTime.Now.ToString() + " Encrypted string to SaveAsSystem: " + text5);
					string text6 = "$DebugPreference = 'continue'; Set-PSWUSettings -SaveAsSystem '" + text5 + "'";
					if ((bool)SendReport)
					{
						text6 += " -SendReport";
					}
					text6 += " -Verbose *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log";
					InvokeWUJob invokeWUJob = new InvokeWUJob();
					invokeWUJob.ComputerName = new string[1] { text };
					if (Credential != null)
					{
						invokeWUJob.Credential = Credential;
					}
					invokeWUJob.Script = text6;
					invokeWUJob.TaskName = "PSWindowsUpdateNext_Settings";
					invokeWUJob.EndBoundary = DateTime.Now.AddMinutes(2.0);
					invokeWUJob.RunNow = true;
					WriteVerbose("Invoke-WUJob: " + invokeWUJob.TaskName + " " + text + " (Now)");
					WriteVerbose("powershell.exe -Command \"" + text6 + "\"");
					IEnumerable enumerable = invokeWUJob.Invoke();
					foreach (object item in enumerable)
					{
						WriteObject(item);
					}
				}
				pSObject.Properties.Add(new PSNoteProperty("SmtpCredential", SmtpCredential.GetNetworkCredential().UserName));
			}
			bool flag = false;
			Hashtable pSWUSettings = WUToolsObj.GetPSWUSettings();
			if (MyInvocation.BoundParameters.ContainsKey("SmtpServer"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("SmtpServer"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change SmtpServer: " + pSWUSettings["SmtpServer"]?.ToString() + " -> " + SmtpServer);
					pSWUSettings["SmtpServer"] = SmtpServer;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add SmtpServer: " + SmtpServer);
					pSWUSettings.Add("SmtpServer", SmtpServer);
				}
				pSObject.Properties.Add(new PSNoteProperty("SmtpServer", SmtpServer));
			}
			if (MyInvocation.BoundParameters.ContainsKey("SmtpPort"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("Port"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change Port: " + pSWUSettings["Port"]?.ToString() + " -> " + SmtpPort);
					pSWUSettings["Port"] = SmtpPort;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add Port: " + SmtpPort);
					pSWUSettings.Add("Port", SmtpPort);
				}
				pSObject.Properties.Add(new PSNoteProperty("Port", SmtpPort));
			}
			if (MyInvocation.BoundParameters.ContainsKey("SmtpEnableSsl"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("EnableSsl"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change Port: " + pSWUSettings["EnableSsl"]?.ToString() + " -> " + SmtpEnableSsl);
					pSWUSettings["EnableSsl"] = SmtpEnableSsl;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add Port: " + SmtpEnableSsl);
					pSWUSettings.Add("EnableSsl", SmtpEnableSsl);
				}
				pSObject.Properties.Add(new PSNoteProperty("EnableSsl", SmtpEnableSsl));
			}
			if (MyInvocation.BoundParameters.ContainsKey("SmtpTo"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("To"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change To: " + pSWUSettings["To"]?.ToString() + " -> " + SmtpTo);
					pSWUSettings["To"] = SmtpTo;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add To: " + SmtpTo);
					pSWUSettings.Add("To", SmtpTo);
				}
				pSObject.Properties.Add(new PSNoteProperty("To", SmtpTo));
			}
			if (MyInvocation.BoundParameters.ContainsKey("SmtpFrom"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("From"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change From: " + pSWUSettings["From"]?.ToString() + " -> " + SmtpFrom);
					pSWUSettings["From"] = SmtpFrom;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add From: " + SmtpFrom);
					pSWUSettings.Add("From", SmtpFrom);
				}
				pSObject.Properties.Add(new PSNoteProperty("From", SmtpFrom));
			}
			if (MyInvocation.BoundParameters.ContainsKey("SmtpSubject"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("Subject"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change Subject: " + pSWUSettings["Subject"]?.ToString() + " -> " + SmtpSubject);
					pSWUSettings["Subject"] = SmtpSubject;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add Subject: " + SmtpSubject);
					pSWUSettings.Add("Subject", SmtpSubject);
				}
				pSObject.Properties.Add(new PSNoteProperty("Subject", SmtpSubject));
			}
			if (MyInvocation.BoundParameters.ContainsKey("Properties"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("Properties"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change Properties: " + pSWUSettings["Properties"]?.ToString() + " -> " + Properties);
					pSWUSettings["Properties"] = Properties;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add Properties: " + Properties);
					pSWUSettings.Add("Properties", Properties);
				}
				pSObject.Properties.Add(new PSNoteProperty("Properties", Properties));
			}
			if (MyInvocation.BoundParameters.ContainsKey("Style"))
			{
				flag = true;
				if (pSWUSettings.ContainsKey("Style"))
				{
					WriteDebug(DateTime.Now.ToString() + " Change Style: " + pSWUSettings["Style"]?.ToString() + " -> " + Style);
					pSWUSettings["Style"] = Style;
				}
				else
				{
					WriteDebug(DateTime.Now.ToString() + " Add Style: " + Style);
					pSWUSettings.Add("Style", Style);
				}
				pSObject.Properties.Add(new PSNoteProperty("Style", Style));
			}
			if (flag)
			{
				WUToolsObj.SetPSWUSettings(pSWUSettings);
				string text7 = WUToolsObj.PSWUModulePath();
				string text8 = text7 + "\\PSWUSettings.xml";
				WriteVerbose("Save PSWUSetting to file: " + text8);
			}
			collection.Add(pSObject);
		}
		WriteObject(collection);
		OutputObj = new Collection<PSObject>(collection);
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
