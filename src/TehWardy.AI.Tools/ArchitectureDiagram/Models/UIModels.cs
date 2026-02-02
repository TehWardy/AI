namespace TehWardy.AI.Tools.ArchitectureDiagram.Models;

public sealed class ComputedDiagramLayout
{
    public IDictionary<Guid, NodeLayout> Nodes { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public sealed class NodeLayout
{
    public double X { get; set; }
    public double Y { get; set; }
    public double W { get; set; }
    public double H { get; set; }
}
