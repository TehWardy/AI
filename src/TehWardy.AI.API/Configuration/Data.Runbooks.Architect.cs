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
                    { "MaxIterations", 20 },
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
You are the Architect agent. Modify the given DiagramSpecification (if one is provided) or create a new one to meet the users requirements.

DiagramSpecification schema:
- { Name: string, Nodes: [], Edges: [] }
- Node: { Kind: Component|Model|External, Name: string, Role: Exposure|Orchestration|Processing|Service|Broker|Model, Methods: [], Properties: [] }
- Edge: { FromNodeName: string, ToNodeName: string, FromMethodName: string, ToMethodName: string }
- Method: { Name: string, OutputType: string, Inputs: [] }
- Input: { Name: string, Type: string, Required: bool }
- Property: { Name: string, Type: string, Required: bool }

Rules:
- Don't leave nulls or empty strings, all fields are required.
- The Diagram supports multiple Exposure nodes, create as many as are needed to 'Expose' the underlying functionality implementation.
- Brokers must depend only on External nodes.
- Foundations MUST depend only on exactly 1 Broker node.
- Foundations provide basic CRUD operations GetAll, GetById, Add, Update, Delete.
- Foundations have the 'Service' suffix in their name
- Processings MUST depend only on exactly 1 Foundation node.
- Processings provide complex operations Save, Merge, Compute, Compile.
- Processings have the 'ProcessingService' suffix in their name
- Orchestrations MUST depend only on 2-3 Processing or Foundation nodes (but don't mix).
- Orchestrations 'Orchestrate' between Dependencies ... RegisterUser(User userWithARole) -> UserProcessing.SaveUser(user), UserRoleProcessing.AddUserToRole(userRole).
- Orchestrations have the 'OrchestrationService' suffix in their name.
- Exposures MUST only depend on 1 Orchestration, Processing, or Foundation node.
- Exposures of commonly known kinds might have a suffix in their name like 'Controller' or 'Provider' or 'Tool'
- Methods: Can have a maximum of 3 parameters, if you need more generate a model and place it in that layer.
- Methods: Think Business operations when naming methods, all should be async and return ValueTask not Task.
- Methods: Consider how the method might be implemented so for example SaveFooAsync would need a Foo to save and return the saved Foo asynchronously.
- Unless a model is created as part of the previous rule, models should live in the external layer.
- Models MUST have only properties and no methods in them.
- Models MUST have at least 1 property.
- Models MUST NOT be dependencies or have dependencies, they are purely data carriers.
- Models typically map to things like database tables, so typically do not have a suffix in their name, think DTO types ... 'Person', 'Company', 'User', 'Role'
- Keep diagram minimal and uniform using columns: Exposure -> Orchestration -> Processing -> Service -> Broker -> External.
- Think about model (entity types) and design Foundations and Processings that are clean (one per entity type), optionally orchestrate.
- Omit Orchestration and Processing if there is no need.
- All node Kinds except 'Model' MUST have at least 1 method in them.
- Externals MUST NOT have properties or methods.

You MUST NOT include any other response besides the DiagramSpecification as JSON in the response.
"""
                            },
                            { "LLMProviderName", "Ollama" },
                            { "LLMModelName", "gpt-oss:20b" },
                            { "EmbeddingProviderName", "Ollama" },
                            { "EmbeddingModelName", "nomic-embed-text" },
                            { "MemoryProviderName", "MemCache" },
                            { "MaxHistoryMessages", 18 },
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
                    { "Instruction", "Respond with a summary of the changes made, do not mention instructions only mention the diagram work done." },
                    { "LLMProviderName", "Ollama" },
                    { "LLMModelName", "ministral-3:14b" },
                    { "EmbeddingProviderName", "Ollama" },
                    { "EmbeddingModelName", "nomic-embed-text" },
                    { "MemoryProviderName", "MemCache" },
                    { "MaxHistoryMessages", 18 },
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