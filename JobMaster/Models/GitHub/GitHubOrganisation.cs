using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.GitHub;

[Table("github_organisations")]
public class GitHubOrganisation
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public long GitHubId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Login { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Name { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? Company { get; set; }
    
    [MaxLength(500)]
    public string? Blog { get; set; }
    
    [MaxLength(255)]
    public string? Location { get; set; }
    
    [MaxLength(255)]
    public string? Email { get; set; }
    
    [MaxLength(255)]
    public string? TwitterUsername { get; set; }
    
    public int PublicRepos { get; set; }
    
    public int PublicGists { get; set; }
    
    public int Followers { get; set; }
    
    public int Following { get; set; }
    
    [MaxLength(500)]
    public string? HtmlUrl { get; set; }
    
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
    
    public DateTime? GitHubCreatedAt { get; set; }
    
    public DateTime? GitHubUpdatedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<GitHubRepository> Repositories { get; set; } = new List<GitHubRepository>();
}
