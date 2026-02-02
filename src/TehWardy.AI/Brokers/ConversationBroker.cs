using TehWardy.AI.Models;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Brokers;

internal class ConversationBroker(IDataCacheProviderFactory dataCacheProviderFactory)
    : IConversationBroker
{
    public async ValueTask<List<Conversation>> RetrieveAllConversationsAsync()
    {
        var provider = await dataCacheProviderFactory
            .CreateDataCacheProviderAsync<Conversation>("Default");

        return await provider.GetAllItemsAsync();
    }

    public async ValueTask<Conversation> GetConversationByIdAsync(Guid conversationId)
    {
        var provider = await dataCacheProviderFactory
            .CreateDataCacheProviderAsync<Conversation>("Default");

        return await provider.GetItemByKeyAsync(conversationId.ToString());
    }

    public async ValueTask SaveConversationAsync(Conversation conversation)
    {
        var provider = await dataCacheProviderFactory
            .CreateDataCacheProviderAsync<Conversation>("Default");

        await provider.AddOrUpdateAsync(conversation.Id.ToString(), conversation);
    }
}
