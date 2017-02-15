using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class AsyncAwaitError
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read web using Async Await ");
            
            StartTask();
            Console.WriteLine("Reading from http://localhost:9240/Faulty.ashx");
            Console.WriteLine("Return");


            Console.Read();

        }


        // Async method can have a try catch block and the exception will not be an aggrigate exception.
        // The last cause of the exception will be shown.
        private static async void StartTask()
        {
            try
            {
                string content = await getContentAsync();
                Console.WriteLine(content);

            }
            catch (Exception x)
            {
                Console.WriteLine("Exception in StartTask.");
                Console.WriteLine(x);                
            }

        }

        // WebClient is automatically disposed if async-await mathod is used
        private static async Task<string> getContentAsync()
        {
            using (var wc = new WebClient())
            {
                string content = await wc.DownloadStringTaskAsync("http://localhost:9240/Faulty.ashx");
                Console.WriteLine("Got content from http://localhost:9240/Faulty.ashx");
                return content;

            }
            
        }
    }
}
