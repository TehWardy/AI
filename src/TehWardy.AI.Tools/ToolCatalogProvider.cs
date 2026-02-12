using System.Net.Http.Json;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.Configurations;
using TehWardy.AI.Tools.Models;

namespace TehWardy.AI.Tools;

internal class ToolDiscoveryProvider(
    IToolDiscoveryOrchestrationService toolDiscoveryOrchestrationService) : IToolDiscoveryProvider
{
    public IAsyncEnumerable<Tool> DiscoverToolsAsync() =>
        toolDiscoveryOrchestrationService.DiscoverToolsAsync();
}

internal class ToolDiscoveryOrchestrationService(
    ToolCatalogConfiguration toolCatalogConfiguration,
    IToolCatalogService toolCatalogService,
    IToolService toolService) : IToolDiscoveryOrchestrationService
{
    public async IAsyncEnumerable<Tool> DiscoverToolsAsync()
    {
        foreach (string catalogServerUrl in toolCatalogConfiguration.CatalogServerUrls)
        {
            CatalogServer catalogServer =
                await toolCatalogService.GetCatalogServerAsync(catalogServerUrl);

            Tool[] tools = await toolService.GetToolsAsync(catalogServer);

            foreach (Tool tool in tools)
                yield return tool;
        }
    }
}

internal class ToolCatalogService(IToolCatalogBroker toolCatalogBroker) : IToolCatalogService
{
    public async ValueTask<CatalogServer> GetCatalogServerAsync(string url)
    {
        try
        {
            return await toolCatalogBroker
                .GetCatalogServerAsync(url);
        }
        catch (Exception ex)
        {
            //TODO: handle this better
            return null;
        }
    }
}

internal class ToolCatalogBroker(HttpClient httpClient) : IToolCatalogBroker
{
    public async ValueTask<CatalogServer> GetCatalogServerAsync(string url) =>
        await httpClient.GetFromJsonAsync<CatalogServer>(url);
}

internal class ToolService(IToolBroker toolBroker) : IToolService
{
    public async ValueTask<Tool[]> GetToolsAsync(CatalogServer catalogServer)
    {
        try
        {
            return await toolBroker.GetToolsAsync(catalogServer.Endpoint);
        }
        catch (Exception ex)
        {
            //TODO: handle this better
            return null;
        }
    }
}

internal class ToolBroker(HttpClient httpClient) : IToolBroker
{
    public async ValueTask<Tool[]> GetToolsAsync(string url) =>
        await httpClient.GetFromJsonAsync<Tool[]>(url);
}