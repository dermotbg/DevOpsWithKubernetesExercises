using dotenv.net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
  private readonly HttpClient _client;
  public IndexModel(HttpClient client)
  {
    _client = client;
  }
  public string BACKEND_URI {get; set;}
  public class TodoItem 
  { 
    public int Id { get; set; } 
    public string Text { get; set; } = null!;
    public bool IsDone { get; set; }
  }

  public List<TodoItem>? Todos { get; set; }

  public async Task OnGetAsync()
  {
    try
    {
      var envVars = DotEnv.Read();
      BACKEND_URI = envVars["BACKEND_URI"];
      var response = await _client.GetAsync("http://todo-backend-svc:2345/todos");
      response.EnsureSuccessStatusCode();
      Console.WriteLine($"HERE IS THE RESPONSE:   {await response.Content.ReadAsStringAsync()}");
      Todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
    }
    catch (System.Exception e)
    {
      Console.WriteLine("An error has occurred:" + e.Message);
      throw;
    }
    // var response = await _client.GetAsync("http://localhost:3001/todos");
  }
  public async Task<IActionResult> OnPostAsync(string todo)
  {
    if (String.IsNullOrWhiteSpace(todo))
    {
      Console.WriteLine($"An error has occurred: Todo is empty");
      return RedirectToPage("./Index");
    }
    var stringContent = new StringContent(todo, System.Text.Encoding.UTF8, "text/plain");
    var response = await _client.PostAsync("http://todo-backend-svc:2345/todos", stringContent);
    // var response = await _client.PostAsync("http://localhost:3001/todos", stringContent);
    response.EnsureSuccessStatusCode();
    await OnGetAsync();
    return RedirectToPage("./Index");
  }
}