using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SimpleWeb
{
    /// <summary>
    /// Summary description for Slow
    /// </summary>
    public class Slow : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Thread.Sleep(5000);
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}