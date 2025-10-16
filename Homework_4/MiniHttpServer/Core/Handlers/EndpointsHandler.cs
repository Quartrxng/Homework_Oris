using MiniHttpServer.Core.Abstracts;
using MiniHttpServer.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;


using System.Text;
using System.Threading.Tasks;
namespace MiniHttpServer.Core.Handlers
{
    class EndpointsHandler : Handler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            // некоторая обработка запроса

            if (true)
            {
                var request = context.Request;
                var endpointName = request.Url.AbsolutePath.Split('/')[^2];

                var assembly = Assembly.GetExecutingAssembly();
                var endpoint = assembly.GetTypes()
                                        .Where(t => t.GetCustomAttribute<EndpointAttribute>() != null)
                                        .FirstOrDefault(end => IsCheckedNameEndpoint(end.Name, endpointName));
                if (endpoint == null) return;
                var method = endpoint.GetMethods().Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name.Equals($"Http{context.Request.HttpMethod}", StringComparison.OrdinalIgnoreCase)))
                    .FirstOrDefault();

                if (method == null) return;

                var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                var body = reader.ReadToEnd();

                var postParams = new Dictionary<string, string>();
                foreach (var pair in body.Split('&'))
                {
                    var kv = pair.Split('=');
                    postParams[WebUtility.UrlDecode(kv[0])] = WebUtility.UrlDecode(kv[1]);
                }

                var parameters = method.GetParameters()
                                         .Select(p => postParams.ContainsKey(p.Name) ? postParams[p.Name] : null)
                                         .ToArray();

                var ret = method.Invoke(Activator.CreateInstance(endpoint), parameters);

            }
            // передача запроса дальше по цепи при наличии в ней обработчиков
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }
        private bool IsCheckedNameEndpoint(string endpointName, string className)=>
            endpointName.Equals(className, StringComparison.OrdinalIgnoreCase) 
            || endpointName.Equals($"{className}EndPoint", StringComparison.OrdinalIgnoreCase);
    }
}
