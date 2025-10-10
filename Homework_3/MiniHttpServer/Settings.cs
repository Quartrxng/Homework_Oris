using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniHttpServer
{
    public sealed class Settings
    {
        private static Settings _instance;
        public static Settings Instance()
        {
            if (_instance == null)
                _instance = new Settings();
            return _instance;
        }

        public string PublicDirectoryPath { get; private set; }
        public string Domain { get; private set; }
        public string Port { get; private set; }

        private Settings()
        {
            try
            {
                var json = File.ReadAllText("settings.json");
                var settings = JsonSerializer.Deserialize<Settings>(json);
                PublicDirectoryPath = settings.PublicDirectoryPath;
                Domain = settings.Domain;
                Port = settings.Port;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл settings.json не найден");
                PublicDirectoryPath = "public/";
                Domain = "localhost";
                Port = "1337";
            }
            catch (JsonException)
            {
                Console.WriteLine("Ошибка формата JSON");
                PublicDirectoryPath = "public/";
                Domain = "localhost";
                Port = "1337";
            }
            catch
            {
                PublicDirectoryPath = "public/";
                Domain = "localhost";
                Port = "1337";
            }
        }
    }
}
