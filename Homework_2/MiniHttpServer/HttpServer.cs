using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace MiniHttpServer
{
    public class HttpServer
    {
        public int Port;
        private Link Settings;

        private HttpListener _listener = new ();

        private CancellationTokenSource _cts;

        public HttpServer(Link settings)
        {
            Settings = settings;
        }

        public void Start(ref bool keepAlive)
        {
            _cts = new CancellationTokenSource();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://" + Settings.Domain + ":" + Settings.Port + "/");
            keepAlive = true;
            _listener.Start();
            Console.WriteLine($"http://{Settings.Domain}:{Settings.Port}/");
            Console.WriteLine("Сервер запущен");

            _ = ListenAsync(_cts.Token);
        }

        public void Stop(ref bool keepRunning)
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
            Console.WriteLine("Сервер остановлен");
            keepRunning = false;
        }

        private async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                _ = ProcessRequest(context, token);
            }
        }

        private async Task ProcessRequest(HttpListenerContext context, CancellationToken token)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                string requestPath = request.Url.AbsolutePath.TrimStart('/');
                if (string.IsNullOrEmpty(requestPath))
                {
                    requestPath = "index.html";
                }

                string fullPath = Path.Combine(Settings.PublicDirectoryPath, requestPath);

                if (File.Exists(fullPath))
                {
                    Console.WriteLine("Запрос отработан");
                    byte[] fileBytes = await File.ReadAllBytesAsync(fullPath, token);
                    response.ContentLength64 = fileBytes.Length;

                    await response.OutputStream.WriteAsync(fileBytes, token);
                    await response.OutputStream.FlushAsync(token);
                }
                else
                {
                    response.StatusCode = 404;
                    string notFoundHtml = "<html><body><h1>404 - Not Found</h1><p>File not found: " + requestPath + "</p></body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(notFoundHtml);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, token);
                    await response.OutputStream.FlushAsync(token);

                    Console.WriteLine($"Файл не найден: {requestPath} (404)");
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки запроса: {ex.Message}");
                try
                {
                    context.Response.StatusCode = 500;
                    string errorHtml = "<html><body><h1>500 - Internal Server Error</h1></body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(errorHtml);
                    context.Response.ContentType = "text/html";
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer);
                    await context.Response.OutputStream.FlushAsync();
                }
                catch { }
            }
        }
    }
}