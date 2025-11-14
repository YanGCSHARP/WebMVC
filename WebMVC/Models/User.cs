using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
}