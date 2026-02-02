namespace TehWardy.AI.Tools.FileSystem;

internal interface IFileSystemTool
{
    ValueTask<bool> CreateDirectoryAsync(string path);
    ValueTask<bool> DirectoryExistsAsync(string path);
    ValueTask<bool> FileExistsAsync(string path);
    ValueTask<string[]> ListAllWithinPathAsync(string path, bool recursive = false);
    ValueTask<string[]> ListDirectoriesAsync(string path);
    ValueTask<string[]> ListFilesAsync(string path);
    ValueTask<string> ReadTextFromFileAsync(string path);
    ValueTask<Stream> StreamFileAsync(string path);
    ValueTask<bool> WriteTextToFileAsync(string path, string text);
}