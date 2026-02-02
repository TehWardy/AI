namespace TehWardy.AI.Tools.Standard.Models;
public enum DiagnosticSeverity
{
    Info,
    Warning,
    Error
}

public sealed class Diagnostic
{
    public string Code { get; set; }
    public DiagnosticSeverity Severity { get; set; }
    public string Message { get; set; }
    public string JsonPath { get; set; }
}

public sealed class ValidationResult
{
    public IList<Diagnostic> Diagnostics { get; set; }

    public bool IsValid { get; set; }
}