using System;

namespace PhotoStudio.Desktop.Models;

public class Session
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ClientId { get; set; }
} 