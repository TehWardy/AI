using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard;

internal interface IStandardArchitectureTool
{
    ValidateAndNormalizeResult ValidateAndNormalize(ArchitectureSpec draft);
    string ValidateAndNormalize(string architectureSpecJson);
}