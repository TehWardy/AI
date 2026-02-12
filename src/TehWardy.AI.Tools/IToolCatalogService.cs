using TehWardy.AI.Tools.Models;

namespace TehWardy.AI.Tools;
internal interface IToolCatalogService
{
    ValueTask<CatalogServer> GetCatalogServerAsync(string url);
}