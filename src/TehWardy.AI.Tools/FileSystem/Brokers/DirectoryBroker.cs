namespace TehWardy.AI.Tools.FileSystem.Brokers;

internal class DirectoryBroker : IDirectoryBroker
{
    public ValueTask CreateDirectoryAsync(string path)
    {
        Directory.CreateDirectory(path);
        return ValueTask.CompletedTask;
    }

    public ValueTask<string[]> ListDirectoriesAsync(string path) =>
        ValueTask.FromResult(Directory.GetDirectories(path));

    public ValueTask<string[]> ListFilesAsync(string path) =>
        ValueTask.FromResult(Directory.GetFiles(path));

    public ValueTask<bool> DirectoryExistsAsync(string path) =>
        ValueTask.FromResult(Directory.Exists(path));
}