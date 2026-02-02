using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Agents.Runbooks.Brokers;

namespace TehWardy.AI.Runbooks.Brokers;

internal class ToolInstanceBroker(IServiceProvider serviceProvider) : IToolInstanceBroker
{
    public ValueTask<object> GetToolInstanceAsync(string toolName) =>
        ValueTask.FromResult(serviceProvider.GetKeyedService<object>(toolName));
}