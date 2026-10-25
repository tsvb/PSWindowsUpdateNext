using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib;

[ComImport]
[CompilerGenerated]
[Guid("9C4C6277-5027-441E-AFAE-CA1F542DA009")]
[TypeIdentifier]
public interface INetFwRules : IEnumerable
{
	void _VtblGap1_1();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(2)]
	void Add([In][MarshalAs(UnmanagedType.Interface)] INetFwRule rule);

	void _VtblGap2_2();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(-4)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();
}
