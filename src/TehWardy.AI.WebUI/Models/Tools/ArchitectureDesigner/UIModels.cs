namespace TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

public sealed class ComputedDiagramLayout
{
    public IDictionary<string, NodeLayout> Nodes { get; set; }
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
