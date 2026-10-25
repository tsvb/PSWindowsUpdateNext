using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[Guid("85DF5081-1B24-4F32-878A-D9D14DF4CB77")]
[TypeIdentifier]
public interface ITriggerCollection : IEnumerable
{
	void _VtblGap1_1();

	[DispId(0)]
	ITrigger this[[In] int index]
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	void _VtblGap2_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(2)]
	[return: MarshalAs(UnmanagedType.Interface)]
	ITrigger Create([In] _TASK_TRIGGER_TYPE2 Type);
}
