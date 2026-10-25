using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WUApiLib;

[ComImport]
[CompilerGenerated]
[Guid("6A92B07A-D821-4682-B423-5C805022CC4D")]
[DefaultMember("Title")]
[TypeIdentifier]
public interface IUpdate
{
	[DispId(0)]
	string Title
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(1610743809)]
	bool AutoSelectOnWebSites
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743809)]
		get;
	}

	void _VtblGap1_2();

	[DispId(1610743812)]
	ICategoryCollection Categories
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743812)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	void _VtblGap2_4();

	[DispId(1610743817)]
	bool EulaAccepted
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743817)]
		get;
	}

	void _VtblGap3_4();

	[DispId(1610743822)]
	IInstallationBehavior InstallationBehavior
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743822)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(1610743823)]
	bool IsBeta
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743823)]
		get;
	}

	[DispId(1610743824)]
	bool IsDownloaded
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743824)]
		get;
	}

	[DispId(1610743825)]
	bool IsHidden
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743825)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743825)]
		[param: In]
		set;
	}

	[DispId(1610743826)]
	bool IsInstalled
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743826)]
		get;
	}

	[DispId(1610743827)]
	bool IsMandatory
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743827)]
		get;
	}

	[DispId(1610743828)]
	bool IsUninstallable
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743828)]
		get;
	}

	void _VtblGap4_2();

	[DispId(1610743831)]
	decimal MaxDownloadSize
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743831)]
		get;
	}

	void _VtblGap5_2();

	[DispId(1610743834)]
	string MsrcSeverity
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743834)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	void _VtblGap6_11();

	[DispId(1610743848)]
	StringCollection KBArticleIDs
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(1610743848)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610743847)]
	void AcceptEula();
}
