using Hangfire;
using Hangfire.SqlServer;
using HangfireMSSQLExample.Jobs;
using HangfireMSSQLExample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHangfire(configuration =>
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"),
                 new SqlServerStorageOptions
                 {
                     CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                     SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                     QueuePollInterval = TimeSpan.Zero,
                     UseRecommendedIsolationLevel = true,
                     DisableGlobalLocks = true
                 }));

// Add Hangfire server to process jobs
builder.Services.AddHangfireServer();
// Register custom services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IScheduleTaskService, ScheduleTaskService>();

// Register job classes
builder.Services.AddScoped<EmailJob>();
builder.Services.AddScoped<ScheduleTaskJob>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// Hangfire Dashboard
app.UseHangfireDashboard();

app.MapControllers();

app.Run();
