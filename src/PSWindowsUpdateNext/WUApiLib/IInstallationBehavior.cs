using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("D9A59339-E245-4DBD-9686-4D5763E39624")]
[TypeIdentifier]
public interface IInstallationBehavior
{
	[DispId(1610743809)]
	bool CanRequestUserInput
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743809)]
		get;
	}

	void _VtblGap1_1();

	[DispId(1610743811)]
	InstallationRebootBehavior RebootBehavior
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743811)]
		get;
	}
}
