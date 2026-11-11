using System.Diagnostics;

namespace ScsFileBrowser.AvaloniaUI.Services;

internal static class ProgramLauncher
{
    /// <summary>
    ///     Starts the program with the path of the file as its only argument.
    ///     Applications that can open files should be able to take a file path as an argument
    /// </summary>
    /// <exception cref="Exception">When the program could not be started (e.g. it is not executable)</exception>
    internal static void OpenWithProgram(string programPath, string filePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = programPath,
            UseShellExecute = false
        };
        // ArgumentList takes care of quoting/escaping, paths can contain spaces and quotes
        startInfo.ArgumentList.Add(filePath);

        using var process = Process.Start(startInfo);
    }
}
