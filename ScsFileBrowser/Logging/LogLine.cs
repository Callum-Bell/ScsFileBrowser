﻿namespace ScsFileBrowser.Logging;

/// <summary>
///     Data to be logged to the log file
/// </summary>
internal class LogLine
{
    internal LogLine(LogLevel logType, string msg, string callerName = "Unknown", string callerPath = "Unknown")
    {
        Type = logType;
        Msg = msg;
        Caller = callerName;
        CallerPath = Path.GetFileName(callerPath);
        Time = DateTime.Now.ToLongTimeString();
    }

    internal LogLevel Type { get; set; }
    internal string Msg { get; set; }
    internal string Caller { get; set; }
    internal string CallerPath { get; set; }
    internal string Time { get; set; }
}
