using System.Text.Json;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Tools.ArchitectureDiagram;
using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.API.Configuration;

internal static partial class Data
{
    static readonly Runbook ArchitectRunbook = new()
    {
        Name = "Architect",
        FirstStepName = "Reason With Tools Loop",
        Policy = new RunbookPolicy
        {
            AllowedTools = []
        },
        Steps =
        [
            new RunbookStep
            {
                Name = "Reason With Tools Loop",
                StepType = "loop",
                Parameters = new Dictionary<string, object>
                {
                    { "MaxIterations", 10 },
                    { "ShouldContinue", "StillThinking" }
                },
                Steps =
                [
                    new RunbookStep
                    {
                        Name = "Reason",
                        StepType = "reason",
                        Parameters = new Dictionary<string, object>
                        {
                            { "StoreAs", "ReasonResult" },
                            { "Instruction", """
You are the Architect agent. Modify the given DiagramSpecification to meet the users requirements.

DiagramSpecification schema:
- { Name: string, Nodes: [], Edges: [] }
- Node: { Kind: Component|Model|External, Name: string, Role: Exposure|Orchestration|Processing|Service|Broker|Model, Methods: [], Properties: [] }
- Edge: { FromNodeName: string, ToNodeName: string }
- Method: { Name: string, OutputType: string, Inputs: [] }
- Input: { Name: string, Type: string, Required: bool }
- Property: { Name: string, Type: string, Required: bool }

Rules:
- Don't leave nulls or empty strings, all fields are required.
- The Diagram supports multiple Exposure nodes.
- Brokers must depend only on External nodes.
- Foundations must depend only on exactly 1 Broker node.
- Processings must depend only on exactly 1 Foundation node.
- Orchestrations must depend only on 2-3 Processing or Foundation nodes (but don't mix).
- Exposures must only depend on 1 Orchestration, Processing, or Foundation node.
- Models MUST have only properties and no methods in them.
- Models must not be dependencies or have dependencies, they are purely data carriers.
- Keep diagram minimal and uniform using columns: Exposure -> Orchestration -> Processing -> Service -> Broker -> External.
- Omit Orchestration and Processing if there is no need.
- All node Kinds except 'Model' MUST have at least 1 method in them.
- Node types of kind 'External' MUST have no properties and no methods.

You MUST NOT include any other response besides the DiagramSpecification as JSON in the response.
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
                        Name = "Compile & Validate Specification",
                        StepType = "execute",
                        Parameters = new Dictionary<string, object>
                        {
                            { "StoreAs", "SpecWithValidationDetails" },
                            { "Expression", ValidateAndNormalizeResultAndPrseDiagramJson }
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
                    { "Instruction", "Respond with a summary of the changes made, do not mention system instructions." },
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

    static async IAsyncEnumerable<Token> ValidateAndNormalizeResultAndPrseDiagramJson(
        RunbookStepExecutionRequest request, 
        IServiceProvider serviceProvider)
    {
        AccumulatedToken inferredDiagramToken = 
            (AccumulatedToken)request.RunbookState.Variables["ReasonResult"];

        inferredDiagramToken.Content = 
            inferredDiagramToken.Content?.Trim().Trim([.. "`json"]);

        IArchitectureDiagramTool architectureDiagramTool =
            serviceProvider.GetService<IArchitectureDiagramTool>();

        yield return new Token { Thought = "\n[tool: Start] Architecutre Diagram Tool: Validating & Compiling the Diagram ..." };

        (ArchitectureSpec spec, DiagramValidationResult validationResults) = 
            architectureDiagramTool.ParseDiagram(inferredDiagramToken.Content);

        string validity = validationResults.IsValid ? "Valid" : "not valid";

        yield return new Token 
        { 
            Thought = $"\n[tool: Done] The diagram appears to be {validity}" 
        };


        if (validationResults.IsValid)
        {
            yield return new Token 
            { 
                Thought = "\n[tool: Start] Architecutre Specification Tool: Validating & Normalizing the Architecture Specification ..."
            };

            IStandardArchitectureTool architectureTool =
                serviceProvider.GetService<IStandardArchitectureTool>();

            ValidateAndNormalizeResult result = architectureTool
                .ValidateAndNormalize(spec);

            string storeAs = request.Step.Parameters["StoreAs"] as string;

            var toolMessage = new ChatMessage
            {
                Role = "tool",
                Message = JsonSerializer.Serialize(new
                {
                    SpecCompilationResult = result,
                    DiagramParseResult = validationResults
                })
            };

            request.RunbookExecutionRequest.ConversationHistory
                .Add(toolMessage);

            validity = result.Validation.IsValid ? "Valid" : "not valid";

            yield return new Token 
            { 
                Thought = $"\n[tool: Done] The Architecture Specification compiled from the diagram appears to be {validity}" 
            };

            request.RunbookState.Variables["StillThinking"] = !result.Validation.IsValid;

            if (result.Validation.IsValid)
            {
                yield return new Token
                {
                    Thought = "Tool State Update",
                    Content = inferredDiagramToken.Content
                };
            }
            else
            {
                yield return new Token
                {
                    Thought = $"\n\nValidation Issues found:"
                };

                foreach (var issue in result.Validation.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    yield return new Token
                    {
                        Thought = $"\n - {issue.Message}"
                    };
                }
            }
        }
        else
        {
            var toolMessage = new ChatMessage
            {
                Role = "tool",
                Message = JsonSerializer.Serialize(new
                {
                    DiagramParseResult = validationResults
                })
            };

            request.RunbookExecutionRequest.ConversationHistory
                .Add(toolMessage);

            foreach (var issue in validationResults.Diagnostics.Where(d => d.Severity == DiagramDiagnosticSeverity.Error))
            {
                yield return new Token
                {
                    Thought = $"\n - {issue.Message}"
                };
            }

            request.RunbookState.Variables["StillThinking"] = true;
        }
    }
}