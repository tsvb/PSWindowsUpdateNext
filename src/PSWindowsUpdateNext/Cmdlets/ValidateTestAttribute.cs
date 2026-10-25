using System.Management.Automation;

namespace PSWindowsUpdateNext;

/// <summary>
/// ValidateTestAttribute
/// </summary>
internal class ValidateTestAttribute : ValidateArgumentsAttribute
{
	protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
	{
	}
}
