var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT =  Environment.GetEnvironmentVariable("PORT") ?? config["Kestrel:Endpoints:Http:Url"];
Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.Run();
