using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.Standard.Processings;

public sealed class ManifestGenerationProcessingService
    : IManifestGenerationProcessingService
{
    public ConformanceManifest Generate(ArchitectureSpec spec)
    {
        var manifest = new ConformanceManifest
        {
            ArchitectureName = spec.Name,
            ArchitectureVersion = spec.Version,
            Projects = [],
            Components = [],
            AllowedDependencies = spec.Dependencies ?? []
        };

        var components = spec.Components ?? [];

        foreach (var c in components)
        {
            if (c == null) continue;

            var cm = new ComponentManifest
            {
                ComponentId = c.Id,
                Layer = c.Layer.ToString(),
                Project = c.Project,
                Namespace = c.Namespace,
                ClassName = c.Name,
                InterfaceName = BuildInterfaceName(spec, c),
                Methods = []
            };

            var methods = c.Methods ?? [];

            foreach (var m in methods)
            {
                if (m == null) continue;

                cm.Methods.Add(new MethodManifest
                {
                    Name = m.Name,
                    Async = m.Async,
                    Output = m.Output,
                    Inputs = m.Inputs ?? []
                });
            }

            manifest.Components.Add(cm);
        }

        manifest.Projects = BuildProjectManifests(components);
        return manifest;
    }

    private static IList<ProjectManifest> BuildProjectManifests(IList<ComponentSpec> components)
    {
        var projects = components
            .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Project))
            .Select(c => c.Project)
            .Distinct()
            .Select(p => new ProjectManifest
            {
                Name = p,
                TargetFramework = "",
                ProjectReferences = []
            })
            .ToList();

        return projects;
    }

    private static string BuildInterfaceName(ArchitectureSpec spec, ComponentSpec component)
    {
        var naming = spec.Policies != null
            ? spec.Policies.Naming
            : null;

        var prefix = naming != null
            ? naming.InterfacePrefix
            : "I";

        return prefix + component.Name;
    }
}
