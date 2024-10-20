namespace HangfireMSSQLExample.Services
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            // Simulate sending email
            Console.WriteLine($"Sending email to {to} with subject: {subject}, body: {body}");
        }
    }
}
