using System;
namespace IdeasTracker.Business.Email.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string recipientEmail, string subject, string messageBody);
        void SendAdoptionEmail(string recipientEmail);
        void SendAdoptionRejectionEmail(string recipientEmail);
        void SendAdoptionAcceptanceEmail(string recepientEmail);
    }
}
