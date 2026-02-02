
namespace TehWardy.AI.Tools.FileSystem.Orchestration;

internal interface IFileSystemOrchestrationService
{
    ValueTask<bool> DirectoryExistsAsync(string path);
    ValueTask<bool> FileExistsAsync(string path);
    ValueTask<string[]> ListDirectoriesAsync(string path);
    ValueTask<string[]> ListFilesAsync(string path);
    ValueTask<string> ReadTextFromFileAsync(string path);
    ValueTask<Stream> StreamFileAsync(string path);
    ValueTask<bool> WriteTextToFileAsync(string path, string text);
    ValueTask<string[]> ListAllWithinPathAsync(string path, bool recursive = false);
    ValueTask<bool> CreateDirectoryAsync(string path);
}