using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[Guid("D98D51E5-C9B4-496A-A9C1-18980261CF0F")]
[TypeIdentifier]
public interface IPrincipal
{
	void _VtblGap1_10();

	[DispId(6)]
	_TASK_RUNLEVEL RunLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(6)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(6)]
		[param: In]
		set;
	}
}
