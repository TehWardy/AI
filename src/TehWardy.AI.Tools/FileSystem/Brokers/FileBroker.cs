namespace TehWardy.AI.Tools.FileSystem.Brokers;

internal class FileBroker : IFileBroker
{
    public async ValueTask<string> ReadTextFromFileAsync(string path) =>
        await File.ReadAllTextAsync(path);

    public async ValueTask WriteTextToFileAsync(string path, string text) =>
        await File.WriteAllTextAsync(path, text);

    public ValueTask<bool> FileExistsAsync(string path) =>
        ValueTask.FromResult(File.Exists(path));

    public ValueTask<Stream> StreamFileAsync(string path) =>
        ValueTask.FromResult<Stream>(
            File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
}