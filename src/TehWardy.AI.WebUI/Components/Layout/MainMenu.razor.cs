using Microsoft.AspNetCore.Components;

namespace TehWardy.AI.WebUI.Components.Layout;

public partial class MainMenu : ComponentBase
{
    private bool collapseNavMenu = false;

    protected string NavMenuCssClass =>
        collapseNavMenu ? "collapse" : string.Empty;

    protected void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}