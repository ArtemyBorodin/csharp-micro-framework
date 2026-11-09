public interface IHttpResponseType
{
    string ContentType { get; }
    byte[] GetBytes();
}