namespace ScsFileBrowser.Settings;

public class Settings
{
    public string LastExportPath { get; set; } = "";
    public string LastSelectedFile { get; set; } = "";
    public string LastSelectedFolder { get; set; } = "";

    public List<string> RecentPaths { get; set; } = new();

    public List<Program> Programs { get; } = new();

    public List<Extension> Extensions { get; } = new();
}
