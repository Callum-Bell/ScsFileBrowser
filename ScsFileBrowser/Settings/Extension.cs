namespace ScsFileBrowser.Settings;

public class Extension
{
    public string DefaultProgram { get; set; } = "";

    public string Name { get; set; } = "";


    public override string ToString()
    {
        return Name;
    }
}
