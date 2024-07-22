var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.UseStaticFiles();

app.MapGet("/", async () => 
{
  var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

  string output = "Generating Hash...";

  Console.WriteLine(output);

  while (await timer.WaitForNextTickAsync())
  {
    output = $"{DateTime.Now}: {Guid.NewGuid()}";
    return output;
    // return Results.Content($"{DateTime.Now}: {Guid.NewGuid()}"); 
  }
  
  return output;

});

app.Run();