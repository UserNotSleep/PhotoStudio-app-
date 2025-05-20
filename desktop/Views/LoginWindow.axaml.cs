using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhotoStudio.Desktop.ViewModels;

namespace PhotoStudio.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new LoginViewModel(this);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 