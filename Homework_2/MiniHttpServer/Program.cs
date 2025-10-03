using MiniHttpServer;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;


// установка адресов прослушки
Link settings = null;
string fs = null;

bool _keepRunning = true;

Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
{
    e.Cancel = true;
    _keepRunning = false;
};

try
{
    fs = System.IO.File.ReadAllText("settings.json");
    settings = JsonSerializer.Deserialize<Link>(fs);
}
catch (FileNotFoundException)
{
    Console.WriteLine("Файл settings.json не найден");
    System.Environment.Exit(101);
}
catch (JsonException)
{
    Console.WriteLine("Ошибка формата JSON");
    System.Environment.Exit(101);
}

HttpServer server = new HttpServer(settings);
server.Start();

while (_keepRunning) 
{
    var a = Console.ReadLine();
    if (a == "/start")
    {
        server.Start();
    }

    if (a == "/restart")
    {
        server.Stop();
        Thread.Sleep(500);
        server.Start();
    }

    if (a == "/stop")
    {
        server.Stop(ref _keepRunning);
        break;
    }

    if (a== "/off")
    {
        server.Stop();
    }
}
