using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniHttpServer.Core.Attributes;
using MiniHttpServer.Services;

namespace MiniHttpServer.Endpoints
{
    [Endpoint]
    internal class AuthEndpoint
    {
        private readonly EmailService _emailService = new EmailService();

        // get /auth/
        [HttpGet]
        public string LoginPage()
        {
            return "index.html";
        }


        //post /auth/
        [HttpPost]
        public void Login(string email, string password)
        {
            // Отпарвка Email

            _emailService.SendEmail(email, "Авторизация прошла успешно", password);
        }

        //post /auth/sendEmail


        [HttpPost("sendEmail")]
        public void SendEmail(string to, string title, string message)
        {
            // Отпарвка Email
        }
    }
}
