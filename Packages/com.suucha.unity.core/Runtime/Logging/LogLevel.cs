using System;
using System.Collections.Generic;
using System.Text;

namespace SuuchaStudio.Unity.Core.Logging
{
    //
    // 摘要:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        //
        // 摘要:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace,
        //
        // 摘要:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug,
        //
        // 摘要:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information,
        //
        // 摘要:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning,
        //
        // 摘要:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error,
        //
        // 摘要:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical,
        //
        // 摘要:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None
    }
}
