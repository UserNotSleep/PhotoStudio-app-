using System;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private LoginViewModel _loginViewModel;
    private RegisterViewModel _registerViewModel;
    private PhotographerDashboardViewModel? _dashboardViewModel;
    private readonly ApiClient _apiClient;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel()
    {
        // Initialize API client
        _apiClient = new ApiClient();
        
        // Initialize login view model
        _loginViewModel = new LoginViewModel(_apiClient);
        _loginViewModel.LoginSuccessful += OnLoginSuccessful;
        _loginViewModel.GoToRegister += OnGoToRegister;
        
        // Initialize register view model
        _registerViewModel = new RegisterViewModel(_apiClient);
        _registerViewModel.RegistrationSuccessful += OnLoginSuccessful; // Reuse the same handler
        _registerViewModel.GoToLogin += OnGoToLogin;
        
        // Start with login view
        _currentViewModel = _loginViewModel;
        
        // Check for saved token
        // In a real app, you would use secure storage to retrieve the token
        // TryAutoLogin();
    }
    
    /*
    private async void TryAutoLogin()
    {
        // In a real app, you would check for a saved token
        // var savedToken = AppSettings.GetSetting("AuthToken", string.Empty);
        
        // if (!string.IsNullOrEmpty(savedToken))
        // {
        //     try
        //     {
        //         _apiClient.AuthToken = savedToken;
        //         var profile = await _apiClient.GetPhotographerProfileAsync();
        //         _dashboardViewModel = new PhotographerDashboardViewModel(_apiClient, profile);
        //         _dashboardViewModel.LogoutRequested += OnLogoutRequested;
        //         CurrentViewModel = _dashboardViewModel;
        //     }
        //     catch
        //     {
        //         // Token invalid or expired, proceed to login
        //         _apiClient.AuthToken = null;
        //     }
        // }
    }
    */

    private void OnLoginSuccessful(object? sender, PhotographerProfile profile)
    {
        // Create dashboard with the profile from login response
        _dashboardViewModel = new PhotographerDashboardViewModel(_apiClient, profile);
        _dashboardViewModel.LogoutRequested += OnLogoutRequested;
        
        // Switch to dashboard view
        CurrentViewModel = _dashboardViewModel;
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        // Clear credentials and token
        _apiClient.AuthToken = string.Empty;
        _loginViewModel.Username = string.Empty;
        _loginViewModel.Password = string.Empty;
        _loginViewModel.ErrorMessage = string.Empty;
        
        // Switch back to login view
        CurrentViewModel = _loginViewModel;
        
        // Clean up dashboard to avoid memory leaks
        if (_dashboardViewModel != null)
        {
            _dashboardViewModel.LogoutRequested -= OnLogoutRequested;
            _dashboardViewModel = null;
        }
    }
    
    private void OnGoToRegister(object? sender, EventArgs e)
    {
        // Switch to register view
        CurrentViewModel = _registerViewModel;
    }
    
    private void OnGoToLogin(object? sender, EventArgs e)
    {
        // Switch back to login view
        CurrentViewModel = _loginViewModel;
    }
}
