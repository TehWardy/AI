namespace AIServer.Memory;

public class ConversationHistoryProvider<TMessage> 
    : IConversationHistoryProvider<TMessage> 
        where TMessage : class
{
    IDictionary<string, List<TMessage>> conversations =
        new Dictionary<string, List<TMessage>>();

    public async ValueTask<List<TMessage>> GetConversationAsync(string id)
    {
        return conversations.ContainsKey(id)
            ? conversations[id]
            : [];
    }

    public async ValueTask UpdateConversationHistoryAsync(string id, List<TMessage> newHistory) =>
        conversations[id] = newHistory;
}