using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.GitHub;

[Table("github_repositories")]
public class GitHubRepository
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
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public bool Private { get; set; }
    
    public bool Fork { get; set; }
    
    [MaxLength(500)]
    public string? Homepage { get; set; }
    
    [MaxLength(255)]
    public string? Language { get; set; }
    
    public int ForksCount { get; set; }
    
    public int StargazersCount { get; set; }
    
    public int WatchersCount { get; set; }
    
    public int Size { get; set; }
    
    [MaxLength(255)]
    public string? DefaultBranch { get; set; }
    
    public int OpenIssuesCount { get; set; }
    
    public bool HasIssues { get; set; }
    
    public bool HasProjects { get; set; }
    
    public bool HasWiki { get; set; }
    
    public bool HasPages { get; set; }
    
    public bool HasDownloads { get; set; }
    
    public bool Archived { get; set; }
    
    public bool Disabled { get; set; }
    
    [MaxLength(500)]
    public string? HtmlUrl { get; set; }
    
    [MaxLength(500)]
    public string? CloneUrl { get; set; }
    
    [MaxLength(500)]
    public string? SshUrl { get; set; }
    
    public DateTime? GitHubCreatedAt { get; set; }
    
    public DateTime? GitHubUpdatedAt { get; set; }
    
    public DateTime? GitHubPushedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key
    [ForeignKey(nameof(Organisation))]
    public int? OrganisationId { get; set; }
    
    // Navigation properties
    public virtual GitHubOrganisation? Organisation { get; set; }
    
    public virtual ICollection<GitHubWorkflow> Workflows { get; set; } = new List<GitHubWorkflow>();
}
