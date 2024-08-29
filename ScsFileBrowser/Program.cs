using ScsFileBrowser.UI.Forms;

namespace ScsFileBrowser;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var mainForm = new MainForm();
        var args = Environment.GetCommandLineArgs();
        if (args is {Length: > 1})
        {
            var possibleFilePath = args[1];
            if (File.Exists(possibleFilePath) &&
                Consts.AllowedDropExtensions.Contains(Path.GetExtension(possibleFilePath)))
                mainForm.OpenArchiveFile(possibleFilePath);
            else if (Directory.Exists(possibleFilePath))
                mainForm.OpenArchiveFolder(possibleFilePath);
        }

        Application.Run(mainForm);
    }
}
