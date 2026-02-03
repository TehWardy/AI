namespace TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

public sealed class DiagramValidationResult
{
    public IList<DiagramDiagnostic> Diagnostics { get; set; } = new List<DiagramDiagnostic>();

    public bool IsValid =>
        Diagnostics.All(d => d.Severity != DiagramDiagnosticSeverity.Error);
}

public enum DiagramDiagnosticSeverity
{
    Info,
    Warning,
    Error
}
public sealed class DiagramDiagnostic
{
    public string Code { get; set; }
    public DiagramDiagnosticSeverity Severity { get; set; }
    public string Message { get; set; }

    public string NodeName { get; set; }
    public string EdgeName { get; set; }
}