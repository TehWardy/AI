using TehWardy.AI.Tools.ArchitectureDiagram.Models;
using TehWardy.AI.Tools.Standard.Models;

namespace TehWardy.AI.Tools.ArchitectureDiagram.Processings;

public sealed class ArchitectureDiagramCompilerProcessingService
    : IArchitectureDiagramCompilerProcessingService
{
    public ArchitectureSpec Compile(DiagramSpecification diagram)
    {
        if (diagram == null)
            throw new ArgumentNullException(nameof(diagram));

        var spec = new ArchitectureSpec
        {
            Name = diagram.Name,
            Components = new List<ComponentSpec>(),
            Models = new List<ModelSpec>(),
            ExternalResources = new List<ExternalResourceSpec>(),
            Dependencies = new List<DependencySpec>()
        };

        var nodeMap = diagram.Nodes.ToDictionary(n => n.Id);

        CompileNodes(diagram, spec);
        CompileEdges(diagram, spec, nodeMap);

        return spec;
    }

    private static void CompileNodes(DiagramSpecification diagram, ArchitectureSpec spec)
    {
        foreach (var node in diagram.Nodes)
        {
            switch (node.Kind)
            {
                case DiagramNodeKind.Component:
                    spec.Components.Add(CompileComponent(node));
                    break;

                case DiagramNodeKind.Model:
                    spec.Models.Add(CompileModel(node));
                    break;

                case DiagramNodeKind.External:
                    spec.ExternalResources.Add(CompileExternal(node));
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported DiagramNodeKind '{node.Kind}'.");
            }
        }
    }

    private static ComponentSpec CompileComponent(DiagramNode node)
    {
        if (!node.Role.HasValue)
            throw new InvalidOperationException(
                $"Component node '{node.Name}' has no Role.");

        var (layer, exposureKind, serviceKind) = MapRole(node.Role.Value);

        ComponentSpec component = layer switch
        {
            ComponentLayer.Exposure => new ExposureSpec
            {
                Kind = exposureKind ?? ExposureKind.Api,
                Endpoints = new List<ExposureEndpointSpec>()
            },

            ComponentLayer.Service => new ServiceSpec
            {
                Kind = serviceKind ?? ServiceKind.Foundation,
            },

            ComponentLayer.Broker => new BrokerSpec
            {
                ExternalResourceId = node.ExternalResourceId?.ToString()
            },

            _ => throw new InvalidOperationException()
        };

        component.Id = node.Id.ToString();
        component.Name = node.Name;
        component.Layer = layer;
        component.Methods = CompileMethods(node.Methods);

        return component;
    }
    private static IList<MethodSpec> CompileMethods(IList<DiagramMethod> methods)
    {
        if (methods == null)
            return new List<MethodSpec>();

        return methods.Select(m => new MethodSpec
        {
            Name = m.Name,
            Async = MapAsync(m.Async),
            Output = ParseTypeRef(m.OutputType),
            Inputs = m.Inputs?.Select(p => new ParameterSpec
            {
                Name = p.Name,
                Type = ParseTypeRef(p.Type),
                Optional = p.Optional
            }).ToList()
        }).ToList();
    }

    private static ModelSpec CompileModel(DiagramNode node)
    {
        return new ModelSpec
        {
            Name = node.Name,
            Kind = ModelKind.Record,
            Properties = node.Properties?.Select(p => new ModelPropertySpec
            {
                Name = p.Name,
                Type = ParseTypeRef(p.Type),
                Required = p.Required
            }).ToList()
        };
    }

    private static ExternalResourceSpec CompileExternal(DiagramNode node)
    {
        return new ExternalResourceSpec
        {
            Id = node.Id.ToString(),
            Name = node.Name,
            Type = ExternalResourceType.Other
        };
    }
    private static void CompileEdges(
        DiagramSpecification diagram,
        ArchitectureSpec spec,
        IDictionary<Guid, DiagramNode> nodeMap)
    {
        foreach (var edge in diagram.Edges)
        {
            if (!nodeMap.TryGetValue(edge.FromNodeId, out var from))
                throw new InvalidOperationException("Invalid FromNodeId.");

            if (!nodeMap.TryGetValue(edge.ToNodeId, out var to))
                throw new InvalidOperationException("Invalid ToNodeId.");

            spec.Dependencies.Add(new DependencySpec
            {
                FromComponentId = edge.FromNodeId.ToString(),
                ToComponentId = edge.ToNodeId.ToString(),
                Kind = edge.Kind == DiagramDependencyKind.ExternalBoundary
                    ? DependencyKind.ExternalBoundary
                    : DependencyKind.InProcess
            });
        }
    }

    private static (
        ComponentLayer layer,
        ExposureKind? exposureKind,
        ServiceKind? serviceKind)
        MapRole(DiagramComponentRole role)
    {
        return role switch
        {
            DiagramComponentRole.Exposure =>
                (ComponentLayer.Exposure, ExposureKind.Api, null),

            DiagramComponentRole.Orchestration =>
                (ComponentLayer.Service, null, ServiceKind.Orchestration),

            DiagramComponentRole.Processing =>
                (ComponentLayer.Service, null, ServiceKind.Processing),

            DiagramComponentRole.Service =>
                (ComponentLayer.Service, null, ServiceKind.Foundation),

            DiagramComponentRole.Broker =>
                (ComponentLayer.Broker, null, null),

            _ => throw new InvalidOperationException()
        };
    }

    private static AsyncKind MapAsync(DiagramAsyncKind async) => async switch
    {
        DiagramAsyncKind.Sync => AsyncKind.Sync,
        DiagramAsyncKind.Task => AsyncKind.Task,
        DiagramAsyncKind.ValueTask => AsyncKind.ValueTask,
        _ => AsyncKind.Sync
    };

    private static TypeRef ParseTypeRef(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return new TypeRef { Name = "void" };

        // Minimal parser – extend later
        if (!type.Contains('<'))
            return new TypeRef { Name = type };

        var genericStart = type.IndexOf('<');
        var genericEnd = type.LastIndexOf('>');

        var name = type.Substring(0, genericStart);
        var inner = type.Substring(
            genericStart + 1,
            genericEnd - genericStart - 1);

        return new TypeRef
        {
            Name = name,
            GenericArguments = inner
                .Split(',')
                .Select(t => ParseTypeRef(t.Trim()))
                .ToList()
        };
    }
}
