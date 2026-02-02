using System.Diagnostics;

namespace TehWardy.AI.Providers.Foundations;
internal interface IProcessService
{
    Process CreateProcess(string executablePath, string workingDirectory, string arguments);
}