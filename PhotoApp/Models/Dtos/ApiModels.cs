using System;
using System.Text.Json.Serialization;

namespace PhotoApp.Models.Dtos
{
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
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
        public string Username { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        
        [JsonPropertyName("bio")]
        public string Bio { get; set; }
        
        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; }
    }

    public class PhotoGallery
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
        
        [JsonPropertyName("imageCount")]
        public int ImageCount { get; set; }
        
        [JsonPropertyName("coverImageUrl")]
        public string CoverImageUrl { get; set; }
    }

    public class Photo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        
        [JsonPropertyName("path")]
        public string Path { get; set; }
        
        [JsonPropertyName("session_id")]
        public int SessionId { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class PhotoMetadata
    {
        [JsonPropertyName("cameraModel")]
        public string CameraModel { get; set; }
        
        [JsonPropertyName("exposureTime")]
        public string ExposureTime { get; set; }
        
        [JsonPropertyName("aperture")]
        public string Aperture { get; set; }
        
        [JsonPropertyName("iso")]
        public int Iso { get; set; }
        
        [JsonPropertyName("focalLength")]
        public string FocalLength { get; set; }
        
        [JsonPropertyName("locationName")]
        public string LocationName { get; set; }
    }

    public class Client
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }
        
        [JsonPropertyName("clientName")]
        public string ClientName { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }
        
        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
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
        public string Status { get; set; }
        
        [JsonPropertyName("client_id")]
        public int ClientId { get; set; }
        
        // Дополнительные свойства для UI
        [JsonIgnore]
        public string ClientName { get; set; }
    }

    public class DashboardStats
    {
        public int TotalClients { get; set; }
        public int TotalSessions { get; set; }
        public int TotalPhotos { get; set; }
        public double TotalIncome { get; set; }
        public Session[] UpcomingSessions { get; set; }
    }
} 