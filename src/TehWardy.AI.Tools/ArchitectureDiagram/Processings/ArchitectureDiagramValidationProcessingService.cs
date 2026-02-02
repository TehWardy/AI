using TehWardy.AI.Tools.ArchitectureDiagram.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Processings;

public sealed class ArchitectureDiagramValidationProcessingService
    : IArchitectureDiagramValidationProcessingService
{
    public DiagramValidationResult Validate(DiagramSpecification diagram)
    {
        var result = new DiagramValidationResult();

        if (diagram == null)
        {
            Add(result, "DIA000", DiagramDiagnosticSeverity.Error,
                "Diagram is null.");
            return result;
        }

        if (diagram.Nodes == null || diagram.Nodes.Count == 0)
        {
            Add(result, "DIA001", DiagramDiagnosticSeverity.Error,
                "Diagram contains no nodes.");
            return result;
        }

        var nodes = diagram.Nodes;
        var edges = diagram.Edges ?? new List<DiagramEdge>();

        var nodeMap = nodes.ToDictionary(n => n.Id);

        var outgoing = edges.GroupBy(e => e.FromNodeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var incoming = edges.GroupBy(e => e.ToNodeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        ValidateNoCycles(nodes, outgoing, result);
        ValidateNodeKinds(nodes, outgoing, incoming, result);
        ValidateRoles(nodes, outgoing, incoming, result);

        return result;
    }

    private static void ValidateNoCycles(
        IList<DiagramNode> nodes,
        IDictionary<Guid, List<DiagramEdge>> outgoing,
        DiagramValidationResult result)
    {
        var visited = new HashSet<Guid>();
        var stack = new HashSet<Guid>();

        foreach (var node in nodes)
        {
            if (!visited.Contains(node.Id))
            {
                if (HasCycle(node.Id, outgoing, visited, stack))
                {
                    Add(result, "DIA020", DiagramDiagnosticSeverity.Error,
                        "Diagram contains a cyclic dependency.");
                    return;
                }
            }
        }
    }

    private static bool HasCycle(
        Guid nodeId,
        IDictionary<Guid, List<DiagramEdge>> outgoing,
        HashSet<Guid> visited,
        HashSet<Guid> stack)
    {
        visited.Add(nodeId);
        stack.Add(nodeId);

        if (outgoing.TryGetValue(nodeId, out var edges))
        {
            foreach (var edge in edges)
            {
                var next = edge.ToNodeId;

                if (!visited.Contains(next) &&
                    HasCycle(next, outgoing, visited, stack))
                    return true;

                if (stack.Contains(next))
                    return true;
            }
        }

        stack.Remove(nodeId);
        return false;
    }

    private static void ValidateNodeKinds(
        IList<DiagramNode> nodes,
        IDictionary<Guid, List<DiagramEdge>> outgoing,
        IDictionary<Guid, List<DiagramEdge>> incoming,
        DiagramValidationResult result)
    {
        foreach (var node in nodes)
        {
            outgoing.TryGetValue(node.Id, out var outEdges);
            incoming.TryGetValue(node.Id, out var inEdges);

            outEdges ??= new List<DiagramEdge>();
            inEdges ??= new List<DiagramEdge>();

            if (node.Kind == DiagramNodeKind.Model)
            {
                if (outEdges.Count > 0 || inEdges.Count > 0)
                {
                    Add(result, "DIA030", DiagramDiagnosticSeverity.Error,
                        "Model nodes must not have dependencies.",
                        nodeId: node.Id);
                }
            }

            if (node.Kind == DiagramNodeKind.External)
            {
                if (outEdges.Count > 0)
                {
                    Add(result, "DIA031", DiagramDiagnosticSeverity.Error,
                        "External nodes must not have outgoing dependencies.",
                        nodeId: node.Id);
                }
            }
        }
    }

    private static void ValidateRoles(
        IList<DiagramNode> nodes,
        IDictionary<Guid, List<DiagramEdge>> outgoing,
        IDictionary<Guid, List<DiagramEdge>> incoming,
        DiagramValidationResult result)
    {
        foreach (var node in nodes.Where(n => n.Kind == DiagramNodeKind.Component))
        {
            outgoing.TryGetValue(node.Id, out var outEdges);
            incoming.TryGetValue(node.Id, out var inEdges);

            outEdges ??= new List<DiagramEdge>();
            inEdges ??= new List<DiagramEdge>();

            switch (node.Role)
            {
                case DiagramComponentRole.Exposure:
                    ValidateOutgoing(node, outEdges, 1, result);
                    break;

                case DiagramComponentRole.Orchestration:
                    if (outEdges.Count == 0 || outEdges.Count > 3)
                    {
                        Add(result, "DIA040", DiagramDiagnosticSeverity.Error,
                            "Orchestration component must have 1 to 3 outgoing dependencies.",
                            nodeId: node.Id);
                    }
                    break;

                case DiagramComponentRole.Processing:
                case DiagramComponentRole.Service:
                    ValidateOutgoing(node, outEdges, 1, result);
                    break;

                case DiagramComponentRole.Broker:
                    ValidateOutgoing(node, outEdges, 1, result);

                    var targetIsExternal = outEdges.All(e =>
                    {
                        var target = nodes.First(n => n.Id == e.ToNodeId);
                        return target.Kind == DiagramNodeKind.External;
                    });

                    if (!targetIsExternal)
                    {
                        Add(result, "DIA041", DiagramDiagnosticSeverity.Error,
                            "Broker must only depend on External nodes.",
                            nodeId: node.Id);
                    }
                    break;
            }
        }
    }

    private static void ValidateOutgoing(
        DiagramNode node,
        IList<DiagramEdge> edges,
        int expectedCount,
        DiagramValidationResult result)
    {
        if (edges.Count != expectedCount)
        {
            Add(result, "DIA042", DiagramDiagnosticSeverity.Error,
                $"Component '{node.Name}' must have exactly {expectedCount} outgoing dependency.",
                nodeId: node.Id);
        }
    }

    private static void Add(
        DiagramValidationResult result,
        string code,
        DiagramDiagnosticSeverity severity,
        string message,
        Guid? nodeId = null,
        Guid? edgeId = null)
    {
        result.Diagnostics.Add(new DiagramDiagnostic
        {
            Code = code,
            Severity = severity,
            Message = message,
            NodeId = nodeId,
            EdgeId = edgeId
        });
    }
}