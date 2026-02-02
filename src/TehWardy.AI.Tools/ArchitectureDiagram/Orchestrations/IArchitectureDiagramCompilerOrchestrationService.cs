using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Orchestrations;

internal interface IArchitectureDiagramCompilerOrchestrationService
{
    (ArchitectureSpec, DiagramValidationResult) ValidateAndCompile(string diagramJson);
}