using System.Text;
using System.Text.Json;

class JsonResult : IHttpResponseType
{
    private readonly Object data;

    public JsonResult(Object data)
    {
        this.data = data;
    }

    public string ContentType => "application/json; charset=utf-8";

    public byte[] GetBytes()
    {
        string json = JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }
}