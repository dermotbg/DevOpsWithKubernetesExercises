var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// set port number with export PORT=<PORTNUMBER> command
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "3000";


builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");

Console.WriteLine($"Server running on PORT: {PORT}");

var app = builder.Build();
app.UseStaticFiles();

string prevLine = "";
string prevOutput = "";

app.MapGet("/", () => 
{
  var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

  // StreamReader sr = new StreamReader("/usr/src/app/files/stamp.txt");
  StreamReader sr = new StreamReader("/home/dermot/Desktop/repos/DevOpsWithKubernetesExercises/LogTimeStamp/output/test.txt");
  
  string? line = sr.ReadLine();

  // Only generate new Guid if data in text file has changed. 
  if(prevOutput is not null && line == prevLine)
  {
    return prevOutput;
  }

  if(line is null)
  {
    return "File is empty or does not exist";
  }

  Console.WriteLine(line);

  string output = $"{line}: {Guid.NewGuid()}";

  // store output to memory for requests within the 5 second block. 
  prevLine = line;
  prevOutput = output;
  
  return output;

});

app.Run();