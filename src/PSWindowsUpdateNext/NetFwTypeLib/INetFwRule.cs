using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib;

[ComImport]
[CompilerGenerated]
[Guid("AF230D27-BABA-4E42-ACED-F524F22CFCE2")]
[TypeIdentifier]
public interface INetFwRule
{
	[DispId(1)]
	string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}
}
