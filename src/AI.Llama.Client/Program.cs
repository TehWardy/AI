using AIServer.Llama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLlama(modelsPath: config.GetValue<string>("AI.ModelsPath"));

IHost host = builder.Build();

Console.ForegroundColor = ConsoleColor.White;

ILlamaChatClient chatClient = host.Services
    .GetRequiredService<ILlamaChatClient>();

await chatClient.InitializeChatSession(
    modelName: "Mistral-7B-Instruct-v0.3.Q8_0", 
    systemPrompt: "You are a concise assistant. keep your answers to user prompts short.");

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("Assistant: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Hello! How can I assist you today?");

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
    await foreach (string token in chatClient.SendAsync(nextPrompt))
        Console.Write(token);

    Console.WriteLine();
}