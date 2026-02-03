using System.Text.Json;
using System.Text.Json.Serialization;
using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;
using TehWardy.AI.Tools.Standard.Processings;

namespace TehWardy.AI.Tools.Standard.Orchestrators;

internal class ArchitectureSpecificationOrchestrationService(
    IArchitectureNormalizerProcessingService architectureNormalizerProcessingService,
    IArchitectureValidatorProcessingService architectureValidatorProcessingService,
    IManifestGenerationProcessingService manifestGenerationProcessingService)
        : IArchitectureSpecificationOrchestrationService
{

    static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public ValidateAndNormalizeResult ValidateAndNormalize(ArchitectureSpec draft)
    {
        var normalized = architectureNormalizerProcessingService
            .Normalize(draft);

        var validation = architectureValidatorProcessingService
            .Validate(normalized);

        var result = new ValidateAndNormalizeResult
        {
            ValidatedArchitecture = normalized,
            Validation = validation,
            Manifest = null
        };

        if (validation != null && validation.IsValid)
        {
            result.Manifest = manifestGenerationProcessingService
                .Generate(normalized);
        }

        return result;
    }

    public string ValidateAndNormalize((ArchitectureSpec, DiagramValidationResult) validatedDiagram)
    {
        (ArchitectureSpec spec, DiagramValidationResult validationResult) = validatedDiagram;

        var result = ValidateAndNormalize(spec);

        if (validationResult.IsValid is not true)
        {
            foreach (var diagramDiagnostic in validationResult.Diagnostics)
            {
                List<Diagnostic> specDiagnostics = 
                    MapToSpecDiagnostics(diagramDiagnostic);

                foreach (var specDiagnostic in specDiagnostics)
                    result.Validation.Diagnostics.Insert(0, specDiagnostic);
            }
        }

        return JsonSerializer.Serialize(result.ValidatedArchitecture, serializerOptions);
    }

    static List<Diagnostic> MapToSpecDiagnostics(DiagramDiagnostic diagramDiagnostic)
    {
        List<Diagnostic> results = [];

        string nodePath = diagramDiagnostic.NodeName != null
            ? $"Nodes/{diagramDiagnostic.NodeName}"
            : null;

        string edgePath = diagramDiagnostic.EdgeName != null
            ? $"Edges/{diagramDiagnostic.EdgeName}"
            : null;

        if (nodePath is not null)
        {
            results.Add(new()
            {
                Message = diagramDiagnostic.Message,
                Severity = MapSeverity(diagramDiagnostic.Severity),
                Code = diagramDiagnostic.Code,
                JsonPath = nodePath
            });
        }

        if (edgePath is not null)
        {
            results.Add(new()
            {
                Message = diagramDiagnostic.Message,
                Severity = MapSeverity(diagramDiagnostic.Severity),
                Code = diagramDiagnostic.Code,
                JsonPath = edgePath
            });
        }

        return results;
    }

    static DiagnosticSeverity MapSeverity(DiagramDiagnosticSeverity severity) => severity switch
    {
        DiagramDiagnosticSeverity.Info => DiagnosticSeverity.Info,
        DiagramDiagnosticSeverity.Warning => DiagnosticSeverity.Warning,
        DiagramDiagnosticSeverity.Error => DiagnosticSeverity.Error,
        _ => throw new NotImplementedException()
    };

    public string ValidateAndNormalize(string architectureSpecJson)
    {
        try
        {
            var spec = JsonSerializer.Deserialize<ArchitectureSpec>(architectureSpecJson, serializerOptions);
            var result = ValidateAndNormalize(spec);
            return JsonSerializer.Serialize(result.ValidatedArchitecture, serializerOptions);
        }
        catch (Exception ex)
        {
            return "The given Architecture Specification failed to deserialize due to exception:\n" +
                ex.Message;
        }
    }
}