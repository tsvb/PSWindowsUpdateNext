using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("ADE87BF7-7B56-4275-8FAB-B9B0E591844B")]
[TypeIdentifier]
public interface ISystemInformation
{
	void _VtblGap1_1();

	[DispId(1610743810)]
	bool RebootRequired
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743810)]
		get;
	}
}
