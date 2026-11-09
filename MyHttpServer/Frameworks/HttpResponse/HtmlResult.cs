using System.Text;

public class HtmlResult : IHttpResponseType
{
    private readonly string html;

    public HtmlResult(string html)
    {
        this.html = html;
    }

    public string ContentType => "text/html; charset=utf-8";

    public byte[] GetBytes()
    {
        return Encoding.UTF8.GetBytes(html);
    }
}