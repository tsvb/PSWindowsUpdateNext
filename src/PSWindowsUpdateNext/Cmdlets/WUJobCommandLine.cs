using System;
using System.Text;

namespace PSWindowsUpdateNext;

/// <summary>
/// Builds the powershell.exe command line that <see cref="InvokeWUJob"/> registers in the scheduled task.
/// </summary>
internal static class WUJobCommandLine
{
	/// <summary>
	/// Arguments for powershell.exe. <c>-EncodedCommand</c> carries the script verbatim (Base64 of its
	/// UTF-16LE bytes): no quoting rules apply, so no character inside the script can break out of the
	/// command line.
	/// </summary>
	public static string BuildArguments(string script)
	{
		return "-NoProfile -NonInteractive -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
	}
}
