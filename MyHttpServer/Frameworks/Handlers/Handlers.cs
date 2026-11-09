using System.Net;

public abstract class Handler
{
    public abstract Task HandleAsync(HttpListenerContext context);
}