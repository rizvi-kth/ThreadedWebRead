using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class SimpleTask
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read web using Task<T>");

            var _wc = new WebClient();

            // This is multithreaded programming. Threads created from Factory() method runs on a threadpool. 
            // This thread is mostly blocked waiting for a response from server. 
            // Inefficient because of the thread is blocked consuming resources from the threadpool.
            //Task<string> tsk1 = Task<string>.Factory.StartNew(() =>
            //{
            //    var _webContent = _wc.DownloadString("http://localhost:9240/Slow.ashx");
            //    return _webContent;
            //});

            // .NET 4.5 HAS A BETTER WAY TO READ STRING.
            Task<string> tsk1 = _wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");

            //Console.WriteLine(tsk1.Result);
            tsk1.ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
            });

            Console.WriteLine(@"Reading the web http://localhost:9240/Slow.ashx ...");
            

            Console.Read();

        }
    }
}
