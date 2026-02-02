using TehWardy.AI.Tools.FileSystem.Brokers;

namespace TehWardy.AI.Tools.FileSystem.Foundations;

internal class DirectoryService(IDirectoryBroker directoryBroker)
    : IDirectoryService
{
    public ValueTask CreateDirectoryAsync(string path)
    {
        ValidatePath(path);
        return directoryBroker.CreateDirectoryAsync(path);
    }

    public ValueTask<string[]> ListDirectoriesAsync(string path)
    {
        ValidatePath(path);
        return directoryBroker.ListDirectoriesAsync(path);
    }

    public ValueTask<string[]> ListFilesAsync(string path)
    {
        ValidatePath(path);
        return directoryBroker.ListFilesAsync(path);
    }


    public ValueTask<bool> DirectoryExistsAsync(string path)
    {
        ValidatePath(path);
        return directoryBroker.DirectoryExistsAsync(path);
    }

    static void ValidatePath(string path)
    {
        if (Path.HasExtension(path))
            throw new ArgumentException("Path is not a directory path.", nameof(path));
    }
}