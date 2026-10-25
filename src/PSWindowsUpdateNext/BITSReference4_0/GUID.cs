using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BITSReference4_0;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[CompilerGenerated]
[TypeIdentifier("BB6DF56A-CACE-11DC-9992-0019B93A3A84", "BITSReference4_0.GUID")]
public struct GUID
{
	public uint Data1;

	public ushort Data2;

	public ushort Data3;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] Data4;
}
