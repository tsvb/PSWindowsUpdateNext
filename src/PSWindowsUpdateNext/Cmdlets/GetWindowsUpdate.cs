using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using WUApiLib;

namespace PSWindowsUpdateNext;

/// <summary>
/// <para type="synopsis">Get list of available updates meeting the criteria.</para>
/// <para type="description">Use Get-WindowsUpdate (aka Get-WUList) cmdlet to get list of available or installed updates meeting specific criteria.</para>
/// <para type="description">Use Download-WindowsUpdate alias to get list of updates and download it. Equivalent Get-WindowsUpdate -Download.</para>
/// <para type="description">Use Install-WindowsUpdate (aka Get-WUInstall) alias to get list of updates and install it. Equivalent Get-WindowsUpdate -Install.</para>
/// <para type="description">Use Hide-WindowsUpdate alias to get list of updates and hide it. Equivalent Get-WindowsUpdate -Hide.</para>
/// <para type="description">Use Show-WindowsUpdate (aka UnHide-WindowsUpdate) alias to get list of updates and unhide it. Equivalent Get-WindowsUpdate -Hide:$false.</para>
/// <para type="description">There are two types of filtering update: Pre search criteria, Post search criteria.</para>
/// <para type="description">- Pre search works on server side, like example: (IsInstalled = 0 and IsHidden = 0 and CategoryIds contains '0fa1201d-4330-4fa8-8ae9-b877473b6441' )</para>
/// <para type="description">- Post search work on client side after get the pre-filtered list of updates, like example $KBArticleID -match $Update.KBArticleIDs</para>
/// <para type="description">Status info list:\r\n[A|R]DIMHUB\r\nA-IsAccetped\r\nR-IsRejected\r\n D-IsDownloaded\r\n F-DownloadFailed\r\n ?-IsInvoked\r\n  I-IsInstalled\r\n  F-InstallFailed\r\n  ?-IsInvoked\r\n  R-RebootRequired\r\n   M-IsMandatory\r\n    H-IsHidden\r\n     U-IsUninstallable\r\n      B-IsBeta</para>
/// </summary>
/// <para type="link" uri="https://commandlinegeeks.wordpress.com/">Author Blog</para> 
///  <example>
///  <code>
///  <para>Get windows updates available from default service manager.</para>
///
///  Get-WindowsUpdate -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Windows Server Update Service server. Please wait...</para>
///  <para>VERBOSE: Found[4] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[4] Updates in post search criteria</para>
///  <para></para>
///  <para>ComputerName Status     KB          Size Title</para>
///  <para>------------ ------     --          ---- -----</para>
///  <para>MG-PC        -------    KB890830    44MB Narzędzie Windows do usuwania złośliwego oprogramowania dla systemów Window...</para>
///  <para>MG-PC        -------    KB4034658    1GB 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla syste...</para>
///  <para>MG-PC        -------    KB4034662   21MB 2017-08 Aktualizacja zabezpieczeń Adobe Flash Player w Windows 10 Version 1...</para>
///  <para>MG-PC        -------    KB4035631   11MB 2017-08 Aktualizacja Windows 10 Version 1607 dla systemów opartych na archi...</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Get all installed drivers that are available at Windows Update. Additionaly show pre search criteria.</para>
///
///  Get-WindowsUpdate -WindowsUpdate -UpdateType Driver -IsInstalled -ShowPreSearchCriteria -Verbose
///
///  <para>PreSearchCriteria: IsInstalled = 0 and Type = 'Driver' and IsHidden = 0</para>
///  <para>VERBOSE: MG-PC: Connecting to Windows Update server.Please wait...</para>
///  <para>VERBOSE: Found[1] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[1] Updates in post search criteria</para>
///  <para></para>
///  <para>ComputerName Status     KB          Size Title</para>
///  <para>------------ ------     --          ---- -----</para>
///  <para>MGAJDALAP3   -DI----                 3MB Intel - Other hardware - Intel(R) Watchdog Timer Driver (Intel(R) WDT)</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Get all available update on remote machine MG-PC, that contains in Title this two words 'Aktualizacja' and 'Windows 10' (as regular expression).</para>
///
///  Get-WindowsUpdate -ComputerName MG-PC -MicrosoftUpdate -Title "Aktualizacja.*Windows 10" -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Microsoft Update server. Please wait...</para>
///  <para>VERBOSE: Found[14] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[5] Updates in post search criteria</para>
///  <para></para>
///  <para>ComputerName Status     KB          Size Title</para>
///  <para>------------ ------     --          ---- -----</para>
///  <para>MG-PC        -------    KB3150513    2MB 2017-06 Aktualizacja Windows 10 Version 1607 dla systemów opartych na archi...</para>
///  <para>MG-PC        -------    KB4034658    1GB 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla syste...</para>
///  <para>MG-PC        -------    KB4034662   21MB 2017-08 Aktualizacja zabezpieczeń Adobe Flash Player w Windows 10 Version 1...</para>
///  <para>MG-PC        -------    KB4035631   11MB 2017-08 Aktualizacja Windows 10 Version 1607 dla systemów opartych na archi...</para>
///  <para>MG-PC        -------    KB4033637    4MB Aktualizacja systemu Windows 10 Version 1607 dla komputerów z procesorami x...</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Hide update with KBArticleID: KB4034658.</para>
///
///  Get-WindowsUpdate -KBArticleID KB4034658 -Hide -Verbose
///  <para>or use alias</para>
///  Hide-WindowsUpdate -KBArticleID KB4034658 -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Windows Server Update Service server. Please wait...</para>
///  <para>VERBOSE: Found[4] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[1] Updates in post search criteria</para>
///  <para></para>
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Hide 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla systemów opartych na architekturze x64 (KB4034658)[1GB]" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  <para></para>
///  <para>ComputerName Status     KB          Size Title</para>
///  <para>------------ ------     --          ---- -----</para>
///  <para>MG-PC        ---H--     KB4034658    1GB 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla syste...</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Unhide update with KBArticleID: KB4034658.</para>
///
///  Get-WindowsUpdate -KBArticleID KB4034658 -WithHidden -Hide:$false -Verbose 
///  <para>or use alias</para>
///  Show-WindowsUpdate -KBArticleID KB4034658 -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Windows Server Update Service server. Please wait...</para>
///  <para>VERBOSE: Found[4] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[1] Updates in post search criteria</para>
///  <para></para>
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Show 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla systemów opartych na architekturze x64 (KB4034658)[1GB]" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  <para></para>
///  <para>ComputerName Status     KB          Size Title</para>
///  <para>------------ ------     --          ---- -----</para>
///  <para>MG-PC        ------     KB4034658    1GB 2017-08 Aktualizacja zbiorcza dla systemu Windows 10 Version 1607 dla syste...</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Schedule job at 6:00 PM to install update with UpdateId='ddb74579-7a1f-4d1f-80c8-e8647055314e' and RevisionNumber=200. Update will be automaticaly accepted and after all serwer will be automaticaly restarted if needed. </para>
///
///  Get-WindowsUpdate -MicrosoftUpdate -UpdateID ddb74579-7a1f-4d1f-80c8-e8647055314e -RevisionNumber 200 -ScheduleJob (Get-Date -Hour 18 -Minute 0 -Second 0) -Install -AcceptAll -AutoReboot -Verbose
///  <para>or use alias</para>
///  Install-WindowsUpdate -MicrosoftUpdate -UpdateID ddb74579-7a1f-4d1f-80c8-e8647055314e -RevisionNumber 200 -ScheduleJob (Get-Date -Hour 18 -Minute 0 -Second 0) -AcceptAll -AutoReboot -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Microsoft Update server. Please wait...</para>
///  <para>VERBOSE: Found[1] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[1] Updates in post search criteria</para>
///  <para>VERBOSE: Choosed pre Search Criteria: (UpdateID = 'ddb74579-7a1f-4d1f-80c8-e8647055314e' and RevisionNumber = 200)</para>
///  <para></para>
///  <para>X ComputerName Result     KB          Size Title</para>
///  <para>- ------------ ------     --          ---- -----</para>
///  <para>1 MG-PC        Accepted   KB4023307   13MB Microsoft Silverlight(KB4023307)</para>
///  <para>VERBOSE: Accepted[1] Updates ready to Download</para>
///  <para>VERBOSE: Invoke-WUJob: MG-PC(31.08.2017 18:00:00):</para>
///  <para>VERBOSE: powershell.exe -Command "Get-WindowsUpdate -Criteria \"(UpdateID = 'ddb74579-7a1f-4d1f-80c8-e8647055314e' and RevisionNumber = 200)\" -AcceptAll -AutoReboot -Download -Install -MicrosoftUpdate -Verbose *&gt;&amp;1 | Out-File $Env:TEMP\PSWindowsUpdateNext.log"</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Install updates on remote computer. After all send a report from the installation process.</para>
///
///  Install-WindowsUpdate -ComputerName MG-PC -MicrosoftUpdate -AcceptAll -AutoReboot -SendReport -PSWUSettings @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";Port=25} -Verbose
///  <para>or use global PSWUSettings</para>
///  @{SmtpServer="your.smtp.server";From="sender@email.address";To="recipient@email.address";Port=25} | Export-Clixml -Path 'C:\Program Files\WindowsPowerShell\Modules\PSWindowsUpdateNext\PSWUSettings.xml'
///  Install-WindowsUpdate -ComputerName MG-PC -MicrosoftUpdate -AcceptAll -AutoReboot -SendReport -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Microsoft Update server. Please wait...</para>
///  <para>VERBOSE: Found[4] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[4] Updates in post search criteria</para>
///  <para></para>
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Microsoft Silverlight (KB4023307)[13MB]" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  <para></para>
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "2017-06 Aktualizacja Windows 10 Version 1607 dla systemów opartych na architekturze x64 (KB3150513)[2MB]" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): Y</para>
///  <para></para>
///  <para>Confirm</para>
///  <para>Are you sure you want to perform this action?</para>
///  <para>Performing the operation "Aktualizacja pakietu językowego usługi Microsoft Dynamics 365 2.1[47MB]" on target "MG-PC".</para>
///  <para>[Y] Yes[A] Yes to All  [N] No[L] No to All  [S] Suspend[?] Help (default is "Y"): L</para>
///  <para></para>
///  <para>X ComputerName Result     KB          Size Title</para>
///  <para>- ------------ ------     --          ---- -----</para>
///  <para>1 MG-PC Accepted KB4023307   13MB Microsoft Silverlight (KB4023307)</para>
///  <para>1 MG-PC Accepted KB3150513    2MB 2017-06 Aktualizacja Windows 10 Version 1607 dla systemów opartych na arc...</para>
///  <para>1 MG-PC Rejected KB4013759   47MB Aktualizacja pakietu językowego usługi Microsoft Dynamics 365 2.1</para>
///  <para>1 MG-PC Rejected KB3186568   67MB Program Microsoft .NET Framework 4.7 w syst. Windows 10 Version 1607 i Wi...</para>
///  <para>VERBOSE: Accepted [2]</para>
///  <para>Updates ready to Download</para>
///  <para>VERBOSE: Invoke-WUJob: MG-PC (Now):</para>
///  <para>VERBOSE: powershell.exe -Command "Get-WindowsUpdate -Criteria \"(UpdateID = 'ddb74579-7a1f-4d1f-80c8-e8647055314e' and RevisionNumber = 200) or (UpdateID = '151c4402-513c-4f39-8da1-f84d0956b5e3' and RevisionNumber = 200)\" -AcceptAll -Download -Install -AutoReboot -MicrosoftUpdate -SendReport -ProofOfLife -Verbose *&gt;&amp;1 | Out-File $Env:TEMP\PSWindowsUpdateNext.log"</para>
///  </code>
///  </example>
///  <example>
///  <code>
///  <para>Schedule Job to install all available updates and automatically reboot system if needed. Also send report after installation (but before reboot if needed) and send second instalation history report after reboot.</para>
///
///  Set-PSWUSettings -SmtpServer smtp.office365.com -SmtpPort 587 -SmtpEnableSsl $true -SmtpSubject "PSWindowsUpdateNext Report" -SmtpTo mgajda@psmvp.pl -SmtpFrom mgajda@psmvp.pl -SmtpCredential (Get-Credential mgajda@psmvp.pl)
///  Install-WindowsUpdate -MicrosoftUpdate -SendReport -SendHistory -AcceptAll -AutoReboot -ScheduleJob (Get-Date -Hour 18 -Minute 30 -Second 0) -ComputerName MG-PC -Verbose
///
///  <para>VERBOSE: MG-PC: Connecting to Microsoft Update server. Please wait...</para>
///  <para>VERBOSE: Found[4] Updates in pre search criteria</para>
///  <para>VERBOSE: Found[4] Updates in post search criteria</para>
///  <para></para>
///  <para>X ComputerName Result     KB          Size Title</para>
///  <para>- ------------ ------     --          ---- -----</para>
///  <para>1 MG-PC        Accepted   KB3038936    5MB Aktualizacja systemu Windows 8.1 dla komputerów z procesorami x64(KB3038...</para>
///  <para>1 MG-PC        Accepted   KB3186606    4MB Pakiety językowe programu Microsoft.NET Framework 4.7 w syst. Windows 8....</para>
///  <para>1 MG-PC        Accepted   KB4035038   53MB Sierpień 2017: wersja zapozn. pak.zb.aktual.jakości dla pr. .NET Frame...</para>
///  <para>1 MG-PC        Accepted   KB2267602  309MB Aktualizacja definicji dla: Windows Defender — KB2267602 (Definicja 1.251...</para>
///  <para>VERBOSE: Accepted[4] Updates ready to Download</para>
///  <para>VERBOSE: Invoke-WUJob: MG-PC (02.09.2017 08:30:00):</para>
///  <para>VERBOSE: powershell.exe -Command "Get-WindowsUpdate -Criteria \"(UpdateID = 'e69c9679-7ce8-489a-a21c-62fb920be67a' and RevisionNumber = 201) or(UpdateID = 'de44604d-ec38-4a7f-ac63-28b3edfdb382' and RevisionNumber = 207) or(UpdateID = '9cf1d8c9-a7c3-4603-90e8-f22131ff6d7e' and RevisionNumber = 201) or(UpdateID = 'b51935f9-0e40-4624-9c26-b29bff92dcf9' and RevisionNumber = 200)\" -AcceptAll -Install -AutoReboot -MicrosoftUpdate -SendReport -SendHistory -Verbose *&gt;&amp;1 | Out-File $Env:TEMP\PSWindowsUpdateNext.log"</para>
///  <para>VERBOSE: Send report</para>
///  </code>
///  </example>
[Cmdlet("Get", "WindowsUpdate", SupportsShouldProcess = true, DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.High)]
[OutputType(new Type[] { typeof(WindowsUpdate) })]
public class GetWindowsUpdate : PSCmdlet
{
	private Hashtable _PSWUSettings = new Hashtable();

	/// <summary>
	/// <para type="description">Specify one or more computer names for remote connection. Interactive remote connection works only for checking updates. For download or install cmdlet creates an Invoke-WUJob task.</para>
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
	/// <para type="description">Requires the parameter -PSWUSettings or declare the PSWUSettings.xml file (more preferred) in ModuleBase path.</para>
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
	/// <para type="description">Send install history (Get-WUHistory) report after successful update installation or system startup.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter SendHistory { get; set; }

	/// <summary>
	/// <para type="description">Specify time when job will start.</para>
	/// </summary>
	[Parameter]
	[ValidateDateTime]
	public DateTime ScheduleJob { get; set; }

	/// <summary>
	/// <para type="description">Do not ask confirmation for updates. Download or Install all available updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AcceptAll { get; set; }

	/// <summary>
	/// <para type="description">Specify number of cycles for check updates after successful update installation or system startup. First run is always main cycle (-RecurseCycle 1 or none). Second (-RecurseCycle 2) and n (-RecurseCycle n) cycle are recursive.</para>
	/// </summary>
	[Parameter]
	[ValidateRecurseCycle]
	public int RecurseCycle { get; set; }

	/// <summary>
	/// <para type="description">Get list of updates and hide/unhide approved updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Hide { get; set; }

	/// <summary>
	/// <para type="description">Get list of updates and download approved updates, but do not install it.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Download { get; set; }

	/// <summary>
	/// <para type="description">Forces the download of updates that are already installed or that cannot be installed. Works only with -Download.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter ForceDownload { get; set; }

	/// <summary>
	/// <para type="description">Get list of updates and install approved updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Install { get; set; }

	/// <summary>
	/// <para type="description">A forced installation is an installation in which an update is installed even if the metadata indicates that the update is already installed. Before you use ForceInstall to force an installation, determine whether the update is installed and available. If an update is not installed, a forced installation fails. Works only with -Install.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter ForceInstall { get; set; }

	/// <summary>
	/// <para type="description">Do not ask for reboot if it needed.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AutoReboot { get; set; }

	/// <summary>
	/// <para type="description">Do not ask for reboot if it needed, but do not reboot automaticaly.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IgnoreReboot { get; set; }

	/// <summary>
	/// <para type="description">Specify time when system will be rebooted.</para>
	/// </summary>
	[Parameter]
	[ValidateDateTime]
	public DateTime ScheduleReboot { get; set; }

	/// <summary>
	/// <para type="description">Use specific Service Manager if it's available. </para>
	/// <para type="description">Examples Of ServiceID: 
	/// \r\n
	/// \r\n -- Windows Update 				    9482f4b4-e343-43b6-b170-9a65bc822c77
	/// \r\n -- Microsoft Update 				7971f918-a847-4430-9279-4a52d1efe18d
	/// \r\n -- Windows Store 					117cab2d-82b1-4b5a-a08c-4d62dbee7782
	/// \r\n -- Windows Server Update Service 	3da21691-e39d-4da6-8a4b-b43877bcb1b7</para>
	/// </summary>
	[Parameter(ParameterSetName = "ServiceID")]
	public string ServiceID { get; set; }

	/// <summary>
	/// <para type="description">Use Microsoft Update Service Manager - '7971f918-a847-4430-9279-4a52d1efe18d'</para>
	/// </summary>
	[Parameter(ParameterSetName = "WindowsUpdate")]
	public SwitchParameter WindowsUpdate { get; set; }

	/// <summary>
	/// <para type="description">Use Windows Update Service Manager - '9482f4b4-e343-43b6-b170-9a65bc822c77'</para>
	/// </summary>
	[Parameter(ParameterSetName = "MicrosoftUpdate")]
	public SwitchParameter MicrosoftUpdate { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Set own string that specifies the search criteria. https://docs.microsoft.com/pl-pl/windows/desktop/api/wuapi/nf-wuapi-iupdatesearcher-search </para>
	/// </summary>
	[Parameter]
	public string Criteria { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates with a specific type, such as 'Driver' and 'Software'. Default value contains all updates.</para>
	/// </summary>
	[ValidateSet(new string[] { "Driver", "Software" }, IgnoreCase = true)]
	[Parameter]
	public string UpdateType { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that are deployed for a specific action, such as an installation or uninstallation that the administrator of a server specifies. "DeploymentAction='Installation'" finds updates that are deployed for installation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.</para>
	/// <para type="description">"DeploymentAction='Uninstallation'" finds updates that are deployed for uninstallation on a destination computer. "DeploymentAction='Uninstallation'" depends on the other query criteria.</para>
	/// <para type="description">If this criterion is not explicitly specified, each group of criteria that is joined to an AND operator implies "DeploymentAction='Installation'".</para>
	/// </summary>
	[ValidateSet(new string[] { "Installation", "Uninstallation" }, IgnoreCase = true)]
	[Parameter]
	public string DeploymentAction { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that are intended for deployment by Automatic Updates. "IsAssigned=1" finds updates that are intended for deployment by Automatic Updates, which depends on the other query criteria.At most, one assigned Windows-based driver update is returned for each local device on a destination computer.</para>
	/// <para type="description">"IsAssigned=0" finds updates that are not intended to be deployed by Automatic Updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IsAssigned { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. When set to 1, finds updates that are present on a computer.</para>
	/// <para type="description">"IsPresent=1" finds updates that are present on a destination computer.If the update is valid for one or more products, the update is considered present if it is installed for one or more of the products.</para>
	/// <para type="description">"IsPresent=0" finds updates that are not installed for any product on a destination computer.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IsPresent { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. "BrowseOnly=1" finds updates that are considered optional. "BrowseOnly=0" finds updates that are not considered optional.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter BrowseOnly { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates where the AutoSelectOnWebSites property has the specified value.</para>
	/// <para type="description">"AutoSelectOnWebSites=1" finds updates that are flagged to be automatically selected by Windows Update.</para>
	/// <para type="description">"AutoSelectOnWebSites=0" finds updates that are not flagged for Automatic Updates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AutoSelectOnWebSites { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates with a specific UUID (or sets of UUIDs), such as '12345678-9abc-def0-1234-56789abcdef0'.</para>
	/// </summary>
	[Parameter]
	public string[] UpdateID { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates without a specific UUID (or sets of UUIDs), such as '12345678-9abc-def0-1234-56789abcdef0'.</para>
	/// </summary>
	[Parameter]
	public string[] NotUpdateID { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates with a specific RevisionNumber, such as '100'. This criterion must be combined with the UpdateID param.</para>
	/// </summary>
	[Parameter]
	public int RevisionNumber { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that belong to a specified category (or sets of UUIDs), such as '0fa1201d-4330-4fa8-8ae9-b877473b6441'.</para>
	/// </summary>
	[Parameter]
	public string[] CategoryIDs { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that are installed on the destination computer.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IsInstalled { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that are marked as hidden on the destination computer. Default search criteria is only not hidden upadates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IsHidden { get; set; }

	/// <summary>
	/// <para type="description">Pre search criteria - native for WUAPI. Finds updates that are both hidden and not on the destination computer. Overwrite IsHidden param. Default search criteria is only not hidden upadates.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter WithHidden { get; set; }

	/// <summary>
	/// <para type="description">Show choosen search criteria. Only works for pre search criteria.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter ShowPreSearchCriteria { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that contain a specified root category name 'Critical Updates', 'Definition Updates', 'Drivers', 'Feature Packs', 'Security Updates', 'Service Packs', 'Tools', 'Update Rollups', 'Updates', 'Upgrades', 'Microsoft'.</para>
	/// </summary>
	[ValidateSet(new string[]
	{
		"Critical Updates", "Definition Updates", "Drivers", "Feature Packs", "Security Updates", "Service Packs", "Tools", "Update Rollups", "Updates", "Upgrades",
		"Microsoft"
	})]
	[Parameter]
	public string[] RootCategories { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that contain a specified category name (or sets of categories name), such as 'Updates', 'Security Updates', 'Critical Updates', etc...</para>
	/// </summary>
	[Parameter]
	public string[] Category { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that contain a KBArticleID (or sets of KBArticleIDs), such as 'KB982861'.</para>
	/// </summary>
	[Parameter]
	public string[] KBArticleID { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that match part of title (case sensitive), such as '.NET Framework 4'.</para>
	/// </summary>
	[Parameter]
	public string Title { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that match part of severity, such as 'Important', 'Critical', 'Moderate', etc...</para>
	/// </summary>
	[ValidateSet(new string[] { "Critical", "Important", "Moderate", "Low", "Unspecified" })]
	[Parameter]
	public string[] Severity { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that not contain a specified category name (or sets of categories name), such as 'Updates', 'Security Updates', 'Critical Updates', etc...</para>
	/// </summary>
	[Parameter]
	public string[] NotCategory { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that not contain a KBArticleID (or sets of KBArticleIDs), such as 'KB982861'.</para>
	/// </summary>
	[Parameter]
	public string[] NotKBArticleID { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that not match part of title (case sensitive).</para>
	/// </summary>
	[Parameter]
	public string NotTitle { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that not match part of severity.</para>
	/// </summary>
	[ValidateSet(new string[] { "Critical", "Important", "Moderate", "Low", "Unspecified" })]
	[Parameter]
	public string[] NotSeverity { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that the installation or uninstallation of an update can't prompt for user input.</para>
	/// </summary>
	[Alias(new string[] { "Silent" })]
	[Parameter]
	public SwitchParameter IgnoreUserInput { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that specifies the restart behavior that not occurs when you install or uninstall the update.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter IgnoreRebootRequired { get; set; }

	/// <summary>
	/// <para type="description">Install only the updates that have status AutoSelectOnWebsites on true.</para>
	/// </summary>
	[Parameter]
	public SwitchParameter AutoSelectOnly { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that have MaxDownloadSize less or equal. Size is in Bytes.</para>
	/// </summary>
	[Parameter]
	public long MaxSize { get; set; }

	/// <summary>
	/// <para type="description">Post search criteria. Finds updates that have MaxDownloadSize greater or equal. Size is in Bytes.</para>
	/// </summary>
	[Parameter]
	public long MinSize { get; set; }

	/// <summary>
	/// <para type="description">Debuger return original exceptions. For additional debug information use $DebugPreference = "Continue"</para>
	/// </summary>
	[Parameter]
	public SwitchParameter Debuger { get; set; }

	private static WUTools WUToolsObj { get; set; }

	private static Collection<PSObject> OutputObj { get; set; }

	private static UpdateSession UpdateSessionObj { get; set; }

	private static IUpdateSearcher SearcherObj { get; set; }

	private static UpdateServiceManager ServiceManagerObj { get; set; }

	private static bool NeedsReboot { get; set; }

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
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "Status", "KB", "Size", "Title" });
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
		WriteDebug(DateTime.Now.ToString() + " ParameterSetName: " + ParameterSetName);
		if (string.Equals(invocationName.ToString(), "Install-WindowsUpdate", StringComparison.OrdinalIgnoreCase))
		{
			Download = true;
			Install = true;
		}
		if (string.Equals(invocationName.ToString(), "Download-WindowsUpdate", StringComparison.OrdinalIgnoreCase))
		{
			Download = true;
		}
		if (string.Equals(invocationName.ToString(), "Hide-WindowsUpdate", StringComparison.OrdinalIgnoreCase))
		{
			Hide = true;
		}
		if (string.Equals(invocationName.ToString(), "Show-WindowsUpdate", StringComparison.OrdinalIgnoreCase) || string.Equals(invocationName.ToString(), "UnHide-WindowsUpdate", StringComparison.OrdinalIgnoreCase))
		{
			Hide = false;
			WithHidden = true;
		}
		string text;
		if (Criteria != null)
		{
			text = Criteria;
			WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: " + text);
		}
		else
		{
			if ((bool)IsInstalled)
			{
				text = "IsInstalled = 1";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsInstalled = 1");
			}
			else
			{
				text = "IsInstalled = 0";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsInstalled = 0");
			}
			if (UpdateType != null)
			{
				text = text + " and Type = '" + UpdateType + "'";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: Type = '" + UpdateType + "'");
			}
			if ((bool)IsHidden)
			{
				text += " and IsHidden = 1";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsHidden = 1");
			}
			else if ((bool)WithHidden)
			{
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsHidden = 1 or IsHidden = 0");
			}
			else
			{
				text += " and IsHidden = 0";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsHidden = 0");
			}
			if ((bool)IgnoreRebootRequired)
			{
				text += " and RebootRequired = 0";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: RebootRequired = 0");
			}
			if (DeploymentAction != null)
			{
				text = text + " and DeploymentAction = '" + DeploymentAction + "'";
				WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: DeploymentAction = '" + DeploymentAction + "'");
			}
			if (MyInvocation.BoundParameters.ContainsKey("IsAssigned"))
			{
				if ((bool)IsAssigned)
				{
					text += " and IsAssigned = 1";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsAssigned = 1");
				}
				else
				{
					text += " and IsAssigned = 0";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsAssigned = 0");
				}
			}
			if (MyInvocation.BoundParameters.ContainsKey("IsPresent"))
			{
				if ((bool)IsPresent)
				{
					text += " and IsPresent = 1";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsPresent = 1");
				}
				else
				{
					text += " and IsPresent = 0";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: IsPresent = 0");
				}
			}
			if (MyInvocation.BoundParameters.ContainsKey("BrowseOnly"))
			{
				if ((bool)BrowseOnly)
				{
					text += " and BrowseOnly = 1";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: BrowseOnly = 1");
				}
				else
				{
					text += " and BrowseOnly = 0";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: BrowseOnly = 0");
				}
			}
			if (MyInvocation.BoundParameters.ContainsKey("AutoSelectOnWebSites"))
			{
				if ((bool)AutoSelectOnWebSites)
				{
					text += " and AutoSelectOnWebSites = 1";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: AutoSelectOnWebSites = 1");
				}
				else
				{
					text += " and AutoSelectOnWebSites = 0";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: AutoSelectOnWebSites = 0");
				}
			}
			if (NotUpdateID != null)
			{
				string[] notUpdateID = NotUpdateID;
				foreach (string text2 in notUpdateID)
				{
					text = text + " and UpdateID != '" + text2 + "'";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: NotUpdateID = " + text2);
				}
			}
			if (UpdateID != null)
			{
				string text3 = text;
				text = "";
				int num = 0;
				string[] updateID = UpdateID;
				foreach (string text4 in updateID)
				{
					if (num > 0)
					{
						text += " or ";
						WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: or ");
					}
					if (RevisionNumber > 0)
					{
						text = text + "(" + text3 + " and UpdateID = '" + text4 + "' and RevisionNumber = " + RevisionNumber + ")";
						WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: UpdateID = " + text4 + "and RevisionNumber = " + RevisionNumber);
					}
					else
					{
						text = text + "(" + text3 + " and UpdateID = '" + text4 + "')";
						WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: UpdateID = " + text4);
					}
					num++;
				}
			}
			if (CategoryIDs != null)
			{
				string text5 = text;
				text = "";
				int num2 = 0;
				string[] categoryIDs = CategoryIDs;
				foreach (string text6 in categoryIDs)
				{
					if (num2 > 0)
					{
						text += " or ";
						WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: or ");
					}
					text = text + "(" + text5 + " and CategoryIDs contains '" + text6 + "')";
					WriteDebug(DateTime.Now.ToString() + " Set pre search criteria: CategoryIDs = " + text6);
					num2++;
				}
			}
		}
		WriteDebug(DateTime.Now.ToString() + " Search criteria is: " + text);
		if ((bool)ShowPreSearchCriteria)
		{
			Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, "PreSearchCriteria: " + text);
		}
		string[] computerName = ComputerName;
		foreach (string text7 in computerName)
		{
			WriteDebug(DateTime.Now.ToString() + " " + text7 + ": Connecting...");
			try
			{
				PSObject pSWUModule = WUToolsObj.GetPSWUModule(text7);
				WriteDebug(DateTime.Now.ToString() + " Module version: " + pSWUModule.Properties["Version"].Value);
				WriteDebug(DateTime.Now.ToString() + " Dll version: " + pSWUModule.Properties["PSWUDllVersion"].Value);
			}
			catch
			{
			}
			if ((bool)Download)
			{
				ErrorRecord errorRecord = WUToolsObj.CheckPSWUModule(text7);
				if (errorRecord != null)
				{
					WriteError(errorRecord);
					continue;
				}
			}
			WUAPIObject wUApiUpdateSessionObj = WUToolsObj.GetWUApiUpdateSessionObj(text7);
			WriteDebug(DateTime.Now.ToString() + " UpdateSessionObj mode: " + wUApiUpdateSessionObj.Mode);
			if (wUApiUpdateSessionObj.Status)
			{
				UpdateSessionObj = (UpdateSession)wUApiUpdateSessionObj.Object;
				SearcherObj = UpdateSessionObj.CreateUpdateSearcher();
				WUAPIObject wUApiServiceManagerObj = WUToolsObj.GetWUApiServiceManagerObj(text7);
				WriteDebug(DateTime.Now.ToString() + " ServiceManagerObj mode: " + wUApiServiceManagerObj.Mode);
				if (wUApiServiceManagerObj.Status)
				{
					ServiceManagerObj = (UpdateServiceManager)wUApiServiceManagerObj.Object;
					string text8 = "";
					if ((bool)WindowsUpdate)
					{
						WriteDebug(DateTime.Now.ToString() + " Set source of updates to Windows Update");
						SearcherObj.ServerSelection = ServerSelection.ssWindowsUpdate;
						text8 = "Windows Update";
					}
					else if ((bool)MicrosoftUpdate)
					{
						WriteDebug(DateTime.Now.ToString() + " Set source of updates to Microsof Update");
						ServiceID = "7971f918-a847-4430-9279-4a52d1efe18d";
						bool flag = false;
						foreach (IUpdateService2 service in ServiceManagerObj.Services)
						{
							if (service.ServiceID == ServiceID)
							{
								WriteDebug(DateTime.Now.ToString() + " Search for ServiceID. Set source of updates to " + service.Name);
								SearcherObj.ServerSelection = ServerSelection.ssOthers;
								SearcherObj.ServiceID = ServiceID;
								text8 = service.Name;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							try
							{
								Thread.Sleep(500);
								ServiceController serviceController = new ServiceController("Windows Update", text7);
								serviceController.Stop();
								serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
								Thread.Sleep(500);
								serviceController.Start();
								serviceController.WaitForStatus(ServiceControllerStatus.Running);
								ServiceManagerObj.AddService2("7971f918-a847-4430-9279-4a52d1efe18d", 2, null);
								WriteDebug(DateTime.Now.ToString() + " Register WUA Microsof Update service");
								Thread.Sleep(500);
								serviceController.Stop();
								serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
								Thread.Sleep(500);
								serviceController.Start();
								serviceController.WaitForStatus(ServiceControllerStatus.Running);
							}
							catch (COMException ex)
							{
								WUApiCode wUApiCodeDetails = WUToolsObj.GetWUApiCodeDetails(ex.ErrorCode);
								if (wUApiCodeDetails != null)
								{
									switch (wUApiCodeDetails.CodeType)
									{
									case 2:
										WriteError(new ErrorRecord(new Exception(wUApiCodeDetails.Description), wUApiCodeDetails.HResult, ErrorCategory.CloseError, null));
										break;
									case 3:
										WriteWarning(wUApiCodeDetails.HResult + ": " + wUApiCodeDetails.Description);
										break;
									}
								}
								else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
								{
									ErrorRecord errorRecord2 = new ErrorRecord(ex, "Debug", ErrorCategory.CloseError, null);
									ThrowTerminatingError(errorRecord2);
								}
							}
							SearcherObj.ServerSelection = ServerSelection.ssOthers;
							SearcherObj.ServiceID = "7971f918-a847-4430-9279-4a52d1efe18d";
							text8 = "Microsoft Update";
						}
					}
					else
					{
						try
						{
							foreach (IUpdateService2 service2 in ServiceManagerObj.Services)
							{
								if (ServiceID != null)
								{
									if (service2.ServiceID == ServiceID)
									{
										WriteDebug(DateTime.Now.ToString() + " Try ServiceID. Set source of updates to " + service2.Name);
										SearcherObj.ServerSelection = ServerSelection.ssOthers;
										SearcherObj.ServiceID = ServiceID;
										text8 = service2.Name;
										break;
									}
								}
								else if (service2.IsDefaultAUService)
								{
									WriteDebug(DateTime.Now.ToString() + " Try Default. Set source of updates to " + service2.Name);
									text8 = service2.Name;
									break;
								}
							}
						}
						catch
						{
							if (ServiceID != null)
							{
								WriteDebug(DateTime.Now.ToString() + " Catch ServiceID. Set source of updates to " + ServiceID);
								SearcherObj.ServerSelection = ServerSelection.ssOthers;
								SearcherObj.ServiceID = ServiceID;
								text8 = ServiceID;
							}
							else
							{
								WriteDebug(DateTime.Now.ToString() + " Catch. Set source of updates to Default");
								text8 = "default for " + text7 + ".";
							}
						}
					}
					if (text8 == "Windows Server Update Service")
					{
						RegistryKey registryKey = null;
						registryKey = ((!WUToolsObj.IsLocalHost(text7)) ? RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, text7) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default));
						RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\");
						string text9 = registryKey2.GetValue("WUServer").ToString();
						text8 = text8 + " (" + text9 + ")";
					}
					WriteVerbose(text7 + " (" + DateTime.Now.ToString() + "): Connecting to " + text8 + " server. Please wait...");
					ISearchResult searchResult;
					try
					{
						searchResult = SearcherObj.Search(text);
					}
					catch (COMException ex2)
					{
						WUApiCode wUApiCodeDetails2 = WUToolsObj.GetWUApiCodeDetails(ex2.ErrorCode);
						if (wUApiCodeDetails2 != null)
						{
							switch (wUApiCodeDetails2.CodeType)
							{
							case 2:
								WriteError(new ErrorRecord(new Exception(wUApiCodeDetails2.Description), wUApiCodeDetails2.HResult, ErrorCategory.CloseError, null));
								break;
							case 3:
								WriteWarning(wUApiCodeDetails2.HResult + ": " + wUApiCodeDetails2.Description);
								break;
							}
						}
						else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
						{
							ErrorRecord errorRecord3 = new ErrorRecord(ex2, "Debug", ErrorCategory.CloseError, null);
							ThrowTerminatingError(errorRecord3);
						}
						WriteDebug(DateTime.Now.ToString() + " Skip to next computer");
						continue;
					}
					int count = searchResult.Updates.Count;
					WriteVerbose("Found [" + count + "] Updates in pre search criteria");
					if (count == 0)
					{
						continue;
					}
					UpdateCollection updateCollection = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
					if (RootCategories != null)
					{
						UpdateCollection updateCollection2 = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
						string[] rootCategories = RootCategories;
						foreach (string text10 in rootCategories)
						{
							int index = -1;
							switch (text10)
							{
							case "Critical Updates":
								index = 0;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Definition Updates":
								index = 1;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Drivers":
								index = 2;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Feature Packs":
								index = 3;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Security Updates":
								index = 4;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Service Packs":
								index = 5;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Tools":
								index = 6;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Update Rollups":
								index = 7;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Updates":
								index = 8;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Upgrades":
								index = 9;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							case "Microsoft":
								index = 10;
								WriteDebug(DateTime.Now.ToString() + " RootCategory: " + text10 + " - " + index);
								break;
							}
							try
							{
								foreach (IUpdate update6 in searchResult.RootCategories[index].Updates)
								{
									updateCollection2.Add(update6);
								}
							}
							catch
							{
								WriteWarning("RootCategiries Updates are empty. Use classic filters.");
							}
						}
						updateCollection = updateCollection2;
					}
					else
					{
						updateCollection = searchResult.Updates;
					}
					int num3 = 0;
					int activityId = 0;
					string activity = "Post search updates for " + text7;
					string statusDescription = "[" + num3 + "/" + count + "]";
					ProgressRecord progressRecord = new ProgressRecord(activityId, activity, statusDescription);
					Collection<PSObject> collection = new Collection<PSObject>();
					foreach (IUpdate item in updateCollection)
					{
						WriteDebug(DateTime.Now.ToString() + " " + item.Title);
						string text11 = WUToolsObj.ConvertSize(item.MaxDownloadSize);
						progressRecord.StatusDescription = "[" + num3 + "/" + count + "] " + item.Title + " " + text11;
						progressRecord.PercentComplete = num3 * 100 / count;
						WriteProgress(progressRecord);
						num3++;
						bool flag2 = true;
						if (Category != null || NotCategory != null)
						{
							string text12 = "";
							foreach (ICategory category3 in item.Categories)
							{
								text12 = text12 + category3.Name + "|";
							}
							WriteDebug(DateTime.Now.ToString() + " " + text12);
							if (Category != null)
							{
								WriteDebug(DateTime.Now.ToString() + " Set post search criteria: Categories = " + string.Join(", ", Category));
								string[] category2 = Category;
								foreach (string pattern in category2)
								{
									if (!Regex.IsMatch(text12, pattern, RegexOptions.IgnoreCase))
									{
										flag2 = false;
										continue;
									}
									flag2 = true;
									break;
								}
							}
							if ((NotCategory != null) & flag2)
							{
								WriteDebug(DateTime.Now.ToString() + " Set post search criteria: NotCategories = " + string.Join(", ", NotCategory));
								string[] notCategory = NotCategory;
								foreach (string pattern2 in notCategory)
								{
									if (Regex.IsMatch(text12, pattern2, RegexOptions.IgnoreCase))
									{
										flag2 = false;
										break;
									}
								}
							}
							WriteDebug(DateTime.Now.ToString() + " UpdateAccess: " + flag2);
						}
						if ((KBArticleID != null) & flag2)
						{
							string text13 = string.Join(", ", KBArticleID);
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: KBArticleIDs = " + text13);
							if (item.KBArticleIDs.Count > 0)
							{
								foreach (string kBArticleID in item.KBArticleIDs)
								{
									if (!Regex.IsMatch(text13, kBArticleID.ToString(), RegexOptions.IgnoreCase))
									{
										flag2 = false;
										break;
									}
								}
							}
							else
							{
								flag2 = false;
							}
							WriteDebug(DateTime.Now.ToString() + " UpdateAccess: " + flag2);
						}
						if ((NotKBArticleID != null) & flag2)
						{
							string text15 = string.Join(", ", NotKBArticleID);
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: NotKBArticleID = " + text15);
							if (item.KBArticleIDs.Count > 0)
							{
								foreach (string kBArticleID2 in item.KBArticleIDs)
								{
									if (Regex.IsMatch(text15, kBArticleID2, RegexOptions.IgnoreCase))
									{
										flag2 = false;
										break;
									}
								}
							}
							WriteDebug(DateTime.Now.ToString() + " UpdateAccess: " + flag2);
						}
						if ((Title != null) & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: Title = " + Title);
							if (!Regex.IsMatch(item.Title.ToString(), Title, RegexOptions.IgnoreCase))
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess: " + flag2);
							}
						}
						if ((NotTitle != null) & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: NotTitle = " + NotTitle);
							if (Regex.IsMatch(item.Title.ToString(), NotTitle, RegexOptions.IgnoreCase))
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess: " + flag2);
							}
						}
						if ((Severity != null) & flag2)
						{
							string text16 = string.Join(", ", Severity);
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: Severity = " + text16);
							if (Severity.Contains("Unspecified"))
							{
								Severity = string.Join(",", Severity).Replace("Unspecified", "").Split(',');
							}
							if (!Severity.Contains(Convert.ToString((object)item.MsrcSeverity)))
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((NotSeverity != null) & flag2)
						{
							string text17 = string.Join(", ", NotSeverity);
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: NotSeverity = " + text17);
							if (NotSeverity.Contains("Unspecified"))
							{
								NotSeverity = string.Join(",", NotSeverity).Replace("Unspecified", "").Split(',');
							}
							if (NotSeverity.Contains(Convert.ToString((object)item.MsrcSeverity)))
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((MaxSize > 0) & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: MaxDownloadSize <= " + MaxSize);
							if ((decimal)MaxSize <= item.MaxDownloadSize)
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((MinSize > 0) & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: MaxDownloadSize >= " + MinSize);
							if ((decimal)MinSize >= item.MaxDownloadSize)
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((bool)IgnoreUserInput & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: CanRequestUserInput");
							if (item.InstallationBehavior.CanRequestUserInput)
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((bool)IgnoreRebootRequired & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: RebootBehavior");
							if (item.InstallationBehavior.RebootBehavior != InstallationRebootBehavior.irbNeverReboots)
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if ((bool)AutoSelectOnly & flag2)
						{
							WriteDebug(DateTime.Now.ToString() + " Set post search criteria: AutoSelectOnWebsites");
							if (!item.AutoSelectOnWebSites)
							{
								flag2 = false;
								WriteDebug(DateTime.Now.ToString() + " UpdateAccess" + flag2);
							}
						}
						if (!flag2)
						{
							continue;
						}
						WriteDebug(DateTime.Now.ToString() + " Update was not filtered");
						string text18 = "";
						foreach (string kBArticleID3 in item.KBArticleIDs)
						{
							text18 = ((!(text18 == "")) ? (text18 + ", KB" + kBArticleID3) : (text18 + "KB" + kBArticleID3));
						}
						string text20 = "-";
						text20 = ((!item.IsDownloaded) ? (text20 + "-") : (text20 + "D"));
						text20 = ((!item.IsInstalled) ? (text20 + "-") : (text20 + "I"));
						text20 = ((!item.IsMandatory) ? (text20 + "-") : (text20 + "M"));
						text20 = ((!item.IsHidden) ? (text20 + "-") : (text20 + "H"));
						text20 = ((!item.IsUninstallable) ? (text20 + "-") : (text20 + "U"));
						text20 = ((!item.IsBeta) ? (text20 + "-") : (text20 + "B"));
						PSObject pSObject = new PSObject(item);
						pSObject.Properties.Add(new PSNoteProperty("Size", text11));
						pSObject.Properties.Add(new PSNoteProperty("Status", text20));
						pSObject.Properties.Add(new PSNoteProperty("ComputerName", text7));
						pSObject.Properties.Add(new PSNoteProperty("KB", text18));
						pSObject.TypeNames.Clear();
						pSObject.TypeNames.Add("PSWindowsUpdateNext.WindowsUpdate");
						collection.Add(pSObject);
					}
					progressRecord.RecordType = ProgressRecordType.Completed;
					WriteProgress(progressRecord);
					int count2 = collection.Count;
					WriteVerbose("Found [" + count2 + "] Updates in post search criteria");
					if (MyInvocation.BoundParameters.ContainsKey("Hide") || string.Equals(MyInvocation.InvocationName.ToString(), "Hide-WindowsUpdate", StringComparison.OrdinalIgnoreCase) || string.Equals(MyInvocation.InvocationName.ToString(), "Show-WindowsUpdate", StringComparison.OrdinalIgnoreCase) || string.Equals(MyInvocation.InvocationName.ToString(), "UnHide-WindowsUpdate", StringComparison.OrdinalIgnoreCase))
					{
						string text21 = "";
						text21 = ((!Hide) ? "Show" : "Hide");
						int num5 = 0;
						int activityId2 = 1;
						string activity2 = text21 + " updates for " + text7;
						string statusDescription2 = "[" + num5 + "/" + count2 + "]";
						ProgressRecord progressRecord2 = new ProgressRecord(activityId2, activity2, statusDescription2);
						foreach (PSObject item2 in collection)
						{
							IUpdate update2 = (IUpdate)item2.BaseObject;
							bool flag3 = false;
							progressRecord2.StatusDescription = "[" + num5 + "/" + count2 + "] " + update2.Title + " " + item2.Properties["Size"].Value.ToString();
							progressRecord2.PercentComplete = num5 * 100 / count2;
							WriteProgress(progressRecord2);
							num5++;
							if ((bool)AcceptAll)
							{
								flag3 = true;
							}
							else if (ShouldProcess(text7, "(" + DateTime.Now.ToString() + ") " + text21 + " " + update2.Title + "[" + item2.Properties["Size"].Value.ToString() + "]"))
							{
								flag3 = true;
							}
							if (flag3)
							{
								try
								{
									update2.IsHidden = Hide;
									item2.Properties.Add(new PSNoteProperty("HideResult", text21));
								}
								catch
								{
									ErrorRecord errorRecord4 = new ErrorRecord(new Exception(text7 + ": You don't have permission to perform this task."), "0x80240044", ErrorCategory.CloseError, null);
									ThrowTerminatingError(errorRecord4);
								}
							}
							string text22 = "";
							text22 = ((!update2.IsDownloaded) ? (text22 + "-") : (text22 + "D"));
							text22 = ((!update2.IsInstalled) ? (text22 + "-") : (text22 + "I"));
							text22 = ((!update2.IsMandatory) ? (text22 + "-") : (text22 + "M"));
							text22 = ((!update2.IsHidden) ? (text22 + "-") : (text22 + "H"));
							text22 = ((!update2.IsUninstallable) ? (text22 + "-") : (text22 + "U"));
							text22 = ((!update2.IsBeta) ? (text22 + "-") : (text22 + "B"));
							item2.Properties["Status"].Value = text22;
						}
						progressRecord2.RecordType = ProgressRecordType.Completed;
						WriteProgress(progressRecord2);
					}
					if (!Download && !Install)
					{
						WriteDebug(DateTime.Now.ToString() + " Return update list only");
						// Enumerate so each update is its own pipeline object; the host unwraps a bare
						// collection at the prompt, but a pipeline or runspace consumer sees one blob.
						WriteObject(collection, enumerateCollection: true);
						OutputObj = new Collection<PSObject>(collection);
						continue;
					}
					int num6 = 0;
					int num7 = 0;
					string text23 = "";
					if ((bool)Download || (bool)Install)
					{
						int num8 = 0;
						int activityId3 = 1;
						string activity3 = "Choose updates for " + text7;
						string statusDescription3 = "[" + num8 + "/" + count2 + "]";
						ProgressRecord progressRecord3 = new ProgressRecord(activityId3, activity3, statusDescription3);
						string text24 = "";
						foreach (PSObject item3 in collection)
						{
							item3.Properties.Add(new PSNoteProperty("X", 1));
							item3.TypeNames.Clear();
							item3.TypeNames.Add("PSWindowsUpdateNext.WindowsUpdateJob");
							IUpdate update3 = (IUpdate)item3.BaseObject;
							progressRecord3.StatusDescription = "[" + num8 + "/" + count2 + "] " + update3.Title + " " + item3.Properties["Size"].Value.ToString();
							progressRecord3.PercentComplete = num8 * 100 / count2;
							WriteProgress(progressRecord3);
							num8++;
							WriteDebug(DateTime.Now.ToString() + " Show update to accept: " + update3.Title);
							bool flag4 = false;
							flag4 = (bool)AcceptAll || (AutoSelectOnly ? (update3.AutoSelectOnWebSites ? true : false) : (ShouldProcess(text7, "(" + DateTime.Now.ToString() + ") " + update3.Title + "[" + item3.Properties["Size"].Value.ToString() + "]") ? true : false));
							string text25 = "";
							string text26 = "";
							if (flag4)
							{
								if (!update3.EulaAccepted)
								{
									WriteDebug(DateTime.Now.ToString() + " EulaAccepted");
									try
									{
										update3.AcceptEula();
									}
									catch (Exception exception)
									{
										if ((bool)Debuger)
										{
											ErrorRecord errorRecord5 = new ErrorRecord(exception, "Debug", ErrorCategory.CloseError, null);
											WriteError(errorRecord5);
										}
										else
										{
											WriteDebug(DateTime.Now.ToString() + " Can't accept Eula.");
										}
									}
								}
								try
								{
									PSObject pSObject2 = new PSObject(item3.Properties["Identity"].Value);
									string text27 = (string)pSObject2.Properties["UpdateID"].Value;
									int num9 = (int)pSObject2.Properties["RevisionNumber"].Value;
									text24 = ((!(text24 == "")) ? (text24 + " or (UpdateID = '" + text27 + "' and RevisionNumber = " + num9 + ")") : ("(UpdateID = '" + text27 + "' and RevisionNumber = " + num9 + ")"));
								}
								catch (Exception ex3)
								{
									flag4 = false;
									ErrorRecord errorRecord6 = new ErrorRecord(new Exception("Something goes wrong: " + update3.Title + "; " + ex3.Message), "Debug", ErrorCategory.CloseError, null);
									WriteError(errorRecord6);
								}
								if (flag4)
								{
									text25 += "A";
									text26 = "Accepted";
									WriteDebug(DateTime.Now.ToString() + " " + text26);
									item3.Properties.Add(new PSNoteProperty("ChooseResult", text26));
									item3.Properties.Add(new PSNoteProperty("Result", text26));
								}
							}
							if (!flag4)
							{
								try
								{
									PSObject pSObject3 = new PSObject(item3.Properties["Identity"].Value);
									string text28 = (string)pSObject3.Properties["UpdateID"].Value;
									int num10 = (int)pSObject3.Properties["RevisionNumber"].Value;
									text23 = ((!(text23 == "")) ? (text23 + ",'" + text28 + "'") : ("'" + text28 + "'"));
								}
								catch (Exception ex4)
								{
									flag4 = false;
									ErrorRecord errorRecord7 = new ErrorRecord(new Exception("Something goes wrong: " + update3.Title + "; " + ex4.Message), "Debug", ErrorCategory.CloseError, null);
									WriteError(errorRecord7);
								}
								text25 += "R";
								text26 = "Rejected";
								WriteDebug(DateTime.Now.ToString() + " " + text26);
								item3.Properties.Add(new PSNoteProperty("ChooseResult", text26));
								item3.Properties.Add(new PSNoteProperty("Result", text26));
							}
							if (!flag4 || (WUToolsObj.IsLocalHost(text7) && !(ScheduleJob != DateTime.MinValue)))
							{
								text25 = ((!update3.IsDownloaded) ? (text25 + "-") : (text25 + "D"));
								text25 = ((!update3.IsInstalled) ? (text25 + "-") : (text25 + "I"));
							}
							else
							{
								if ((bool)Download || (bool)Install)
								{
									text25 += "?";
								}
								if ((bool)Install)
								{
									text25 += "?";
								}
							}
							text25 = ((!update3.IsMandatory) ? (text25 + "-") : (text25 + "M"));
							text25 = ((!update3.IsHidden) ? (text25 + "-") : (text25 + "H"));
							text25 = ((!update3.IsUninstallable) ? (text25 + "-") : (text25 + "U"));
							text25 = ((!update3.IsBeta) ? (text25 + "-") : (text25 + "B"));
							item3.Properties["Status"].Value = text25;
						}
						progressRecord3.RecordType = ProgressRecordType.Completed;
						WriteProgress(progressRecord3);
						if ((bool)ShowPreSearchCriteria)
						{
							WriteVerbose("Choosed pre Search Criteria: " + text24);
						}
						int num11 = collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Accepted").Count();
						WriteObject(collection, enumerateCollection: true);
						WriteVerbose("Accepted [" + num11 + "] Updates ready to Download");
						if (num11 > 0 && (!WUToolsObj.IsLocalHost(text7) || ScheduleJob != DateTime.MinValue))
						{
							string text29 = "";
							text29 = ((!Debuger) ? "Get-WindowsUpdate -AcceptAll" : "$DebugPreference = 'Continue'; Get-WindowsUpdate -AcceptAll");
							text29 = text29 + " -Criteria \\\"" + text24 + "\\\"";
							if (RecurseCycle > 1)
							{
								if (text23 != "")
								{
									text29 = text29 + " -NotUpdateID " + text23;
								}
								if ((bool)IsInstalled)
								{
									text29 += " -IsInstalled";
								}
								if ((bool)IsHidden)
								{
									text29 += " -IsHidden";
								}
								if ((bool)WithHidden)
								{
									text29 += " -WithHidden";
								}
								if (UpdateType != null)
								{
									text29 = text29 + " -UpdateType " + UpdateType;
								}
								if (DeploymentAction != null)
								{
									text29 = text29 + " -DeploymentAction " + DeploymentAction;
								}
								if (UpdateID != null)
								{
									text29 = text29 + " -UpdateID '" + string.Join("','", UpdateID) + "'";
								}
								if (RevisionNumber > 0)
								{
									text29 = text29 + " -RevisionNumber " + RevisionNumber;
								}
								if (CategoryIDs != null)
								{
									text29 = text29 + " -CategoryIDs " + string.Join(",", CategoryIDs);
								}
								if (MyInvocation.BoundParameters.ContainsKey("IsAssigned"))
								{
									text29 = ((!IsAssigned) ? (text29 + " -IsAssigned:$false") : (text29 + " -IsAssigned"));
								}
								if (MyInvocation.BoundParameters.ContainsKey("IsPresent"))
								{
									text29 = ((!IsPresent) ? (text29 + " -IsPresent:$false") : (text29 + " -IsPresent"));
								}
								if (MyInvocation.BoundParameters.ContainsKey("AutoSelectOnWebSites"))
								{
									text29 = ((!AutoSelectOnWebSites) ? (text29 + " -AutoSelectOnWebSites:$false") : (text29 + " -AutoSelectOnWebSites"));
								}
								if (RootCategories != null)
								{
									text29 = text29 + " -RootCategories '" + string.Join("','", RootCategories) + "'";
								}
								if (Category != null)
								{
									text29 = text29 + " -Category '" + string.Join("','", Category) + "'";
								}
								if (KBArticleID != null)
								{
									text29 = text29 + " -KBArticleID '" + string.Join("','", KBArticleID) + "'";
								}
								if (Title != null)
								{
									text29 = text29 + " -Title '" + Title + "'";
								}
								if (Severity != null)
								{
									text29 = text29 + " -Severity '" + string.Join("','", Severity) + "'";
								}
								if (NotCategory != null)
								{
									text29 = text29 + " -NotCategory '" + string.Join("','", NotCategory) + "'";
								}
								if (NotKBArticleID != null)
								{
									text29 = text29 + " -NotKBArticleID '" + string.Join("','", NotKBArticleID) + "'";
								}
								if (NotTitle != null)
								{
									text29 = text29 + " -NotTitle '" + NotTitle + "'";
								}
								if (NotSeverity != null)
								{
									text29 = text29 + " -NotSeverity '" + string.Join("','", NotSeverity) + "'";
								}
								if ((bool)IgnoreUserInput)
								{
									text29 += " -IgnoreUserInput";
								}
								if ((bool)IgnoreRebootRequired)
								{
									text29 += " -IgnoreRebootRequired";
								}
								if ((bool)AutoSelectOnly)
								{
									text29 += " -AutoSelectOnly";
								}
								if (MaxSize > 0)
								{
									text29 = text29 + " -MaxSize " + MaxSize;
								}
								if (MinSize > 0)
								{
									text29 = text29 + " -MinSize " + MinSize;
								}
							}
							if ((bool)Download)
							{
								text29 += " -Download";
							}
							if ((bool)Install)
							{
								text29 += " -Install";
							}
							if ((bool)IgnoreReboot)
							{
								text29 += " -IgnoreReboot";
							}
							else if ((bool)AutoReboot)
							{
								text29 += " -AutoReboot";
							}
							else if (ScheduleReboot != DateTime.MinValue)
							{
								text29 = text29 + " -ScheduleReboot '" + ScheduleReboot.ToString() + "'";
							}
							if ((bool)WindowsUpdate)
							{
								text29 += " -WindowsUpdate";
							}
							else if ((bool)MicrosoftUpdate)
							{
								text29 += " -MicrosoftUpdate";
							}
							else if (ServiceID != null)
							{
								text29 = text29 + " -ServiceID '" + ServiceID + "'";
							}
							if ((bool)SendReport)
							{
								text29 += " -SendReport";
							}
							if ((bool)SendHistory)
							{
								text29 += " -SendHistory";
							}
							if (RecurseCycle > 1)
							{
								text29 = text29 + " -RecurseCycle " + RecurseCycle;
							}
							if (!AutoReboot && ScheduleReboot == DateTime.MinValue && !IgnoreReboot)
							{
								text29 += " -IgnoreReboot";
							}
							text29 += " -Verbose *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log";
							InvokeWUJob invokeWUJob = new InvokeWUJob();
							invokeWUJob.ComputerName = new string[1] { text7 };
							if (Credential != null)
							{
								invokeWUJob.Credential = Credential;
							}
							invokeWUJob.Script = text29;
							invokeWUJob.Debuger = true;
							if (ScheduleJob != DateTime.MinValue)
							{
								if (ScheduleJob.AddSeconds(10.0) <= DateTime.Now)
								{
									WriteVerbose("Execution time is gone. ScheduleJob was set to 1 minute delay from now.");
									ScheduleJob = DateTime.Now.AddMinutes(1.0);
								}
								invokeWUJob.TriggerDate = ScheduleJob;
								WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext " + text7 + " (" + ScheduleJob.ToString() + "): ");
							}
							else
							{
								invokeWUJob.RunNow = true;
								WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext " + text7 + " (Now): ");
							}
							WriteVerbose("powershell.exe -Command \"" + text29 + "\"");
							IEnumerable enumerable = invokeWUJob.Invoke();
							foreach (object item4 in enumerable)
							{
								WriteObject(item4);
							}
							WriteDebug(DateTime.Now.ToString() + " Return invoked update list");
							OutputObj = new Collection<PSObject>(collection.ToList());
							continue;
						}
						int num12 = 0;
						int activityId4 = 1;
						string activity4 = "Download updates for " + text7;
						string statusDescription4 = "[" + num12 + "/" + num11 + "]";
						ProgressRecord progressRecord4 = new ProgressRecord(activityId4, activity4, statusDescription4);
						foreach (PSObject item5 in collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Accepted"))
						{
							item5.Properties.Add(new PSNoteProperty("X", 2));
							IUpdate update4 = (IUpdate)item5.BaseObject;
							UpdateCollection updateCollection3 = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
							updateCollection3.Add(update4);
							progressRecord4.StatusDescription = "[" + num12 + "/" + num11 + "] " + update4.Title + " " + item5.Properties["Size"].Value.ToString();
							progressRecord4.PercentComplete = num12 * 100 / num11;
							WriteProgress(progressRecord4);
							num12++;
							WriteDebug(DateTime.Now.ToString() + " Show update to download: " + update4.Title);
							IUpdateDownloader updateDownloader = UpdateSessionObj.CreateUpdateDownloader();
							updateDownloader.Updates = updateCollection3;
							if ((bool)ForceDownload)
							{
								updateDownloader.IsForced = true;
							}
							IDownloadResult downloadResult;
							try
							{
								downloadResult = updateDownloader.Download();
								WriteDebug(DateTime.Now.ToString() + " Downloaded");
							}
							catch (COMException ex5)
							{
								WUApiCode wUApiCodeDetails3 = WUToolsObj.GetWUApiCodeDetails(ex5.ErrorCode);
								bool flag5 = false;
								if (wUApiCodeDetails3 != null)
								{
									switch (wUApiCodeDetails3.CodeType)
									{
									case 2:
										WriteError(new ErrorRecord(new Exception(wUApiCodeDetails3.Description), wUApiCodeDetails3.HResult, ErrorCategory.CloseError, null));
										flag5 = true;
										break;
									case 3:
										WriteWarning(wUApiCodeDetails3.HResult + ": " + wUApiCodeDetails3.Description);
										break;
									}
									if (flag5)
									{
										WriteDebug(DateTime.Now.ToString() + " Skip to next computer");
										break;
									}
								}
								else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
								{
									ErrorRecord errorRecord8 = new ErrorRecord(ex5, "Debug", ErrorCategory.CloseError, null);
									ThrowTerminatingError(errorRecord8);
								}
								WriteDebug(DateTime.Now.ToString() + " Skip to next update");
								continue;
							}
							string value2 = "";
							switch ((int)downloadResult.ResultCode)
							{
							case 0:
								value2 = "NotStarted";
								break;
							case 1:
								value2 = "InProgress";
								break;
							case 2:
								value2 = "Downloaded";
								break;
							case 3:
								value2 = "DownloadedWithErrors";
								break;
							case 4:
								value2 = "Failed";
								break;
							case 5:
								value2 = "Aborted";
								break;
							}
							item5.Properties.Add(new PSNoteProperty("DownloadResult", value2));
							item5.Properties.Add(new PSNoteProperty("Result", value2));
							string text30 = "";
							text30 = ((!(item5.Properties["ChooseResult"].Value.ToString() == "Accepted")) ? (text30 + "R") : (text30 + "A"));
							text30 = ((!(item5.Properties["DownloadResult"].Value.ToString() == "Downloaded")) ? (text30 + "F") : (text30 + "D"));
							text30 = ((!update4.IsInstalled) ? (text30 + "-") : (text30 + "I"));
							text30 = ((!update4.IsMandatory) ? (text30 + "-") : (text30 + "M"));
							text30 = ((!update4.IsHidden) ? (text30 + "-") : (text30 + "H"));
							text30 = ((!update4.IsUninstallable) ? (text30 + "-") : (text30 + "U"));
							text30 = ((!update4.IsBeta) ? (text30 + "-") : (text30 + "B"));
							item5.Properties["Status"].Value = text30;
							WriteObject(item5, enumerateCollection: true);
						}
						progressRecord4.RecordType = ProgressRecordType.Completed;
						WriteProgress(progressRecord4);
						num6 = collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Downloaded").Count();
						WriteVerbose("Downloaded [" + num6 + "] Updates ready to Install");
						if (!Install)
						{
							WriteDebug(DateTime.Now.ToString() + " Return downloaded update list");
							OutputObj = new Collection<PSObject>(collection.ToList());
							continue;
						}
					}
					if (!Install)
					{
						continue;
					}
					NeedsReboot = false;
					int num13 = 0;
					int activityId5 = 1;
					string activity5 = "Install updates for " + text7;
					string statusDescription5 = "[" + num13 + "/" + num6 + "]";
					ProgressRecord progressRecord5 = new ProgressRecord(activityId5, activity5, statusDescription5);
					foreach (PSObject item6 in collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Downloaded"))
					{
						item6.Properties.Add(new PSNoteProperty("X", 3));
						IUpdate update5 = (IUpdate)item6.BaseObject;
						UpdateCollection updateCollection4 = (UpdateCollection)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("13639463-00DB-4646-803D-528026140D88")));
						updateCollection4.Add(update5);
						progressRecord5.StatusDescription = "[" + num13 + "/" + num6 + "] " + update5.Title + " " + item6.Properties["Size"].Value.ToString();
						progressRecord5.PercentComplete = num13 * 100 / num6;
						WriteProgress(progressRecord5);
						num13++;
						WriteDebug(DateTime.Now.ToString() + " Show update to install: " + update5.Title);
						IUpdateInstaller updateInstaller = UpdateSessionObj.CreateUpdateInstaller();
						updateInstaller.Updates = updateCollection4;
						if ((bool)ForceInstall)
						{
							updateInstaller.IsForced = true;
						}
						IInstallationResult installationResult;
						try
						{
							installationResult = updateInstaller.Install();
							WriteDebug(DateTime.Now.ToString() + " Installed");
						}
						catch (COMException ex6)
						{
							WUApiCode wUApiCodeDetails4 = WUToolsObj.GetWUApiCodeDetails(ex6.ErrorCode);
							bool flag6 = false;
							if (wUApiCodeDetails4 != null)
							{
								switch (wUApiCodeDetails4.CodeType)
								{
								case 2:
									WriteError(new ErrorRecord(new Exception(wUApiCodeDetails4.Description), wUApiCodeDetails4.HResult, ErrorCategory.CloseError, null));
									flag6 = true;
									break;
								case 3:
									WriteWarning(wUApiCodeDetails4.HResult + ": " + wUApiCodeDetails4.Description);
									break;
								}
								if (flag6)
								{
									WriteDebug(DateTime.Now.ToString() + " Skip to next computer");
									break;
								}
							}
							else if (MyInvocation.BoundParameters.ContainsKey("Debuger"))
							{
								ErrorRecord errorRecord9 = new ErrorRecord(ex6, "Debug", ErrorCategory.CloseError, null);
								ThrowTerminatingError(errorRecord9);
							}
							WriteDebug(DateTime.Now.ToString() + " Skip to next update");
							continue;
						}
						if (!NeedsReboot)
						{
							NeedsReboot = installationResult.RebootRequired;
							WriteDebug(DateTime.Now.ToString() + " Reboot is required");
						}
						string value3 = "";
						switch ((int)installationResult.ResultCode)
						{
						case 0:
							value3 = "NotStarted";
							break;
						case 1:
							value3 = "InProgress";
							break;
						case 2:
							value3 = "Installed";
							break;
						case 3:
							value3 = "InstalledWithErrors";
							break;
						case 4:
							value3 = "Failed";
							break;
						case 5:
							value3 = "Aborted";
							break;
						}
						item6.Properties.Add(new PSNoteProperty("InstallResult", value3));
						item6.Properties.Add(new PSNoteProperty("Result", value3));
						string text31 = "";
						text31 = ((!(item6.Properties["ChooseResult"].Value.ToString() == "Accepted")) ? (text31 + "R") : (text31 + "A"));
						text31 = ((!(item6.Properties["DownloadResult"].Value.ToString() == "Downloaded")) ? (text31 + "F") : (text31 + "D"));
						if (item6.Properties["InstallResult"].Value.ToString() == "Installed")
						{
							text31 = ((!installationResult.RebootRequired) ? (text31 + "I") : (text31 + "R"));
						}
						else
						{
							text31 += "F";
						}
						text31 = ((!update5.IsMandatory) ? (text31 + "-") : (text31 + "M"));
						text31 = ((!update5.IsHidden) ? (text31 + "-") : (text31 + "H"));
						text31 = ((!update5.IsUninstallable) ? (text31 + "-") : (text31 + "U"));
						text31 = ((!update5.IsBeta) ? (text31 + "-") : (text31 + "B"));
						item6.Properties["Status"].Value = text31;
						WriteObject(item6, enumerateCollection: true);
					}
					progressRecord5.RecordType = ProgressRecordType.Completed;
					WriteProgress(progressRecord5);
					num7 = collection.Where((PSObject x) => x.Properties["Result"].Value.ToString() == "Installed").Count();
					WriteVerbose("Installed [" + num7 + "] Updates");
					WriteDebug(DateTime.Now.ToString() + " Return installed update list");
					OutputObj = new Collection<PSObject>(collection.ToList());
					if (num7 > 0 && (bool)SendHistory)
					{
						string text32 = "Get-WUHistory -SendReport -Last " + num7 + " -Verbose *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log -Append";
						InvokeWUJob invokeWUJob2 = new InvokeWUJob();
						invokeWUJob2.ComputerName = new string[1] { text7 };
						if (Credential != null)
						{
							invokeWUJob2.Credential = Credential;
						}
						invokeWUJob2.Script = text32;
						invokeWUJob2.TaskName = "PSWindowsUpdateNext_History";
						if (NeedsReboot)
						{
							invokeWUJob2.TriggerAtStart = true;
							WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext_History " + text7 + " (AtStart): powershell.exe -Command \"" + text32 + "\"");
						}
						else
						{
							invokeWUJob2.RunNow = true;
							WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext_History " + text7 + " (Now): powershell.exe -Command \"" + text32 + "\"");
						}
						IEnumerable enumerable2 = invokeWUJob2.Invoke();
						foreach (object item7 in enumerable2)
						{
							WriteObject(item7);
						}
					}
					if (RecurseCycle <= 1)
					{
						continue;
					}
					RecurseCycle--;
					string text33 = "";
					text33 = ((!Debuger) ? "Get-WindowsUpdate -AcceptAll" : "$DebugPreference = 'Continue'; Get-WindowsUpdate -AcceptAll");
					if ((bool)IsInstalled)
					{
						text33 += " -IsInstalled";
					}
					if ((bool)IsHidden)
					{
						text33 += " -IsHidden";
					}
					if ((bool)WithHidden)
					{
						text33 += " -WithHidden";
					}
					if (UpdateType != null)
					{
						text33 = text33 + " -UpdateType " + UpdateType;
					}
					if (DeploymentAction != null)
					{
						text33 = text33 + " -DeploymentAction " + DeploymentAction;
					}
					if (UpdateID != null)
					{
						text33 = text33 + " -UpdateID '" + string.Join("','", UpdateID) + "'";
					}
					if (NotUpdateID != null)
					{
						text33 = text33 + " -NotUpdateID '" + string.Join("','", NotUpdateID) + "'";
					}
					else if (text23 != "")
					{
						text33 = text33 + " -NotUpdateID " + text23;
					}
					if (RevisionNumber > 0)
					{
						text33 = text33 + " -RevisionNumber " + RevisionNumber;
					}
					if (CategoryIDs != null)
					{
						text33 = text33 + " -CategoryIDs '" + string.Join("','", CategoryIDs) + "'";
					}
					if (MyInvocation.BoundParameters.ContainsKey("IsAssigned"))
					{
						text33 = ((!IsAssigned) ? (text33 + " -IsAssigned:$false") : (text33 + " -IsAssigned"));
					}
					if (MyInvocation.BoundParameters.ContainsKey("IsPresent"))
					{
						text33 = ((!IsPresent) ? (text33 + " -IsPresent:$false") : (text33 + " -IsPresent"));
					}
					if (MyInvocation.BoundParameters.ContainsKey("AutoSelectOnWebSites"))
					{
						text33 = ((!AutoSelectOnWebSites) ? (text33 + " -AutoSelectOnWebSites:$false") : (text33 + " -AutoSelectOnWebSites"));
					}
					if (RootCategories != null)
					{
						text33 = text33 + " -RootCategories '" + string.Join("','", RootCategories) + "'";
					}
					if (Category != null)
					{
						text33 = text33 + " -Category '" + string.Join("','", Category) + "'";
					}
					if (KBArticleID != null)
					{
						text33 = text33 + " -KBArticleID '" + string.Join("','", KBArticleID) + "'";
					}
					if (Title != null)
					{
						text33 = text33 + " -Title '" + Title + "'";
					}
					if (Severity != null)
					{
						text33 = text33 + " -Severity '" + string.Join("','", Severity) + "'";
					}
					if (NotCategory != null)
					{
						text33 = text33 + " -NotCategory '" + string.Join("','", NotCategory) + "'";
					}
					if (NotKBArticleID != null)
					{
						text33 = text33 + " -NotKBArticleID '" + string.Join("','", NotKBArticleID) + "'";
					}
					if (NotTitle != null)
					{
						text33 = text33 + " -NotTitle '" + NotTitle + "'";
					}
					if (NotSeverity != null)
					{
						text33 = text33 + " -NotSeverity '" + string.Join("','", NotSeverity) + "'";
					}
					if ((bool)IgnoreUserInput)
					{
						text33 += " -IgnoreUserInput";
					}
					if ((bool)IgnoreRebootRequired)
					{
						text33 += " -IgnoreRebootRequired";
					}
					if ((bool)AutoSelectOnly)
					{
						text33 += " -AutoSelectOnly";
					}
					if (MaxSize > 0)
					{
						text33 = text33 + " -MaxSize " + MaxSize;
					}
					if (MinSize > 0)
					{
						text33 = text33 + " -MinSize " + MinSize;
					}
					if ((bool)Download)
					{
						text33 += " -Download";
					}
					if ((bool)Install)
					{
						text33 += " -Install";
					}
					if ((bool)IgnoreReboot)
					{
						text33 += " -IgnoreReboot";
					}
					else if ((bool)AutoReboot)
					{
						text33 += " -AutoReboot";
					}
					else if (ScheduleReboot != DateTime.MinValue)
					{
						text33 = text33 + " -ScheduleReboot '" + ScheduleReboot.ToString() + "'";
					}
					if ((bool)WindowsUpdate)
					{
						text33 += " -WindowsUpdate";
					}
					else if ((bool)MicrosoftUpdate)
					{
						text33 += " -MicrosoftUpdate";
					}
					else if (ServiceID != null)
					{
						text33 = text33 + " -ServiceID '" + ServiceID + "'";
					}
					if ((bool)SendReport)
					{
						text33 += " -SendReport";
					}
					if ((bool)SendHistory)
					{
						text33 += " -SendHistory";
					}
					if (RecurseCycle > 1)
					{
						text33 = text33 + " -RecurseCycle " + RecurseCycle;
					}
					text33 += " -Verbose *>&1 | Out-File $Env:TEMP\\PSWindowsUpdateNext.log -Append";
					InvokeWUJob invokeWUJob3 = new InvokeWUJob();
					invokeWUJob3.ComputerName = new string[1] { text7 };
					if (Credential != null)
					{
						invokeWUJob3.Credential = Credential;
					}
					invokeWUJob3.Script = text33;
					invokeWUJob3.TaskName = "PSWindowsUpdateNext_Recurse" + RecurseCycle;
					invokeWUJob3.Debuger = true;
					if (NeedsReboot)
					{
						invokeWUJob3.TriggerAtStart = true;
						WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext " + text7 + " (AtStart): ");
					}
					else
					{
						ScheduleJob = DateTime.Now.AddMinutes(5.0);
						invokeWUJob3.TriggerDate = ScheduleJob;
						WriteVerbose("Invoke-WUJob: PSWindowsUpdateNext " + text7 + " (" + ScheduleJob.ToString() + "): ");
					}
					WriteVerbose("powershell.exe -Command \"" + text33 + "\"");
					IEnumerable enumerable3 = invokeWUJob3.Invoke();
					foreach (object item8 in enumerable3)
					{
						WriteObject(item8);
					}
				}
				else if ((bool)Debuger)
				{
					ErrorRecord errorRecord10 = new ErrorRecord(wUApiServiceManagerObj.Exception, "Debug", ErrorCategory.CloseError, null);
					WriteError(errorRecord10);
				}
				else
				{
					ErrorRecord error = wUApiServiceManagerObj.Error;
					WriteError(error);
				}
			}
			else if ((bool)Debuger)
			{
				ErrorRecord errorRecord11 = new ErrorRecord(wUApiUpdateSessionObj.Exception, "Debug", ErrorCategory.CloseError, null);
				WriteError(errorRecord11);
			}
			else
			{
				ErrorRecord error2 = wUApiUpdateSessionObj.Error;
				WriteError(error2);
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
				logonType = WUImpersonator.LogonSessionType.Interactive;
				logonProvider = WUImpersonator.LogonProvider.Default;
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
		if ((bool)SendReport && OutputObj.Count > 0)
		{
			WriteDebug(DateTime.Now.ToString() + " Send report");
			if (!PSWUSettings.ContainsKey("Properties"))
			{
				PSWUSettings.Add("Properties", new string[5] { "ComputerName", "Status", "KB", "Size", "Title" });
			}
			PSObject pSObject2 = WUToolsObj.SendMail(PSWUSettings, OutputObj, pSObject);
			if (pSObject2.Properties.Match("ErrorRecord").Count == 1)
			{
				WriteError((ErrorRecord)pSObject2.Properties["ErrorRecord"].Value);
			}
		}
		if (NeedsReboot)
		{
			if (ScheduleReboot != DateTime.MinValue)
			{
				WriteDebug(DateTime.Now.ToString() + " Schedule Reboot " + ScheduleReboot.ToString());
				string text = WUToolsObj.ScheduleReboot("localhost", ScheduleReboot);
				WriteVerbose(text);
			}
			else if ((bool)AutoReboot)
			{
				WriteDebug(DateTime.Now.ToString() + " Auto Reboot");
				string text2 = WUToolsObj.RunReboot("localhost");
				WriteVerbose(text2);
			}
			else if ((bool)IgnoreReboot)
			{
				Host.UI.WriteLine(ConsoleColor.White, Host.UI.RawUI.BackgroundColor, "Reboot is required, but do it manually.");
			}
			else
			{
				Host.UI.WriteLine(ConsoleColor.White, Host.UI.RawUI.BackgroundColor, "Reboot is required. Do it now? [Y / N] (default is 'N')");
				string text3 = Host.UI.ReadLine();
				if (text3.ToUpper() == "Y")
				{
					WriteDebug(DateTime.Now.ToString() + " Manually Reboot");
					string text4 = WUToolsObj.RunReboot("localhost");
					WriteVerbose(text4);
				}
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
