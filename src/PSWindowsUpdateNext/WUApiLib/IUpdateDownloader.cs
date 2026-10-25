using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("68F1C6F9-7ECC-4666-A464-247FE12496C3")]
[TypeIdentifier]
public interface IUpdateDownloader
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

	void _VtblGap2_2();

	[DispId(1610743812)]
	UpdateCollection Updates
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743812)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743812)]
		[param: In]
		[param: MarshalAs(UnmanagedType.Interface)]
		set;
	}

	void _VtblGap3_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743814)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IDownloadResult Download();
}
