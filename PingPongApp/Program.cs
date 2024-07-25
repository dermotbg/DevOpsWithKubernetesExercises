var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();

app.MapGet("/", () => 
{
  return Results.Content($"Pong {CounterService.IncrementCounter()}");
  
});

app.Run();

public static class CounterService
{
  private static int _counter = 0;
  public static int IncrementCounter()
  {
    return _counter++;
  }
}