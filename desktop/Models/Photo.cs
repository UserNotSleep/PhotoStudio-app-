using System;

namespace PhotoStudio.Desktop.Models;

public class Photo
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public DateTime CreatedAt { get; set; }
} 