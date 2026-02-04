using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Components.Tools.ArchitectureDesigner;

public partial class DiagramNodeComponent : ComponentBase
{
    [Parameter]
    public DiagramNode Node { get; set; }

    [Parameter]
    public ComputedDiagramLayout ComputedLayout { get; set; }

    [Parameter]
    public double HeaderH { get; set; }

    [Parameter]
    public double Padding { get; set; }

    [Parameter]
    public double MethodGap { get; set; }

    [Parameter]
    public double LineH { get; set; }

    [Parameter]
    public double MethodTopPad { get; set; }

    [Parameter]
    public double MethodBottomPad { get; set; }

    [Parameter]
    public double MethodLabelBlockH { get; set; }

    // ---- Cloud ----
    protected static string BuildCloudPath(double w, double h)
    {
        // Draw.io-like: 4 top lobes, rounded sides, flatter bottom.
        // Tuned to look good around ~300x110 but scales reasonably.

        double left = w * 0.08;
        double right = w * 0.92;
        double top = h * 0.20;
        double mid = h * 0.45;
        double bot = h * 0.72;

        double a = w * 0.18;
        double b = w * 0.34;
        double c = w * 0.52;
        double d = w * 0.70;

        return
            $"M {a} {bot} " +
            // left bump
            $"C {left} {bot}, {left} {mid}, {a} {mid} " +
            // top-left lobe
            $"C {a} {top}, {b} {top}, {b} {mid} " +
            // top-middle lobe
            $"C {b} {top * 0.95}, {c} {top * 0.95}, {c} {mid} " +
            // top-right lobe
            $"C {c} {top}, {d} {top * 1.05}, {d} {mid} " +
            // right bump down
            $"C {right} {mid}, {right} {bot}, {d} {bot} " +
            // bottom sweep back to start
            $"C {c} {bot * 1.02}, {b} {bot * 1.02}, {a} {bot} Z";
    }

    protected static string GetNodeCss(DiagramNode node) => node.Role switch
    {
        DiagramComponentRole.Exposure => "node node-root",
        DiagramComponentRole.Orchestration => "node node-orchestration",
        DiagramComponentRole.Processing => "node node-processing",
        DiagramComponentRole.Service => "node node-service",
        DiagramComponentRole.Broker => "node node-broker",
        DiagramComponentRole.Model => "node node-model",
        DiagramComponentRole.External => "node node-external",
        _ => "node"
    };
}