using System.Reflection;
using System.Text;
using System.Text.Json;
using AIServer.MCPTools.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers; 

namespace AIServer.MCPTools;

public class HttpDiscovery
{
    private readonly IServiceProvider _sp;

    public HttpDiscovery(IServiceProvider serviceProvider) => _sp = serviceProvider;

    public Tool[] DiscoverHttpTools(string branchRoot)
    {
        branchRoot = NormalizeBranch(branchRoot);
        var dataSources = _sp.GetServices<EndpointDataSource>();
        var tools = new List<Tool>();

        foreach (var ds in dataSources)
        {
            foreach (var ep in ds.Endpoints)
            {
                if (ep is not RouteEndpoint re) continue;

                var template = re.RoutePattern.RawText ?? re.RoutePattern.ToString();
                var path = TemplateToPath(template);

                if (!IsUnderBranch(path, branchRoot)) continue;

                // .NET 9: prefer the interfaces for better compatibility
                var methodMeta = ep.Metadata.GetMetadata<IHttpMethodMetadata>();
                var methods = (methodMeta?.HttpMethods?.Count > 0 ? methodMeta.HttpMethods : new[] { "GET" });

                var action = ep.Metadata.GetMetadata<ControllerActionDescriptor>();

                foreach (var m in methods)
                {
                    var acceptsMeta = ep.Metadata.GetMetadata<IAcceptsMetadata>();
                    var fromBodyMeta = ep.Metadata.GetOrderedMetadata<IFromBodyMetadata>()?.FirstOrDefault();

                    var props = new HttpToolProperties
                    {
                        Endpoint = new ToolProperty 
                        { 
                            Type = "string", 
                            Description = path 
                        },
                        Verb = new ToolProperty 
                        { 
                            Type = "string", 
                            Description = m 
                        },
                        Body = BuildBodySample(action, m, acceptsMeta, fromBodyMeta),
                        Headers = BuildHeaders(action, acceptsMeta)
                    };

                    var httpFn = new HttpToolFunction
                    {
                        Name = MakeToolName(m, template, action),
                        Description = MakeDescription(action, m, template),
                        Parameters = new HttpToolParameters
                        {
                            Properties = props
                        }
                    };

                    tools.Add(new Tool
                    {
                        Type = "function",
                        Function = httpFn
                    });
                }
            }
        }

        return tools
            .OrderBy(t => ((HttpToolFunction)t.Function).Parameters.Properties.Endpoint.Description, StringComparer.OrdinalIgnoreCase)
            .ThenBy(t => ((HttpToolFunction)t.Function).Parameters.Properties.Verb.Description, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeBranch(string branch)
    {
        if (string.IsNullOrWhiteSpace(branch)) return "/";
        branch = branch.Trim();
        if (!branch.StartsWith("/")) branch = "/" + branch;
        if (branch.Length > 1 && branch.EndsWith("/")) branch = branch.TrimEnd('/');
        return branch;
    }

    private static bool IsUnderBranch(string path, string branch) =>
        path.Equals(branch, StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith(branch + "/", StringComparison.OrdinalIgnoreCase);

    private static string TemplateToPath(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) return "/";
        template = template.Trim();
        return template.StartsWith("/") ? template : "/" + template;
    }

    private static string MakeToolName(string verb, string template, ControllerActionDescriptor action)
    {
        var normalized = template.Trim('/').Replace("/", ".", StringComparison.OrdinalIgnoreCase);
        var actionHint = action != null ? $"{action.ControllerName}.{action.ActionName}" : normalized;
        return $"{verb.ToUpperInvariant()} {actionHint}";
    }

    private static string MakeDescription(ControllerActionDescriptor action, string method, string template)
    {
        if (action != null)
        {
            var sb = new StringBuilder();
            sb.Append($"{method.ToUpperInvariant()} {TemplateToPath(template)} via {action.ControllerName}.{action.ActionName}.");
            if (!string.IsNullOrWhiteSpace(action.DisplayName))
                sb.Append($" ({action.DisplayName})");
            return sb.ToString();
        }
        return $"{method.ToUpperInvariant()} {TemplateToPath(template)}.";
    }

    private static ToolProperty BuildHeaders(ControllerActionDescriptor action, IAcceptsMetadata accepts)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Accept header from [Produces] or IAcceptsMetadata
        var produces = action?.EndpointMetadata?.OfType<ProducesAttribute>().FirstOrDefault();
        var accept = produces?.ContentTypes?.FirstOrDefault()
                     ?? accepts?.ContentTypes?.FirstOrDefault()
                     ?? "application/json";
        dict["Accept"] = accept;

        // Content-Type if the endpoint declares accepted content types
        var consumes = action?.EndpointMetadata?.OfType<ConsumesAttribute>().FirstOrDefault();
        var contentType = consumes?.ContentTypes?.FirstOrDefault()
                          ?? accepts?.ContentTypes?.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(contentType))
            dict["Content-Type"] = contentType;

        return new ToolProperty 
        { 
            Type = "object", 
            Description = JsonSerializer.Serialize(dict) 
        };
    }

    private static ToolProperty BuildBodySample(
        ControllerActionDescriptor action,
        string method,
        IAcceptsMetadata accepts,
        IFromBodyMetadata fromBodyMeta)
    {
        // GET/HEAD typically have no body
        if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            return null;

        // Minimal APIs can signal body via IFromBodyMetadata
        if (fromBodyMeta != null && fromBodyMeta.AllowEmpty) 
            return null;

        // Prefer a controller action's [FromBody] parameter if present
        var bodyParam = action?.Parameters?
            .OfType<ControllerParameterDescriptor>()
            .FirstOrDefault(p =>
                p.ParameterInfo.GetCustomAttribute<FromBodyAttribute>() != null ||
                (p.ParameterInfo.GetCustomAttribute<FromQueryAttribute>() == null &&
                 p.ParameterInfo.GetCustomAttribute<FromRouteAttribute>() == null &&
                 IsComplexType(p.ParameterType)));

        if (bodyParam == null) 
            return null;

        var sample = CreateSampleObject(bodyParam.ParameterType, maxDepth: 2);

        return new ToolProperty 
        { 
            Type = "object", 
            Description = JsonSerializer.Serialize(sample, new JsonSerializerOptions { WriteIndented = true }) 
        };
    }

    private static bool IsComplexType(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        if (t.IsPrimitive) return false;
        if (t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) ||
            t == typeof(Guid) || t == typeof(DateTimeOffset) || t == typeof(TimeSpan))
            return false;
        return true;
    }

    private static object CreateSampleObject(Type type, int maxDepth, int depth = 0)
    {
        if (depth >= maxDepth) return GetLeafSample(type);

        // Arrays
        if (type.IsArray)
        {
            var elem = type.GetElementType()!;
            var arr = Array.CreateInstance(elem, 1);
            arr.SetValue(CreateSampleObject(elem, maxDepth, depth + 1), 0);
            return arr;
        }

        // IEnumerable<T>
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var elem = type.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(elem);
            var list = (System.Collections.IList)Activator.CreateInstance(listType)!;
            list.Add(CreateSampleObject(elem, maxDepth, depth + 1));
            return list;
        }

        var leaf = GetLeafSample(type);
        if (leaf != null) return leaf;

        var obj = Activator.CreateInstance(type);
        if (obj == null) return null;

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanWrite) continue;
            try
            {
                var value = CreateSampleObject(prop.PropertyType, maxDepth, depth + 1);
                prop.SetValue(obj, value);
            }
            catch { /* ignore */ }
        }

        return obj;
    }

    private static object GetLeafSample(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;

        if (t == typeof(string)) return "string";
        if (t == typeof(int)) return 0;
        if (t == typeof(long)) return 0L;
        if (t == typeof(short)) return (short)0;
        if (t == typeof(byte)) return (byte)0;
        if (t == typeof(bool)) return false;
        if (t == typeof(decimal)) return 0.0m;
        if (t == typeof(double)) return 0.0d;
        if (t == typeof(float)) return 0.0f;
        if (t == typeof(Guid)) return Guid.Empty;
        if (t == typeof(DateTime)) return DateTime.UtcNow;
        if (t == typeof(DateTimeOffset)) return DateTimeOffset.UtcNow;
        if (t == typeof(TimeSpan)) return TimeSpan.Zero;
        return null;
    }
}