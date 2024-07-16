var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

while (await timer.WaitForNextTickAsync())
{
  Console.WriteLine($"{DateTime.Now}: {Guid.NewGuid()}");
}

