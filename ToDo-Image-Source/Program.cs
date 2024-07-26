class Program
{
  static async Task Main()
  {
  HttpClient client = new HttpClient();
  var timer = new PeriodicTimer(TimeSpan.FromSeconds(60*60));

  //initial image load
  await LoadImage(client);

  while (await timer.WaitForNextTickAsync())
  {
    try
    {
      await LoadImage(client);
    }
    catch(Exception e)
    {
      Console.WriteLine("Exception: " + e.Message);
    }
  }

  }
  static async Task LoadImage(HttpClient client)
  {
    var response = await client.GetAsync("https://picsum.photos/1200");

    System.Net.Http.HttpContent content = response.Content;

    using (var file = System.IO.File.Create("/usr/src/app/files/image.png"))
    {
      var contentStream = await content.ReadAsStreamAsync();
      await contentStream.CopyToAsync(file);
    }
  }
}




