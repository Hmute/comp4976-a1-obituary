using System;

namespace Assignment1.Models;

public class Obituary
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public DateOnly DateOfDeath { get; set; }
    public string Biography { get; set; } = null!;

    // Store Base64 data URL here (e.g., "data:image/jpeg;base64,/9j/4AAQ...")
    public string? PrimaryPhotoBase64 { get; set; }

    // auditing / ownership (as you had)
    public string CreatedByUserId { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

