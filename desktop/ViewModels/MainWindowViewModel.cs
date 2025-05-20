using System;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using PhotoStudio.Desktop.Models;
using ReactiveUI;

namespace PhotoStudio.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private ObservableCollection<Client> _clients;
    private ObservableCollection<Session> _sessions;
    private ObservableCollection<Photo> _photos;
    private int _selectedTabIndex;

    public MainWindowViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000/api/") };
        _clients = new ObservableCollection<Client>();
        _sessions = new ObservableCollection<Session>();
        _photos = new ObservableCollection<Photo>();

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        AddClientCommand = ReactiveCommand.CreateFromTask(AddClientAsync);
        AddSessionCommand = ReactiveCommand.CreateFromTask(AddSessionAsync);
        AddPhotoCommand = ReactiveCommand.CreateFromTask(AddPhotoAsync);
    }

    public ObservableCollection<Client> Clients
    {
        get => _clients;
        set => this.RaiseAndSetIfChanged(ref _clients, value);
    }

    public ObservableCollection<Session> Sessions
    {
        get => _sessions;
        set => this.RaiseAndSetIfChanged(ref _sessions, value);
    }

    public ObservableCollection<Photo> Photos
    {
        get => _photos;
        set => this.RaiseAndSetIfChanged(ref _photos, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> AddClientCommand { get; }
    public ReactiveCommand<Unit, Unit> AddSessionCommand { get; }
    public ReactiveCommand<Unit, Unit> AddPhotoCommand { get; }

    private async Task LoadDataAsync()
    {
        try
        {
            var clients = await _httpClient.GetFromJsonAsync<Client[]>("clients");
            var sessions = await _httpClient.GetFromJsonAsync<Session[]>("sessions");
            var photos = await _httpClient.GetFromJsonAsync<Photo[]>("photos");

            if (clients != null) Clients = new ObservableCollection<Client>(clients);
            if (sessions != null) Sessions = new ObservableCollection<Session>(sessions);
            if (photos != null) Photos = new ObservableCollection<Photo>(photos);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error loading data", ex.Message);
        }
    }

    private async Task AddClientAsync()
    {
        try
        {
            var client = new Client
            {
                Name = "New Client",
                Phone = "123-456-7890",
                Email = "client@example.com"
            };

            var response = await _httpClient.PostAsJsonAsync("clients", client);
            if (response.IsSuccessStatusCode)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error adding client", ex.Message);
        }
    }

    private async Task AddSessionAsync()
    {
        try
        {
            var session = new Session
            {
                Date = DateTime.Now,
                Duration = 60,
                Price = 1000,
                Status = "scheduled",
                ClientId = 1
            };

            var response = await _httpClient.PostAsJsonAsync("sessions", session);
            if (response.IsSuccessStatusCode)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error adding session", ex.Message);
        }
    }

    private async Task AddPhotoAsync()
    {
        try
        {
            var photo = new Photo
            {
                Filename = "photo.jpg",
                Path = "/photos/photo.jpg",
                SessionId = 1
            };

            var response = await _httpClient.PostAsJsonAsync("photos", photo);
            if (response.IsSuccessStatusCode)
            {
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Error adding photo", ex.Message);
        }
    }

    private async Task ShowErrorAsync(string title, string message)
    {
        var dialog = new MessageBoxManager.MessageBoxParams
        {
            Title = title,
            Message = message,
            Buttons = MessageBoxManager.MessageBoxButtons.Ok
        };

        await MessageBoxManager.GetMessageBoxStandard(dialog).ShowAsync();
    }
} 