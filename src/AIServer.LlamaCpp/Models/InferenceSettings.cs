namespace AIServer.LlamaCpp.Models;

internal class InferenceSettings
{
    public int MaxTokens { get; set; } = -1;
    public float Temperature { get; set; } = 0.7f;
    public float TopP { get; set; } = 0.9f;
    public float MinP { get; set; } = 0.05f;
    public int TopK { get; set; } = 40;
    public float RepeatPenalty { get; set; } = 1.1f;
    public string[] AntiPrompts { get; set; } = []; // ["<|user|>", "<|end|>", "User:"];
}