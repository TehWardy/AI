using TehWardy.AI.Providers;

namespace TehWardy.AI.Tools;

public static class AIProviderConfigurationExtensions
{
    public static void WithTools(
        this AIProviderConfiguration aiConfig,
        Action<ToolConfiguration> toolConfigurationAction)
    {
        var toolConfig = new ToolConfiguration(aiConfig.ServiceCollection);
        toolConfigurationAction(toolConfig);
    }
}