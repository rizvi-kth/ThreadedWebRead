using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SimpleWeb
{
    /// <summary>
    /// Summary description for Faulty
    /// </summary>
    public class Faulty : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //Thread.Sleep(5000);
            //context.Response.ContentType = "text/plain";
            context.Response.StatusCode = 404;
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