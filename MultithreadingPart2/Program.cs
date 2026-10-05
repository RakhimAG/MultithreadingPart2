using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

int processedOrders = 0;
SemaphoreSlim semaphoreSlim = new SemaphoreSlim(2,2);
ManualResetEvent manualResetEvent = new ManualResetEvent(false);

async Task Process100Orders()
{
    await semaphoreSlim.WaitAsync();
    try
    {
        Thread.Sleep(1000);
        for (int i = 0; i < 100; i++)
        {
            Interlocked.Increment(ref processedOrders);
        }
    }
    finally { semaphoreSlim.Release(); }
}

Task[] tasks = new Task[5];

for (int i = 0; i < 5; i++)
{
    tasks[i] = Task.Run(Process100Orders);
    manualResetEvent.Set();
}

await Task.WhenAll(tasks);

var newThread = new Thread
(
    () =>
    {
        manualResetEvent.WaitOne();
        Console.WriteLine("All orders have been processed!");
    }
);
newThread.Start();

Console.WriteLine(processedOrders);

