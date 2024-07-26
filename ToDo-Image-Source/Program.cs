HttpClient client = new HttpClient();
var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

while (await timer.WaitForNextTickAsync())
{
  try
  {
    var response = await client.GetAsync("https://picsum.photos/200");

    System.Net.Http.HttpContent content = response.Content;

    DirectoryInfo di = Directory.CreateDirectory("test-images");
    
    using (var file = System.IO.File.Create("test-images/image.png"))
    {
      var contentStream = await content.ReadAsStreamAsync();
      await contentStream.CopyToAsync(file);
    }
  }
  catch(Exception e)
  {
    Console.WriteLine("Exception: " + e.Message);
  }
}

  