using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public sealed class DiagramAutoLayoutProcessingService : IDiagramAutoLayoutProcessingService
{
    // Columns remain fixed
    const double ColumnWidth = 400;
    const double NodeWidth = 320;

    const double MarginX = 40;
    const double MarginY = 40;

    // Vertical spacing between lanes
    const double LaneGap = 40;

    // ---- Rendering constants (keep in sync with layout service) ----
    public const double HeaderH = 36;
    public const double Padding = 10;
    public const double MethodGap = 12;
    public const double LineH = 18;

    // Method box internal layout
    public const double MethodTopPad = 10;
    public const double MethodBottomPad = 10;
    public const double MethodLabelBlockH = 20;

    public ComputedDiagramLayout Compute(DiagramSpecification diagram)
    {
        if (diagram == null) 
            throw new ArgumentNullException(nameof(diagram));

        if (diagram.Nodes == null) 
            throw new InvalidOperationException("Diagram.Nodes is null.");

        var nodes = diagram.Nodes;
        var edges = diagram.Edges ?? new List<DiagramEdge>();

        var nodeMap = nodes.ToDictionary(n => n.Name, StringComparer.Ordinal);

        var outgoing = edges
            .GroupBy(e => e.FromNodeName)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var roots = nodes
            .Where(n => n.Kind == DiagramNodeKind.Component && n.Role == DiagramComponentRole.Exposure)
            .OrderBy(n => n.Name)
            .ToList();

        if (roots.Count == 0)
            return FallbackGrid(nodes);

        // Instead of storing NodeLayout directly, we store lane assignment first
        var laneByNode = new Dictionary<string, int>(StringComparer.Ordinal);
        var colByNode = new Dictionary<string, int>(StringComparer.Ordinal);
        var occupied = new HashSet<string>(StringComparer.Ordinal);

        int lane = 0;

        foreach (var root in roots)
        {
            LayoutFrom(root.Name, depth: 0, laneRef: ref lane, fixedLane: lane, outgoing, nodeMap, colByNode, laneByNode, occupied);
            lane++;
        }

        // Place any unoccupied nodes into new lanes
        foreach (var node in nodes)
        {
            if (occupied.Contains(node.Name)) 
                continue;

            colByNode[node.Name] = GetColumn(node);
            laneByNode[node.Name] = lane;
            lane++;
        }

        // Compute per-node required heights
        var nodeHeights = new Dictionary<string, double>(StringComparer.Ordinal);
        
        foreach (var node in nodes)
            nodeHeights[node.Name] = ComputeNodeHeight(node);

        // Compute lane heights (max node height in each lane)
        var laneCount = Math.Max(1, laneByNode.Values.DefaultIfEmpty(0).Max() + 1);
        var laneHeights = new double[laneCount];

        foreach (var kvp in laneByNode)
        {
            var nodeName = kvp.Key;
            var laneIndex = kvp.Value;
            laneHeights[laneIndex] = Math.Max(laneHeights[laneIndex], nodeHeights[nodeName]);
        }

        // Compute lane Y offsets (cumulative)
        var laneY = new double[laneCount];
        double y = MarginY;
        for (int i = 0; i < laneCount; i++)
        {
            laneY[i] = y;
            y += laneHeights[i] + LaneGap;
        }

        // Build final layouts
        var layouts = new Dictionary<string, NodeLayout>(StringComparer.Ordinal);

        foreach (var node in nodes)
        {
            var col = colByNode[node.Name];
            var laneIndex = laneByNode[node.Name];

            layouts[node.Name] = new NodeLayout
            {
                X = MarginX + col * ColumnWidth,
                Y = laneY[laneIndex],
                W = NodeWidth,
                H = nodeHeights[node.Name]
            };
        }

        // Compute canvas size
        var width = MarginX * 2 + (6 * ColumnWidth);
        var height = y; // already includes bottom gap; okay

        return new ComputedDiagramLayout
        {
            Nodes = layouts,
            Width = width,
            Height = height
        };
    }

    private static void LayoutFrom(
        string nodeName,
        int depth,
        ref int laneRef,
        int fixedLane,
        IDictionary<string, List<DiagramEdge>> outgoing,
        IDictionary<string, DiagramNode> nodeMap,
        IDictionary<string, int> colByNode,
        IDictionary<string, int> laneByNode,
        HashSet<string> occupied)
    {
        if (!nodeMap.TryGetValue(nodeName, out var node))
            return;

        if (!occupied.Contains(nodeName))
        {
            colByNode[nodeName] = GetColumn(node);
            laneByNode[nodeName] = fixedLane;
            occupied.Add(nodeName);
        }

        if (!outgoing.TryGetValue(nodeName, out var outs) || outs.Count == 0)
            return;

        if (outs.Count == 1)
        {
            LayoutFrom(outs[0].ToNodeName, depth + 1, ref laneRef, fixedLane, outgoing, nodeMap, colByNode, laneByNode, occupied);
            return;
        }

        LayoutFrom(outs[0].ToNodeName, depth + 1, ref laneRef, fixedLane, outgoing, nodeMap, colByNode, laneByNode, occupied);

        for (int i = 1; i < outs.Count; i++)
        {
            laneRef++;
            LayoutFrom(outs[i].ToNodeName, depth + 1, ref laneRef, laneRef, outgoing, nodeMap, colByNode, laneByNode, occupied);
        }
    }

    private static double ComputeNodeHeight(DiagramNode node)
    {
        // 1) Models: render as header + properties box (even if placed in "external" column)
        if (node.Role == DiagramComponentRole.Model)
        {
            var propCount = node.Properties?.Count ?? 0;
            var propsH = ComputePropertiesBoxHeight(propCount);

            return HeaderH + Padding + propsH + Padding;
        }

        // 2) True externals: cloud
        // (Do NOT treat Kind==External as cloud here if you want "external column models" to be boxes;
        //  those are already handled by Role==Model above.)
        if (node.Role == DiagramComponentRole.External)
            return 110;

        // 3) Components: header + method boxes (or empty body)
        var methods = node.Methods ?? Array.Empty<DiagramMethod>();

        if (methods.Count == 0)
            return HeaderH + Padding * 2 + 40;

        double methodsHeight = 0;

        foreach (var m in methods)
        {
            var inputCount = m.Inputs?.Count ?? 0;
            methodsHeight += ComputeMethodBoxHeight(inputCount);
        }

        methodsHeight += (methods.Count - 1) * MethodGap;

        return HeaderH + Padding + methodsHeight + Padding;
    }

    static double ComputeMethodBoxHeight(int inputCount)
    {
        // Lines: Input(s) + Name + Output
        var lines = inputCount + 2;

        return MethodTopPad
             + MethodLabelBlockH
             + (lines * LineH)
             + MethodBottomPad;
    }

    static double ComputePropertiesBoxHeight(int propCount)
    {
        // label + top pad + lines + bottom pad
        var min = 52.0;
        var h = MethodTopPad + (propCount * LineH) + MethodBottomPad;
        return Math.Max(min, h);
    }


    private static ComputedDiagramLayout FallbackGrid(IList<DiagramNode> nodes)
    {
        // Keep it simple: 3 columns grid, but use dynamic heights and per-row max heights.
        // Minimal change: just stack by index with a constant gap.
        const double GridGapY = 40;
        const int Cols = 3;

        var layouts = new Dictionary<string, NodeLayout>(StringComparer.Ordinal);

        double[] colHeights = new double[Cols];
        for (int i = 0; i < Cols; i++) colHeights[i] = MarginY;

        foreach (var node in nodes.OrderBy(n => n.Kind).ThenBy(n => n.Name))
        {
            var col = Array.IndexOf(colHeights, colHeights.Min());
            var h = ComputeNodeHeight(node);

            layouts[node.Name] = new NodeLayout
            {
                X = MarginX + col * ColumnWidth,
                Y = colHeights[col],
                W = NodeWidth,
                H = h
            };

            colHeights[col] += h + GridGapY;
        }

        var width = MarginX * 2 + Cols * ColumnWidth;
        var height = colHeights.Max() + MarginY;

        return new ComputedDiagramLayout
        {
            Nodes = layouts,
            Width = width,
            Height = height
        };
    }

    private static int GetColumn(DiagramNode node)
    {
        if (node.Kind == DiagramNodeKind.External) return 5;
        if (node.Kind == DiagramNodeKind.Model) return 3;

        if (node.Kind == DiagramNodeKind.Component)
        {
            return node.Role switch
            {
                DiagramComponentRole.Exposure => 0,
                DiagramComponentRole.Orchestration => 1,
                DiagramComponentRole.Processing => 2,
                DiagramComponentRole.Service => 3,
                DiagramComponentRole.Broker => 4,
                _ => 3
            };
        }

        return 3;
    }
}
