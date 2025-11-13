using System;
using System.ComponentModel.DataAnnotations;

namespace MemorialRegistry.Shared.Models;

public class Obituary
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(200, ErrorMessage = "Full name cannot exceed 200 characters")]
    public string FullName { get; set; } = null!;
    
    [Required(ErrorMessage = "Date of birth is required")]
    public DateOnly DateOfBirth { get; set; }
    
    [Required(ErrorMessage = "Date of death is required")]
    public DateOnly DateOfDeath { get; set; }
    
    [Required(ErrorMessage = "Biography is required")]
    [StringLength(5000, ErrorMessage = "Biography cannot exceed 5000 characters")]
    public string Biography { get; set; } = null!;

    // Store Base64 data URL here (e.g., "data:image/jpeg;base64,/9j/4AAQ...")
    public string? PrimaryPhotoBase64 { get; set; }

    // auditing / ownership
    public string CreatedByUserId { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}