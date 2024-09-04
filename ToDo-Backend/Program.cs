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

app.MapPut("/todos/{id}", async (int id) =>
{
  try
  {
    await ToDoService.MarkAsDoneAsync(id);
    return Results.Ok();
  }
  catch (System.Exception e)
  {
    Console.WriteLine("An error occurred " + e.Message);
    throw new InvalidOperationException($"An error occurred  + {e.Message}");
  }
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
  private static List<TodoItem>? _todos = new List<TodoItem>{};
  private static Boolean _needsInit = true;

  public static async Task<Boolean> InitAsync()
  {
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
      // init DB if needed
      Console.WriteLine("Database Initialization needed: INIT STARTED");
      await using (var initTableCmd = dataSource.CreateCommand(
      "CREATE TABLE IF NOT EXISTS todos (" +
      "id SERIAL PRIMARY KEY, " +
      "todo TEXT," +
      "done BOOLEAN DEFAULT false" +
      "); "))
      {
          await initTableCmd.ExecuteNonQueryAsync();
          Console.WriteLine("INIT COMPLETE");
      }
      return _needsInit = false;
  }
    public class TodoItem 
  { 
    public int Id { get; set; } 
    public string Text { get; set; } = null!;
    public bool IsDone { get; set; }
  }
  
  public static async Task<List<TodoItem>> GetAsync()
  {
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    
    if(_needsInit)
    {
      await InitAsync();
    }

    List<TodoItem> todoList = new List<TodoItem>{};
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM todos;"))
    
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
      while (await reader.ReadAsync())
      {
        // todoList.Add(reader.GetString(0));
        int id = reader.GetInt32(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("todo"));
        bool isDone = reader.GetBoolean(reader.GetOrdinal("done"));

        Console.WriteLine($"Added to TODO list: ID:{id}-TEXT:{text}-isDone:{isDone}");

        TodoItem currentTodo = new TodoItem
        {
          Id = id,
          Text = text,
          IsDone = isDone
        };

        todoList.Add(currentTodo);
      }
    }

    _todos = todoList;
    return todoList;
  }
  public static async Task<IResult> MarkAsDoneAsync(int id)
  {
    if(id <= 0)
    {
      Console.WriteLine($"Update Rejected: ID: {id} was less than 0");
      return Results.BadRequest("Update Rejected: ID was less than 0");
    }
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    await using (var cmd = dataSource.CreateCommand($"UPDATE todos SET done = NOT done WHERE id = {id};"))
    {
      await cmd.ExecuteNonQueryAsync();
      Console.WriteLine($"Todo with ID: ${id} has been updated.");
    }
    return Results.Ok();
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
    }
    
    await GetAsync();

    return Results.Ok(_todos);
  }
}