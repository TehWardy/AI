using System.Text.Json.Serialization;

namespace TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

public sealed class DiagramSpecification
{
    public string Name { get; set; }

    public IList<DiagramNode> Nodes { get; set; }

    public IList<DiagramEdge> Edges { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiagramNodeKind
{
    Component,
    Model,
    External
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiagramComponentRole
{
    Exposure,
    Orchestration,
    Processing,
    Service,
    Broker,
    External
}

public sealed class DiagramNode
{
    public DiagramNodeKind Kind { get; set; }
    public string Name { get; set; }
    public DiagramComponentRole? Role { get; set; }
    public IList<DiagramMethod> Methods { get; set; }
    public IList<DiagramProperty> Properties { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiagramAsyncKind
{
    Sync,
    Task,
    ValueTask
}

public sealed class DiagramMethod
{
    public string Name { get; set; }
    public DiagramAsyncKind Async { get; set; }
    public string OutputType { get; set; }
    public IList<DiagramParameter> Inputs { get; set; }
}

public sealed class DiagramParameter
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Optional { get; set; }
}

public sealed class DiagramProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Required { get; set; }
}

public sealed class DiagramEdge
{
    public string FromNodeName { get; set; }
    public string ToNodeName { get; set; }
}