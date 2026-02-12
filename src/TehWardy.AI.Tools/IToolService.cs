using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.Models;

namespace TehWardy.AI.Tools;
internal interface IToolService
{
    ValueTask<Tool[]> GetToolsAsync(CatalogServer catalogServer);
}