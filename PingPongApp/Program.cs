using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";

builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();

app.MapGet("/", async () => {
  int pongs = await CounterService.GetCounterAsync();
  return Results.Content($"Pongs: {pongs}");
});

app.MapGet("/pingpong", async () => 
{
  int counter = 0;
  try
  {
    counter = await CounterService.IncrementCounterAsync();  
    // old file system
    // StreamWriter sw = new StreamWriter("/usr/src/app/files/pong.txt");
    // sw.WriteLine($"Pongs: {CounterService.GetCounter()}");
    // sw.Close();
  }
  catch(Exception e)
  {
    Console.WriteLine("Exception: " + e.Message);
  }
  return Results.Content($"Pongs: {counter}");
});

app.Run();

public static class CounterService
{
  private static int _counter;
  private static Boolean _needsInit = true;
  private static string _connectionString = "Host=postgres-svc;Port=5432;Username=ps_user;Password=SecurePassword;Database=ps_db";
  // private static string _connectionString = "postgres://ps_user:SecurePassword@postgres-svc:5432/ps_db";

  public static async Task<Boolean> InitAsync()
  {
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);

    await using (var initTableCmd = dataSource.CreateCommand(
    "CREATE TABLE IF NOT EXISTS pingpong (" +
    "id SERIAL PRIMARY KEY, " +
    "endpoint TEXT UNIQUE, " +
    "count INT DEFAULT 0" +
    "); " +
    "INSERT INTO pingpong (endpoint, count) VALUES ('pingpong', 0) ON CONFLICT DO NOTHING;"))
    {
        await initTableCmd.ExecuteNonQueryAsync();
    }
      return _needsInit = false;
  }
  public async static Task<int> GetCounterAsync()
  {
    if(_needsInit)
    {
      await InitAsync();
    }
    
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    
    await using (var cmd = dataSource.CreateCommand("SELECT count FROM pingpong WHERE endpoint = 'pingpong';"))
    
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
      if (await reader.ReadAsync())
      {
        _counter = reader.GetInt32(0);
      }
    }
    return _counter;
  }
  public async static Task<int> IncrementCounterAsync()
  {
    // int currentCount = await GetCounterAsync();
    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
    await using (var cmd = dataSource.CreateCommand($"UPDATE pingpong SET count = count + 1 WHERE endpoint = 'pingpong' RETURNING count;"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
      if (await reader.ReadAsync())
      {
        Console.WriteLine($"Updated Count: {reader.GetInt32(0)}");
        return reader.GetInt32(0);
      }
    }
    throw new Exception("No count found");
  }
}