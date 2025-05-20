using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PhotoStudio.Desktop.Views;
using ReactiveUI;

namespace PhotoStudio.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly Window _parentWindow;

    public LoginViewModel(Window parentWindow)
    {
        _parentWindow = parentWindow;
        LoginCommand = ReactiveCommand.Create(Login);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private void Login()
    {
        try
        {
            // В реальном приложении здесь должна быть проверка учетных данных
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
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка при входе в систему: {ex.Message}");
        }
    }
} 