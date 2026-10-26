using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

internal static class Fixture
{
	public static string Load(string name)
	{
		return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));
	}
}

/// <summary>
/// A <see cref="FactAttribute"/> that reports as skipped, with a reason, when not running on Windows.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class WindowsFactAttribute : FactAttribute
{
	public WindowsFactAttribute()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Skip = "Windows only: needs DPAPI (CryptProtectData), which is not available on this platform.";
		}
	}
}

/// <summary>
/// A <see cref="TheoryAttribute"/> for cases that pin how System.Uri parses unusual authorities
/// (backslashes, control characters, look-alike Unicode). System.Uri is implemented differently on
/// .NET Framework, where these expectations have not been verified, so they only run on modern .NET.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NetCoreTheoryAttribute : TheoryAttribute
{
	public NetCoreTheoryAttribute()
	{
#if NETFRAMEWORK
		Skip = "Pins System.Uri parsing on modern .NET; the .NET Framework implementation differs and these expectations are not verified there.";
#endif
	}
}
