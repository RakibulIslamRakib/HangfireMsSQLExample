To handle background jobs in .NET Core using Hangfire with MS SQL Server as the persistent storage, we'll follow a step-by-step approach to set up Hangfire, configure MS SQL Server, and create background jobs.

**Project Structure Overview**
API Layer: Contains controllers to trigger background jobs.
Service Layer: Business logic for the background jobs.
Hangfire Configuration: Set up Hangfire with MS SQL Server for job persistence.
Background Job Layer: Separate background job logic (adhering to SOLID principles).

**Step-by-Step Guide**
Step 1: Set Up Project and Install NuGet Packages
Create a new .NET Core Web API project.

Step 2 : Install the required NuGet packages:
//inside project folder
dotnet add package Hangfire
dotnet add package Hangfire.SqlServer

Step 3: create database(I have used sql server database, you can use any ...)
CREATE DATABASE HangfireSqlDB;

Step 2: Configure Program.cs
In .NET 6 and later, the Program.cs file includes the configuration. We will configure Hangfire with MS SQL Server here.
(see program.cs )

**For older version use startup.cs:**
Configure Hangfire in Startup.cs
We will configure Hangfire to use MS SQL Server for storage and register services in Startup.cs
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using HangfireMSSQLExample.Services;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Hangfire configuration to use MS SQL Server
        services.AddHangfire(configuration => 
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                         .UseSimpleAssemblyNameTypeSerializer()
                         .UseRecommendedSerializerSettings()
                         .UseSqlServerStorage("Server=localhost;Database=HangfireDB;User Id=sa;Password=your_password;", 
                         new SqlServerStorageOptions
                         {
                             CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                             SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                             QueuePollInterval = TimeSpan.Zero,
                             UseRecommendedIsolationLevel = true,
                             DisableGlobalLocks = true
                         }));

        // Add Hangfire server to process jobs
        services.AddHangfireServer();

        // Register custom services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IScheduleTaskService, ScheduleTaskService>();

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        // Hangfire dashboard setup
        app.UseHangfireDashboard();

        // Configure routing
        app.UseRouting();

        // Configure endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

**Hangfire Configuration: **
The AddHangfire method configures Hangfire to use MS SQL Server.
Connection String: It uses a connection string from appsettings.json (which we'll define shortly).
AddHangfireServer: Adds the Hangfire background job server that will process background jobs.

**Step 3: Define the Services and Job Classes**
**Services/IEmailService.cs:**
namespace HangfireMSSQLExample.Services
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}


**Services/IScheduleTaskService.cs:**

namespace HangfireMSSQLExample.Services
{
    public interface IScheduleTaskService
    {
        void ExecuteScheduledTask();
    }
}


**Services/EmailService.cs:**

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


**Services/ScheduleTaskService.cs:**

namespace HangfireMSSQLExample.Services
{
    public class ScheduleTaskService : IScheduleTaskService
    {
        public void ExecuteScheduledTask()
        {
            // Simulate scheduled task execution
            Console.WriteLine("Executing scheduled task...");
        }
    }
}


**Jobs/EmailJob.cs:**

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


**Jobs/ScheduleTaskJob.cs:**
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


**Step 4: Create a Controller to Trigger Background Jobs**
Create a controller to handle API requests for triggering background jobs.

**Controllers/JobController.cs:**
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

**Step 5: Configure appsettings.json**
Add the MS SQL Server connection string in appsettings.json:

appsettings.json:

json
Copy code
{
  "ConnectionStrings": {
    "HangfireConnection": "Server=[use your server name];Database=[your db name];User Id=[user name];Password=[your_password];TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

**Step 6: Run the Application **
dotnet run
The Hangfire dashboard will be available at http://localhost:[your url]/hangfire.

**Conclusion**
In this project, we configured Hangfire with MS SQL Server in a .NET 6+ project using the Program.cs file. 
The background jobs are managed via Hangfire with a clear and clean code structure, adhering to SOLID principles.

in hangfire dashboard: https://localhost:7127/swagger/index.html
![image](https://github.com/user-attachments/assets/00b31a19-250b-4dd7-849d-caa4a380a67e)
![image](https://github.com/user-attachments/assets/b805cafa-40ee-40f5-89e9-2f5ac5ad9ba8)
