using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedWebRead
{
    class TaskFail
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read web using Task<T>");
            
            var _wc = new WebClient();

            

            // ERROR HANDLING.
            Task<string> tsk2 = _wc.DownloadStringTaskAsync("http://localhost:9240/Faulty.ashx");

            //Console.WriteLine(tsk1.Result);
            tsk2.ContinueWith(t =>
            {

                //Exception will be hidden with other statements without t.Result
                Console.WriteLine(@"Didn't ran into exception even though the link returned 404.");

                ////Exception can be revailed with t.IsFaulted or use
                //if (t.IsFaulted)
                //{
                //    Console.WriteLine(t.Exception);
                //}
                //else
                //{
                //    Console.WriteLine(t.Result);
                //}
            });
            Console.WriteLine(@"Reading the web http://localhost:9240/Faulty.ashx ...");
            
            Console.Read();

        }
    }
}
