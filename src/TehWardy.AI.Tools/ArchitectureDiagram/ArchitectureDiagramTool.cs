using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.ArchitectureDiagram.Orchestrations;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram;

internal class ArchitectureDiagramTool(
    IArchitectureDiagramCompilerOrchestrationService architectureDiagramCompilerOrchestrationService)
{
    public (ArchitectureSpec, DiagramValidationResult) ParseDiagram(string diagramJson) =>
        architectureDiagramCompilerOrchestrationService.ValidateAndCompile(diagramJson);
}