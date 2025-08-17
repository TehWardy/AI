namespace AIServer.LlamaCpp.Configurations;

public class LlamaCppConfiguration
{
    public string ModelsPath { get; set; }
    public int ServerPort { get; set; } = 8080;
    public int ContextSize { get; set; } = 4096;
    public int BatchSize { get; set; } = 128;
    public int GpuLayerCount { get; set; } = 128;
    public bool UseMemoryLock { get; set; } = true;
}
