using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Management.Infrastructure;

namespace PSWindowsUpdateNext;

internal class WUCimSession
{
	private string ComputerName;

	public WUCimSession(string computerName)
	{
		ComputerName = computerName;
	}

	public CimMethodResult InvokeCimMethod(string cimClassName, string cimNamespace, string cimMethod, CimMethodParametersCollection cimParameters)
	{
		CimInstance instance = new CimInstance(cimClassName, cimNamespace);
		CimSession cimSession = CimSession.Create(ComputerName);
		CimMethodResult cimMethodResult = cimSession.InvokeMethod(instance, cimMethod, cimParameters);
		int num = Convert.ToInt32(cimMethodResult.ReturnValue.Value, CultureInfo.CurrentCulture);
		if (num != 0)
		{
			throw new Win32Exception(num);
		}
		return cimMethodResult;
	}

	public CimInstance[] ScanForUpdates(string searchCriteria = "IsInstalled = 0")
	{
		string className = "MSFT_WUOperations";
		string namespaceName = "root/Microsoft/Windows/WindowsUpdate";
		string methodName = "ScanForUpdates";
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		cimMethodParametersCollection.Add(CimMethodParameter.Create("SearchCriteria", searchCriteria, CimType.String, CimFlags.None));
		CimInstance instance = new CimInstance(className, namespaceName);
		CimSession cimSession = CimSession.Create(ComputerName);
		CimMethodResult cimMethodResult = cimSession.InvokeMethod(instance, methodName, cimMethodParametersCollection);
		return (CimInstance[])cimMethodResult.OutParameters["Updates"].Value;
	}

	public CimMethodResult InstallUpdates(object updates, bool downloadOnly = false, bool rebootRequired = false)
	{
		string className = "MSFT_WUOperations";
		string namespaceName = "root/Microsoft/Windows/WindowsUpdate";
		string methodName = "InstallUpdates";
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		cimMethodParametersCollection.Add(CimMethodParameter.Create("Updates", updates, CimFlags.None));
		if (downloadOnly)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DownloadOnly", true, CimFlags.None));
		}
		if (rebootRequired)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RebootRequired", true, CimFlags.None));
		}
		CimInstance instance = new CimInstance(className, namespaceName);
		CimSession cimSession = CimSession.Create(ComputerName);
		CimMethodResult result = null;
		try
		{
			result = cimSession.InvokeMethod(instance, methodName, cimMethodParametersCollection);
		}
		catch (Exception ex)
		{
			throw;
		}
		return result;
	}
}
