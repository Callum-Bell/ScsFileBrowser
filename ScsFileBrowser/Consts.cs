using ScsFileBrowser.Logging;

namespace ScsFileBrowser;

internal static class Consts
{
    /// <summary>
    ///     Magic mark that should be at the start of the file, 'SCS#' as utf-8 bytes
    /// </summary>
    internal const uint ScsMagic = 592659283;

    internal const LogLevel MinimumLogLevel = LogLevel.Debug;

    internal static readonly string AppDirPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TruckSim",
        "ScsFileBrowser");

    internal static readonly string TempFileDirPath = Path.Combine(Path.GetTempPath(), "ScsFileBrowser");

    internal static readonly string[] AllowedDropExtensions = {".scs", ".zip", ".mp"};
}
