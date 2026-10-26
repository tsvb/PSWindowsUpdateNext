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

	// C# has no syntax for a parameterized property, so 'IsRuleGroupCurrentlyEnabled' is its getter.
	// ILSpy also emitted '[property: DispId(14)]' here; the compiler ignored it (CS0657), and the
	// method-level DispId below carries the same value.
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(14)]
	bool get_IsRuleGroupCurrentlyEnabled([In][MarshalAs(UnmanagedType.BStr)] string group);
}
