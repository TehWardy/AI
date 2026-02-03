using System.Text.Json.Serialization;

namespace TehWardy.AI.Tools.Standard.Models;

public sealed class ArchitectureSpec
{
    public string Name { get; set; }
    public string Version { get; set; }
    public ArchitecturePolicies Policies { get; set; }
    public IList<ComponentSpec> Components { get; set; }
    public IList<ModelSpec> Models { get; set; }
    public IList<ExternalResourceSpec> ExternalResources { get; set; }
    public IList<DependencySpec> Dependencies { get; set; }
}

public sealed class ArchitecturePolicies
{
    public NamingPolicy Naming { get; set; }
    public ErrorHandlingPolicy ErrorHandling { get; set; }
    public LoggingPolicy Logging { get; set; }
    public LayeringPolicy Layering { get; set; }
}

public sealed class NamingPolicy
{
    public string RootNamespace { get; set; }
    public string SolutionName { get; set; }
    public string ExposureProjectSuffix { get; set; }
    public string ServicesProjectSuffix { get; set; }
    public string BrokersProjectSuffix { get; set; }
    public string ModelsProjectSuffix { get; set; }
    public string InterfacePrefix { get; set; }
    public string AsyncMethodSuffix { get; set; }
}

public sealed class ErrorHandlingPolicy
{
    public bool BrokersDeclareThrows { get; set; }
    public bool ExposureMapsExceptions { get; set; }
}

public sealed class LoggingPolicy
{
    public bool BrokersLog { get; set; }
    public bool ServicesLog { get; set; }
    public bool ExposureLogs { get; set; }
    public string LoggerAbstractionType { get; set; }
}

public sealed class LayeringPolicy
{
    public bool EnforceStrictLayering { get; set; }
    public bool AllowServiceToServiceDependencies { get; set; }
    public bool AllowExposureToExposureDependencies { get; set; }
    public bool AllowBrokerToBrokerDependencies { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ComponentLayer
{
    Exposure,
    Service,
    Broker
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExposureKind
{
    Api,
    Mvc,
    MinimalApi,
    Worker,
    Grpc
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServiceKind
{
    Foundation,
    Processing,
    Integration,
    Orchestration,
    Domain
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ExposureSpec), "exposure")]
[JsonDerivedType(typeof(ServiceSpec), "service")]
[JsonDerivedType(typeof(BrokerSpec), "broker")]
public abstract class ComponentSpec
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ComponentLayer Layer { get; set; }
    public string Namespace { get; set; }
    public string Project { get; set; }
    public IList<MethodSpec> Methods { get; set; }
    public IDictionary<string, string> Tags { get; set; }
}

public sealed class ExposureSpec : ComponentSpec
{
    public ExposureKind Kind { get; set; }
    public IList<ExposureEndpointSpec> Endpoints { get; set; }
}

public sealed class ServiceSpec : ComponentSpec
{
    public ServiceKind Kind { get; set; }
}

public sealed class BrokerSpec : ComponentSpec
{
    public string ExternalResourceId { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AsyncKind
{
    Sync,
    Task,
    ValueTask
}

public sealed class MethodSpec
{
    public string Name { get; set; }
    public AsyncKind Async { get; set; }
    public TypeRef Output { get; set; }
    public IList<ParameterSpec> Inputs { get; set; }
    public IList<TypeRef> Throws { get; set; }
    public IList<PolicySpec> Policies { get; set; }
}

public sealed class ParameterSpec
{
    public string Name { get; set; }
    public TypeRef Type { get; set; }
    public bool Optional { get; set; }
    public string DefaultValueExpression { get; set; }
}

public sealed class TypeRef
{
    public string Name { get; set; }
    public bool IsArray { get; set; }
    public IList<TypeRef> GenericArguments { get; set; }
    public bool Nullable { get; set; }

    public override string ToString()
    {
        var core = Name;

        if (GenericArguments != null && GenericArguments.Count > 0)
        {
            core += $"<{string.Join(", ", GenericArguments.Select(a => a.ToString()))}>";
        }

        if (IsArray)
        {
            core += "[]";
        }

        if (Nullable)
        {
            core += "?";
        }

        return core;
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DependencyKind
{
    InProcess,
    ExternalBoundary
}

public sealed class DependencySpec
{
    public string FromComponentId { get; set; }
    public string ToComponentId { get; set; }
    public DependencyKind Kind { get; set; }
    public IList<MethodCallMapSpec> CallMap { get; set; }
    public IDictionary<string, string> Tags { get; set; }
}

public sealed class MethodCallMapSpec
{
    public string FromMethodName { get; set; }
    public string ToMethodName { get; set; }
    public IList<ParameterMapSpec> ParameterMap { get; set; }
}

public sealed class ParameterMapSpec
{
    public string FromParameterName { get; set; }
    public string ToParameterName { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpVerb
{
    Get,
    Post,
    Put,
    Patch,
    Delete
}

public sealed class ExposureEndpointSpec
{
    public string MethodName { get; set; }
    public HttpVerb Verb { get; set; }
    public string Route { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModelKind
{
    Record,
    Class,
    Enum
}

public sealed class ModelSpec
{
    public string Name { get; set; }
    public ModelKind Kind { get; set; }
    public string Namespace { get; set; }
    public string Project { get; set; }
    public IList<ModelPropertySpec> Properties { get; set; }
    public IList<EnumMemberSpec> Members { get; set; }
    public IDictionary<string, string> Tags { get; set; }
}

public sealed class ModelPropertySpec
{
    public string Name { get; set; }
    public TypeRef Type { get; set; }
    public bool Required { get; set; }
    public string Summary { get; set; }
}

public sealed class EnumMemberSpec
{
    public string Name { get; set; }
    public int Value { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExternalResourceType
{
    HttpApi,
    Database,
    FileSystem,
    Queue,
    Cache,
    MemoryStore,
    SearchIndex,
    Other
}

public sealed class ExternalResourceSpec
{
    public ExternalResourceType Type { get; set; }
    public string Name { get; set; }
    public string ClientType { get; set; }
    public IDictionary<string, string> Configuration { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolicyKind
{
    Retry,
    Timeout,
    Caching,
    CircuitBreaker,
    RateLimit,
    Validation,
    Authorization,
    Mapping,
    Other
}

public sealed class PolicySpec
{
    public PolicyKind Kind { get; set; }
    public string Name { get; set; }
    public IDictionary<string, string> Settings { get; set; }
}
