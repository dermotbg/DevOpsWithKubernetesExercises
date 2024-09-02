using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
// var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";
builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

builder.Services.AddRazorPages(options => 
{
  options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

Console.WriteLine($"Server running on PORT: {PORT}");

builder.Services.AddHttpClient();

builder.Services.AddRazorPages();

var app = builder.Build();


app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "files")),
});

app.UseRouting();

app.MapRazorPages();

HttpClient client = new HttpClient();

app.MapGet("/healthz", () => {
  return Results.Ok();
});

app.MapGet("/backend-health", async () => {
  try
  {
    var response = await client.GetAsync("http://todo-backend-svc:2345/healthz");
    response.EnsureSuccessStatusCode();
    return Results.Ok();
  }
  catch (Exception e)
  {
    Console.WriteLine($"Connection to backend failed: {e.Message}");
    return Results.StatusCode(500);
  }
});

app.Run();