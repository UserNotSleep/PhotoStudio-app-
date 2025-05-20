using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoApp.Models;
using PhotoApp.Models.Dtos;
using ReactiveUI;

namespace PhotoApp.ViewModels
{
    public class PhotographerDashboardViewModel : ViewModelBase
    {
        private readonly ApiClient _apiClient;
        private PhotographerProfile _photographerProfile;
        private string _currentView = "Dashboard";
        private bool _isLoading = false;
        private DashboardStats _dashboardStats;
        private ObservableCollection<PhotoGallery> _galleries;
        private ObservableCollection<Client> _clients;
        private ObservableCollection<Order> _orders;

        public string Username => _photographerProfile?.Name ?? _photographerProfile?.Username ?? "Фотограф";

        public string CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public DashboardStats DashboardStats
        {
            get => _dashboardStats;
            set => this.RaiseAndSetIfChanged(ref _dashboardStats, value);
        }

        public ObservableCollection<PhotoGallery> Galleries
        {
            get => _galleries;
            set => this.RaiseAndSetIfChanged(ref _galleries, value);
        }

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => this.RaiseAndSetIfChanged(ref _orders, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand RefreshDataCommand { get; }

        public event EventHandler LogoutRequested;

        public PhotographerDashboardViewModel(ApiClient apiClient, PhotographerProfile profile)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _photographerProfile = profile ?? throw new ArgumentNullException(nameof(profile));
            
            // Initialize collections
            Galleries = new ObservableCollection<PhotoGallery>();
            Clients = new ObservableCollection<Client>();
            Orders = new ObservableCollection<Order>();
            
            // Initialize commands
            LogoutCommand = ReactiveCommand.Create(ExecuteLogout);
            NavigateCommand = ReactiveCommand.Create<string>(ExecuteNavigate);
            RefreshDataCommand = ReactiveCommand.CreateFromTask(LoadDashboardDataAsync);
            
            // Load data
            LoadDashboardDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;
                
                // Load dashboard statistics
                DashboardStats = await _apiClient.GetDashboardStatsAsync();
                
                // Load galleries if we're on the photos view
                if (CurrentView == "Photos")
                {
                    var galleries = await _apiClient.GetGalleriesAsync();
                    Galleries.Clear();
                    foreach (var gallery in galleries)
                    {
                        Galleries.Add(gallery);
                    }
                }
                
                // Load clients if we're on the clients view
                if (CurrentView == "Clients")
                {
                    var clients = await _apiClient.GetClientsAsync();
                    Clients.Clear();
                    foreach (var client in clients)
                    {
                        Clients.Add(client);
                    }
                }
                
                // Load orders if we're on the orders view
                if (CurrentView == "Orders")
                {
                    var orders = await _apiClient.GetOrdersAsync();
                    Orders.Clear();
                    foreach (var order in orders)
                    {
                        Orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                // In a real app, you would handle exceptions more gracefully
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteLogout()
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteNavigate(string viewName)
        {
            CurrentView = viewName;
            
            // Refresh data when navigating to a new view
            LoadDashboardDataAsync().ConfigureAwait(false);
        }
    }
} 