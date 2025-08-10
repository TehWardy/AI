
namespace AIServer.Memory;

public interface IConversationHistoryProvider<TMessage> 
    where TMessage : class
{
    ValueTask<List<TMessage>> GetConversationAsync(string id);
}