namespace PSWindowsUpdateNext;

/// <summary>
/// OutputType
/// <para type="description">OutputType for RebootStatus objects.</para>
/// </summary>
public class RebootStatus
{
	/// <summary>ComputerName</summary>
	public string ComputerName { get; set; }

	/// <summary>RebootRequired</summary>
	public string RebootRequired { get; set; }

	/// <summary>RebootScheduled</summary>
	public string RebootScheduled { get; set; }
}
