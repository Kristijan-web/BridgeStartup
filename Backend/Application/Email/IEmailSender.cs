namespace Application.Email
{
    public interface IEmailSender
    {
        void SendEmail(string recipient, string subject, string htmlContent);

    }
}
