using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PhotoApp.Models.Dtos;

namespace PhotoApp.Models
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string _authToken = string.Empty;

        public ApiClient(string baseUrl = "http://localhost:5000/api")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string AuthToken
        {
            get => _authToken;
            set
            {
                _authToken = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
        }

        // Аутентификация - имитация, так как сервер пока не имеет этой функциональности
        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            // В реальном приложении здесь был бы запрос к серверу
            // Но так как у текущего сервера нет аутентификации, мы создаем фиктивный ответ
            await Task.Delay(500); // Имитация задержки сети
            
            if (username == "admin" && password == "admin")
            {
                var response = new LoginResponse 
                { 
                    Token = "demo-token" 
                };
                AuthToken = response.Token;
                return response;
            }
            
            throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
        }
        
        // Регистрация нового пользователя
        public async Task<PhotographerProfile> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // В реальном приложении здесь был бы запрос к серверу
                // Но так как у текущего сервера нет регистрации, мы создаем фиктивный ответ
                await Task.Delay(800); // Имитация задержки сети
                
                // Проверка, что пользователь с таким именем не существует
                if (request.Username == "admin")
                {
                    throw new InvalidOperationException("Пользователь с таким именем уже существует");
                }
                
                // Создаем фиктивный профиль
                var profile = new PhotographerProfile
                {
                    Id = new Random().Next(100, 999), // Генерируем случайный ID
                    Username = request.Username,
                    Name = request.Name,
                    Email = request.Email,
                    Phone = "",
                    Bio = "",
                    ProfileImageUrl = "https://example.com/default-profile.jpg"
                };
                
                // Имитируем успешный вход после регистрации
                var loginResponse = await LoginAsync(request.Username, request.Password);
                
                return profile;
            }
            catch (Exception ex)
            {
                // Перебрасываем исключение выше
                throw new InvalidOperationException($"Ошибка при регистрации: {ex.Message}");
            }
        }

        // Получение списка клиентов
        public async Task<List<Client>> GetClientsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/clients");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Client>>();
        }

        // Создание нового клиента
        public async Task<Client> CreateClientAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/clients", client);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Client>();
        }

        // Получение списка фотосессий
        public async Task<List<Session>> GetSessionsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/sessions");
            response.EnsureSuccessStatusCode();
            
            var sessions = await response.Content.ReadFromJsonAsync<List<Session>>();
            
            // Получаем клиентов для заполнения имен
            var clients = await GetClientsAsync();
            foreach (var session in sessions)
            {
                var client = clients.FirstOrDefault(c => c.Id == session.ClientId);
                session.ClientName = client?.Name ?? "Неизвестный клиент";
            }
            
            return sessions;
        }

        // Создание новой фотосессии
        public async Task<Session> CreateSessionAsync(Session session)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/sessions", session);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Session>();
        }

        // Получение списка фотографий
        public async Task<List<Photo>> GetPhotosAsync(int? sessionId = null)
        {
            string url = $"{_baseUrl}/photos";
            if (sessionId.HasValue)
            {
                url += $"?session_id={sessionId.Value}";
            }
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Photo>>();
        }

        // Создание новой фотографии
        public async Task<Photo> CreatePhotoAsync(Photo photo)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/photos", photo);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Photo>();
        }

        // Получение статистики для дашборда
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            // В текущем сервере нет готового эндпоинта для статистики,
            // поэтому собираем данные из нескольких запросов
            var clients = await GetClientsAsync();
            var sessions = await GetSessionsAsync();
            var photos = await GetPhotosAsync();

            return new DashboardStats
            {
                TotalClients = clients.Count,
                TotalSessions = sessions.Count,
                TotalPhotos = photos.Count,
                TotalIncome = sessions.Sum(s => s.Price),
                UpcomingSessions = sessions.Where(s => s.Date > DateTime.Now && s.Status != "Завершена")
                                          .OrderBy(s => s.Date)
                                          .Take(5)
                                          .ToArray()
            };
        }

        // Получение списка фотогалерей
        public async Task<List<PhotoGallery>> GetGalleriesAsync()
        {
            // В текущем API нет эндпоинта для галерей, поэтому создаем демо-данные
            await Task.Delay(300); // Имитация задержки сети
            
            return new List<PhotoGallery>
            {
                new PhotoGallery 
                { 
                    Id = 1, 
                    Name = "Свадебные фотографии", 
                    Description = "Коллекция свадебных фотографий", 
                    CreatedDate = DateTime.Now.AddDays(-30), 
                    ImageCount = 124, 
                    CoverImageUrl = "https://example.com/cover1.jpg"
                },
                new PhotoGallery 
                { 
                    Id = 2, 
                    Name = "Семейная фотосессия", 
                    Description = "Фотосессия для семьи Ивановых", 
                    CreatedDate = DateTime.Now.AddDays(-14), 
                    ImageCount = 56, 
                    CoverImageUrl = "https://example.com/cover2.jpg"
                },
                new PhotoGallery 
                { 
                    Id = 3, 
                    Name = "Корпоративное мероприятие", 
                    Description = "Фотосессия корпоративного мероприятия компании ABC", 
                    CreatedDate = DateTime.Now.AddDays(-7), 
                    ImageCount = 78, 
                    CoverImageUrl = "https://example.com/cover3.jpg"
                }
            };
        }

        // Получение списка заказов
        public async Task<List<Order>> GetOrdersAsync()
        {
            // В текущем API нет эндпоинта для заказов, поэтому создаем демо-данные
            await Task.Delay(300); // Имитация задержки сети
            
            // Получаем список клиентов для заполнения имен
            var clients = await GetClientsAsync();
            
            return new List<Order>
            {
                new Order 
                { 
                    Id = 1, 
                    ClientId = 1, 
                    ClientName = clients.FirstOrDefault(c => c.Id == 1)?.Name ?? "Неизвестный клиент", 
                    Title = "Свадебная фотосессия", 
                    Description = "Полный день съемки свадебного мероприятия", 
                    OrderDate = DateTime.Now.AddDays(-15), 
                    DueDate = DateTime.Now.AddDays(15), 
                    Status = "В процессе", 
                    Amount = 25000m
                },
                new Order 
                { 
                    Id = 2, 
                    ClientId = 2, 
                    ClientName = clients.FirstOrDefault(c => c.Id == 2)?.Name ?? "Неизвестный клиент", 
                    Title = "Портретная фотосессия", 
                    Description = "Студийная фотосессия для бизнес-портретов", 
                    OrderDate = DateTime.Now.AddDays(-7), 
                    DueDate = DateTime.Now.AddDays(3), 
                    Status = "В процессе", 
                    Amount = 8000m
                },
                new Order 
                { 
                    Id = 3, 
                    ClientId = 3, 
                    ClientName = clients.FirstOrDefault(c => c.Id == 3)?.Name ?? "Неизвестный клиент", 
                    Title = "Съемка детского праздника", 
                    Description = "Съемка детского дня рождения", 
                    OrderDate = DateTime.Now.AddDays(-30), 
                    DueDate = DateTime.Now.AddDays(-20), 
                    Status = "Завершено", 
                    Amount = 12000m
                }
            };
        }
        
        // Получение профиля фотографа
        public async Task<PhotographerProfile> GetPhotographerProfileAsync()
        {
            // В текущем API нет эндпоинта для профиля, поэтому создаем демо-данные
            await Task.Delay(300); // Имитация задержки сети
            
            return new PhotographerProfile
            {
                Id = 1,
                Username = "admin",
                Name = "Иван Фотограф",
                Email = "ivan@photo.com",
                Phone = "+7 (999) 123-45-67",
                Bio = "Профессиональный фотограф с 10-летним опытом работы.",
                ProfileImageUrl = "https://example.com/profile.jpg"
            };
        }
    }
} 