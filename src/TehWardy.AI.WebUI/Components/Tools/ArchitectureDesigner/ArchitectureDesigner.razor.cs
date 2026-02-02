using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Models;
using TehWardy.AI.WebUI.Models.Tools.ArchitectureDesigner;
using TehWardy.AI.WebUI.Processings;

namespace TehWardy.AI.WebUI.Components.Tools.ArchitectureDesigner;

public partial class ArchitectureDesigner : ComponentBase, IToolComponent
{
    [Parameter, EditorRequired]
    public ToolState ToolState { get; set; }

    [Parameter]
    public EventCallback<string> OnToolStateChanged { get; set; }

    [Inject]
    public IDiagramAutoLayoutProcessingService AutoLayoutService { get; set; }

    public DiagramSpecification Diagram { get; set; }
    public ComputedDiagramLayout Layout { get; set; }

    public void ApplyAssistantState(string newToolStateJson)
    {
        Diagram = JsonSerializer
            .Deserialize<DiagramSpecification>(newToolStateJson);

        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        Diagram = SampleDiagramFactory.CreateSample();
        AutoLayout();
    }

    private void LoadSample()
    {
        Diagram = SampleDiagramFactory.CreateSample();
        AutoLayout();
    }

    private void AutoLayout()
    {
        Layout = AutoLayoutService.Compute(Diagram);
    }
}