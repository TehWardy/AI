using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard;

public interface IStandardArchitectureTool
{
    ValidateAndNormalizeResult ValidateAndNormalize(ArchitectureSpec draft);
    string ValidateAndNormalize(string architectureSpecJson);
}