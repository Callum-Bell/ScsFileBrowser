using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ScsFileBrowser.Logging;

/// <summary>
///     Logger class, logs data to file located in "%localappdata%/TruckSim/ScsFileBrowser/log.log"
/// </summary>
internal class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    private readonly BlockingCollection<LogLine> _bc = new();

    internal Logger()
    {
        if (!Directory.Exists(Consts.AppDirPath)) Directory.CreateDirectory(Consts.AppDirPath);
        StreamWriter? sw = null;
        bool success;
        var tryCount = 0;
        do
        {
            success = true;
            try
            {
                var fs = new FileStream(
                    Path.Combine(Consts.AppDirPath, $"log{(tryCount++ == 0 ? "" : $"-{tryCount}")}.log"),
                    FileMode.Create);
                sw = new StreamWriter(fs);
            }
            catch (Exception)
            {
                success = false;
            }
        } while (!success && tryCount < 5);

        if (success)
        {
            Task.Run(() =>
            {
                foreach (var logLine in _bc.GetConsumingEnumerable())
                {
                    var text =
                        $"[{logLine.Time}|{logLine.Type,7}] [{logLine.CallerPath}::{logLine.Caller}] {logLine.Msg}";

                    sw?.WriteLine(text);
                    sw?.Flush();
                    Console.WriteLine(text);
                }
            });
        }
    }

    internal static Logger Instance => _instance.Value;

    internal void Debug(string msg, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
        if (Consts.MinimumLogLevel <= LogLevel.Debug) _bc.Add(new LogLine(LogLevel.Debug, msg, callerName, callerPath));
    }

    internal void Info(string msg, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
        if (Consts.MinimumLogLevel <= LogLevel.Info) _bc.Add(new LogLine(LogLevel.Info, msg, callerName, callerPath));
    }

    internal void Warning(string msg, [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerPath = "")
    {
        if (Consts.MinimumLogLevel <= LogLevel.Warning)
            _bc.Add(new LogLine(LogLevel.Warning, msg, callerName, callerPath));
    }

    internal void Error(string msg, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
        if (Consts.MinimumLogLevel <= LogLevel.Error) _bc.Add(new LogLine(LogLevel.Error, msg, callerName, callerPath));
    }
}
