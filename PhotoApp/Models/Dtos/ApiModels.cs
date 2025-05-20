using System;
using System.Text.Json.Serialization;

namespace PhotoApp.Models.Dtos
{
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = "demo-token"; // Временный токен для демонстрации
    }

    public class PhotographerProfile
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
        
        [JsonPropertyName("bio")]
        public string Bio { get; set; } = string.Empty;
        
        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; } = string.Empty;
    }

    public class PhotoGallery
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
        
        [JsonPropertyName("imageCount")]
        public int ImageCount { get; set; }
        
        [JsonPropertyName("coverImageUrl")]
        public string CoverImageUrl { get; set; } = string.Empty;
    }

    public class Photo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("filename")]
        public string Filename { get; set; } = string.Empty;
        
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
        
        [JsonPropertyName("session_id")]
        public int SessionId { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class PhotoMetadata
    {
        [JsonPropertyName("cameraModel")]
        public string CameraModel { get; set; } = string.Empty;
        
        [JsonPropertyName("exposureTime")]
        public string ExposureTime { get; set; } = string.Empty;
        
        [JsonPropertyName("aperture")]
        public string Aperture { get; set; } = string.Empty;
        
        [JsonPropertyName("iso")]
        public int Iso { get; set; }
        
        [JsonPropertyName("focalLength")]
        public string FocalLength { get; set; } = string.Empty;
        
        [JsonPropertyName("locationName")]
        public string LocationName { get; set; } = string.Empty;
    }

    public class Client
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }

    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }
        
        [JsonPropertyName("clientName")]
        public string ClientName { get; set; } = string.Empty;
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }
        
        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    public class Session
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        
        [JsonPropertyName("price")]
        public double Price { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonPropertyName("client_id")]
        public int ClientId { get; set; }
        
        // Дополнительные свойства для UI
        [JsonIgnore]
        public string ClientName { get; set; } = string.Empty;
    }

    public class DashboardStats
    {
        public int TotalClients { get; set; }
        public int TotalSessions { get; set; }
        public int TotalPhotos { get; set; }
        public double TotalIncome { get; set; }
        public Session[] UpcomingSessions { get; set; } = Array.Empty<Session>();
    }
} 