using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Orchestrators;

internal interface IArchitectureSpecificationOrchestrationService
{
    ValidateAndNormalizeResult ValidateAndNormalize(ArchitectureSpec draft);
    string ValidateAndNormalize((ArchitectureSpec, DiagramValidationResult) validatedDiagram);
    string ValidateAndNormalize(string architectureSpecJson);
}