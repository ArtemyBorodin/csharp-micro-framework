using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class StaticFileHandler : Handler
{
    private readonly string rootFolder;

    public StaticFileHandler(string rootFolder)
    {
        this.rootFolder = rootFolder;
    }

    public override async Task HandleAsync(HttpListenerContext context)
    {
        string urlPath = context.Request.Url.AbsolutePath;

        if (urlPath == "/" || urlPath == "/connection/")
        {
            urlPath = "/index.html";
        }

        string relativePath = urlPath.TrimStart('/');

        if (relativePath.StartsWith("connection/"))
        {
            relativePath = relativePath.Substring("connection/".Length);
        }

        string filePath = Path.Combine(rootFolder, relativePath);

        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            byte[] notFoundBytes = Encoding.UTF8.GetBytes("<h1>404 Not Found</h1>");
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = notFoundBytes.Length;

            using Stream output = context.Response.OutputStream;
            await output.WriteAsync(notFoundBytes);
            return;
        }

        string contentType = GetContentType(filePath);
        context.Response.ContentType = contentType;

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        context.Response.ContentLength64 = fileBytes.Length;

        using Stream outputStream = context.Response.OutputStream;
        await outputStream.WriteAsync(fileBytes);
    }

    private string GetContentType(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        return ext switch
        {
            ".html" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream" // по умолчанию (неизвестный файл) 
        };
    }
        
    



}