using System;
using System.Diagnostics;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private LoginViewModel _loginViewModel;
    private RegistrationViewModel? _registrationViewModel;
    private PhotographerDashboardViewModel? _dashboardViewModel;
    private readonly ApiClient _apiClient;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel()
    {
        try
        {
            Console.WriteLine("Initializing MainWindowViewModel");
            
            // Initialize API client
            _apiClient = new ApiClient();
            
            // Initialize login view model
            _loginViewModel = new LoginViewModel(_apiClient);
            _loginViewModel.LoginSuccessful += OnLoginSuccessful;
            _loginViewModel.RegisterRequested += OnRegisterRequested;
            
            // Start with login view
            _currentViewModel = _loginViewModel;
            
            // We'll lazily initialize the registration view model to avoid any startup issues
            
            // Check for saved token
            // In a real app, you would use secure storage to retrieve the token
            // TryAutoLogin();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in MainWindowViewModel constructor: {ex.Message}");
            Debug.WriteLine($"Error in MainWindowViewModel constructor: {ex}");
        }
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
        try 
        {
            Console.WriteLine("Login successful, creating dashboard");
            // Create dashboard with the profile from login response
            _dashboardViewModel = new PhotographerDashboardViewModel(_apiClient, profile);
            _dashboardViewModel.LogoutRequested += OnLogoutRequested;
            
            // Switch to dashboard view
            CurrentViewModel = _dashboardViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnLoginSuccessful: {ex.Message}");
            Debug.WriteLine($"Error in OnLoginSuccessful: {ex}");
        }
    }
    
    private void OnRegistrationSuccessful(object? sender, PhotographerProfile profile)
    {
        try
        {
            Console.WriteLine("Registration successful, creating dashboard");
            // Create dashboard with the profile from registration response
            _dashboardViewModel = new PhotographerDashboardViewModel(_apiClient, profile);
            _dashboardViewModel.LogoutRequested += OnLogoutRequested;
            
            // Switch to dashboard view
            CurrentViewModel = _dashboardViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnRegistrationSuccessful: {ex.Message}");
            Debug.WriteLine($"Error in OnRegistrationSuccessful: {ex}");
        }
    }
    
    private void OnRegisterRequested(object? sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("Register requested, initializing registration viewmodel");
            
            // Lazy initialization of registration view model
            if (_registrationViewModel == null)
            {
                _registrationViewModel = new RegistrationViewModel(_apiClient);
                _registrationViewModel.RegistrationSuccessful += OnRegistrationSuccessful;
                _registrationViewModel.CancelRequested += OnCancelRegistration;
            }
            
            // Switch to registration view
            CurrentViewModel = _registrationViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnRegisterRequested: {ex.Message}");
            Debug.WriteLine($"Error in OnRegisterRequested: {ex}");
        }
    }
    
    private void OnCancelRegistration(object? sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("Registration cancelled, returning to login");
            // Switch back to login view
            CurrentViewModel = _loginViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnCancelRegistration: {ex.Message}");
            Debug.WriteLine($"Error in OnCancelRegistration: {ex}");
        }
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("Logout requested, clearing credentials");
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnLogoutRequested: {ex.Message}");
            Debug.WriteLine($"Error in OnLogoutRequested: {ex}");
        }
    }
}
