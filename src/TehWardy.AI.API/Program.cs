using TehWardy.AI.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

AIFramework.Configure(builder, Data.UserConfiguration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

AIFramework.ServiceProvider = app.Services;

await AIFramework.CacheAgents(Data.Agents());
await AIFramework.CacheRunbooks(Data.Runbooks());

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();