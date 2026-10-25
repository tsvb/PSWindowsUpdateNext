using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("0BB8531D-7E8D-424F-986C-A0B8F60A3E7B")]
[TypeIdentifier]
public interface IUpdateServiceManager2 : IUpdateServiceManager
{
	[DispId(1610743809)]
	IUpdateServiceCollection Services
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743809)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	void _VtblGap1_2();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743812)]
	void RemoveService([In][MarshalAs(UnmanagedType.BStr)] string ServiceID);

	void _VtblGap2_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743814)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IUpdateService AddScanPackageService([In][MarshalAs(UnmanagedType.BStr)] string serviceName, [In][MarshalAs(UnmanagedType.BStr)] string scanFileLocation, [In] int flags = 0);

	void _VtblGap3_4();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809347)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IUpdateServiceRegistration AddService2([In][MarshalAs(UnmanagedType.BStr)] string ServiceID, [In] int flags, [In][MarshalAs(UnmanagedType.BStr)] string authorizationCabPath);
}
