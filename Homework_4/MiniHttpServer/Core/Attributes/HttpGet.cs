using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class HttpGet:Attribute
    {
        public string? Route {  get; set; } 
        public HttpGet(string route) 
        { 
            Route = route;
        }

        public HttpGet()
        {
        }
    }
}
