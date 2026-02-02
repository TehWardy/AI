namespace TehWardy.AI.Agents.Runbooks.Brokers;

internal interface IToolExecutionService
{
    ValueTask<object> ExecuteToolFunctionAsync(string toolName, string functionName, IDictionary<string, object> arguments);
}