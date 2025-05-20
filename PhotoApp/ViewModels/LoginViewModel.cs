using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ApiClient _apiClient;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _rememberMe = false;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => this.RaiseAndSetIfChanged(ref _rememberMe, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        // Initialize the event with an empty handler to avoid null reference exceptions
        public event EventHandler<PhotographerProfile> LoginSuccessful = delegate { };

        public LoginViewModel(ApiClient? apiClient = null)
        {
            _apiClient = apiClient ?? new ApiClient();
            LoginCommand = ReactiveCommand.CreateFromTask(ExecuteLoginAsync);
        }

        private async Task ExecuteLoginAsync()
        {
            // Reset error message
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пожалуйста, введите имя пользователя и пароль";
                    return;
                }

                // Try to login via API
                await _apiClient.LoginAsync(Username, Password);
                
                // Get user profile
                var profile = await _apiClient.GetPhotographerProfileAsync();
                
                // Notify success
                LoginSuccessful?.Invoke(this, profile);
            }
            catch (Exception ex)
            {
                // Show error message
                ErrorMessage = "Неверное имя пользователя или пароль";
                
                // In a real app, you might want to log this exception
                Console.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
} 