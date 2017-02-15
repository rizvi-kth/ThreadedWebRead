using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class TaskComposition2
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
            // Wait for all tasks to complete.
            Task.Factory.ContinueWhenAll(new[] {tsk, tsk2}, 
                (Task<string>[] tsks) =>
                {
                    // If one task is failed - is tracked seperately.
                    // Get individual task results seperately (failure or success).
                    foreach (var t in tsks)
                    {
                        if (t.IsFaulted)
                        {
                            Console.WriteLine(t.Exception);
                        }
                        else
                        {
                            Console.WriteLine(t.Result);
                        }
                    }
                });


            Console.WriteLine(@"Reading the web http://localhost:9240/Slow.ashx ...");
            Console.WriteLine(@"Reading the web http://localhost:9240/Faulty.ashx ...");

            Console.Read();

        }
    }
}
