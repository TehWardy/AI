using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public static class SampleDiagramFactory
{
    public static DiagramSpecification CreateSample()
    {
        var diagramId = Guid.NewGuid();

        // Nodes
        var fooModel = NewModel("Foo", DiagramComponentRole.Exposure);
        var foo = NewComponent("Foo", DiagramComponentRole.Exposure);
        var fooProc = NewComponent("FooProcessingService", DiagramComponentRole.Processing);
        var fooSvc = NewComponent("FooService", DiagramComponentRole.Service);
        var fooBroker = NewComponent("FooBroker", DiagramComponentRole.Broker);

        var bar = NewComponent("Bar", DiagramComponentRole.Exposure);
        var barSvc = NewComponent("BarService", DiagramComponentRole.Service);
        var barBroker = NewComponent("BarBroker", DiagramComponentRole.Broker);

        var procExternal = NewExternal("External Dependency");
        fooBroker.ExternalResourceId = procExternal.Id;

        var barExternal = NewExternal("Some External");
        barBroker.ExternalResourceId = barExternal.Id;

        // Edges (left -> right)
        var edges = new List<DiagramEdge>
        {
            NewEdge(foo.Id, fooProc.Id, DiagramDependencyKind.InProcess),
            NewEdge(fooProc.Id, fooSvc.Id, DiagramDependencyKind.InProcess),
            NewEdge(fooSvc.Id, fooBroker.Id, DiagramDependencyKind.InProcess),
            NewEdge(fooBroker.Id, procExternal.Id, DiagramDependencyKind.ExternalBoundary),

            NewEdge(bar.Id, barSvc.Id, DiagramDependencyKind.InProcess),
            NewEdge(barSvc.Id, barBroker.Id, DiagramDependencyKind.InProcess),
            NewEdge(barBroker.Id, barExternal.Id, DiagramDependencyKind.ExternalBoundary),
        };

        return new DiagramSpecification
        {
            Id = diagramId,
            Name = "Sample Diagram",
            Nodes = 
            [
                foo, fooModel, fooProc, fooSvc, fooBroker,
                bar, barSvc, barBroker,
                procExternal, barExternal
            ],
            Edges = edges
        };
    }

    static DiagramNode NewComponent(string name, DiagramComponentRole role)
    {
        return new DiagramNode
        {
            Id = Guid.NewGuid(),
            Kind = DiagramNodeKind.Component,
            Name = name,
            Role = role,
            Methods = 
            [
                new DiagramMethod
                {
                    Name = "GetByIdAsync",
                    OutputType = "Foo",
                    Inputs =
                    [
                        new DiagramParameter { Name = "Id", Type = "guid", Optional = false }    
                    ]
                }
            ]
        };
    }

    static DiagramNode NewModel(string name, DiagramComponentRole role)
    {
        return new DiagramNode
        {
            Id = Guid.NewGuid(),
            Kind = DiagramNodeKind.Model,
            Name = name,
            Role = role,
            Properties =
            [
                new DiagramProperty { Name = "Id", Type = "Guid", Required = true },
                new DiagramProperty { Name = "Name", Type = "string", Required = true }
            ]
        };
    }

    static DiagramNode NewExternal(string name)
    {
        return new DiagramNode
        {
            Id = Guid.NewGuid(),
            Kind = DiagramNodeKind.External,
            Name = name,
            Methods = new List<DiagramMethod>(),
            Properties = new List<DiagramProperty>()
        };
    }

    static DiagramEdge NewEdge(Guid from, Guid to, DiagramDependencyKind kind)
    {
        return new DiagramEdge
        {
            Id = Guid.NewGuid(),
            FromNodeId = from,
            ToNodeId = to,
            Kind = kind
        };
    }
}