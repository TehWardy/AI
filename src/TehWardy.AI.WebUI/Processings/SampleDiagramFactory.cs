using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;

namespace TehWardy.AI.WebUI.Processings;

public static class SampleDiagramFactory
{
    public static DiagramSpecification CreateSample()
    {
        var diagramId = Guid.NewGuid();

        // Nodes
        var fooModel = NewModel("Foo", DiagramComponentRole.Exposure);
        var foo = NewComponent("FooThing", DiagramComponentRole.Exposure);
        var fooProc = NewComponent("FooProcessingService", DiagramComponentRole.Processing);
        var fooSvc = NewComponent("FooService", DiagramComponentRole.Service);
        var fooBroker = NewComponent("FooBroker", DiagramComponentRole.Broker);

        var bar = NewComponent("Bar", DiagramComponentRole.Exposure);
        var barSvc = NewComponent("BarService", DiagramComponentRole.Service);
        var barBroker = NewComponent("BarBroker", DiagramComponentRole.Broker);

        var procExternal = NewExternal("External Dependency");
        var barExternal = NewExternal("Some External");

        // Edges (left -> right)
        var edges = new List<DiagramEdge>
        {
            NewEdge(foo.Name, fooProc.Name),
            NewEdge(fooProc.Name, fooSvc.Name),
            NewEdge(fooSvc.Name, fooBroker.Name),
            NewEdge(fooBroker.Name, procExternal.Name),

            NewEdge(bar.Name, barSvc.Name),
            NewEdge(barSvc.Name, barBroker.Name),
            NewEdge(barBroker.Name, barExternal.Name)
        };

        return new DiagramSpecification
        {
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
            Kind = DiagramNodeKind.External,
            Name = name,
            Methods = new List<DiagramMethod>(),
            Properties = new List<DiagramProperty>()
        };
    }

    static DiagramEdge NewEdge(string from, string to)
    {
        return new DiagramEdge
        {
            FromNodeName = from,
            ToNodeName = to
        };
    }
}