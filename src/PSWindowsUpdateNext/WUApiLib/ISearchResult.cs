using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("D40CFF62-E08C-4498-941A-01E25F0FD33C")]
[TypeIdentifier]
public interface ISearchResult
{
	void _VtblGap1_1();

	[DispId(1610743810)]
	ICategoryCollection RootCategories
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(1610743811)]
	UpdateCollection Updates
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743811)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
