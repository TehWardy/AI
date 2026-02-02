using System.Text.Json;
using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.ArchitectureDiagram.Processings;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Orchestrations;

internal class ArchitectureDiagramCompilerOrchestrationService(
    IArchitectureDiagramValidationProcessingService architectureDiagramValidationProcessingService,
    IArchitectureDiagramCompilerProcessingService architectureDiagramCompilerProcessingService) :
        IArchitectureDiagramCompilerOrchestrationService
{
    public (ArchitectureSpec, DiagramValidationResult) ValidateAndCompile(string diagramJson)
    {
        DiagramSpecification diagram = JsonSerializer
            .Deserialize<DiagramSpecification>(diagramJson);

        DiagramValidationResult diagramValidationResult = 
            architectureDiagramValidationProcessingService.Validate(diagram);

        ArchitectureSpec specification = 
            architectureDiagramCompilerProcessingService.Compile(diagram);

        return (specification, diagramValidationResult);
    }
}