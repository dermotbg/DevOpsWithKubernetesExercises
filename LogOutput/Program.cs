var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";


builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.UseStaticFiles();

string prevStamp = "";
string prevPong = "";
string prevOutput = "";

app.MapGet("/", () => 
{
  try
  {
    StreamReader sr = new StreamReader("/usr/src/app/files/stamp.txt");
    StreamReader sr2 = new StreamReader("/usr/src/app/files/pong.txt");
  
    string? stamp = sr.ReadLine();
    string? pong = sr2.ReadLine();

    // Only generate new Guid if data in text file has changed. 
    if(prevOutput is not null && stamp == prevStamp && pong == prevPong)
    {
      return $"{prevOutput}";
    }

    if(stamp is null || pong is null)
    {
      return "Generating data...";
    }

    Console.WriteLine(stamp);

    string output = $"{stamp}\n{pong}";

    // store output to memory for requests within the 5 second block. 
    prevStamp = stamp;
    prevPong = pong;
    prevOutput = output;
    
    return output;
  }
  catch (System.Exception e)
  {
    Console.WriteLine("An error occurred" + e.Message);
    return "File is empty or does not exist";
  }

});

app.Run();