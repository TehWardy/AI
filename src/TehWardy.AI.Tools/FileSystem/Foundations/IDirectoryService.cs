
namespace TehWardy.AI.Tools.FileSystem.Foundations;

internal interface IDirectoryService
{
    ValueTask CreateDirectoryAsync(string path);
    ValueTask<bool> DirectoryExistsAsync(string path);
    ValueTask<string[]> ListDirectoriesAsync(string path);
    ValueTask<string[]> ListFilesAsync(string path);
}