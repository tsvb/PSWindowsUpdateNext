using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("DAA4FDD0-4727-4DBE-A1E7-745DCA317144")]
[TypeIdentifier]
public interface IDownloadResult
{
	void _VtblGap1_1();

	[DispId(1610743810)]
	OperationResultCode ResultCode
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		get;
	}
}
