using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[DefaultMember("RegistrationState")]
[Guid("DDE02280-12B3-4E0B-937B-6747F6ACB286")]
[TypeIdentifier]
public interface IUpdateServiceRegistration
{
	[DispId(0)]
	UpdateServiceRegistrationState RegistrationState
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		get;
	}

	void _VtblGap1_2();

	[DispId(1610743811)]
	IUpdateService2 Service
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743811)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
