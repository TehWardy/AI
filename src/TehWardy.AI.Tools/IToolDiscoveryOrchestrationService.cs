using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;
internal interface IToolDiscoveryOrchestrationService
{
    IAsyncEnumerable<Tool> DiscoverToolsAsync();
}