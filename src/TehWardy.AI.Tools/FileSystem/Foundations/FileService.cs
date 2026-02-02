using TehWardy.AI.Tools.FileSystem.Brokers;

namespace TehWardy.AI.Tools.FileSystem.Foundations;

internal class FileService(IFileBroker fileBroker) : IFileService
{
    public ValueTask<string> ReadTextFromFileAsync(string path)
    {
        ValidatePath(path);
        return fileBroker.ReadTextFromFileAsync(path);
    }

    public ValueTask WriteTextToFileAsync(string path, string text)
    {
        ValidatePath(path);
        return fileBroker.WriteTextToFileAsync(path, text);
    }

    public ValueTask<bool> FileExistsAsync(string path)
    {
        ValidatePath(path);
        return fileBroker.FileExistsAsync(path);
    }

    public ValueTask<Stream> StreamFileAsync(string path)
    {
        ValidatePath(path);
        return fileBroker.StreamFileAsync(path);
    }

    static void ValidatePath(string path)
    {
        if (!Path.HasExtension(path))
            throw new ArgumentException("Path is not a file path.", nameof(path));
    }
}