namespace ScsFileBrowser.Settings;

public class Program
{
    public string Name { get; set; } = "";
    public string Path { get; init; } = "";

    public override string ToString()
    {
        return Name;
    }
}
