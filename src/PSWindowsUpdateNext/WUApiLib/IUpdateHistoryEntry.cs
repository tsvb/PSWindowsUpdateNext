using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("BE56A644-AF0E-4E0E-A311-C1D8E695CBFF")]
[TypeIdentifier]
public interface IUpdateHistoryEntry
{
	[DispId(1610743809)]
	tagUpdateOperation Operation
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743809)]
		get;
	}

	[DispId(1610743810)]
	OperationResultCode ResultCode
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		get;
	}

	void _VtblGap1_1();

	[DispId(1610743812)]
	DateTime Date
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743812)]
		get;
	}

	void _VtblGap2_1();

	[DispId(1610743814)]
	string Title
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743814)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
