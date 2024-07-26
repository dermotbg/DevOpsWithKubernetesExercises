var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();

app.MapGet("/pingpong", () => 
{
  try
  {
    StreamWriter sw = new StreamWriter("/usr/src/app/files/pong.txt");

    CounterService.IncrementCounter();

    sw.WriteLine($"Pongs: {CounterService.GetCounter()}");

    sw.Close();
  }
  catch(Exception e)
  {
    Console.WriteLine("Exception: " + e.Message);
  }
  return Results.Content($"Pongs: {CounterService.GetCounter()}");
});

app.Run();

public static class CounterService
{
  private static int _counter = 0;
  public static int GetCounter()
  {
    return _counter;
  }
  public static int IncrementCounter()
  {
    return _counter++;
  }
}