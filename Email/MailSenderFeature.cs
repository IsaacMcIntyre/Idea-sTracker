using System.Net;
using System.Net.Mail;
//using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;

namespace IdeasTracker.Email
{
    static class MailSenderFeature
    {
        public static string SendEmail( string email, string message, string subject)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("emailsendingtestaddress@gmail.com", "Testing-Password123");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("emailsendingtestaddress@gmail.com"),
                    Subject = subject,
                    Body = message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(email));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
                return "Email Sent Successfully!";
            }
            catch (System.Exception e)
            {
                return e.Message;
            }

        }
    }
}