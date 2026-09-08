
using Application.Email;

namespace Implementation.Emails
{
    public class DevEmailSender : IEmailSender
    {
        public void SendEmail(string recipient, string subject, string htmlContent)
        {
            Console.WriteLine("Sending an email to: " + recipient);
            Console.WriteLine("Subject" + subject);
            Console.WriteLine("Content: " + htmlContent);
        }
    }
}
