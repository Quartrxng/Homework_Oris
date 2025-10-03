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

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://" + Settings.Domain + ":" + Settings.Port + "/");
            _listener.Start();
            Console.WriteLine($"http://{Settings.Domain}:{Settings.Port}/");
            Console.WriteLine("Сервер запущен");
            _ = ListenAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
            Console.WriteLine("Сервер остановлен");
        }

        private async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested) // Цикл вместо рекурсии
            {
                var context = await _listener.GetContextAsync(); // await вместо callback
                _ = ProcessRequest(context, token);
            }
        }

        private async Task ProcessRequest(HttpListenerContext context, CancellationToken token)
        {
            try 
            {
                // отправляемый в ответ код htmlвозвращает
                string responseText = System.IO.File.ReadAllText($"{Settings.PublicDirectoryPath}/index.html");

                var request = context.Request;
                var response = context.Response;

                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                // получаем поток ответа и пишем в него ответ
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                // отправляем данные
                await output.WriteAsync(buffer);
                await output.FlushAsync();

            Console.WriteLine("Запрос обработан");
                
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл index.html не найден");
            }
        }
    }
}