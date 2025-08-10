using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.MCP;

public class ToolsController : Controller
{
    private readonly ILogger<ToolsController> log;

    public ToolsController(ILogger<ToolsController> log) =>
        this.log = log;

    [HttpGet("api/tools")]
    public async ValueTask<IActionResult> GetAsync()
    {
        log.LogInformation("All Tools Requested");

        var tools = await OpenApiToTools.BuildFromAsync(
            openApiJsonUri: new Uri("https://localhost:7181/swagger/v1/swagger.json"),
            baseUri: "https://localhost:7181/",
            branchRoot: "/api");

        return Ok(tools);
    }

    [HttpGet("api/tools/search")]
    public async ValueTask<IActionResult> GetAsync([FromQuery] string searchTerm)
    {
        log.LogInformation("Tool Search Requested for terms: " + searchTerm);

        var tools = await OpenApiToTools.BuildFromAsync(
            openApiJsonUri: new Uri("https://localhost:7181/swagger/v1/swagger.json"),
            baseUri: "https://localhost:7181/",
            branchRoot: "/api");

        var termParts = searchTerm.Split(' ');

        var results = termParts
            .SelectMany(term => tools.Where(tool => tool.Function.Name.Contains(term) || tool.Function.Description.Contains(term)).ToArray())
            .GroupBy(toolResult => toolResult.Function.Name)
            .OrderByDescending(group => group.Count())
            .Select(group => group.First())
            .ToArray();

        log.LogInformation($"Found {results.Length} results");

        return Ok(OpenApiToTools.ProjectToOllamaTools(results));
    }

    [HttpGet("api/tools/from-swagger-spec")]
    public async ValueTask<IActionResult> GetAsync(string websiteRootUri, string fromPath, string searchTerm)
    {
        log.LogInformation($"Tool Search Requested for site {websiteRootUri} on branch path {fromPath}");

        var tools = await OpenApiToTools.BuildFromAsync(
            openApiJsonUri: new Uri($"{websiteRootUri}swagger/v1/swagger.json"),
            baseUri: websiteRootUri,
            branchRoot: fromPath.TrimStart('/'));

        if (searchTerm is not null)
        {
            log.LogInformation($"Filtering using search term: {searchTerm}");
            var termParts = searchTerm.Split(' ');

            tools = termParts
                .SelectMany(term => tools.Where(tool => tool.Function.Name.Contains(term) || tool.Function.Description.Contains(term)).ToArray())
                .GroupBy(toolResult => toolResult.Function.Name)
                .OrderByDescending(group => group.Count())
                .Select(group => group.First())
                .ToArray();
        }

        log.LogInformation($"Found {tools.Length} results");

        return Ok(OpenApiToTools.ProjectToOllamaTools(tools));
    }
}