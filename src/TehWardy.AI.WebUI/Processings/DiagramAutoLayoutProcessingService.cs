using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public sealed class DiagramAutoLayoutProcessingService : IDiagramAutoLayoutProcessingService
{
    // Tweakable constants (UI-only)
    const double ColumnWidth = 360;
    const double RowHeight = 260;
    const double NodeWidth = 320;
    const double NodeHeight = 220;

    const double MarginX = 40;
    const double MarginY = 40;

    public ComputedDiagramLayout Compute(DiagramSpecification diagram)
    {
        if (diagram == null) throw new ArgumentNullException(nameof(diagram));
        if (diagram.Nodes == null) throw new InvalidOperationException("Diagram.Nodes is null.");

        var nodes = diagram.Nodes;
        var edges = diagram.Edges ?? new List<DiagramEdge>();

        var nodeMap = nodes.ToDictionary(n => n.Id);

        var outgoing = edges
            .GroupBy(e => e.FromNodeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Treat "Root" role as an Exposure column entry point.
        var roots = nodes
            .Where(n => n.Kind == DiagramNodeKind.Component && n.Role == DiagramComponentRole.Exposure)
            .OrderBy(n => n.Name)
            .ToList();

        // If there are no roots yet, just lay nodes out in a simple grid
        if (roots.Count == 0)
        {
            return FallbackGrid(nodes);
        }

        var layouts = new Dictionary<Guid, NodeLayout>();
        var occupied = new HashSet<Guid>();

        int lane = 0;

        foreach (var root in roots)
        {
            // Walk each root chain (for now assume linear; if multiple outgoing, we create more lanes)
            LayoutFrom(root.Id, depth: 0, laneRef: ref lane, fixedLane: lane, outgoing, nodeMap, layouts, occupied);
            lane++;
        }

        // Lay out any models/external nodes not reached (keep simple: place below in extra lanes)
        foreach (var node in nodes)
        {
            if (occupied.Contains(node.Id)) continue;

            var col = GetColumn(node);
            layouts[node.Id] = MakeNodeLayout(col, lane);
            lane++;
        }

        var width = MarginX * 2 + (6 * ColumnWidth);
        var height = MarginY * 2 + Math.Max(1, lane) * RowHeight;

        return new ComputedDiagramLayout
        {
            Nodes = layouts,
            Width = width,
            Height = height
        };
    }

    private static void LayoutFrom(
        Guid nodeId,
        int depth,
        ref int laneRef,
        int fixedLane,
        IDictionary<Guid, List<DiagramEdge>> outgoing,
        IDictionary<Guid, DiagramNode> nodeMap,
        IDictionary<Guid, NodeLayout> layouts,
        HashSet<Guid> occupied)
    {
        if (!nodeMap.TryGetValue(nodeId, out var node))
            return;

        if (!occupied.Contains(nodeId))
        {
            var col = GetColumn(node);
            layouts[nodeId] = MakeNodeLayout(col, fixedLane);
            occupied.Add(nodeId);
        }

        if (!outgoing.TryGetValue(nodeId, out var outs) || outs.Count == 0)
            return;

        // For now: prefer linear chains; if branching occurs, each branch gets its own lane.
        if (outs.Count == 1)
        {
            LayoutFrom(outs[0].ToNodeId, depth + 1, ref laneRef, fixedLane, outgoing, nodeMap, layouts, occupied);
            return;
        }

        // Branching: keep first in current lane, others in new lanes
        LayoutFrom(outs[0].ToNodeId, depth + 1, ref laneRef, fixedLane, outgoing, nodeMap, layouts, occupied);

        for (int i = 1; i < outs.Count; i++)
        {
            laneRef++;
            LayoutFrom(outs[i].ToNodeId, depth + 1, ref laneRef, laneRef, outgoing, nodeMap, layouts, occupied);
        }
    }

    private static ComputedDiagramLayout FallbackGrid(IList<DiagramNode> nodes)
    {
        var layouts = new Dictionary<Guid, NodeLayout>();
        int i = 0;

        foreach (var node in nodes.OrderBy(n => n.Kind).ThenBy(n => n.Name))
        {
            var col = i % 3;
            var row = i / 3;
            layouts[node.Id] = new NodeLayout
            {
                X = MarginX + col * ColumnWidth,
                Y = MarginY + row * RowHeight,
                W = NodeWidth,
                H = NodeHeight
            };
            i++;
        }

        return new ComputedDiagramLayout
        {
            Nodes = layouts,
            Width = MarginX * 2 + 3 * ColumnWidth,
            Height = MarginY * 2 + Math.Max(1, (i + 2) / 3) * RowHeight
        };
    }

    private static int GetColumn(DiagramNode node)
    {
        // Fixed columns:
        // 0 Exposure(Root), 1 Orchestration, 2 Processing, 3 Service, 4 Broker, 5 External
        if (node.Kind == DiagramNodeKind.External) return 5;
        if (node.Kind == DiagramNodeKind.Model) return 3; // keep models near services for now

        if (node.Kind == DiagramNodeKind.Component && node.Role.HasValue)
        {
            return node.Role.Value switch
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

    private static NodeLayout MakeNodeLayout(int col, int lane)
    {
        return new NodeLayout
        {
            X = MarginX + col * ColumnWidth,
            Y = MarginY + lane * RowHeight,
            W = NodeWidth,
            H = NodeHeight
        };
    }
}
