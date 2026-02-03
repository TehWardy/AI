using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

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

        var nodeMap = nodes.ToDictionary(n => n.Name);

        var outgoing = edges.GroupBy(e => e.FromNodeName)
            .ToDictionary(g => g.Key, g => g.ToList());

        var incoming = edges.GroupBy(e => e.ToNodeName)
            .ToDictionary(g => g.Key, g => g.ToList());

        ValidateNoCycles(nodes, outgoing, result);
        ValidateNodeKinds(nodes, outgoing, incoming, result);
        ValidateRoles(nodes, outgoing, incoming, result);

        return result;
    }

    private static void ValidateNoCycles(
        IList<DiagramNode> nodes,
        IDictionary<string, List<DiagramEdge>> outgoing,
        DiagramValidationResult result)
    {
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        foreach (var node in nodes)
        {
            if (!visited.Contains(node.Name))
            {
                if (HasCycle(node.Name, outgoing, visited, stack))
                {
                    Add(result, "DIA020", DiagramDiagnosticSeverity.Error,
                        "Diagram contains a cyclic dependency.");
                    return;
                }
            }
        }
    }

    private static bool HasCycle(
        string nodeName,
        IDictionary<string, List<DiagramEdge>> outgoing,
        HashSet<string> visited,
        HashSet<string> stack)
    {
        visited.Add(nodeName);
        stack.Add(nodeName);

        if (outgoing.TryGetValue(nodeName, out var edges))
        {
            foreach (var edge in edges)
            {
                var next = edge.ToNodeName;

                if (!visited.Contains(next) &&
                    HasCycle(next, outgoing, visited, stack))
                    return true;

                if (stack.Contains(next))
                    return true;
            }
        }

        stack.Remove(nodeName);
        return false;
    }

    private static void ValidateNodeKinds(
        IList<DiagramNode> nodes,
        IDictionary<string, List<DiagramEdge>> outgoing,
        IDictionary<string, List<DiagramEdge>> incoming,
        DiagramValidationResult result)
    {
        foreach (var node in nodes)
        {
            outgoing.TryGetValue(node.Name, out var outEdges);
            incoming.TryGetValue(node.Name, out var inEdges);

            outEdges ??= new List<DiagramEdge>();
            inEdges ??= new List<DiagramEdge>();

            if (node.Kind == DiagramNodeKind.Model)
            {
                if (outEdges.Count > 0 || inEdges.Count > 0)
                {
                    Add(result, "DIA030", DiagramDiagnosticSeverity.Error,
                        "Model nodes must not have dependencies.",
                        nodeName: node.Name);
                }
            }

            if (node.Kind == DiagramNodeKind.External)
            {
                if (outEdges.Count > 0)
                {
                    Add(result, "DIA031", DiagramDiagnosticSeverity.Error,
                        "External nodes must not have outgoing dependencies.",
                        nodeName: node.Name);
                }
            }
        }
    }

    private static void ValidateRoles(
        IList<DiagramNode> nodes,
        IDictionary<string, List<DiagramEdge>> outgoing,
        IDictionary<string, List<DiagramEdge>> incoming,
        DiagramValidationResult result)
    {
        foreach (var node in nodes.Where(n => n.Kind == DiagramNodeKind.Component))
        {
            outgoing.TryGetValue(node.Name, out var outEdges);
            incoming.TryGetValue(node.Name, out var inEdges);

            outEdges ??= new List<DiagramEdge>();
            inEdges ??= new List<DiagramEdge>();

            switch (node.Role)
            {
                case DiagramComponentRole.Exposure:
                    ValidateOutgoing(node, outEdges, 1, result);
                    break;

                case DiagramComponentRole.Orchestration:
                    if (outEdges.Count < 2 || outEdges.Count > 3)
                    {
                        Add(result, "DIA040", DiagramDiagnosticSeverity.Error,
                            "Orchestration component must have 2 to 3 outgoing dependencies.",
                            nodeName: node.Name);
                    }
                    break;

                case DiagramComponentRole.Processing:
                    ValidateOutgoing(node, outEdges, 1, result);
                    break;

                case DiagramComponentRole.Service:
                    ValidateOutgoing(node, outEdges, 1, result);
                    break;

                case DiagramComponentRole.Broker:
                    ValidateOutgoing(node, outEdges, 1, result);

                    var targetIsExternal = outEdges.All(e =>
                    {
                        var target = nodes.First(n => n.Name == e.ToNodeName);
                        return target.Kind == DiagramNodeKind.External;
                    });

                    if (!targetIsExternal)
                    {
                        Add(result, "DIA041", DiagramDiagnosticSeverity.Error,
                            "Broker must only depend on External nodes.",
                            nodeName: node.Name);
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
                nodeName: node.Name);
        }
    }

    private static void Add(
        DiagramValidationResult result,
        string code,
        DiagramDiagnosticSeverity severity,
        string message,
        string nodeName = null,
        string edgeName = null)
    {
        result.Diagnostics.Add(new DiagramDiagnostic
        {
            Code = code,
            Severity = severity,
            Message = message,
            NodeName = nodeName,
            EdgeName = edgeName
        });
    }
}