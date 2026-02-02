using TehWardy.AI.Tools.FileSystem.Foundations;

namespace TehWardy.AI.Tools.FileSystem.Orchestration;

internal class FileSystemOrchestrationService(
    IDirectoryService directoryService,
    IFileService fileService,
    FileSystemConfiguration config)
        : IFileSystemOrchestrationService
{
    public ValueTask<string> ReadTextFromFileAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        return fileService.ReadTextFromFileAsync(path);
    }

    public async ValueTask<bool> WriteTextToFileAsync(string path, string text)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        await fileService.WriteTextToFileAsync(path, text);
        return await fileService.FileExistsAsync(path);
    }

    public ValueTask<bool> FileExistsAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        return fileService.FileExistsAsync(path);
    }

    public ValueTask<Stream> StreamFileAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        return fileService.StreamFileAsync(path);
    }

    public ValueTask<string[]> ListDirectoriesAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        return directoryService.ListDirectoriesAsync(path);
    }

    public ValueTask<string[]> ListFilesAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        return directoryService.ListFilesAsync(path);
    }

    public ValueTask<bool> DirectoryExistsAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);

        return directoryService.DirectoryExistsAsync(path);
    }

    public async ValueTask<bool> CreateDirectoryAsync(string path)
    {
        path = SanitizePath(path);
        ValidatePath(path);

        if (!await directoryService.DirectoryExistsAsync(path))
            await directoryService.CreateDirectoryAsync(path);

        return true;
    }

    public async ValueTask<string[]> ListAllWithinPathAsync(string path, bool recursive = false)
    {
        path = SanitizePath(path);
        ValidatePath(path);
        List<string> results = [];

        string[] subDirectories = await ListDirectoriesAsync(path);

        foreach (string directory in subDirectories)
        {
            results.Add(directory);

            if (recursive)
                results.AddRange(await ListAllWithinPathAsync(directory, recursive));
        }

        results.AddRange(await ListFilesAsync(path));
        return results.ToArray();
    }

    void ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(config.RootWorkingDirectory))
            throw new ArgumentException("A configured root directory is required.", nameof(config.RootWorkingDirectory));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is required.", nameof(path));

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!path.StartsWith(config.RootWorkingDirectory, comparison))
            throw new UnauthorizedAccessException(
                $"The Path '{path}' is not within the permitted contextual root '{config.RootWorkingDirectory}'.");
    }

    string SanitizePath(string path) =>
        Path.IsPathFullyQualified(path)
            ? path
            : Path.Combine(config.RootWorkingDirectory, path);
}