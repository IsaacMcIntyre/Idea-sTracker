using System;
using System.Net;
using System.Net.Mail;
using IdeasTracker.Business.Email.Interfaces;

namespace IdeasTracker.Business.Email
{
    public class EmailSender: IEmailSender
    {
        public  void SendEmail(string recipientEmail, string subject, string messageBody)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("emailsendingtestaddress@gmail.com");
                message.To.Add(new MailAddress(recipientEmail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("emailsendingtestaddress@gmail.com", "Testing-password123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public  void SendAdoptionEmail(string recipientEmail)
        {
            SendEmail(recipientEmail, "Adoption Request Received", "Thank you, your adoption request has been submitted. We will notify you when it is accepted or declined.");
            SendEmail("emailsendingaddress@gmail.com", "Adoption Request Received", "There is a new adoption request on the IdeaTracker system. Please check the system to view.");
        }
    }
}
