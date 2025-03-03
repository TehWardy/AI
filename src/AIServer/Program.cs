using AIServer.AI;
using AIServer.IG;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

AIModelHost aiHost = new(config.GetValue<string>("AI.Model"));
aiHost.Start();

var builder = WebApplication
    .CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(ctx =>
{
    var config = ctx.GetService<IConfiguration>();

    return new AIChatClient(
        config.GetValue<string>("AI.ServerUrl"), 
        config.GetValue<string>("AI.Model"));
});

builder.Services.AddSingleton(ctx =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();