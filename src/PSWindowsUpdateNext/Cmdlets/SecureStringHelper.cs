using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PSWindowsUpdateNext;

[SuppressUnmanagedCodeSecurity]
internal static class SecureStringHelper
{
	internal unsafe static SecureString CreateSecureString(string plainString)
	{
		if (string.IsNullOrEmpty(plainString))
		{
			return new SecureString();
		}
		SecureString secureString;
		fixed (char* ptr = plainString)
		{
			char* value = ptr;
			secureString = new SecureString(value, plainString.Length);
			secureString.MakeReadOnly();
		}
		return secureString;
	}

	internal static string CreateString(SecureString secureString)
	{
		if (secureString == null || secureString.Length == 0)
		{
			return string.Empty;
		}
		IntPtr intPtr = IntPtr.Zero;
		string result;
		try
		{
			intPtr = Marshal.SecureStringToBSTR(secureString);
			result = Marshal.PtrToStringBSTR(intPtr);
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.ZeroFreeBSTR(intPtr);
			}
		}
		return result;
	}
}
