using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("81DDC1B8-9D35-47A6-B471-5B80F519223B")]
[DefaultMember("Name")]
[TypeIdentifier]
public interface ICategory
{
	[DispId(0)]
	string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	void _VtblGap1_7();

	[DispId(1610743816)]
	UpdateCollection Updates
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743816)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
