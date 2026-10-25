using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[DefaultMember("InstanceGuid")]
[Guid("653758FB-7B9A-4F1E-A471-BEEB8E9B834E")]
[TypeIdentifier]
public interface IRunningTask
{
	[DispId(1)]
	string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(0)]
	string InstanceGuid
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
