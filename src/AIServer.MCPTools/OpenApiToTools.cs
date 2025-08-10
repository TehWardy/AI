using System.Text.Json;
using System.Text.RegularExpressions;
using AIServer.MCPTools.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

public static class OpenApiToTools
{
    public static IEnumerable<IDictionary<string, object>> ProjectToOllamaTools(IEnumerable<HttpTool> tools)
    {
        var list = new List<IDictionary<string, object>>();

        foreach (var t in tools ?? Enumerable.Empty<HttpTool>())
        {
            var fn = t?.Function;
            if (fn == null) continue;

            // --- parameters (root)
            var parameters = new Dictionary<string, object>
            {
                ["type"] = "object"
            };

            // --- properties
            var props = new Dictionary<string, object>();

            // Endpoint
            if (!string.IsNullOrWhiteSpace(fn.Parameters?.Properties?.Endpoint?.Type))
            {
                var desc = fn.Parameters.Properties.Endpoint.Description ?? "";
                props["endpoint"] = new Dictionary<string, object>
                {
                    ["type"] = "string",
                    ["description"] = desc
                };
            }

            // Verb – prefer enum so it's a schema, not a sample in description
            if (!string.IsNullOrWhiteSpace(fn.Parameters?.Properties?.Verb?.Type))
            {
                var v = fn.Parameters.Properties.Verb.Description?.Trim();
                var verbSchema = new Dictionary<string, object> { ["type"] = "string" };
                if (!string.IsNullOrEmpty(v))
                    verbSchema["enum"] = new[] { v.ToUpperInvariant() };
                props["verb"] = verbSchema;
            }

            // Headers
            if (!string.IsNullOrWhiteSpace(fn.Parameters?.Properties?.Headers?.Type))
            {
                var hdrDesc = fn.Parameters.Properties.Headers.Description ?? "Optional HTTP headers.";
                props["headers"] = new Dictionary<string, object>
                {
                    ["type"] = "object",
                    ["description"] = hdrDesc
                };
            }

            // Body (optional)
            var body = fn.Parameters?.Properties?.Body;
            if (body != null && !string.IsNullOrWhiteSpace(body.Type))
            {
                var bodySchema = new Dictionary<string, object>
                {
                    ["type"] = body.Type
                };
                if (!string.IsNullOrWhiteSpace(body.Description))
                    bodySchema["description"] = body.Description;
                props["body"] = bodySchema;
            }

            // Query (optional, ensure object)
            var query = fn.Parameters?.Properties?.Query;
            if (query != null)
            {
                var querySchema = new Dictionary<string, object>
                {
                    ["type"] = "object"
                };
                if (!string.IsNullOrWhiteSpace(query.Description))
                    querySchema["description"] = query.Description;

                // If you want, you can infer known query param names into "properties" here.

                props["query"] = querySchema;
            }

            // Attach properties if we have any
            if (props.Count > 0)
                parameters["properties"] = props;

            // required – MUST match lower-case keys present above
            var required = new List<string>();
            foreach (var name in (fn.Parameters?.Required ?? Array.Empty<string>()))
            {
                var lower = name.ToLowerInvariant();
                if (props.ContainsKey(lower))
                    required.Add(lower);
            }
            if (required.Count > 0)
                parameters["required"] = required;

            // --- function object
            var function = new Dictionary<string, object>
            {
                ["name"] = fn.Name,
                ["description"] = string.IsNullOrWhiteSpace(fn.Description) ? "" : fn.Description,
                ["parameters"] = parameters
            };

            // --- tool wrapper
            var tool = new Dictionary<string, object>
            {
                ["type"] = "function",
                ["function"] = function
            };

            list.Add(tool);
        }

        return list;
    }

    public static async Task<HttpTool[]> BuildFromAsync(
        Uri openApiJsonUri,
        string branchRoot = "",
        string baseUri = null)
    {
        using var http = new HttpClient();
        await using var stream = await http.GetStreamAsync(openApiJsonUri);
        var reader = new OpenApiStreamReader();
        var doc = reader.Read(stream, out var _);
        return BuildFrom(doc, branchRoot, baseUri);
    }

    public static HttpTool[] BuildFrom(
        OpenApiDocument doc,
        string branchRoot = "/",
        string baseUri = null)
    {
        branchRoot = NormalizeBranch(branchRoot);
        baseUri = NormalizeBase(baseUri);

        var tools = new List<HttpTool>();

        foreach (var (rawPath, pathItem) in doc.Paths)
        {
            var relPath = NormalizePath(rawPath);
            if (!IsUnderBranch(relPath, branchRoot)) continue;

            foreach (var (opType, op) in pathItem.Operations)
            {
                var verb = opType.ToString().ToUpperInvariant();

                // --- Headers to suggest (example/defaults)
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var accept = FirstResponseContentType(op) ?? "application/json";
                headers["Accept"] = accept;

                foreach (var p in AllParams(pathItem, op).Where(p => p.In == ParameterLocation.Header))
                    headers[p.Name] = ExampleOrDefault(p) ?? "";

                // --- Body
                string bodyExample = string.Empty;
                var reqContentType = FirstRequestContentType(op);
                string bodySchemaType = null; // "object" | "string"

                if (!string.IsNullOrWhiteSpace(reqContentType))
                {
                    bodySchemaType = reqContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
                        ? "object"
                        : "string";

                    bodyExample = GuessBody(op);

                    if (!headers.ContainsKey("Content-Type"))
                        headers["Content-Type"] = reqContentType!;
                }

                // --- Query
                var queryParams = AllParams(pathItem, op)
                    .Where(p => p.In == ParameterLocation.Query)
                    .ToList();

                var queryExample = BuildQuery(queryParams);

                // --- Absolute endpoint (keep {placeholders})
                var abs = baseUri != null ? JoinBase(baseUri, relPath) : relPath;

                // Build a friendly example for description
                string queryObjectExample = null;
                if (!string.IsNullOrWhiteSpace(queryExample))
                {
                    var paramArray = queryExample
                        .Split('&', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Contains("=") ? p[..p.IndexOf('=')] : p)
                        .Where(k => !string.IsNullOrWhiteSpace(k))
                        .Select(k => $"\"{k}\": \"value\"");

                    var joined = string.Join(", ", paramArray);
                    queryObjectExample = $"An object like {{ {joined} }}";
                }

                var fn = new HttpToolFunction
                {
                    Name = SanitizeFunctionName($"{verb}_{relPath.Trim('/').Replace('/', '_').Replace('-', '_')}"),
                    Description = !string.IsNullOrWhiteSpace(op.Summary) ? op.Summary : $"{verb} {abs}",

                    Parameters = new HttpToolParameters
                    {
                        // JSON Schema (root)
                        Type = "object",

                        Properties = new HttpToolProperties
                        {
                            // NOTE: property names here must match the required casing below
                            Endpoint = new ToolProperty
                            {
                                Type = "string",
                                Description = abs
                            },
                            Verb = new ToolProperty
                            {
                                Type = "string",
                                Description = verb
                            },
                            Headers = new ToolProperty
                            {
                                Type = "object",
                                Description = $"Optional HTTP headers. Example: {JsonSerializer.Serialize(headers)}"
                            },
                            Body = !string.IsNullOrWhiteSpace(bodySchemaType)
                                ? new ToolProperty
                                {
                                    Type = bodySchemaType,
                                    Description = string.IsNullOrWhiteSpace(bodyExample)
                                        ? (bodySchemaType == "object"
                                            ? "Request body (JSON). Provide an object matching the API schema."
                                            : "Request body (non-JSON). Provide raw payload as string.")
                                        : (bodySchemaType == "object"
                                            ? $"Request body (JSON). Example: {bodyExample}"
                                            : $"Request body (string). Example: {bodyExample}")
                                }
                                : null,
                            // IMPORTANT: Query is an OBJECT schema (not string). Only include when present.
                            Query = queryObjectExample != null
                                ? new ToolProperty
                                {
                                    Type = "object",
                                    Description = $"Query parameters. {queryObjectExample}"
                                }
                                : null
                        },

                        Required = BuildRequired(queryObjectExample != null)

                    }
                };

                tools.Add(new HttpTool { Type = "function", Function = fn });
            }
        }

        return tools
            .OrderBy(t => t.Function?.Parameters?.Properties?.Endpoint?.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ThenBy(t => t.Function?.Parameters?.Properties?.Verb?.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }


    // ---------- helpers ----------

    private static string[] BuildRequired(bool hasQuery) => hasQuery
        ? new[] { "verb", "headers", "query" }
        : new[] { "verb", "headers" };

    private static string SanitizeFunctionName(string name)
    {
        // Keep only [A-Za-z0-9_-]; replace others with underscore
        var sanitized = Regex.Replace(name, @"[^A-Za-z0-9_\-]", "_");
        // Avoid leading digits if your runner dislikes them
        if (char.IsDigit(sanitized.FirstOrDefault()))
            sanitized = "_" + sanitized;
        // Avoid empty
        return string.IsNullOrEmpty(sanitized) ? "fn" : sanitized;
    }

    private static IEnumerable<OpenApiParameter> AllParams(OpenApiPathItem pathItem, OpenApiOperation op) =>
        (pathItem.Parameters ?? Enumerable.Empty<OpenApiParameter>())
            .Concat(op.Parameters ?? Enumerable.Empty<OpenApiParameter>());

    private static string FirstRequestContentType(OpenApiOperation op) =>
        op.RequestBody?.Content?.Keys?.FirstOrDefault();

    private static string FirstResponseContentType(OpenApiOperation op)
    {
        return op.Responses?.FirstOrDefault(kv => kv.Key.StartsWith("2"))
            .Value?.Content?.Keys?.FirstOrDefault()
           ??
        op.Responses?.FirstOrDefault().Value?.Content?.Keys?.FirstOrDefault();
    }

    private static string BuildQuery(IEnumerable<OpenApiParameter> qps)
    {
        var parts = new List<string>();

        foreach (var p in qps)
        {
            var val = ExampleOrDefault(p) ?? "";
            // leave empty if no example/default; caller can fill in later
            parts.Add(Uri.EscapeDataString(p.Name) + "=" + Uri.EscapeDataString(val));
        }

        return string.Join("&", parts);
    }

    private static string ExampleOrDefault(OpenApiParameter p)
    {
        if (p.Example != null)
            return OpenApiAnyToJsonScalar(p.Example);

        if (p.Schema?.Example != null)
            return OpenApiAnyToJsonScalar(p.Schema.Example);

        if (p.Schema?.Default != null)
            return OpenApiAnyToJsonScalar(p.Schema.Default);

        // tiny type-based hint
        return p.Schema?.Type switch
        {
            "integer" => "0",
            "number" => "0",
            "boolean" => "false",
            "string" => "",
            _ => ""
        };
    }

    private static string GuessBody(OpenApiOperation op)
    {
        var rb = op.RequestBody;
        if (rb == null || rb.Content.Count == 0) return string.Empty;

        var mt = rb.Content.First().Value;

        if (mt.Example != null) return OpenApiAnyToJson(mt.Example);
        if (mt.Examples != null && mt.Examples.Count > 0)
            return OpenApiAnyToJson(mt.Examples.First().Value.Value);

        var schema = mt.Schema;
        if (schema == null) return string.Empty;

        // Minimal stubs based on schema
        if (string.Equals(schema.Type, "object", StringComparison.OrdinalIgnoreCase))
            return "{}";
        if (string.Equals(schema.Type, "array", StringComparison.OrdinalIgnoreCase))
            return "[]";

        return string.Empty;
    }

    private static string OpenApiAnyToJson(IOpenApiAny any)
    {
        using var sw = new StringWriter();
        var writer = new Microsoft.OpenApi.Writers.OpenApiJsonWriter(sw);
        any.Write(writer, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
        writer.Flush();
        return sw.ToString();
    }

    private static string OpenApiAnyToJsonScalar(IOpenApiAny any)
    {
        // Returns a *scalar* as plain text (not quoted JSON), else a compact JSON string
        return any switch
        {
            OpenApiString s => s.Value,
            OpenApiInteger i => i.Value.ToString(),
            OpenApiLong l => l.Value.ToString(),
            OpenApiFloat f => f.Value.ToString(),
            OpenApiDouble d => d.Value.ToString(),
            OpenApiBoolean b => b.Value ? "true" : "false",
            OpenApiNull => "",
            _ => OpenApiAnyToJson(any)
        };
    }

    private static string NormalizeBranch(string branch)
    {
        if (string.IsNullOrWhiteSpace(branch))
            return "/";

        branch = branch.Trim();

        if (!branch.StartsWith("/"))
            branch = "/" + branch;

        if (branch.Length > 1 && branch.EndsWith("/"))
            branch = branch[..^1];

        return branch;
    }

    private static string NormalizePath(string p)
    {
        if (string.IsNullOrWhiteSpace(p)) return "/";
        p = p.Trim();
        return p.StartsWith("/") ? p : "/" + p;
    }

    private static bool IsUnderBranch(string path, string branch) =>
        path.Equals(branch, StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith(branch + "/", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeBase(string baseUri)
    {
        if (string.IsNullOrWhiteSpace(baseUri))
            return null;

        if (!baseUri.EndsWith("/"))
            baseUri += "/";

        return baseUri;
    }

    private static string JoinBase(string baseUri, string relPath)
    {
        if (relPath.StartsWith("/"))
            relPath = relPath[1..];

        return baseUri + relPath;
    }
}