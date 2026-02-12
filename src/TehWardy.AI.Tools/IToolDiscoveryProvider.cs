using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;
internal interface IToolDiscoveryProvider
{
    IAsyncEnumerable<Tool> DiscoverToolsAsync();
}