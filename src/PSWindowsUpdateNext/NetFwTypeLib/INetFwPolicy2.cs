using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib;

[ComImport]
[CompilerGenerated]
[Guid("98325047-C671-4174-8D81-DEFCD3F03186")]
[TypeIdentifier]
public interface INetFwPolicy2
{
	void _VtblGap1_11();

	[DispId(7)]
	INetFwRules Rules
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(7)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	void _VtblGap2_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(9)]
	void EnableRuleGroup([In] int profileTypesBitmask, [In][MarshalAs(UnmanagedType.BStr)] string group, [In] bool enable);

	void _VtblGap3_6();

	// C# has no syntax for parameterized property 'IsRuleGroupCurrentlyEnabled'.
	// Its 'property:' attributes below are ignored by the compiler (CS0657).
	[property: DispId(14)]
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(14)]
	bool get_IsRuleGroupCurrentlyEnabled([In][MarshalAs(UnmanagedType.BStr)] string group);
}
