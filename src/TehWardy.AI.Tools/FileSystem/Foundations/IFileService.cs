
namespace TehWardy.AI.Tools.FileSystem.Foundations;

internal interface IFileService
{
    ValueTask<bool> FileExistsAsync(string path);
    ValueTask<string> ReadTextFromFileAsync(string path);
    ValueTask<Stream> StreamFileAsync(string path);
    ValueTask WriteTextToFileAsync(string path, string text);
}