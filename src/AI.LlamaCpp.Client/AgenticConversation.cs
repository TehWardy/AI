using System.Text;
using System.Text.Json;
using AI.LlamaCpp.Client.Models;
using AIServer.LlamaCpp;
using AIServer.LlamaCpp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AI.LlamaCpp.Client;

internal class AgenticConversation
{
    List<QuestionItem> questions = [];
    ILlamaCppChatClient chatClient;

    public AgenticConversation(IServiceProvider serviceProvider)
    {
        chatClient = serviceProvider
            .GetService<ILlamaCppChatClient>();

        chatClient.ModelId = "mistral";

        LoadQuestionData();
    }

    public async IAsyncEnumerable<ResponseToken> SendAsync(string userMessage)
    {
        var matchingQuestions = questions
            .Where(q => q.Topic
                .Contains(userMessage, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToArray();

        string context = "\nTopic: " + userMessage + "\nContext:\n" + 
            string.Join("\n", matchingQuestions.Select(q => JsonSerializer.Serialize(q)));

        yield return new ResponseToken { Message = new ResponseMessageData { Thought = context } };

        StringBuilder responseBuilder = new();
        StringBuilder verificationBuilder = new();

        bool finished = false;

        do
        {
            responseBuilder.Clear();
            verificationBuilder.Clear();

            yield return new ResponseToken { Message = new ResponseMessageData { Thought = "\nGenerating potential answer ...\n" } };

            chatClient.SetSystemPrompt("You are a concise and helpful assistant for generating exam question details based on the users given topic.\n" +
                "The given topic has been used as a search term to build the context.\n" +
                "Example questions are in the context, use only the context and generate a new question in the same format and on the same topic.\n" +
                "When outputting your result, use the same JSON structure as the context questions but indent so that the result can be easily read." +
                "The result should match the JSON format expressed for items in the context, and the topic in them, there should only be 1 resulting item in the response.");

            await foreach (ResponseToken token in chatClient.SendAsync(context))
            {
                yield return token;
                responseBuilder.Append(token.Message.Content);
            }

            yield return new ResponseToken { Message = new ResponseMessageData { Thought = "\n\nVerifying answer ...\n" } };

            finished = await ResponseIsGood(userMessage, context, responseBuilder.ToString());
            yield return new ResponseToken { Message = new ResponseMessageData { Thought = "Validation Passed: " + finished } };

        } while (!finished);

        yield return new ResponseToken { Message = new ResponseMessageData { Thought = "\n\nNew Question ...\n" } };
        yield return new ResponseToken { Message = new ResponseMessageData { Content = responseBuilder.ToString() } };
    }

    async ValueTask<bool> ResponseIsGood(string userMessage, string context, string response)
    {
        string structureResult = string.Empty;
        string topicResult = string.Empty;
        string relevanceResult = string.Empty;

        chatClient.SetSystemPrompt("You are a verifying assistant, " +
            "it is your job to verify that another assistant has done their job correctly, " +
            "every response MUST only be the word \"yes\" or \"no\", nothing else." +
            "Verify that 'Sample' and 'Output' json blobs given are deserializable into the same object definition.");

        IAsyncEnumerable<ResponseToken> structureCheck = chatClient
            .SendAsync($"Sample:\n{JsonSerializer.Serialize(questions.First())}\n\nOutput:\n{response}");

        await foreach (ResponseToken token in structureCheck)
            structureResult += token.Message.Content;

        chatClient.SetSystemPrompt("You are a verifying assistant, " +
            "it is your job to verify that another assistant has done their job correctly, " +
            "every response MUST only be the word \"yes\" or \"no\", nothing else." +
            "Verify that 'Sample' and 'Output' json blobs match in the \"Topic\" field?");

        IAsyncEnumerable<ResponseToken> topicCheck = chatClient
            .SendAsync($"Context:\n{context}\n\nOutput:\n{response}");

        chatClient.SetSystemPrompt("You are a verifying assistant, " +
            "it is your job to verify that another assistant has done their job correctly, " +
            "every response MUST only be the word \"yes\" or \"no\", nothing else." +
            "Verify that Topic, Context and Response are all on the same subject matter and that the question asked is correctly answered by the selection made in the JSON.");

        await foreach (ResponseToken token in topicCheck)
            topicResult += token.Message.Content;

        IAsyncEnumerable<ResponseToken> relevanceCheck = chatClient
            .SendAsync($"Topic:{userMessage}\nContext:\n{context}\n\nResponse:\n{response}\n\nHas the response correctly met the requirements?");

        await foreach (ResponseToken token in relevanceCheck)
            relevanceResult += token.Message.Content;

        return structureResult == topicResult && structureResult == relevanceResult;
    }

    void LoadQuestionData()
    {
        string[] rawCsvData = File
            .ReadAllText("E:\\AI\\Data\\Questions.csv")
            .Split("\n")
            .Skip(1)
            .ToArray();

        string[] parts;

        foreach (string csvItem in rawCsvData)
        {
            try
            {
                parts = csvItem.Split(";");
                questions.Add(new QuestionItem
                {
                    ItemName = parts[0],
                    Domain = parts[1].Split("-").Last(),
                    Topic = parts[2].Split("-").Last(),
                    ItemStatus = parts[3].Split("-").Last(),
                    TestType = parts[4],
                    FormHistory = parts[5],
                    Rationale = parts[6],
                    EvidenceStatement1 = parts[7],
                    EvidenceStatement2 = parts[8],
                    EvidenceStatement3 = parts[9],
                    Difficulty = double.TryParse(parts[10], out var diff) ? diff : null,
                    DifficultyDescription = parts[11],
                    DifficultyRating = double.TryParse(parts[12], out var diffRating) ? diffRating : null,
                    Discrimination = double.TryParse(parts[13], out var discrim) ? discrim : null,
                    DiscriminationDescription = parts[14],
                    DiscriminationRating = double.TryParse(parts[15], out var discrimRating) ? discrimRating : null,
                    AverageSecondsToAnswer = double.TryParse(parts[16], out var avgSecs) ? avgSecs : null,
                    SecondsDescription = parts[17],
                    Question = parts[18],
                    OptionA = parts[19].Trim(),
                    OptionB = parts[20].Trim(),
                    OptionC = parts[21].Trim(),
                    OptionD = parts[22].Trim(),
                    OptionE = parts[23].Trim(),
                    Answer = parts[24].Trim()
                });
            }
            catch { }
        }
    }
}