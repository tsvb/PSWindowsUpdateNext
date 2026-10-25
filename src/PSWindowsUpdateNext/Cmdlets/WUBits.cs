using System;
using System.Runtime.InteropServices;
using System.Threading;
using BITSReference4_0;

namespace PSWindowsUpdateNext;

internal class WUBits
{
	/// <summary>
	/// Download a file from a URL using BITS. Blocks until the job finishes.
	/// </summary>
	/// <param name="Source">Source URL</param>
	/// <param name="Destination">Destination path</param>
	/// <returns>"ACKNOWLEDGED" on success, "ERROR" or "CANCELED" otherwise.</returns>
	public string StartBitsTransfer(string Source, string Destination)
	{
		BackgroundCopyManager4_0 manager = (BackgroundCopyManager4_0)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("BB6DF56B-CACE-11DC-9992-0019B93A3A84")));
		manager.CreateJob("PSWindowsUpdateNext download", BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out var _, out IBackgroundCopyJob job);
		job.AddFile(Source, Destination);
		job.Resume();
		bool done = false;
		string result = "";
		while (!done)
		{
			job.GetState(out BG_JOB_STATE state);
			switch (state)
			{
			case BG_JOB_STATE.BG_JOB_STATE_ERROR:
				result = "ERROR";
				done = true;
				break;
			case BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED:
				job.Complete();
				result = "TRANSFERRED";
				break;
			case BG_JOB_STATE.BG_JOB_STATE_CANCELLED:
				result = "CANCELED";
				done = true;
				break;
			case BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
				result = "ACKNOWLEDGED";
				done = true;
				break;
			}
			if (!done)
			{
				// The original code spun at 100% CPU here; BITS state changes are not instant.
				Thread.Sleep(250);
			}
		}
		return result;
	}
}
