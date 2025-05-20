using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using PhotoApp.ViewModels;
using PhotoApp.Views;

namespace PhotoApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Устанавливаем обработчик необработанных исключений
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        LogException("Unhandled Exception", exception);
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogException("Unobserved Task Exception", e.Exception);
        e.SetObserved(); // Отмечаем как обработанное
    }
    
    private void LogException(string type, Exception? exception)
    {
        try
        {
            string message = $"[{DateTime.Now}] {type}: ";
            
            if (exception != null)
            {
                message += $"{exception.Message}\n{exception.StackTrace}";
                
                if (exception.InnerException != null)
                {
                    message += $"\nInner Exception: {exception.InnerException.Message}\n{exception.InnerException.StackTrace}";
                }
            }
            else
            {
                message += "Unknown error occurred.";
            }
            
            Console.WriteLine(message);
            
            // Логирование в файл
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            File.AppendAllText(logPath, message + "\n\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log exception: {ex.Message}");
        }
    }
}