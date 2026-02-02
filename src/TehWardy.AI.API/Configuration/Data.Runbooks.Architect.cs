using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Tools;

namespace TehWardy.AI.API.Configuration;

internal static partial class Data
{
    static readonly Runbook ArchitectRunbook = new()
    {
        Name = "Architect",
        FirstStepName = "Reason With Tools Loop",
        Policy = new RunbookPolicy
        {
            AllowedTools =
            [
                ToolDefinitions.FileSystem,
                ToolDefinitions.Github,
                ToolDefinitions.StandardArchitecture,
                ToolDefinitions.Variable
            ]
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
                            { "EmitTokens", true },
                            { "StoreAs", "ReasonResult" },
                            { "Instruction", """
You are the Architect agent. Modify the given DiagramSpecification.
You MUST:
 1) Emit a complete DiagramSpecification JSON
 2) Call the following tools in this order ...
    a. Architecture Diagram Tool to validate and compile the diagram into an ArchitectureSpec
    b. Standard Architecture tool to validate and normalize the compiled diagram

DiagramSpecification schema:
- { id: guid, name: string, nodes: [], edges: [] }
- Node: { id: guid, kind: Component|Model|External, name: string, role: Exposure|Orchestration|Processing|Service|Broker, methods: [], properties: [] }
- Edge: { id: guid, fromNodeId: guid, toNodeId: guid, kind: InProcess|ExternalBoundary }
- Method: { name: string, outputType: string, inputs: [] }
- Input: { name: string, type: string, required: bool }
- Property: { name: string, type: string, required: bool }

Rules:
- DiagramSpec Id properties need to be unique or the edge map won't make sense.
- The Diagram supports multiple Exposure nodes.
- Brokers must depend only on External nodes.
- Foundations must depend only on exactly 1 Broker node.
- Processings must depend only on exactly 1 Foundation node.
- Orchestrations must depend only on 2-3 Processing or Foundation nodes (but don't mix).
- Exposures must only depend on 1 Orchestration, Processing, or Foundation node.
- Keep diagram minimal and uniform using columns: Exposure -> Orchestration -> Processing -> Service -> Broker -> External.
- Omit Orchestration and Processing if there is no need.

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
                        StepType = "toolcall",
                        Parameters = new Dictionary<string, object>
                        {
                            { "MaxCalls", 2 },
                            { "SourceKey", "ReasonResult" },
                            { "StoreAs", "DiagramCompilationResult" },
                            { "AppendToHistory", true },
                            { "EmitCallsMadeTo", "StillThinking" }
                        }
                    },
                    new RunbookStep
                    {
                        Name = "Evaluate Validation Results",
                        StepType = "reason",
                        Parameters = new Dictionary<string, object>
                        {
                            { "EmitTokens", true },
                            { "SourceKey", "DiagramCompilationResult" },
                            { "StoreAs", "ValidationResult" },
                            { "Instruction", """
Summarize the results from the previous tool call result.
The tool call in question has done an architecture validation. 
The resulting JSON includes a validation summary and the validated architecture specification generated from a diagram specification produced by you previously.

You MUST take one of the following actions:
 1) If you think the DiagramSpecification is not valid ...
    Emit a summary of the required validation fixes that are needed plus the DiagramSpecification JSON that needs fixing.
 2) If you think the DiagramSpecification is valid ...
    Respond with ONLY the updated JSON DiagramSpeciication blob and a single tool call to Call "Variable.SetBool" and set the "StillThinking" variable to the boolean value of false.
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
                            { "MaxCalls", 1 },
                            { "SourceKey", "ValidationResult" },
                            { "StoreAs", "ValidationResults" },
                            { "AppendToHistory", true },
                            { "EmitCallsMadeTo", "StillThinking" }
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
}