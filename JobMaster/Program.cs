using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage.MySql;
using JobMaster.Data;
using JobMaster.Jobs;
using JobMaster.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<JobMasterDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Add Hangfire services with MySQL storage
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(10),
        JobExpirationCheckInterval = TimeSpan.FromHours(1),
        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        PrepareSchemaIfNecessary = true,
        DashboardJobListLimit = 25000,
        TransactionTimeout = TimeSpan.FromMinutes(1),
        TablesPrefix = "hangfire_"
    })));

builder.Services.AddHangfireServer();

// Add application services
builder.Services.AddHttpClient<GitHubService>();
builder.Services.AddHttpClient<JiraService>();
builder.Services.AddScoped<GitHubSyncJob>();
builder.Services.AddScoped<JiraSyncJob>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapControllers();

// Schedule recurring jobs on startup
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    
    // Schedule GitHub sync to run daily at 2 AM
    recurringJobManager.AddOrUpdate<GitHubSyncJob>(
        "github-sync",
        job => job.SyncAsync(),
        Cron.Daily(2)
    );
    
    // Schedule Jira sync to run daily at 3 AM
    recurringJobManager.AddOrUpdate<JiraSyncJob>(
        "jira-sync",
        job => job.SyncAsync(),
        Cron.Daily(3)
    );
}

app.Run();

// Custom authorization filter for Hangfire Dashboard (for development)
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, add proper authentication
        return true;
    }
}
