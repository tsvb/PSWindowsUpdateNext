using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("85713FA1-7796-4FA2-BE3B-E2D6124DD373")]
[TypeIdentifier]
public interface IWindowsUpdateAgentInfo
{
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743809)]
	[return: MarshalAs(UnmanagedType.Struct)]
	object GetInfo([In][MarshalAs(UnmanagedType.Struct)] object varInfoIdentifier);
}
