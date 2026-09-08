using System.Net;
using System.Net.Mail;

namespace Implementation.Emails
{
    //https://myaccount.google.com/apppasswords - postavljanje app password-a za gmail
    public class SmtpEmailSender : Application.Email.IEmailSender
    {
        private string _fromEmail;
        private string _appPassword;

        // Da li mi treba kod sa gmail-a?
        public SmtpEmailSender(string fromEmail, string appPassword)
        {
            this._fromEmail = fromEmail;
            this._appPassword = appPassword;
        }

        public void SendEmail(string recipient, string subject, string htmlContent)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, "ASP ICT 2026"),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            message.To.Add(recipient);

            using SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromEmail, _appPassword)
            };

            client.Send(message);
        }
    }
}
