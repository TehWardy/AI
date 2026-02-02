using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Components.Tools.ArchitectureDesigner;

public partial class DesignSurface : ComponentBase
{
    [Parameter, EditorRequired]
    public DiagramSpecification Diagram { get; set; }

    [Parameter, EditorRequired]
    public ComputedDiagramLayout ComputedLayout { get; set; }

    private static string BuildElbowPath(NodeLayout from, NodeLayout to)
    {
        var x1 = from.X + from.W;
        var y1 = from.Y + from.H / 2.0;

        var x2 = to.X;
        var y2 = to.Y + to.H / 2.0;

        var mx = (x1 + x2) / 2.0;

        return $"M {x1} {y1} L {mx} {y1} L {mx} {y2} L {x2} {y2}";
    }

    private static string GetNodeCss(DiagramNode node)
    {
        if (node.Kind == DiagramNodeKind.External)
            return "node node-external";

        if (node.Kind == DiagramNodeKind.Model)
            return "node node-model";

        return node.Role switch
        {
            DiagramComponentRole.Exposure => "node node-root",
            DiagramComponentRole.Orchestration => "node node-orchestration",
            DiagramComponentRole.Processing => "node node-processing",
            DiagramComponentRole.Service => "node node-service",
            DiagramComponentRole.Broker => "node node-broker",
            _ => "node"
        };
    }

    private static string GetEdgeCss(DiagramEdge edge)
        => edge.Kind == DiagramDependencyKind.ExternalBoundary
            ? "edge edge-external"
            : "edge edge-inprocess";
}