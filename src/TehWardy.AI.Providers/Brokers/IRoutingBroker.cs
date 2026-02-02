namespace TehWardy.AI.Providers.Brokers;

internal interface IRoutingBroker
{
    ValueTask<object> RetrieveAllRoutingInformationAsync<T>();
}