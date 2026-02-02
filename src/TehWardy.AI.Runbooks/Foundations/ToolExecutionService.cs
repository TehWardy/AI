using System.Reflection;
using System.Text.Json;
using TehWardy.AI.Agents.Runbooks;
using TehWardy.AI.Agents.Runbooks.Brokers;

namespace TehWardy.AI.Runbooks.Foundations;

internal class ToolExecutionService(IToolInstanceBroker toolInstanceBroker)
    : IToolExecutionService
{
    public async ValueTask<object> ExecuteToolFunctionAsync(string toolName, string functionName, IDictionary<string, object> arguments)
    {
        if (string.IsNullOrWhiteSpace(toolName))
            throw new ArgumentException("Tool name is required.", nameof(toolName));

        if (string.IsNullOrWhiteSpace(functionName))
            throw new ArgumentException("Function name is required.", nameof(functionName));

        object toolInstance = await toolInstanceBroker.GetToolInstanceAsync(toolName);

        if (toolInstance is null)
            throw new ArgumentNullException(nameof(toolInstance));

        return await ExecuteAsync(toolInstance, functionName, arguments);
    }

    private static async ValueTask<object> ExecuteAsync(
            object toolInstance,
            string functionName,
            IDictionary<string, object> arguments)
    {
        MethodInfo method = toolInstance
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(m => string.Equals(m.Name, functionName, StringComparison.OrdinalIgnoreCase));

        if (method is null)
        {
            string error = $"Method '{functionName}' not found on tool '{toolInstance.GetType().Name}'.";
            throw new InvalidOperationException(error);
        }

        object[] boundArgs = BindArguments(method, arguments);
        object rawResult = method.Invoke(toolInstance, boundArgs);
        return await rawResult.AwaitIfAwaitable();
    }

    private static object[] BindArguments(MethodInfo method, IDictionary<string, object> arguments)
    {
        ParameterInfo[] parameters = method.GetParameters();
        object[] bound = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];

            if (arguments is null || !arguments.TryGetValue(p.Name, out var raw) || raw is null)
            {
                if (p.HasDefaultValue)
                {
                    bound[i] = p.DefaultValue;
                    continue;
                }

                if (IsNullable(p.ParameterType))
                {
                    bound[i] = null;
                    continue;
                }

                throw new InvalidOperationException($"Missing required argument '{p.Name}'.");
            }

            bound[i] = ConvertArg(raw, p.ParameterType);
        }

        return bound;
    }

    private static object ConvertArg(object raw, Type targetType)
    {
        if (raw is null)
            return null;

        Type nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (nonNullable.IsInstanceOfType(raw))
            return raw;

        if (raw is JsonElement je)
        {
            return JsonSerializer.Deserialize(je.GetRawText(), nonNullable);
        }

        try
        {
            return Convert.ChangeType(raw, nonNullable);
        }
        catch
        {
            var options = new JsonSerializerOptions
            {
                RespectNullableAnnotations = true,
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(raw);
            return JsonSerializer.Deserialize(json, nonNullable, options);
        }
    }

    private static bool IsNullable(Type t) =>
        !t.IsValueType || Nullable.GetUnderlyingType(t) is not null;
}