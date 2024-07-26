using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "files")),
});

app.MapGet("/", () => 
{
  return Results.File("index.html", "text/html");
});

app.Run();