using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("7B929C68-CCDC-4226-96B1-8724600B54C2")]
[TypeIdentifier]
public interface IUpdateInstaller
{
	void _VtblGap1_2();

	[DispId(1610743810)]
	bool IsForced
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		[param: In]
		set;
	}

	void _VtblGap2_4();

	[DispId(1610743813)]
	UpdateCollection Updates
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743813)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743813)]
		[param: In]
		[param: MarshalAs(UnmanagedType.Interface)]
		set;
	}

	void _VtblGap3_4();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743818)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IInstallationResult Install();

	void _VtblGap4_2();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743821)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IInstallationResult Uninstall();
}
