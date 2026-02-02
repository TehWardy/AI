
namespace TehWardy.AI.Tools.FileSystem.Brokers;

internal interface IDirectoryBroker
{
    ValueTask CreateDirectoryAsync(string path);
    ValueTask<bool> DirectoryExistsAsync(string path);
    ValueTask<string[]> ListDirectoriesAsync(string path);
    ValueTask<string[]> ListFilesAsync(string path);
}