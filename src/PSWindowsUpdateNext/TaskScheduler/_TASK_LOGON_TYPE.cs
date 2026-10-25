using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TaskScheduler;

[CompilerGenerated]
[TypeIdentifier("e34cb9f1-c7f7-424c-be29-027dcc09363a", "TaskScheduler._TASK_LOGON_TYPE")]
public enum _TASK_LOGON_TYPE
{
	TASK_LOGON_NONE,
	TASK_LOGON_PASSWORD,
	TASK_LOGON_S4U,
	TASK_LOGON_INTERACTIVE_TOKEN,
	TASK_LOGON_GROUP,
	TASK_LOGON_SERVICE_ACCOUNT,
	TASK_LOGON_INTERACTIVE_TOKEN_OR_PASSWORD
}
