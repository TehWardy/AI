using AIServer.Ollama.Models;

namespace AIServer.Ollama.Processings;
internal interface IMCPToolCallRequestProcessingService
{
    ValueTask<ResponseToken> ExecuteToolCallAsync(string model, ToolFunctionDetails callDetails);
}