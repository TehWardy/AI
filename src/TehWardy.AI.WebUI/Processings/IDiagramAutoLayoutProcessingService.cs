using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public interface IDiagramAutoLayoutProcessingService
{
    ComputedDiagramLayout Compute(DiagramSpecification diagram);
}
