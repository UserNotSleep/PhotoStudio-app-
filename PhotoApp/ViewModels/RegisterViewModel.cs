using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ApiClient _apiClient;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _email = string.Empty;
        private string _name = string.Empty;
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
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

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLogin { get; }
        
        // Initialize the event with an empty handler to avoid null reference exceptions
        public event EventHandler<PhotographerProfile> RegistrationSuccessful = delegate { };
        public event EventHandler GoToLogin = delegate { };

        public RegisterViewModel(ApiClient? apiClient = null)
        {
            _apiClient = apiClient ?? new ApiClient();
            RegisterCommand = ReactiveCommand.CreateFromTask(ExecuteRegisterAsync);
            NavigateToLogin = ReactiveCommand.Create(() => GoToLogin?.Invoke(this, EventArgs.Empty));
        }

        private async Task ExecuteRegisterAsync()
        {
            // Reset error message
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || 
                    string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Email) || 
                    string.IsNullOrWhiteSpace(Name))
                {
                    ErrorMessage = "Пожалуйста, заполните все поля";
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Пароли не совпадают";
                    return;
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "Пароль должен содержать не менее 6 символов";
                    return;
                }

                if (!Email.Contains("@") || !Email.Contains("."))
                {
                    ErrorMessage = "Пожалуйста, введите корректный email";
                    return;
                }

                // Create register request
                var request = new RegisterRequest
                {
                    Username = Username,
                    Password = Password,
                    Email = Email,
                    Name = Name,
                    DeviceId = Guid.NewGuid().ToString() // Generate a random device ID
                };

                // Try to register via API
                var profile = await _apiClient.RegisterAsync(request);
                
                // Notify success
                RegistrationSuccessful?.Invoke(this, profile);
            }
            catch (Exception ex)
            {
                // Show error message
                ErrorMessage = $"Ошибка при регистрации: {ex.Message}";
                
                // In a real app, you might want to log this exception
                Console.WriteLine($"Registration error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }


    }
}
