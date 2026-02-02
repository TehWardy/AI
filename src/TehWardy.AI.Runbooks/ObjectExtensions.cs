using TehWardy.AI.Agents.Runbooks.Processings;

namespace TehWardy.AI.Agents.Runbooks;

internal static class ObjectExtensions
{
    public static Task<object> AwaitIfAwaitable(this object result) =>
        ObjectExtensionsProcessingService.AwaitIfAwaitable(result);
}
