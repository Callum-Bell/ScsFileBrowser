namespace ScsFileBrowser.FileSystem.Hash;

internal class BaseHashArchiveHeader
{
    internal uint Magic { get; set; }
    internal ushort Version { get; set; }
    internal ushort Salt { get; set; }
    internal uint HashMethod { get; set; }
    internal uint EntryCount { get; set; }
}
