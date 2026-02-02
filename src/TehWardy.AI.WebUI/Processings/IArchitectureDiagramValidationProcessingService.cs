using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public interface IArchitectureDiagramValidationProcessingService
{
    DiagramValidationResult Validate(DiagramSpecification diagram);
}
