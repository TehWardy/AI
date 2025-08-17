using AIServer.LlamaCpp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
await host.StartLlamaHostAsync("gpt-oss-20b-mxfp4");

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("Assistant: ");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Hello, I am a helpfull AI assistant, how can I help you today?");

ILlamaCppChatClient chatClient = host.Services
    .GetRequiredService<ILlamaCppChatClient>();

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