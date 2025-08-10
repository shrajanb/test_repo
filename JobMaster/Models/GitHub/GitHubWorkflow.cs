using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.GitHub;

[Table("github_workflows")]
public class GitHubWorkflow
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public long GitHubId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string State { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? HtmlUrl { get; set; }
    
    [MaxLength(500)]
    public string? BadgeUrl { get; set; }
    
    public DateTime? GitHubCreatedAt { get; set; }
    
    public DateTime? GitHubUpdatedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key
    [Required]
    [ForeignKey(nameof(Repository))]
    public int RepositoryId { get; set; }
    
    // Navigation properties
    public virtual GitHubRepository Repository { get; set; } = null!;
    
    public virtual ICollection<GitHubWorkflowRun> WorkflowRuns { get; set; } = new List<GitHubWorkflowRun>();
}
