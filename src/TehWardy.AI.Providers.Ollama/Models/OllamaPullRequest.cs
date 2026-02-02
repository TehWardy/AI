namespace TehWardy.AI.Providers.Ollama.Models;

internal class OllamaPullRequest
{
    public string Model { get; set; } = "";
    public bool Stream { get; set; } = false; // easier to handle
    public bool? Insecure { get; set; }
}
