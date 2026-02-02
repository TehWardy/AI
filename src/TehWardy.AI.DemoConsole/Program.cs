using TehWardy.AI;
using TehWardy.AI.DemoConsole;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

AIFrameworkBuilder.Build(args, Data.UserConfiguration);

await AIFrameworkBuilder.CacheAgents(Data.Agents());
await AIFrameworkBuilder.CacheRunbooks(Data.Runbooks());

IAgenticConversation conversation = AIFrameworkBuilder
    .GetAgenticConversation();

while (true)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

    // Ask user for prompt
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("User: ");
    Console.ForegroundColor = ConsoleColor.White;

    string nextPrompt = Console.ReadLine()?.Trim();

    if (nextPrompt == "exit")
        break;

    if (string.IsNullOrEmpty(nextPrompt))
        continue;

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"\n[{DateTime.Now:HH:mm:ss}] ");

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Assistant: ");

    // Start streaming tokens from agent
    var prompt = new Prompt { Input = nextPrompt };

    IAsyncEnumerable<Token> response = conversation
        .InferStreamingAsync(prompt);

    bool thinking = true;

    await foreach (Token token in response)
    {
        if (thinking != token.Thought is not null)
            Console.Write("\n");

        thinking = token.Thought is not null;

        if (thinking)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(token.Thought);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(token.Content);
        }
    }

    Console.WriteLine();
}