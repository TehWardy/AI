using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Components.Tools.ArchitectureDesigner;

public partial class DesignSurface : ComponentBase
{
    [Parameter, EditorRequired] 
    public DiagramSpecification Diagram { get; set; }

    [Parameter, EditorRequired] 
    public ComputedDiagramLayout ComputedLayout { get; set; }

    // ---- Rendering constants (keep in sync with layout service) ----
    public const double HeaderH = 36;
    public const double Padding = 10;
    public const double MethodGap = 12;
    public const double LineH = 18;

    // Method box internal layout
    public const double MethodTopPad = 10;
    public const double MethodBottomPad = 10;
    public const double MethodLabelBlockH = 20; 

    // ---- Pan/Zoom state ----
    protected double Zoom { get; set; } = 1.0;
    protected double PanX { get; set; } = 0.0;
    protected double PanY { get; set; } = 0.0;

    private bool panning;
    private double lastClientX;
    private double lastClientY;

    // ---- Anchors ----
    protected enum AnchorSide { Left, Right }

    protected readonly struct Anchor
    {
        public Anchor(double x, double y) { X = x; Y = y; }
        public double X { get; }
        public double Y { get; }
    }

    private bool dirty;

    protected override bool ShouldRender() => dirty;

    protected override void OnAfterRender(bool firstRender) =>
        dirty = false;

    protected override void OnParametersSet() =>
        dirty = true;

    protected void OnWheel(WheelEventArgs e)
    {
        var factor = e.DeltaY < 0 ? 1.1 : 0.9;
        var newZoom = Math.Clamp(Zoom * factor, 0.10, 9.0);

        if (Math.Abs(newZoom - Zoom) > 0.0001)
        {
            Zoom = newZoom;
            dirty = true;
        }
    }

    protected void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button != 0) 
            return;

        panning = true;
        lastClientX = e.ClientX;
        lastClientY = e.ClientY;
    }

    protected void OnMouseMove(MouseEventArgs e)
    {
        if (!panning)
            return;

        var dx = e.ClientX - lastClientX;
        var dy = e.ClientY - lastClientY;

        if (dx == 0 && dy == 0)
            return;

        // 1) keep pan consistent across zoom levels
        var zoom = Math.Max(Zoom, 0.0001);

        // 2) optional size-based speedup (bounded)
        // baseline tuned around 2000x1200 "comfortable" diagrams
        var area = ComputedLayout.Width * ComputedLayout.Height;
        var baselineArea = 2000.0 * 1200.0;

        // sqrt so it grows gently with area
        var sizeBoost = Math.Sqrt(area / baselineArea);

        // clamp so it doesn't get crazy
        sizeBoost = Math.Clamp(sizeBoost, 1.0, 3.0);

        var speed = sizeBoost; // or let user configure

        PanX += (dx / zoom) * speed;
        PanY += (dy / zoom) * speed;

        lastClientX = e.ClientX;
        lastClientY = e.ClientY;

        dirty = true;
    }

    protected void OnMouseUp(MouseEventArgs e)
    {
        panning = false;
    }

    // ---- Edge path ----
    protected static string BuildElbowPath(double x1, double y1, double x2, double y2)
    {
        var mx = (x1 + x2) / 2.0;
        return $"M {x1} {y1} L {mx} {y1} L {mx} {y2} L {x2} {y2}";
    }

    protected static string AnchorKey(string nodeName, string methodName, AnchorSide side) => 
        $"{nodeName}:{methodName}:{(side == AnchorSide.Left ? "L" : "R")}";

    protected Dictionary<string, Anchor> BuildAnchors(Dictionary<string, DiagramNode> nodeMap)
    {
        var anchors = new Dictionary<string, Anchor>(StringComparer.Ordinal);

        foreach (var (nodeName, node) in nodeMap)
        {
            if (!ComputedLayout.Nodes.TryGetValue(nodeName, out var rect))
                continue;

            // If no methods, we won’t create method anchors, only center fallback
            var methods = node.Methods ?? Array.Empty<DiagramMethod>();
            if (methods.Count == 0)
                continue;

            double yCursor = rect.Y + HeaderH + Padding;

            foreach (var m in methods)
            {
                var inputCount = m.Inputs?.Count ?? 0;
                var methodH = ComputeMethodBoxHeight(inputCount);

                var centerY = yCursor + methodH / 2.0;

                anchors[AnchorKey(nodeName, m.Name, AnchorSide.Left)] = new Anchor(rect.X, centerY);
                anchors[AnchorKey(nodeName, m.Name, AnchorSide.Right)] = new Anchor(rect.X + rect.W, centerY);

                yCursor += methodH + MethodGap;
            }
        }

        return anchors;
    }

    public static double ComputeMethodBoxHeight(int inputCount)
    {
        // Lines: Input(s) + Name + Output
        var lines = inputCount + 2;

        return MethodTopPad
             + MethodLabelBlockH
             + (lines * LineH)
             + MethodBottomPad;
    }

    // ---- Method resolution (edge -> method) ----
    protected static string ResolveFromMethodName(DiagramNode fromNode, DiagramEdge edge)
    {
        if (!string.IsNullOrWhiteSpace(edge.FromMethodName))
            return edge.FromMethodName!;

        // fallback to first method
        return fromNode.Methods?.FirstOrDefault()?.Name ?? "Method";
    }

    protected static string ResolveToMethodName(DiagramNode toNode, DiagramEdge edge)
    {
        if (!string.IsNullOrWhiteSpace(edge.ToMethodName))
            return edge.ToMethodName!;

        return toNode.Methods?.FirstOrDefault()?.Name ?? "Method";
    }
}