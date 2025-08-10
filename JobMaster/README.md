# JobMaster

JobMaster is a .NET 9 ASP.NET Core Web API application for managing and orchestrating jobs using Hangfire and MySQL. It supports jobs such as sync_github, sync_jira, sync_bitbucket, sync_jenkins, and sync_metrics.

## Features
- Job management and orchestration with Hangfire
- MySQL backend using Entity Framework Core
- RESTful API for job control and status
- Hangfire Dashboard for job monitoring
- Example: Full implementation of GitHub sync job

## Prerequisites
- .NET 9 SDK
- MySQL server running on localhost
- GitHub Personal Access Token (for GitHub sync)

## Getting Started

### 1. Database Setup
```sql
CREATE DATABASE job_master
    DEFAULT CHARACTER SET = 'utf8mb4';
```

### 2. Configuration
Update `appsettings.json` with your MySQL connection string and GitHub token:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=job_master;Uid=root;Pwd=your-password;"
  },
  "GitHub": {
    "Token": "your-github-token-here"
  }
}
```

### 3. Run Database Migrations
```bash
dotnet ef database update
```

### 4. Start the API
```bash
dotnet run
```

### 5. Access Services
- **API**: http://localhost:5276
- **Hangfire Dashboard**: http://localhost:5276/hangfire
- **OpenAPI/Swagger**: http://localhost:5276/swagger (in development)

## API Endpoints

### Job Status
- `GET /api/jobs/status` - Get status of all jobs

### GitHub Sync Job
- `POST /api/jobs/github/sync` - Trigger GitHub sync immediately
- `POST /api/jobs/github/schedule` - Schedule daily GitHub sync at 2 AM
- `DELETE /api/jobs/github/unschedule` - Remove scheduled GitHub sync

## Database Tables

### GitHub Sync Tables
- `github_organisations` - GitHub organization data
- `github_repositories` - Repository information
- `github_workflows` - GitHub Actions workflows
- `github_workflow_runs` - Workflow execution data
- `github_workflow_jobs` - Individual job data within workflow runs

### Hangfire Tables
Hangfire automatically creates its tables in the same MySQL database for job storage and management.

## Jobs

### GitHub Sync Job (`sync_github`) - ✅ Implemented
- Syncs GitHub organisations, repositories, workflows, workflow runs, and jobs
- Scheduled to run daily at 2 AM
- Can be triggered manually via API
- Stores data in MySQL with proper relationships

### Planned Jobs
- `sync_jira` - Jira data synchronization (follow GitHub pattern)
- `sync_bitbucket` - Bitbucket data synchronization (follow GitHub pattern)  
- `sync_jenkins` - Jenkins data synchronization (follow GitHub pattern)
- `sync_metrics` - Metrics aggregation (follow GitHub pattern)

## Architecture

### Job Pattern
Each job follows this established pattern:
1. **Models**: Entity classes in `Models/{JobType}/`
2. **Service**: API client service in `Services/`
3. **Job**: Background job class in `Jobs/`
4. **DbContext**: Entity configuration in `Data/JobMasterDbContext.cs`
5. **Controller**: API endpoints in `Controllers/`

### Technologies
- **.NET 9**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM with MySQL provider
- **Hangfire**: Background job processing with MySQL storage
- **MySQL**: Database for both application and Hangfire data

## Development

### Build
```bash
dotnet build
```

### Test API
Use the `api-tests.http` file with REST client tools or test the endpoints manually.

### View Jobs
Monitor job execution and history via the Hangfire Dashboard at `/hangfire`.

## Contributing
When adding new jobs, follow the GitHub sync job pattern for consistency and maintainability.

---
