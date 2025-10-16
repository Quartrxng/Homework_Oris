using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniHttpServer.Services
{
    internal class EmailService
    {
        private readonly List<SmtpSettings> _smtpList;

        public EmailService()
        {
            // Список SMTP-серверов в порядке приоритета
            _smtpList = new List<SmtpSettings>
            {
                new SmtpSettings
                {
                    Name = "Gmail",
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Username = "newsightminecraft@gmail.com",
                    Password = "cwej gqwe qvfg soye"
                },
                new SmtpSettings
                {
                    Name = "Yandex",
                    Host = "smtp.yandex.ru",
                    Port = 587,
                    EnableSsl = true,
                    Username = "generalovvictory@yandex.ru",
                    Password = "duuavdaqyxcathew"
                },
                new SmtpSettings
                {
                    Name = "Mail.ru",
                    Host = "smtp.mail.ru",
                    Port = 587,
                    EnableSsl = true,
                    Username = "gfdsofgklsdf@inbox.ru",
                    Password = "G7acOJ6f3yf0ZGgiHOoP"
                }
            };
        }

        public void SendEmail(string _to, string _title, string _message)
        {
            foreach (var smtpSettings in _smtpList) {
                try
                {
                    MailAddress from = new MailAddress(smtpSettings.Username, "Виктор Генералов 11-409");
                    // кому отправляем
                    MailAddress to = new MailAddress(_to);
                    // создаем объект сообщения
                    MailMessage m = new MailMessage(from, to);
                    // тема письма
                    m.Subject = _title;
                    // текст письма
                    m.Body = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif; color: #333;'>
                                    <h2 style='color:#2e6c80;'>Здравствуйте!</h2>
                                    <p>Вы успешно авторизовались на сайте.</p>
                                    <p>Ваши данные для входа:</p>
                                    <ul>
                                        <li><b>Логин:</b> {_to}</li>
                                        <li><b>Пароль:</b> {_message}</li>
                                    </ul>
                                </body>
                            </html>";
                    // письмо представляет код html
                    m.IsBodyHtml = true;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", "HomeWork_4.zip");
                    m.Attachments.Add(new Attachment(path));
                    // адрес smtp-сервера и порт, с которого будем отправлять письмо
                    SmtpClient smtp = new SmtpClient(smtpSettings.Host, smtpSettings.Port);
                    // логин и пароль
                    smtp.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    smtp.EnableSsl = smtpSettings.EnableSsl;
                    smtp.Send(m);

                    Console.WriteLine("Письмо отправлено");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось через {smtpSettings.Name}: {ex.Message}");
                }
            }

            Console.WriteLine("Ни с одного почтового ящика не удалось отправить письмо");
        }
    }
}
