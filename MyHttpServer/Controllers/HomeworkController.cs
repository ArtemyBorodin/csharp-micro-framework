using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.IO.Compression;
using System.Globalization;

[HttpController("home-work")]
public class HomeworkController : ControllerBase
{
    [Get]
    public IHttpResponseType Send([FromQuery] string to)
    {
        if (string.IsNullOrEmpty(to))
            return new JsonResult(new { error = "Укажите email в query параметре ?to=..." });

        try
        {
            
            string zipPath = Path.Combine(Path.GetTempPath(), "project.zip");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(zipPath)) File.Delete(zipPath);
            
            string  projectFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");

            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                var filesInRoot = Directory.GetFiles(projectFolder);
                foreach (var file in filesInRoot)
                {
                    string fileName = Path.GetFileName(file);
                    if (fileName.EndsWith(".cs") || fileName.EndsWith(".json") || fileName.EndsWith(".csproj") || fileName.EndsWith(".sln"))
                    {
                        zip.CreateEntryFromFile(file, fileName);
                    }
                }

                foreach (var file in Directory.GetFiles(Path.Combine(projectFolder, "Controllers")))
                    zip.CreateEntryFromFile(file, "Controllers/" + Path.GetFileName(file));

                foreach (var file in Directory.GetFiles(Path.Combine(projectFolder, "Attributes")))
                    zip.CreateEntryFromFile(file, "Attributes/" + Path.GetFileName(file));

                foreach (var file in Directory.GetFiles(Path.Combine(projectFolder, "frameworks"), "*.*", SearchOption.AllDirectories))
                    zip.CreateEntryFromFile(file, "frameworks/" + Path.GetRelativePath(Path.Combine(projectFolder, "frameworks"), file));

                foreach (var file in Directory.GetFiles(Path.Combine(projectFolder, "static"), "*.*", SearchOption.AllDirectories))
                    zip.CreateEntryFromFile(file, "static/" + Path.GetRelativePath(Path.Combine(projectFolder, "static"), file));
            }

            string myEmail = App.Settings.Email;
            string myPassword = App.Settings.Password;

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(myEmail, myPassword),
                EnableSsl = true
            };

            using var mail = new MailMessage(myEmail, to)
            {
                Subject = "Домашняя работа",
                Body = "Группа: 11-541\nФИО: Бородин Артемий Аркадьевич\nАрхив с проектом во вложении."
            };

            mail.Attachments.Add(new Attachment(zipPath));

            smtp.Send(mail);

            return new JsonResult(new { status = "success", message = $"Письмо отправлено на {to}" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { status = "error", message = ex.Message });
        }
    }
}