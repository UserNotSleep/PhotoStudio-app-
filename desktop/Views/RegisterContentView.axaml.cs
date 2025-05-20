using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PhotoStudio.Desktop.ViewModels;

namespace PhotoStudio.Desktop.Views;

public partial class RegisterContentView : UserControl
{
    public RegisterContentView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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