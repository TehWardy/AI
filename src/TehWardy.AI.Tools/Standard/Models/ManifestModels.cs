namespace TehWardy.AI.Tools.Standard.Models;

public sealed class ConformanceManifest
{
    public string ArchitectureName { get; set; }
    public string ArchitectureVersion { get; set; }

    public IList<ProjectManifest> Projects { get; set; }
    public IList<ComponentManifest> Components { get; set; }
    public IList<DependencySpec> AllowedDependencies { get; set; }
}

public sealed class ProjectManifest
{
    public string Name { get; set; }
    public string TargetFramework { get; set; }
    public IList<string> ProjectReferences { get; set; }
}

public sealed class ComponentManifest
{
    public string ComponentId { get; set; }
    public string Layer { get; set; }
    public string Project { get; set; }
    public string Namespace { get; set; }
    public string ClassName { get; set; }
    public string InterfaceName { get; set; }
    public IList<MethodManifest> Methods { get; set; }
}

public sealed class MethodManifest
{
    public string Name { get; set; }
    public AsyncKind Async { get; set; }
    public TypeRef Output { get; set; }
    public IList<ParameterSpec> Inputs { get; set; }
}