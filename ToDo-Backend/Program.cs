using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();

// ENDPOINTS
app.MapGet("/healthz", () => {
  return Results.Ok();
});

app.MapGet("/dbhealth", async () => {
  try
    {  
      await using var dataSource = NpgsqlDataSource.Create("Host=todo-postgres-svc;Port=5432;Username=ps_user;Password=SecurePassword;Database=ps_db");
      await using (var healthCheckCmd = dataSource.CreateCommand("SELECT 1"))
      {
            await healthCheckCmd.ExecuteNonQueryAsync();
      }
      return Results.Ok();
    }
    catch (Exception e)
    {
      Console.WriteLine($"Database connection failed: {e.Message}");
      return Results.StatusCode(500);
    }
});

app.MapGet("/todos", async () => 
{
  var todos = await ToDoService.GetAsync();
  return Results.Json(todos);
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
    await ToDoService.AddAsync(newTodo.ToString());
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
  private static string _connectionString = "Host=todo-postgres-svc;Port=5432;Username=ps_user;Password=SecurePassword;Database=ps_db";
  // postgres://ps_user:SecurePassword@todo-postgres-svc:5432/ps_db
  private static List<string>? _todos = new List<string>{};
  private static Boolean _needsInit = true;

  public static async Task<Boolean> InitAsync()
  {
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
      // init DB if needed
      Console.WriteLine("Database Initialization needed: INIT STARTED");
      await using (var initTableCmd = dataSource.CreateCommand(
      "CREATE TABLE IF NOT EXISTS todos (" +
      "id SERIAL PRIMARY KEY, " +
      "todo TEXT" +
      "); "))
      {
          await initTableCmd.ExecuteNonQueryAsync();
          Console.WriteLine("INIT COMPLETE");
      }
      return _needsInit = false;
  }
  
  public static async Task<List<string>> GetAsync()
  {
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    
    if(_needsInit)
    {
      await InitAsync();
    }

    List<string> todoList = new List<string>{};
    await using (var cmd = dataSource.CreateCommand("SELECT todo FROM todos;"))
    
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
      while (await reader.ReadAsync())
      {
        todoList.Add(reader.GetString(0));
      }
    }

    _todos = todoList;
    return todoList;
  }
  public static async Task<IResult> AddAsync(string newTodo)
  {
    if(String.IsNullOrEmpty(newTodo))
    {
      Console.WriteLine("Todo Rejected: String is empty");
      return Results.BadRequest("Todo cannot be empty!");
    }

    if(newTodo.Length > 140)
    {
      Console.WriteLine("Todo Rejected: String is over 140 chars");
      return Results.BadRequest("Todo must be under 140 characters.");
    }

    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    
    await using (var cmd = dataSource.CreateCommand($"INSERT INTO todos (todo) VALUES ('{newTodo}');"))
    {
      await cmd.ExecuteNonQueryAsync();
      Console.WriteLine("Todo Accepted:" + newTodo);
      _todos?.Add(newTodo);
    }
    
    await GetAsync();

    return Results.Ok(_todos);
  }
}