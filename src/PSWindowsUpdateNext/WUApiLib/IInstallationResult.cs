using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("A43C56D6-7451-48D4-AF96-B6CD2D0D9B7A")]
[TypeIdentifier]
public interface IInstallationResult
{
	void _VtblGap1_1();

	[DispId(1610743810)]
	bool RebootRequired
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		get;
	}

	[DispId(1610743811)]
	OperationResultCode ResultCode
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743811)]
		get;
	}
}
