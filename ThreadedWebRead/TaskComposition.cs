using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class TaskComposition
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read web using Task<T>");
            
            var wc = new WebClient();
            var wc2 = new WebClient();
            
            // Task Composition
            Task<string> tsk = wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");  // task succeed
            Task<string> tsk2 = wc2.DownloadStringTaskAsync("http://localhost:9240/Faulty.ashx");  // task failed

            // Compositing two tasks.
            // WhenAll() is available in .NET 4.5 
            Task<string[]> compTsk = Task.WhenAll(tsk, tsk2);
            
            // Wait for all tasks to complete.
            compTsk.ContinueWith(t =>
            {
                // If one task is failed - the compsition result status will fail.
                // No way to get individual task results seperately.
                if (t.IsFaulted)
                {
                    Console.WriteLine(t.Exception);
                }
                else
                {
                    Console.WriteLine(t.Result);
                }
            });
            Console.WriteLine(@"Reading the web http://localhost:9240/Slow.ashx ...");
            Console.WriteLine(@"Reading the web http://localhost:9240/Faulty.ashx ...");

            Console.Read();

        }
    }
}
