namespace ScsFileBrowser.FileSystem;

/// <summary>
///     Represents a file in the filesystem
///     <para>
///         Currently nothing more than just an <see cref="FileSystem.Entry" /> with how the workings of the filesystem
///         changed,
///         just keeping this in case I want to add something specific to files
///     </para>
///     <para>Is unaware of it's own location/path</para>
/// </summary>
internal class UberFile
{
    internal UberFile(Entry entry)
    {
        Entry = entry;
    }

    internal Entry Entry { get; }
}
