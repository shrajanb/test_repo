using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.Jira;

[Table("jira_issues")]
public class JiraIssue
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string IssueKey { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string IssueId { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Summary { get; set; }
    
    [Column(TypeName = "TEXT")]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? IssueType { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(50)]
    public string? Priority { get; set; }
    
    [MaxLength(100)]
    public string? Project { get; set; }
    
    [MaxLength(50)]
    public string? ProjectKey { get; set; }
    
    [MaxLength(255)]
    public string? Reporter { get; set; }
    
    [MaxLength(255)]
    public string? Assignee { get; set; }
    
    [MaxLength(255)]
    public string? Creator { get; set; }
    
    [MaxLength(100)]
    public string? Resolution { get; set; }
    
    public DateTime? ResolutionDate { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [Column(TypeName = "JSON")]
    public string? Labels { get; set; }
    
    [Column(TypeName = "JSON")]
    public string? Components { get; set; }
    
    [Column(TypeName = "JSON")]
    public string? FixVersions { get; set; }
    
    [Column(TypeName = "JSON")]
    public string? AffectedVersions { get; set; }
    
    public int? StoryPoints { get; set; }
    
    public int? TimeOriginalEstimate { get; set; }
    
    public int? TimeRemaining { get; set; }
    
    public int? TimeSpent { get; set; }
    
    [MaxLength(100)]
    public string? Epic { get; set; }
    
    [MaxLength(100)]
    public string? Sprint { get; set; }
    
    [MaxLength(500)]
    public string? IssueUrl { get; set; }
    
    public DateTime? JiraCreatedAt { get; set; }
    
    public DateTime? JiraUpdatedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
