using AI.LlamaCpp.Client;
using AIServer.LlamaCpp;
using AIServer.LlamaCpp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLlamaCpp(llamaCppConfig =>
{
    llamaCppConfig.ModelsPath = config.GetValue<string>("AI.ModelsPath");
});

IHost host = builder.Build();
//IAsyncEnumerable<string> initStream = host.StartLlamaHostAsync("Mistral-7B-Instruct-v0.3.Q8_0");

//await foreach (string initLine in initStream)
//    Console.WriteLine(initLine);

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("Assistant: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Hello, I am a helpfull AI assistant, how can I help you today?");

AgenticConversation chatClient = new(host.Services);

while (true)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

    // Ask user
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("User: ");
    Console.ResetColor();

    string nextPrompt = Console.ReadLine()?.Trim();

    if (nextPrompt == "exit")
    {
        await host.StopLlamaHostAsync();
        break;
    }

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
        Console.Write(token.Message.Thought ?? token.Message.Content);

    Console.WriteLine();
}