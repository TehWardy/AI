using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;
internal interface IToolBroker
{
    ValueTask<Tool[]> GetToolsAsync(string url);
}