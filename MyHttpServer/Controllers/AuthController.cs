using System.Net;

[HttpController("auth")]
public class AuthController : ControllerBase
{
    [Get]
    public IHttpResponseType Index()
    {
        // Возвращаем HTML-код формы прямо из кода
        string html = @"
        <!DOCTYPE html>
        <html>
        <head><meta charset='utf-8'><title>Вход</title></head>
        <body>
            <h2>Вход в систему</h2>
            <form method='POST' action='/connection/auth'>
                <label>Логин: <input type='text' name='name'></label><br><br>
                <label>Пароль: <input type='password' name='password'></label><br><br>
                <button type='submit'>Войти</button>
            </form>
        </body>
        </html>";

        return new HtmlResult(html);
    }

    [Post]
    public IHttpResponseType Login([FormData] string name, [FormData] string password)
    {
        Console.WriteLine($"!!! Пришел POST: name={name}, password={password}");
        if (name == "admin" && password == "123")
        {
            return new JsonResult(new { status = "success", message = "Добро пожаловать, admin!" });
        }
        else
        {
            return new JsonResult(new { status = "error", message = "Неверный логин или пароль" });
        }
    }
}