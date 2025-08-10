namespace AIServer.MCPTools.IG;

public static class IServiceCollectionExtensions
{
    public static void AddIGMCPTools(this IServiceCollection services)
    {
        services.AddSingleton(ctx =>
        {
            var config = ctx.GetService<IConfiguration>();

            var result = new IGClient(
                config.GetValue<string>("IG.Host"),
                config.GetValue<string>("IG.ApiKey"));

            result.AuthenticateAsync(
                config.GetValue<string>("IG.Username"),
                config.GetValue<string>("IG.Password"))
                    .Wait();

            return result;
        });
    }
}