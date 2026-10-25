using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[DefaultMember("Name")]
[Guid("1518B460-6518-4172-940F-C75883B24CEB")]
[TypeIdentifier]
public interface IUpdateService2 : IUpdateService
{
	[DispId(0)]
	new string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	void _VtblGap1_7();

	[DispId(1610743816)]
	string ServiceID
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743816)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	void _VtblGap2_4();

	[DispId(1610809345)]
	bool IsDefaultAUService
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610809345)]
		get;
	}
}
