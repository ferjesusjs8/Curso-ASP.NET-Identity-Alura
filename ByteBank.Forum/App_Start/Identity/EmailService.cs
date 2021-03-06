﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailService : IIdentityMessageService
    {
        private readonly string EMAIL_ORIGEM = ConfigurationManager.AppSettings["Email:EmailOrigem"];
        private readonly string SENHA_EMAIL = ConfigurationManager.AppSettings["Email:EmailSenha"];
        public async Task SendAsync(IdentityMessage message)
        {
            using (var mensagemEmail = new MailMessage())
            {
                mensagemEmail.From = new MailAddress(EMAIL_ORIGEM);

                mensagemEmail.Subject = message.Subject;
                mensagemEmail.To.Add(message.Destination);
                mensagemEmail.Body = message.Body;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(EMAIL_ORIGEM, SENHA_EMAIL);

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;

                    smtpClient.Timeout = 20_000;

                    await smtpClient.SendMailAsync(mensagemEmail);
                }
            }
        }
    }
}