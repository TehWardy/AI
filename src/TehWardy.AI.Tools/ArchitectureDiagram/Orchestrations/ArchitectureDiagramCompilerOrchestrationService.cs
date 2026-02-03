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
        try
        {
            DiagramSpecification diagram = JsonSerializer
                .Deserialize<DiagramSpecification>(diagramJson);

            return ValidateAndCompileInternal(diagram);
        }
        catch (Exception ex)
        {
            var validationResult = new DiagramValidationResult
            {
                Diagnostics =
                [
                    new DiagramDiagnostic
                    {
                        Severity = DiagramDiagnosticSeverity.Error,
                        Message = $"Diagram failed to parse due to exception in the JSON parser: {ex.Message}"
                    }
                ]
            };

            return (null, validationResult);
        }
    }

    (ArchitectureSpec, DiagramValidationResult) ValidateAndCompileInternal(DiagramSpecification diagram)
    {
        DiagramValidationResult diagramValidationResult =
            architectureDiagramValidationProcessingService.Validate(diagram);

        ArchitectureSpec specification = null;

        if (diagramValidationResult.IsValid)
        {
            specification = architectureDiagramCompilerProcessingService
                .Compile(diagram);
        }

        return (specification, diagramValidationResult);
    }
}