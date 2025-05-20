using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PhotoStudio.Desktop.ViewModels;

namespace PhotoStudio.Desktop.Views;

public partial class LoginWindow : Window
{
    private Button? _loginTab;
    private Button? _registerTab;
    private ContentControl? _contentContainer;
    
    // Предзагруженные представления для вкладок
    private readonly LoginContentView _loginContentView;
    private readonly RegisterContentView _registerContentView;
    
    public LoginWindow()
    {
        InitializeComponent();
        
        // Создаем общую ViewModel для всего окна
        var viewModel = new LoginViewModel(this);
        DataContext = viewModel;
        
        // Инициализация элементов управления
        _loginTab = this.FindControl<Button>("LoginTab");
        _registerTab = this.FindControl<Button>("RegisterTab");
        _contentContainer = this.FindControl<ContentControl>("ContentContainer");
        
        // Создаем представления для вкладок
        _loginContentView = new LoginContentView
        {
            DataContext = viewModel
        };
        
        _registerContentView = new RegisterContentView
        {
            DataContext = viewModel
        };
        
        // Устанавливаем начальное содержимое (вкладка входа)
        if (_contentContainer != null)
        {
            _contentContainer.Content = _loginContentView;
        }
        
        // Для тестирования можно использовать:
        // SwitchToRegisterTab();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnLoginTabClick(object sender, RoutedEventArgs e)
    {
        SwitchToLoginTab();
    }
    
    private void OnRegisterTabClick(object sender, RoutedEventArgs e)
    {
        SwitchToRegisterTab();
    }
    
    private void SwitchToLoginTab()
    {
        if (_loginTab == null || _registerTab == null || _contentContainer == null)
            return;
        
        // Первым делом изменяем стиль вкладок
        _loginTab.BorderThickness = new Thickness(0, 0, 0, 2);
        _loginTab.BorderBrush = new SolidColorBrush(Color.Parse("#333333"));
        _loginTab.Foreground = new SolidColorBrush(Color.Parse("#333333"));
        _loginTab.FontWeight = FontWeight.SemiBold;
        
        _registerTab.BorderThickness = new Thickness(0);
        _registerTab.BorderBrush = null;
        _registerTab.Foreground = new SolidColorBrush(Color.Parse("#777777"));
        _registerTab.FontWeight = FontWeight.Normal;
        
        // Обновляем заголовок окна
        Title = "Вход в PhotoSpace";
        
        // Устанавливаем содержимое контейнера (вкладка входа)
        _contentContainer.Content = _loginContentView;
        
        // Принудительно обновляем размер окна
        InvalidateMeasure();
    }
    
    private void SwitchToRegisterTab()
    {
        if (_loginTab == null || _registerTab == null || _contentContainer == null)
            return;
        
        // Первым делом изменяем стиль вкладок
        _registerTab.BorderThickness = new Thickness(0, 0, 0, 2);
        _registerTab.BorderBrush = new SolidColorBrush(Color.Parse("#333333"));
        _registerTab.Foreground = new SolidColorBrush(Color.Parse("#333333"));
        _registerTab.FontWeight = FontWeight.SemiBold;
        
        _loginTab.BorderThickness = new Thickness(0);
        _loginTab.BorderBrush = null;
        _loginTab.Foreground = new SolidColorBrush(Color.Parse("#777777"));
        _loginTab.FontWeight = FontWeight.Normal;
        
        // Обновляем заголовок окна
        Title = "Регистрация в PhotoSpace";
        
        // Устанавливаем содержимое контейнера (вкладка регистрации)
        _contentContainer.Content = _registerContentView;
        
        // Принудительно обновляем размер окна
        InvalidateMeasure();
    }
    
    private void OnAccountTypeSelected(object sender, TappedEventArgs e)
    {
        if (sender is Border selectedBorder)
        {
            var supplierType = this.FindControl<Border>("SupplierType");
            var clientType = this.FindControl<Border>("ClientType");
            
            if (supplierType != null && clientType != null)
            {
                // Сбрасываем выделение для всех типов
                supplierType.Classes.Remove("selected");
                clientType.Classes.Remove("selected");
                
                // Устанавливаем выделение для выбранного типа
                selectedBorder.Classes.Add("selected");
                
                // Обновляем ViewModel с выбранным типом аккаунта
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.AccountType = selectedBorder.Name == "SupplierType" 
                        ? AccountType.Supplier 
                        : AccountType.Client;
                }
            }
        }
    }
} 