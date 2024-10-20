using HangfireMSSQLExample.Services;

namespace HangfireMSSQLExample.Jobs
{
    public class EmailJob
    {
        private readonly IEmailService _emailService;

        public EmailJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void SendEmailInBackground(string to, string subject, string body)
        {
            _emailService.SendEmail(to, subject, body);
        }
    }
}
