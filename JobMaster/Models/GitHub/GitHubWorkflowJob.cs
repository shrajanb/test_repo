using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.GitHub;

[Table("github_workflow_jobs")]
public class GitHubWorkflowJob
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public long GitHubId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Conclusion { get; set; }
    
    [MaxLength(255)]
    public string? HeadSha { get; set; }
    
    [MaxLength(500)]
    public string? HtmlUrl { get; set; }
    
    [MaxLength(255)]
    public string? RunnerName { get; set; }
    
    [MaxLength(255)]
    public string? RunnerGroupName { get; set; }
    
    public long? CheckRunId { get; set; }
    
    public DateTime? GitHubCreatedAt { get; set; }
    
    public DateTime? GitHubStartedAt { get; set; }
    
    public DateTime? GitHubCompletedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key
    [Required]
    [ForeignKey(nameof(WorkflowRun))]
    public int WorkflowRunId { get; set; }
    
    // Navigation properties
    public virtual GitHubWorkflowRun WorkflowRun { get; set; } = null!;
}
