namespace AIServer.Llama;

public static class WebApplicationExtensions
{
    static OllamaModelHost ollamaInstance;

    public static void StartOllamaInstance(this WebApplication app)
    {
        var config = app.Configuration;

        ollamaInstance = new(
            config.GetValue<string>("AI.OllamaExe"),
            config.GetValue<string>("AI.Model"));

        ollamaInstance.Start();
    }
}