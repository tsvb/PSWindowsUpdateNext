using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace PSWindowsUpdateNext;

/// <summary>
/// Runs a block of code under another user's security context.
/// Uses <see cref="WindowsIdentity.RunImpersonated(SafeAccessTokenHandle, Action)"/>, which is
/// available on both .NET Framework 4.6+ and modern .NET, instead of the removed
/// WindowsImpersonationContext API.
/// </summary>
public static class WUImpersonator
{
	/// <summary>
	/// Specifies the type of login session used.
	/// </summary>
	public enum LogonSessionType
	{
		/// <summary>Interactive logon (LOGON32_LOGON_INTERACTIVE).</summary>
		Interactive = 2,
		/// <summary>Network logon.</summary>
		Network,
		/// <summary>Batch logon.</summary>
		Batch,
		/// <summary>Service logon.</summary>
		Service,
		/// <summary>Proxy logon.</summary>
		Proxy,
		/// <summary>Unlock logon.</summary>
		Unlock,
		/// <summary>Network clear-text logon.</summary>
		NetworkCleartext,
		/// <summary>Clone the current token but use new credentials for outbound connections (LOGON32_LOGON_NEW_CREDENTIALS).</summary>
		NewCredentials,
		/// <summary>Remote interactive logon.</summary>
		RemoteInteractive,
		/// <summary>Cached interactive logon.</summary>
		CachedInteractive,
		/// <summary>Cached remote interactive logon.</summary>
		CachedRemoteInteractive,
		/// <summary>Cached unlock logon.</summary>
		CachedUnlock
	}

	/// <summary>
	/// Specifies the logon provider.
	/// </summary>
	public enum LogonProvider
	{
		/// <summary>Standard provider (negotiate, or NTLM when no domain is given).</summary>
		Default,
		/// <summary>Windows NT 3.5 provider.</summary>
		WinNT35,
		/// <summary>NTLM provider.</summary>
		WinNT40,
		/// <summary>Negotiate provider.</summary>
		WinNT50
	}

	[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

	/// <summary>
	/// Logs on with the given credentials and runs <paramref name="action"/> while impersonating that user.
	/// When <paramref name="userName"/> is empty the action runs under the current identity.
	/// </summary>
	/// <param name="userName">User name, or UPN when <paramref name="domainName"/> is null.</param>
	/// <param name="domainName">Domain, "." for the local account database, or null for UPN format.</param>
	/// <param name="password">Plaintext password.</param>
	/// <param name="logonType">Logon session type.</param>
	/// <param name="logonProvider">Logon provider.</param>
	/// <param name="action">Code to run while impersonating.</param>
	public static void Run(string userName, string domainName, string password, LogonSessionType logonType, LogonProvider logonProvider, Action action)
	{
		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		}
		if (string.IsNullOrEmpty(userName))
		{
			action();
			return;
		}
		if (!LogonUser(userName, domainName, password, (int)logonType, (int)logonProvider, out SafeAccessTokenHandle token))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
		using (token)
		{
			WindowsIdentity.RunImpersonated(token, action);
		}
	}

	/// <summary>
	/// Convenience overload with interactive logon and the default provider.
	/// </summary>
	public static void Run(string userName, string domainName, string password, Action action)
	{
		Run(userName, domainName, password, LogonSessionType.Interactive, LogonProvider.Default, action);
	}
}
