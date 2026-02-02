using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;
using TehWardy.AI.Tools.Standard.Orchestrators;

namespace TehWardy.AI.Tools.Standard;

internal sealed class StandardArchitectureTool(
    IArchitectureSpecificationOrchestrationService architectureSpecificationOrchestrationService)
        : IStandardArchitectureTool
{
    public ValidateAndNormalizeResult ValidateAndNormalize(ArchitectureSpec draftSpec) =>
        architectureSpecificationOrchestrationService.ValidateAndNormalize(draftSpec);

    public string ValidateAndNormalize((ArchitectureSpec, DiagramValidationResult) validatedDiagram) =>
        architectureSpecificationOrchestrationService.ValidateAndNormalize(validatedDiagram);

    public string ValidateAndNormalize(string draftSpec) =>
        architectureSpecificationOrchestrationService.ValidateAndNormalize(draftSpec);
}