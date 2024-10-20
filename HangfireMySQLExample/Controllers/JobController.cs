using Hangfire;
using HangfireMSSQLExample.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace HangfireMSSQLExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly EmailJob _emailJob;
        private readonly ScheduleTaskJob _scheduleTaskJob;

        public JobController(EmailJob emailJob, ScheduleTaskJob scheduleTaskJob)
        {
            _emailJob = emailJob;
            _scheduleTaskJob = scheduleTaskJob;
        }

        [HttpPost("send-email")]
        public IActionResult SendEmail(string to, string subject, string body)
        {
            // Enqueue a fire-and-forget background job for sending email
            BackgroundJob.Enqueue(() => _emailJob.SendEmailInBackground(to, subject, body));
            return Ok("Email job enqueued.");
        }

        [HttpPost("schedule-task")]
        public IActionResult ScheduleTask()
        {
            // Schedule a task to run after a delay of 5 minutes
            BackgroundJob.Schedule(() => _scheduleTaskJob.ExecuteTaskInBackground(), TimeSpan.FromMinutes(5));
            return Ok("Task scheduled.");
        }
    }
}
