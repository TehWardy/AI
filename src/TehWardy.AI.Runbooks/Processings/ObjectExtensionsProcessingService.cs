namespace TehWardy.AI.Agents.Runbooks.Processings;

internal class ObjectExtensionsProcessingService
{
    public static async Task<object> AwaitIfAwaitable(object result)
    {
        if (result is null) return null;
        if (result is string s) return s;

        if (result is Task task)
        {
            await task.ConfigureAwait(false);

            Type t = task.GetType();
            return t.IsGenericType
                ? t.GetProperty("Result")?.GetValue(task)
                : null;
        }

        if (result is ValueTask vt)
        {
            await vt.ConfigureAwait(false);
            return null;
        }

        Type type = result.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            // ValueTask<T>.AsTask()
            var asTask = (Task)type.GetMethod("AsTask")!.Invoke(result, null)!;
            await asTask.ConfigureAwait(false);
            return asTask.GetType().GetProperty("Result")?.GetValue(asTask);
        }

        return result;
    }
}