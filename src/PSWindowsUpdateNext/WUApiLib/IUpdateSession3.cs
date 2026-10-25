using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("918EFD1E-B5D8-4C90-8540-AEB9BDC56F9D")]
[TypeIdentifier]
public interface IUpdateSession3 : IUpdateSession2
{
	void _VtblGap1_5();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743812)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IUpdateSearcher CreateUpdateSearcher();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743813)]
	[return: MarshalAs(UnmanagedType.Interface)]
	UpdateDownloader CreateUpdateDownloader();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743814)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IUpdateInstaller CreateUpdateInstaller();
}
