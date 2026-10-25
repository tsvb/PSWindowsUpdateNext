using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[ComImport]
[CompilerGenerated]
[DefaultMember("Path")]
[Guid("8CFAC062-A080-4C15-9A88-AA7C2AF80DFC")]
[TypeIdentifier]
public interface ITaskFolder
{
	void _VtblGap1_1();

	[DispId(0)]
	string Path
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	void _VtblGap2_4();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(7)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IRegisteredTask GetTask([In][MarshalAs(UnmanagedType.BStr)] string Path);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(8)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IRegisteredTaskCollection GetTasks([In] int flags);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(9)]
	void DeleteTask([In][MarshalAs(UnmanagedType.BStr)] string Name, [In] int flags);

	void _VtblGap3_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(11)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IRegisteredTask RegisterTaskDefinition([In][MarshalAs(UnmanagedType.BStr)] string Path, [In][MarshalAs(UnmanagedType.Interface)] ITaskDefinition pDefinition, [In] int flags, [In][MarshalAs(UnmanagedType.Struct)] object UserId, [In][MarshalAs(UnmanagedType.Struct)] object password, [In] _TASK_LOGON_TYPE LogonType, [Optional][In][MarshalAs(UnmanagedType.Struct)] object sddl);
}
