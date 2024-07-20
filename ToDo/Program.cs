var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://localhost:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.UseStaticFiles();

app.MapGet("/", () => 
{
  return Results.File("index.html", "text/html");
});

app.Run();