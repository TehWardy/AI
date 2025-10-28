using AIServer.Ollama;
using Microsoft.AspNetCore.Mvc.Filters;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication
    .CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOllamaClient("E:\\AI\\Ollama\\Ollama.exe");
builder.Services.AddOllamaHost("http://localhost", 1234, "E:\\AI\\LLMs\\Ollama"); 
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseSwagger();
app.UseSwaggerUI();

await app.RunAsync();
