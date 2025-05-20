using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly ApiClient _apiClient;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
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

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => this.RaiseAndSetIfChanged(ref _phone, value);
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
        public ICommand CancelCommand { get; }

        // Event to notify when the registration was successful
        public event EventHandler<PhotographerProfile> RegistrationSuccessful = delegate { };
        
        // Event to notify when the user wants to go back to login
        public event EventHandler CancelRequested = delegate { };

        public RegistrationViewModel(ApiClient? apiClient = null)
        {
            try
            {
                Console.WriteLine("Initializing RegistrationViewModel");
                _apiClient = apiClient ?? new ApiClient();
                RegisterCommand = ReactiveCommand.CreateFromTask(ExecuteRegisterAsync);
                CancelCommand = ReactiveCommand.Create(ExecuteCancel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing RegistrationViewModel: {ex.Message}");
                Debug.WriteLine($"Error initializing RegistrationViewModel: {ex}");
                ErrorMessage = "Ошибка инициализации: " + ex.Message;
            }
        }

        private async Task ExecuteRegisterAsync()
        {
            try
            {
                Console.WriteLine("ExecuteRegisterAsync started");
                ErrorMessage = string.Empty;
                IsLoading = true;

                // Validation
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || 
                    string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Name) || 
                    string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Пожалуйста, заполните все обязательные поля";
                    IsLoading = false;
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Пароли не совпадают";
                    IsLoading = false;
                    return;
                }

                Console.WriteLine("Validation passed, proceeding with registration");

                // В реальном приложении здесь был бы запрос к API
                // Но так как текущий сервер не поддерживает регистрацию, мы имитируем успешную регистрацию
                await Task.Delay(500); // Имитация задержки сети

                try
                {
                    // Логинимся сразу после регистрации
                    Console.WriteLine("Attempting login after registration");
                    await _apiClient.LoginAsync(Username, Password);
                    
                    // Получаем профиль пользователя
                    Console.WriteLine("Creating user profile");
                    var profile = new PhotographerProfile
                    {
                        Id = 1,
                        Username = Username,
                        Name = Name ?? string.Empty,
                        Email = Email ?? string.Empty,
                        Phone = Phone ?? string.Empty,
                        Bio = "Новый пользователь",
                        ProfileImageUrl = "https://example.com/default-profile.jpg"
                    };
                    
                    // Уведомляем об успешной регистрации
                    Console.WriteLine("Registration successful, raising event");
                    RegistrationSuccessful?.Invoke(this, profile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during login/profile creation: {ex.Message}");
                    Debug.WriteLine($"Detailed error: {ex}");
                    ErrorMessage = "Ошибка при входе после регистрации: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General registration error: {ex.Message}");
                Debug.WriteLine($"Detailed registration error: {ex}");
                ErrorMessage = "Ошибка при регистрации: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
                Console.WriteLine("ExecuteRegisterAsync completed");
            }
        }

        private void ExecuteCancel()
        {
            try
            {
                Console.WriteLine("Canceling registration, returning to login");
                // Уведомляем о запросе вернуться к форме входа
                CancelRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cancel: {ex.Message}");
                Debug.WriteLine($"Detailed cancel error: {ex}");
                ErrorMessage = "Ошибка при отмене: " + ex.Message;
            }
        }
    }
} 