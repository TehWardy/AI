using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Processings;

public interface IArchitectureDiagramCompilerProcessingService
{
    ArchitectureSpec Compile(DiagramSpecification diagram);
}
