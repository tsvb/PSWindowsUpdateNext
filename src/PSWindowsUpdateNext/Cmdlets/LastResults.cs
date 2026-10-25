namespace PSWindowsUpdateNext;

/// <summary>
/// OutputType
/// <para type="description">OutputType for LastResult objects.</para>
/// </summary>
public class LastResults
{
	/// <summary>ComputerName</summary>
	public string ComputerName { get; set; }

	/// <summary>LastSearchSuccessDate</summary>
	public string LastSearchSuccessDate { get; set; }

	/// <summary>LastInstallationSuccessDate</summary>
	public string LastInstallationSuccessDate { get; set; }
}
