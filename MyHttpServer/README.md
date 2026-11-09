# C# Micro Framework (Custom Web Server)

Собственный веб-сервер и микро-фреймворк, написанный с нуля на C# (.NET) без использования ASP.NET Core. Проект создан для глубокого понимания того, как работают веб-серверы, HTTP-протоколы и маршрутизация под капотом.

## Реализованный функционал
* **Сетевой уровень:** Собственный HTTP-сервер на базе `HttpListener`. Асинхронная обработка запросов (параллельное принятие соединений).
* **Роутинг (Маршрутизация):** Собственный роутер на основе рефлексии (Reflection) и кастомных атрибутов (`[HttpController]`, `[Get]`, `[Post]`).
* **Обработка данных:** Поддержка Query-параметров (`[FromQuery]`) и тел POST-запросов (форм) (`[FormData]`).
* **Ответы:** Единый интерфейс `IHttpResponseType` для возврата HTML (`HtmlResult`) и JSON (`JsonResult`).
* **Статика:** Раздача статических файлов (HTML, CSS, изображения) с корректным определением `Content-Type`.
* **Дополнительно:** Контроллер для отправки Email с вложением ZIP-архива проекта (через `SmtpClient`).

## Стек технологий
* C# / .NET 8+
* System.Net (`HttpListener`)
* System.Reflection (Рефлексия)
* System.Text.Json
* System.Net.Mail (SMTP)

## Структура проекта
* `Program.cs` — точка входа, инициализация сервера и настроек.
* `HttpServer.cs` — сетевой уровень, прослушивание портов, распределение запросов.
* `frameworks/Handlers/` — обработчики (роутер `ControllerHandler` и статика `StaticFileHandler`).
* `frameworks/Attributes/` — кастомные атрибуты для маршрутизации.
* `frameworks/HttpResponses/` — классы ответов (HTML/JSON).
* `Controllers/` — контроллеры с бизнес-логикой (`AuthController`, `HomeworkController`).

## Как запустить
1. Клонируйте репозиторий.
2. Откройте файл решения (`.sln`) в Visual Studio.
3. В файле `settings.json` укажите свои данные для отправки почты (если хотите проверить контроллер `Homework`).
4. Запустите проект (F5).

## Доступные маршруты (Endpoints)
После запуска сервера доступны следующие адреса:

* **Форма авторизации (HTML):** `http://127.0.0.1:8888/connection/auth`
* **Статический сайт Figma:** `http://127.0.0.1:8888/connection/FigmaSite/index.html`
* **Статический сайт Satis:** `http://127.0.0.1:8888/connection/SatisSite/index.html`
* **Отправка Email с ZIP-архивом проекта (GET):** `http://127.0.0.1:8888/connection/home-work/send?to=your_email@example.com`