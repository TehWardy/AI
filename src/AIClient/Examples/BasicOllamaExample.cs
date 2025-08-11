using AIServer.Ollama;
using AIServer.Ollama.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AIClient.Examples;

internal class BasicOllamaExample
{
    public static async ValueTask RunAsync(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var builder = Host.CreateApplicationBuilder(args);

        //builder.Services.AddOllamaHost(config.GetValue<string>("AI.OllamaExe"));
        //builder.Services.AddOllamaClient(config.GetValue<string>("AI.OllamaService"));

        builder.Services.AddOllamaHost("E:\\AI\\Ollama\\Ollama.exe");
        builder.Services.AddOllamaClient("http://localhost:11434/");

        IHost host = builder.Build();
        //host.StartOllama();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Assistant: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Hello, I am a helpfull AI assistant, how can I help you today?");

        IOllamaChatClient chatClient = host.Services
            .GetRequiredService<IOllamaChatClient>();

        //chatClient.ModelId = config.GetValue<string>("AI.Model");
        chatClient.ModelId = "gpt-oss:20b";

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

            // Ask user
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("User: ");
            Console.ResetColor();

            string nextPrompt = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(nextPrompt))
                continue; // skip empty lines

            // Show assistant label in green
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Assistant: ");

            // print AI repsonse content in yellow for emphasis
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Start streaming tokens
            await foreach (ResponseToken token in chatClient.SendAsync(nextPrompt))
                Console.Write(token.Message.Content);

            Console.WriteLine();
        }
    }
}
