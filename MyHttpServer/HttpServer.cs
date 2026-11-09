using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

class HttpServer
{
    private readonly HttpListener server = new();
    private readonly string[] prefixes;

    private readonly StaticFileHandler staticHandler;
    private readonly ControllerHandler controllerHandler;

    public HttpServer(string[] prefixes)
    {
        this.prefixes = prefixes;

        staticHandler = new StaticFileHandler("static");
        controllerHandler = new ControllerHandler();
    }

    public async Task StartServer()
    {
        foreach (var prefix in prefixes)
            server.Prefixes.Add(prefix);

        
        server.Start();

        Console.WriteLine("Сервер запущен. Ожидание запросов...");
        
        while (true)
        {
            HttpListenerContext context;

            try
            {
                context = await server.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    string url = context.Request.Url.AbsolutePath;
                    string method = context.Request.HttpMethod;

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {method} {url}");
                    

                    if (url.Contains("/auth") || url.Contains("/home-work"))
                    {
                        await controllerHandler.HandleAsync(context);
                    }
                    else
                    {   
                        await staticHandler.HandleAsync(context);
                    }
                }
                catch(Exception ex)
                {   
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Error]: {ex.Message}");
                    Console.ResetColor();

                }
            });
        }
    }

    public void StopServer()
    {
        server.Stop();
    }
}