using System.Text.Json;

class Program
{
    static async Task Main()
    {
        string jsonText = File.ReadAllText("settings.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var settings = JsonSerializer.Deserialize<Settings>(jsonText, options);

        App.Settings = settings;    

        HttpServer server = new HttpServer(settings.Prefixes);

        Task serverTask = server.StartServer();

        while (true)
        {
            Console.Write("Введите команду: ");
            string input = Console.ReadLine();

            if (input == "stop")
            {
                server.StopServer();
                await serverTask;
                break;
            }
        }
    }
}

