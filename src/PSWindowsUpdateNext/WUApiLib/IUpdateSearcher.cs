using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("8F45ABF1-F9AE-4B95-A933-F0F66E5056EA")]
[TypeIdentifier]
public interface IUpdateSearcher
{
	void _VtblGap1_6();

	[DispId(1610743815)]
	ServerSelection ServerSelection
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743815)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743815)]
		[param: In]
		set;
	}

	void _VtblGap2_3();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743819)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IUpdateHistoryEntryCollection QueryHistory([In] int startIndex, [In] int Count);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743820)]
	[return: MarshalAs(UnmanagedType.Interface)]
	ISearchResult Search([In][MarshalAs(UnmanagedType.BStr)] string criteria);

	void _VtblGap3_3();

	[DispId(1610743823)]
	string ServiceID
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743823)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743823)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}
}
