using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PhotoStudio.Desktop.Views;
using ReactiveUI;

namespace PhotoStudio.Desktop.ViewModels;

public enum AccountType
{
    Supplier, // Поставщик (владелец студии)
    Client    // Клиент (фотограф, клиент)
}

public class LoginViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    // Свойства для входа
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe;
    
    // Свойства для регистрации
    private string _registerName = string.Empty;
    private string _registerEmail = string.Empty;
    private string _registerPassword = string.Empty;
    private bool _agreeToTerms;
    private AccountType _accountType = AccountType.Supplier;

    public LoginViewModel(Window parentWindow)
    {
        _parentWindow = parentWindow;
        LoginCommand = ReactiveCommand.Create(Login);
        RegisterCommand = ReactiveCommand.Create(Register);
    }

    // Свойства для входа
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
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

    // Свойства для регистрации
    public string RegisterName
    {
        get => _registerName;
        set => this.RaiseAndSetIfChanged(ref _registerName, value);
    }

    public string RegisterEmail
    {
        get => _registerEmail;
        set => this.RaiseAndSetIfChanged(ref _registerEmail, value);
    }

    public string RegisterPassword
    {
        get => _registerPassword;
        set => this.RaiseAndSetIfChanged(ref _registerPassword, value);
    }

    public bool AgreeToTerms
    {
        get => _agreeToTerms;
        set => this.RaiseAndSetIfChanged(ref _agreeToTerms, value);
    }

    public AccountType AccountType
    {
        get => _accountType;
        set => this.RaiseAndSetIfChanged(ref _accountType, value);
    }

    // Команды
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

    private void Login()
    {
        try
        {
            // Проверка учетных данных в реальном приложении должна быть здесь
            // Для примера просто проверим, что поля не пустые
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                // TODO: показать сообщение об ошибке
                Console.WriteLine("Пожалуйста, заполните все поля");
                return;
            }

            // В реальном приложении здесь должна быть аутентификация пользователя
            ShowMainWindow();
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка при входе в систему: {ex.Message}");
        }
    }
    
    private void Register()
    {
        try
        {
            // Проверка данных регистрации
            if (string.IsNullOrWhiteSpace(RegisterName) || 
                string.IsNullOrWhiteSpace(RegisterEmail) || 
                string.IsNullOrWhiteSpace(RegisterPassword))
            {
                // TODO: показать сообщение об ошибке
                Console.WriteLine("Пожалуйста, заполните все поля");
                return;
            }
            
            if (!AgreeToTerms)
            {
                // TODO: показать сообщение об ошибке
                Console.WriteLine("Необходимо согласиться с условиями использования");
                return;
            }
            
            // В реальном приложении здесь должна быть регистрация пользователя
            Console.WriteLine($"Регистрация: {RegisterName}, {RegisterEmail}, Тип: {AccountType}");
            
            // После успешной регистрации показываем главное окно
            ShowMainWindow();
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
        }
    }
    
    private void ShowMainWindow()
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
        
        // Корректное закрытие текущего окна и показ нового
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Устанавливаем новое главное окно до закрытия текущего
            var oldWindow = desktop.MainWindow;
            desktop.MainWindow = mainWindow;
            
            // Показываем новое окно
            mainWindow.Show();
            
            // Закрываем старое окно
            oldWindow?.Hide();
        }
        else
        {
            // Запасной вариант, если не удалось получить ApplicationLifetime
            mainWindow.Show();
            _parentWindow.Hide();
        }
    }
} 