using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

public class ControllerHandler : Handler
{
    public override async Task HandleAsync(HttpListenerContext context)
    {
        string url = context.Request.Url.AbsolutePath.TrimStart('/');
        if (url.StartsWith("connection/"))
            url = url.Substring("connection/".Length);

        string[] parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        // StringSplitOptions.RemoveEmptyEntries настройка сплита - выкидывать пустые куски 

        if (parts.Length == 0)
        {
            await SendNotFound(context);
            return;
        }

        string controllerName = parts[0];
        string httpMethod = context.Request.HttpMethod;

        Type controllerType = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t => t.GetCustomAttribute<HttpControllerAttribute>()?.Name == controllerName);

        if (controllerType == null)
        {
            await SendNotFound(context);
            return;
        }

        // ищем метод внутри контроллера 
        MethodInfo targetMethod = controllerType.GetMethods()
            .FirstOrDefault(m =>
            {
                if (httpMethod == "GET") return m.GetCustomAttribute<GetAttribute>() != null;
                if (httpMethod == "POST") return m.GetCustomAttribute<PostAttribute>() != null;
                return false;
            });

        Console.WriteLine($"DEBUG: httpMethod={httpMethod}, controller={controllerName}");
        if (targetMethod == null) Console.WriteLine("DEBUG: Метод НЕ найден!");
        else Console.WriteLine($"DEBUG: Метод найден: {targetMethod.Name}");

        // Если метод не найден 404

        if (targetMethod == null)
        {
            await SendNotFound(context);
            return;
        }

        var controllerInstance = Activator.CreateInstance(controllerType);

        var parameters = targetMethod.GetParameters();
        object[] args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];

            if (p.GetCustomAttribute<FormDataAttribute>() != null && httpMethod == "POST")
            {
                using var reader = new System.IO.StreamReader(context.Request.InputStream);
                string body = await reader.ReadToEndAsync();

                var formData = ParseFormData(body);

                if (formData.ContainsKey(p.Name))
                {
                    args[i] = formData[p.Name];
                }
            }
            else if(p.GetCustomAttribute<FromQueryAttribute>() != null && httpMethod == "GET")
            {
                string value = context.Request.QueryString[p.Name];
                if (value != null)
                {
                    args[i] = value;
                }
            }
        }

        var result = targetMethod.Invoke(controllerInstance, args);

        if (result is IHttpResponseType responseType)
        {
            byte[] bytes = responseType.GetBytes();
            context.Response.ContentType = responseType.ContentType;
            context.Response.ContentLength64 = bytes.Length;
            using var output = context.Response.OutputStream;
            await output.WriteAsync(bytes);
        }
        else
        {
            await SendNotFound(context);
        }
    }

    private async Task SendNotFound(HttpListenerContext context)
    {
        context.Response.StatusCode = 404;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes("<h1>404 Not Found (Controller)</h1>");
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength64 = bytes.Length;
        using var output = context.Response.OutputStream;
        await output.WriteAsync(bytes);
    }

    private System.Collections.Generic.Dictionary<string, string> ParseFormData(string body)
    {
        var dict = new System.Collections.Generic.Dictionary<string, string>();
        if (string.IsNullOrEmpty(body)) return dict;

        var pairs = body.Split('&');
        foreach (var pair in pairs)
        {
            var kv = pair.Split('=');
            if (kv.Length == 2)
            {
                dict[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
            }
        }
        return dict;
    }
}
