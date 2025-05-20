using Avalonia;
using System;
using System.Text;
using System.Globalization;
using System.Threading;

namespace PhotoStudio.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Установка русской локализации и кодировки
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
        
        // Регистрация кодировок для поддержки кириллицы
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
} 