using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
  private readonly HttpClient _client;
  public IndexModel(HttpClient client)
  {
    _client = client;
  }
  public List<string>? Todos { get; set; }

  public async Task OnGetAsync()
  {
    var response = await _client.GetAsync("http://todo-backend-svc:2345/todos");
    // var response = await _client.GetAsync("http://localhost:3001/todos");
    response.EnsureSuccessStatusCode();
    Todos = await response.Content.ReadFromJsonAsync<List<string>>();
    Console.WriteLine(Todos);
  }
}