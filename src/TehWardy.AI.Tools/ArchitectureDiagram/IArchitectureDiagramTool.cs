using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram;

public interface IArchitectureDiagramTool
{
    (ArchitectureSpec, DiagramValidationResult) ParseDiagram(string diagramJson);
}