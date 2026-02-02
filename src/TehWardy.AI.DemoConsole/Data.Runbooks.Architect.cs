using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Tools;

namespace TehWardy.AI.DemoConsole;

internal static partial class Data
{
    static readonly Runbook ArchitectRunbook = new()
    {
        Name = "Architect",
        FirstStepName = "ReasonWithToolsLoop",
        Policy = new RunbookPolicy
        {
            AllowedTools =
            [
                ToolDefinitions.FileSystem,
                ToolDefinitions.Github,
                ToolDefinitions.StandardArchitecture
            ]
        },
        Steps =
        [
            new RunbookStep
            {
                Name = "ReasonWithToolsLoop",
                StepType = "loop",
                Parameters = new Dictionary<string, object>
                {
                    { "MaxIterations", 10 },
                    { "ShouldContinue", "CallsWereMade" }
                },
                Steps =
                [
                    new RunbookStep
                    {
                        Name = "Reason",
                        StepType = "reason",
                        Parameters = new Dictionary<string, object>
                        {
                            { "EmitTokens", true },
                            { "StoreAs", "ReasonResult" },
                            { "Instruction", """
Evaluate the users request and plan out next steps or provide additional summarized context for the final response.
If there are tool results in the recent conversation history, summarize the outcome and extract any actionable facts, then make any required follow on tool calls.
Your goal is to produce an Architecture design from the given prompt that is compliant and meets the intentional needs of the user.
Your response should be only a JSON blob that contains the Architecture design to be validated and normalized for later use.
With your response also be sure to call the Architecture tool which will confirm you met the design standards.
"""
                            },
                            { "LLMProviderName", "Ollama" },
                            { "LLMModelName", "gpt-oss:20b" },
                            { "EmbeddingProviderName", "Ollama" },
                            { "EmbeddingModelName", "nomic-embed-text" },
                            { "MemoryProviderName", "MemCache" },
                            { "MaxHistoryMessages", 8 },
                            { "ContextLength", 128000 }
                        }
                    },
                    new RunbookStep
                    {
                        Name = "CallTools",
                        StepType = "toolcall",
                        Parameters = new Dictionary<string, object>
                        {
                            { "MaxCalls", 6 },
                            { "SourceKey", "ReasonResult" },
                            { "StoreAs", "ToolResults" },
                            { "AppendToHistory", true },
                            { "EmitCallsMadeTo", "CallsWereMade" }
                        }
                    }
                ]
            },
            new RunbookStep
            {
                Name = "Respond",
                StepType = "respond",
                Parameters = new Dictionary<string, object>
                {
                    { "Instruction", "Generate a final response to the users question." },
                    { "LLMProviderName", "Ollama" },
                    { "LLMModelName", "ministral-3:14b" },
                    { "EmbeddingProviderName", "Ollama" },
                    { "EmbeddingModelName", "nomic-embed-text" },
                    { "MemoryProviderName", "MemCache" },
                    { "MaxHistoryMessages", 8 },
                    { "ContextLength", 128000 }
                }
            }
        ]
    };
}