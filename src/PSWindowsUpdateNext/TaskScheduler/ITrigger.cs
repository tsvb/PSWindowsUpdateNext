using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[Guid("09941815-EA89-4B5B-89E0-2A773801FAC3")]
[TypeIdentifier]
public interface ITrigger
{
	void _VtblGap1_9();

	[DispId(6)]
	string EndBoundary
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(6)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}
}
