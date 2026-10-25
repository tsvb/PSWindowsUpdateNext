using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[Guid("02820E19-7B98-4ED2-B2E8-FDCCCEFF619B")]
[TypeIdentifier]
public interface IActionCollection : IEnumerable
{
	void _VtblGap1_1();

	[DispId(0)]
	IAction this[[In] int index]
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	void _VtblGap2_3();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(3)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IAction Create([In] _TASK_ACTION_TYPE Type);
}
