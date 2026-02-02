using TehWardy.AI.Tools.ArchitectureDiagram.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Processings;

public interface IArchitectureDiagramValidationProcessingService
{
    DiagramValidationResult Validate(DiagramSpecification diagram);
}
