using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BITSReference4_0;

[ComImport]
[CompilerGenerated]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("37668D37-507E-4160-9316-26306D150B12")]
[TypeIdentifier]
public interface IBackgroundCopyJob
{
	void _VtblGap1_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void AddFile([In][MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [In][MarshalAs(UnmanagedType.LPWStr)] string LocalName);

	void _VtblGap2_2();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void Resume();

	void _VtblGap3_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void Complete();

	void _VtblGap4_4();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void GetState(out BG_JOB_STATE pVal);
}
