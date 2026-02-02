using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;

public sealed class ArchitectureNormalizerProcessingService
    : IArchitectureNormalizerProcessingService
{
    public ArchitectureSpec Normalize(ArchitectureSpec spec)
    {
        if (spec == null)
            return spec;

        EnsureLists(spec);
        NormalizePolicies(spec);
        NormalizeComponents(spec);
        NormalizeModels(spec);

        return spec;
    }

    private static void EnsureLists(ArchitectureSpec spec)
    {
        if (spec.Components == null)
            spec.Components = new List<ComponentSpec>();

        if (spec.Models == null)
            spec.Models = new List<ModelSpec>();

        if (spec.ExternalResources == null)
            spec.ExternalResources = new List<ExternalResourceSpec>();

        if (spec.Dependencies == null)
            spec.Dependencies = new List<DependencySpec>();
    }

    private static void NormalizePolicies(ArchitectureSpec spec)
    {
        if (spec.Policies == null)
            return;

        if (spec.Policies.Naming == null)
            return;

        if (string.IsNullOrWhiteSpace(spec.Policies.Naming.RootNamespace))
            spec.Policies.Naming.RootNamespace = spec.Name;

        if (string.IsNullOrWhiteSpace(spec.Policies.Naming.SolutionName))
            spec.Policies.Naming.SolutionName = spec.Name;
    }

    private static void NormalizeComponents(ArchitectureSpec spec)
    {
        var naming = spec.Policies != null
            ? spec.Policies.Naming
            : null;

        foreach (var component in spec.Components)
        {
            if (component == null)
                continue;

            if (component.Methods == null)
                component.Methods = new List<MethodSpec>();

            if (component.Tags == null)
                component.Tags = new Dictionary<string, string>();

            foreach (var method in component.Methods)
            {
                if (method == null)
                    continue;

                if (method.Inputs == null)
                    method.Inputs = new List<ParameterSpec>();

                if (method.Throws == null)
                    method.Throws = new List<TypeRef>();

                if (method.Policies == null)
                    method.Policies = new List<PolicySpec>();

                if (method.Output != null)
                    NormalizeTypeRef(method.Output);

                foreach (var input in method.Inputs)
                {
                    if (input == null)
                        continue;

                    if (input.Type != null)
                        NormalizeTypeRef(input.Type);
                }

                foreach (var @throw in method.Throws)
                {
                    if (@throw == null)
                        continue;

                    NormalizeTypeRef(@throw);
                }
            }

            if (string.IsNullOrWhiteSpace(component.Namespace) && naming != null && !string.IsNullOrWhiteSpace(naming.RootNamespace))
                component.Namespace = naming.RootNamespace + "." + component.Layer.ToString() + "s";

            if (string.IsNullOrWhiteSpace(component.Project) && naming != null && !string.IsNullOrWhiteSpace(naming.SolutionName))
                component.Project = naming.SolutionName + GetProjectSuffix(naming, component);
        }
    }

    private static string GetProjectSuffix(NamingPolicy naming, ComponentSpec component)
    {
        if (component.Layer == ComponentLayer.Exposure)
            return naming.ExposureProjectSuffix;

        if (component.Layer == ComponentLayer.Service)
            return naming.ServicesProjectSuffix;

        if (component.Layer == ComponentLayer.Broker)
            return naming.BrokersProjectSuffix;

        return string.Empty;
    }

    private static void NormalizeModels(ArchitectureSpec spec)
    {
        var naming = spec.Policies != null
            ? spec.Policies.Naming
            : null;

        foreach (var model in spec.Models)
        {
            if (model == null) continue;

            if (model.Properties == null)
                model.Properties = new List<ModelPropertySpec>();

            if (model.Members == null)
                model.Members = new List<EnumMemberSpec>();

            if (model.Tags == null)
                model.Tags = new Dictionary<string, string>();

            foreach (var p in model.Properties)
            {
                if (p == null) continue;
                if (p.Type != null)
                    NormalizeTypeRef(p.Type);
            }

            if (string.IsNullOrWhiteSpace(model.Namespace) && naming != null && !string.IsNullOrWhiteSpace(naming.RootNamespace))
            {
                model.Namespace = naming.RootNamespace + ".Models";
            }

            if (string.IsNullOrWhiteSpace(model.Project) && naming != null && !string.IsNullOrWhiteSpace(naming.SolutionName))
            {
                model.Project = naming.SolutionName + naming.ModelsProjectSuffix;
            }
        }
    }

    private static void NormalizeTypeRef(TypeRef type)
    {
        if (type.GenericArguments == null)
            type.GenericArguments = new List<TypeRef>();

        foreach (var genericArgument in type.GenericArguments)
        {
            if (genericArgument == null)
                continue;

            NormalizeTypeRef(genericArgument);
        }
    }
}
