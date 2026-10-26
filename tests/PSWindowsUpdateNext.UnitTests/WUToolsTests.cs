using System.Collections;
using System.IO;
using Xunit;

namespace PSWindowsUpdateNext.UnitTests;

/// <summary>
/// WUTools is a PSCmdlet that most cmdlets construct with <c>new WUTools()</c> and never invoke, so it
/// has no command runtime. Cmdlet.WriteWarning throws NotImplementedException in that state, which once
/// turned a corrupt PSWUSettings.xml into a terminating error for every cmdlet with -SendReport.
/// </summary>
public class WUToolsTests
{
	[Fact]
	public void A_corrupt_settings_file_is_ignored_without_a_command_runtime()
	{
		WUTools tools = new WUTools();
		// Same path expression GetPSWUSettings uses, so the file lands where it looks on every platform.
		string path = tools.PSWUModulePath() + "\\PSWUSettings.xml";
		File.WriteAllText(path, "<not valid clixml");
		try
		{
			Hashtable settings = tools.GetPSWUSettings();
			Assert.NotNull(settings);
			Assert.Empty(settings);
		}
		finally
		{
			File.Delete(path);
		}
	}
}
