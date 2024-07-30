using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3001";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();

app.MapGet("/todos", () => 
{
  return Results.Json(ToDoService.Get());
});

app.MapPost("/todos", async (HttpRequest request) => 
{
  using StreamReader reader = new StreamReader(request.Body, System.Text.Encoding.UTF8);
  var newTodo = await reader.ReadToEndAsync();
  
  if(newTodo is null)
  {
    throw new InvalidOperationException("New todo must have a value");
  }

  try
  {
    ToDoService.Add(newTodo.ToString());
  }
  catch (System.Exception e)
  {
    Console.WriteLine("An error occurred " + e.Message);
    throw new InvalidOperationException($"An error occurred  + {e.Message}");
  }

  return Results.Ok();
});

if(app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.Run();


public static class ToDoService
{
  private static List<string>? _todos = new List<string>{"This is the first todo"};
  public static  List<string> Get()
  {
    return _todos;
  }
  public static List<string> Add(string newTodo)
  {
    _todos.Add(newTodo);
    return _todos;
  }
}