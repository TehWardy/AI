namespace TehWardy.AI.Tools.Models;

internal class CatalogServer
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; }
    public string HomepageUrl { get; init; }
    public string Package { get; init; }                 // e.g. npm/pip/container image/etc
    public string Transport { get; init; }               // stdio/http/sse/etc
    public string Endpoint { get; init; }                // e.g. https://... or command
}