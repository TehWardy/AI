using TehWardy.AI.Tools.Models;

namespace TehWardy.AI.Tools;
internal interface IToolCatalogBroker
{
    ValueTask<CatalogServer> GetCatalogServerAsync(string url);
}