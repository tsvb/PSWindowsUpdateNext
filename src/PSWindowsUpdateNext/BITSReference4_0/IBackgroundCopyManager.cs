using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BITSReference4_0;

[ComImport]
[CompilerGenerated]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
[TypeIdentifier]
public interface IBackgroundCopyManager
{
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void CreateJob([In][MarshalAs(UnmanagedType.LPWStr)] string DisplayName, [In] BG_JOB_TYPE Type, out GUID pJobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);
}
