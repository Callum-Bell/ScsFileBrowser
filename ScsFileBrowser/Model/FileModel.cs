using ScsFileBrowser.FileSystem;

namespace ScsFileBrowser.Model;

internal struct FileModel
{
    public string Name { get; set; }
    public string Source { get; set; }
    public uint Size { get; set; }

    public IReadOnlyList<UberFile> FileEntries { get; set; }
}
