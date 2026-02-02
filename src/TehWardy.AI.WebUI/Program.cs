using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TehWardy.AI.WebUI;
using TehWardy.AI.WebUI.Brokers;
using TehWardy.AI.WebUI.Components.Tools;
using TehWardy.AI.WebUI.Components.Tools.ArchitectureDesigner;
using TehWardy.AI.WebUI.Foundations;
using TehWardy.AI.WebUI.Processings;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<IAgenticConversationBroker, AgenticConversationBroker>();
builder.Services.AddTransient<IAgenticConversationService, AgenticConversationService>();

//Architecture designer tool ...
builder.Services.AddTransient<IArchitectureDiagramCompilerProcessingService, ArchitectureDiagramCompilerProcessingService>();
builder.Services.AddTransient<IArchitectureDiagramValidationProcessingService, ArchitectureDiagramValidationProcessingService>();
builder.Services.AddTransient<IDiagramAutoLayoutProcessingService, DiagramAutoLayoutProcessingService>();

builder.Services.AddSingleton<IToolUiRegistry>(sp =>
    new ToolUiRegistry()
        .Register<ArchitectureDesigner>("ArchitectureDesigner")
        .Register<MarketAnalyzer>("MarketAnalyzer")
);


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7049/"),
    Timeout = TimeSpan.FromMinutes(10)
});

await builder.Build().RunAsync();