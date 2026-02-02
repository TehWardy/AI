namespace TehWardy.AI.Agents.Runbooks.Brokers;

internal interface IToolInstanceBroker
{
    ValueTask<object> GetToolInstanceAsync(string toolName);
}