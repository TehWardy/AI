namespace TehWardy.AI.Providers.AI.Foundations;

internal interface IRoutingService
{
    ValueTask<string> RetrieveAllRoutingInformationAsync<T>();
}