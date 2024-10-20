using HangfireMSSQLExample.Services;

namespace HangfireMSSQLExample.Jobs
{
    public class ScheduleTaskJob
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskJob(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        public void ExecuteTaskInBackground()
        {
            _scheduleTaskService.ExecuteScheduledTask();
        }
    }
}
