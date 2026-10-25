using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("3442D4FE-224D-4CEE-98CF-30E0C4D229E6")]
[TypeIdentifier]
public interface IUpdateInstaller2 : IUpdateInstaller
{
	void _VtblGap1_16();

	[DispId(1610743820)]
	bool IsBusy
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743820)]
		get;
	}
}
