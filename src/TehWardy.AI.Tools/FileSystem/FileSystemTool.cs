using TehWardy.AI.Tools.FileSystem.Orchestration;

namespace TehWardy.AI.Tools.FileSystem;

internal class FileSystemTool(
    IFileSystemOrchestrationService fileSystemOrchestrationService) : IFileSystemTool
{
    public ValueTask<string> ReadTextFromFileAsync(string path) =>
        fileSystemOrchestrationService.ReadTextFromFileAsync(path);


    public ValueTask<bool> WriteTextToFileAsync(string path, string text) =>
        fileSystemOrchestrationService.WriteTextToFileAsync(path, text);


    public ValueTask<bool> FileExistsAsync(string path) =>
        fileSystemOrchestrationService.FileExistsAsync(path);


    public ValueTask<Stream> StreamFileAsync(string path) =>
        fileSystemOrchestrationService.StreamFileAsync(path);


    public ValueTask<string[]> ListDirectoriesAsync(string path) =>
        fileSystemOrchestrationService.ListDirectoriesAsync(path);


    public ValueTask<string[]> ListFilesAsync(string path) =>
        fileSystemOrchestrationService.ListFilesAsync(path);


    public ValueTask<bool> DirectoryExistsAsync(string path) =>
        fileSystemOrchestrationService.DirectoryExistsAsync(path);

    public ValueTask<string[]> ListAllWithinPathAsync(string path, bool recursive = false) =>
        fileSystemOrchestrationService.ListAllWithinPathAsync(path, recursive);

    public ValueTask<bool> CreateDirectoryAsync(string path) =>
        fileSystemOrchestrationService.CreateDirectoryAsync(path);
}
