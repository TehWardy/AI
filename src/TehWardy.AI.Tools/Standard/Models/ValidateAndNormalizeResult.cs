namespace TehWardy.AI.Tools.Standard.Models;

public sealed class ValidateAndNormalizeResult
{
    public ArchitectureSpec ValidatedArchitecture { get; set; }
    public ValidationResult Validation { get; set; }
    public ConformanceManifest Manifest { get; set; }
}
