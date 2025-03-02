using AIServer.AI;

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