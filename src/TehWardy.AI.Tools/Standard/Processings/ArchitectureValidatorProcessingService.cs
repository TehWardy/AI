using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;

public sealed class ArchitectureValidatorProcessingService : IArchitectureValidatorProcessingService
{
    public ValidationResult Validate(ArchitectureSpec spec)
    {
        var result = new ValidationResult
        {
            Diagnostics = []
        };

        if (spec == null)
        {
            Add(result, "STD000", DiagnosticSeverity.Error, "ArchitectureSpec is null.", "$");
            return result;
        }

        ValidateRoot(spec, result);
        ValidateCollections(spec, result);

        result.IsValid =
            result.Diagnostics != null &&
            result.Diagnostics.All(d => d.Severity != DiagnosticSeverity.Error);

        if (!result.IsValid)
        {
            return result;
        }

        ValidateComponentIntegrity(spec, result);
        ValidateDependencies(spec, result);
        ValidateExposureEndpoints(spec, result);
        ValidateBrokerConstraints(spec, result);

        result.IsValid =
            result.Diagnostics != null &&
            result.Diagnostics.All(d => d.Severity != DiagnosticSeverity.Error);

        return result;
    }

    private static void ValidateRoot(ArchitectureSpec spec, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(spec.Name))
            Add(result, "STD001", DiagnosticSeverity.Error, "ArchitectureSpec.Name is required.", "$.name");

        if (spec.Policies == null)
            Add(result, "STD002", DiagnosticSeverity.Error, "ArchitectureSpec.Policies is required.", "$.policies");
        else
            ValidatePolicies(spec.Policies, result);
    }

    private static void ValidatePolicies(ArchitecturePolicies policies, ValidationResult result)
    {
        if (policies.Naming == null)
            Add(result, "STD010", DiagnosticSeverity.Error, "Policies.Naming is required.", "$.policies.naming");

        if (policies.ErrorHandling == null)
            Add(result, "STD011", DiagnosticSeverity.Error, "Policies.ErrorHandling is required.", "$.policies.errorHandling");

        if (policies.Logging == null)
            Add(result, "STD012", DiagnosticSeverity.Error, "Policies.Logging is required.", "$.policies.logging");

        if (policies.Layering == null)
            Add(result, "STD013", DiagnosticSeverity.Error, "Policies.Layering is required.", "$.policies.layering");
    }

    private static void ValidateCollections(ArchitectureSpec spec, ValidationResult result)
    {
        if (spec.Components == null)
            Add(result, "STD020", DiagnosticSeverity.Error, "ArchitectureSpec.Components is required.", "$.components");

        if (spec.Models == null)
            Add(result, "STD021", DiagnosticSeverity.Warning, "ArchitectureSpec.Models is missing; type validation will be limited.", "$.models");

        if (spec.Dependencies == null)
            Add(result, "STD022", DiagnosticSeverity.Warning, "ArchitectureSpec.Dependencies is missing; layering checks will be limited.", "$.dependencies");
    }

    private static void ValidateComponentIntegrity(ArchitectureSpec spec, ValidationResult result)
    {
        var components = spec.Components ?? [];

        if (components.Count == 0)
            Add(result, "STD030", DiagnosticSeverity.Error, "No components defined.", "$.components");

        var ids = components
            .Select(c => c.Id)
            .ToList();

        var duplicateIds = ids
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var dup in duplicateIds)
        {
            Add(result, "STD031", DiagnosticSeverity.Error, $"Duplicate component id '{dup}'.", "$.components[*].id");
        }

        for (int i = 0; i < components.Count; i++)
        {
            var c = components[i];
            var basePath = $"$.components[{i}]";

            if (c == null)
            {
                Add(result, "STD032", DiagnosticSeverity.Error, "Component is null.", basePath);
                continue;
            }

            if (string.IsNullOrWhiteSpace(c.Id))
                Add(result, "STD033", DiagnosticSeverity.Error, "Component.Id is required.", basePath + ".id");

            if (string.IsNullOrWhiteSpace(c.Name))
                Add(result, "STD034", DiagnosticSeverity.Error, "Component.Name is required.", basePath + ".name");

            if (c.Methods == null)
                Add(result, "STD035", DiagnosticSeverity.Warning, "Component.Methods is null.", basePath + ".methods");

            ValidateMethods(c, basePath, result);
        }
    }

    private static void ValidateMethods(ComponentSpec component, string componentPath, ValidationResult result)
    {
        var methods = component.Methods ?? [];

        var methodNames = methods
            .Select(m => m?.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToList();

        var duplicates = methodNames
            .GroupBy(n => n)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var dup in duplicates)
        {
            Add(result, "STD040", DiagnosticSeverity.Error, $"Duplicate method name '{dup}' on component '{component.Name}'.", componentPath + ".methods[*].name");
        }

        for (int j = 0; j < methods.Count; j++)
        {
            var m = methods[j];
            var mPath = componentPath + $".methods[{j}]";

            if (m == null)
            {
                Add(result, "STD041", DiagnosticSeverity.Error, "Method is null.", mPath);
                continue;
            }

            if (string.IsNullOrWhiteSpace(m.Name))
                Add(result, "STD042", DiagnosticSeverity.Error, "Method.Name is required.", mPath + ".name");

            if (m.Output == null)
                Add(result, "STD043", DiagnosticSeverity.Error, "Method.Output is required.", mPath + ".output");

            if (m.Inputs == null)
                Add(result, "STD044", DiagnosticSeverity.Warning, "Method.Inputs is null.", mPath + ".inputs");

            if (m.Throws == null)
                Add(result, "STD045", DiagnosticSeverity.Warning, "Method.Throws is null.", mPath + ".throws");

            if (m.Policies == null)
                Add(result, "STD046", DiagnosticSeverity.Warning, "Method.Policies is null.", mPath + ".policies");
        }
    }

    private static void ValidateDependencies(ArchitectureSpec spec, ValidationResult result)
    {
        var deps = spec.Dependencies ?? [];
        var components = spec.Components ?? [];

        var map = components
            .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id))
            .ToDictionary(c => c.Id, c => c);

        for (int i = 0; i < deps.Count; i++)
        {
            var d = deps[i];
            var dPath = $"$.dependencies[{i}]";

            if (d == null)
            {
                Add(result, "STD050", DiagnosticSeverity.Error, "Dependency is null.", dPath);
                continue;
            }

            if (string.IsNullOrWhiteSpace(d.FromComponentId))
            {
                Add(result, "STD051", DiagnosticSeverity.Error, "Dependency.FromComponentId is required.", dPath + ".fromComponentId");
                continue;
            }

            if (string.IsNullOrWhiteSpace(d.ToComponentId))
            {
                Add(result, "STD052", DiagnosticSeverity.Error, "Dependency.ToComponentId is required.", dPath + ".toComponentId");
                continue;
            }

            if (!map.ContainsKey(d.FromComponentId))
                Add(result, "STD053", DiagnosticSeverity.Error, $"Dependency.FromComponentId '{d.FromComponentId}' not found.", dPath + ".fromComponentId");

            if (!map.ContainsKey(d.ToComponentId) && !spec.ExternalResources.Any(r => r.Id == d.ToComponentId))
                Add(result, "STD054", DiagnosticSeverity.Error, $"Dependency.ToComponentId '{d.ToComponentId}' not found.", dPath + ".toComponentId");

            if (map.ContainsKey(d.FromComponentId) && map.ContainsKey(d.ToComponentId))
            {
                ValidateLayering(spec, map[d.FromComponentId], map[d.ToComponentId], dPath, result);
                ValidateCallMap(map[d.FromComponentId], map[d.ToComponentId], d, dPath, result);
            }
        }
    }

    private static void ValidateLayering(
        ArchitectureSpec spec,
        ComponentSpec from,
        ComponentSpec to,
        string depPath,
        ValidationResult result)
    {
        var layering = spec.Policies != null ? spec.Policies.Layering : null;

        if (layering == null)
            return;

        if (!layering.EnforceStrictLayering)
            return;

        if (from.Layer == ComponentLayer.Exposure)
        {
            if (to.Layer != ComponentLayer.Service)
                Add(result, "STD060", DiagnosticSeverity.Error, "Exposure may only depend on Service.", depPath);
        }

        if (from.Layer == ComponentLayer.Service)
        {
            if (to.Layer == ComponentLayer.Exposure)
                Add(result, "STD061", DiagnosticSeverity.Error, "Service may not depend on Exposure.", depPath);

            if (to.Layer == ComponentLayer.Broker)
                return;

            if (to.Layer == ComponentLayer.Service && !layering.AllowServiceToServiceDependencies)
                Add(result, "STD062", DiagnosticSeverity.Error, "Service-to-Service dependencies are disabled by policy.", depPath);
        }

        if (from.Layer == ComponentLayer.Broker)
        {
            Add(result, "STD063", DiagnosticSeverity.Error, "Broker may not depend on other internal components.", depPath);
        }

        if (from.Layer == ComponentLayer.Exposure && to.Layer == ComponentLayer.Exposure && !layering.AllowExposureToExposureDependencies)
        {
            Add(result, "STD064", DiagnosticSeverity.Error, "Exposure-to-Exposure dependencies are disabled by policy.", depPath);
        }

        if (from.Layer == ComponentLayer.Broker && to.Layer == ComponentLayer.Broker && !layering.AllowBrokerToBrokerDependencies)
        {
            Add(result, "STD065", DiagnosticSeverity.Error, "Broker-to-Broker dependencies are disabled by policy.", depPath);
        }
    }

    private static void ValidateCallMap(
        ComponentSpec from,
        ComponentSpec to,
        DependencySpec dep,
        string depPath,
        ValidationResult result)
    {
        var callMap = dep.CallMap ?? [];

        if (callMap.Count == 0)
            return;

        var fromMethods = (from.Methods ?? [])
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
            .ToDictionary(m => m.Name, m => m);

        var toMethods = (to.Methods ?? [])
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Name))
            .ToDictionary(m => m.Name, m => m);

        for (int i = 0; i < callMap.Count; i++)
        {
            var cm = callMap[i];
            var cmPath = depPath + $".callMap[{i}]";

            if (cm == null)
            {
                Add(result, "STD070", DiagnosticSeverity.Error, "CallMap entry is null.", cmPath);
                continue;
            }

            if (string.IsNullOrWhiteSpace(cm.FromMethodName))
                Add(result, "STD071", DiagnosticSeverity.Error, "CallMap.FromMethodName is required.", cmPath + ".fromMethodName");
            else if (!fromMethods.ContainsKey(cm.FromMethodName))
                Add(result, "STD072", DiagnosticSeverity.Error, $"From method '{cm.FromMethodName}' not found on '{from.Name}'.", cmPath + ".fromMethodName");

            if (string.IsNullOrWhiteSpace(cm.ToMethodName))
                Add(result, "STD073", DiagnosticSeverity.Error, "CallMap.ToMethodName is required.", cmPath + ".toMethodName");
            else if (!toMethods.ContainsKey(cm.ToMethodName))
                Add(result, "STD074", DiagnosticSeverity.Error, $"To method '{cm.ToMethodName}' not found on '{to.Name}'.", cmPath + ".toMethodName");
        }
    }

    private static void ValidateExposureEndpoints(ArchitectureSpec spec, ValidationResult result)
    {
        var exposures = (spec.Components ?? [])
            .OfType<ExposureSpec>()
            .ToList();

        for (int i = 0; i < exposures.Count; i++)
        {
            var e = exposures[i];
            var ePath = $"$.components[?(@.id=='{e.Id}')]";

            var endpoints = e.Endpoints ?? [];

            foreach (var ep in endpoints)
            {
                if (ep == null)
                {
                    Add(result, "STD080", DiagnosticSeverity.Error, "Exposure endpoint is null.", ePath + ".endpoints[*]");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(ep.MethodName))
                {
                    Add(result, "STD081", DiagnosticSeverity.Error, "ExposureEndpoint.MethodName is required.", ePath + ".endpoints[*].methodName");
                    continue;
                }

                var methodExists = (e.Methods ?? new List<MethodSpec>())
                    .Any(m => m != null && string.Equals(m.Name, ep.MethodName, StringComparison.Ordinal));

                if (!methodExists)
                    Add(result, "STD082", DiagnosticSeverity.Error, $"Endpoint references method '{ep.MethodName}' which does not exist on exposure '{e.Name}'.", ePath + ".endpoints[*].methodName");

                if (string.IsNullOrWhiteSpace(ep.Route))
                    Add(result, "STD083", DiagnosticSeverity.Error, "ExposureEndpoint.Route is required.", ePath + ".endpoints[*].route");
            }
        }
    }

    private static void ValidateBrokerConstraints(ArchitectureSpec spec, ValidationResult result)
    {
        var brokers = (spec.Components ?? [])
            .OfType<BrokerSpec>()
            .ToList();

        var policies = spec.Policies;
        var brokerThrowsAllowed = policies != null && policies.ErrorHandling != null && policies.ErrorHandling.BrokersDeclareThrows;

        for (int i = 0; i < brokers.Count; i++)
        {
            var b = brokers[i];
            var bPath = $"$.components[?(@.id=='{b.Id}')]";

            if (string.IsNullOrWhiteSpace(b.ExternalResourceId))
                Add(result, "STD090", DiagnosticSeverity.Warning, $"Broker '{b.Name}' has no ExternalResourceId.", bPath + ".externalResourceId");

            var methods = b.Methods ?? [];

            for (int j = 0; j < methods.Count; j++)
            {
                var m = methods[j];
                if (m == null) continue;

                var mPath = bPath + $".methods[{j}]";

                if (!brokerThrowsAllowed && m.Throws != null && m.Throws.Count > 0)
                    Add(result, "STD091", DiagnosticSeverity.Error, $"Broker method '{m.Name}' must not declare Throws.", mPath + ".throws");

                if (m.Policies != null && m.Policies.Count > 0)
                    Add(result, "STD092", DiagnosticSeverity.Warning, $"Broker method '{m.Name}' has Policies; brokers should be thin.", mPath + ".policies");
            }
        }
    }

    private static void Add(
        ValidationResult result,
        string code,
        DiagnosticSeverity severity,
        string message,
        string jsonPath)
    {
        result.Diagnostics.Add(new Diagnostic
        {
            Code = code,
            Severity = severity,
            Message = message,
            JsonPath = jsonPath
        });
    }
}