using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class AsyncAwait
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read web using Async Await ");
            
            StartTask();
            Console.WriteLine("Reading from http://localhost:9240/Slow.ashx");
            Console.WriteLine("Return");


            Console.Read();

        }


        // Async method can return 3 types only
        // 1. void
        // 2. Task
        // 3. Task<T> can return T ie. Task<int> can return int
        // All the statements after the await keyword will execute when the thread finished its job.
        private static async void StartTask()
        {
            string content =  await getContentAsync();
            Console.WriteLine(content);

        }

        // WebClient is automatically disposed if async-await mathod is used
        private static async Task<string> getContentAsync()
        {
            using (var wc = new WebClient())
            {
                string content = await wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");
                Console.WriteLine("Got content from http://localhost:9240/Slow.ashx");
                return content;

            }
            
        }
    }
}
