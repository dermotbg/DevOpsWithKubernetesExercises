var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
while (await timer.WaitForNextTickAsync())
{
  try
  {
    StreamWriter sw = new StreamWriter("/usr/src/app/files/stamp.txt");

    sw.WriteLine($"{DateTime.Now}");

    sw.Close();
  }
  catch(Exception e)
  {
    Console.WriteLine("Exception: " + e.Message);
  }
}
  