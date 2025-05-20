# Photo Studio Management System

This is a Photo Studio Management System consisting of a Flask server for database management and a C# Avalonia UI desktop application for the user interface.

## Project Structure

```
.
├── server/
│   ├── app.py
│   └── requirements.txt
├── desktop/
│   ├── PhotoStudio.Desktop.csproj
│   ├── Program.cs
│   ├── Models/
│   │   ├── Client.cs
│   │   ├── Session.cs
│   │   └── Photo.cs
│   ├── ViewModels/
│   │   └── MainWindowViewModel.cs
│   └── Views/
│       ├── MainWindow.axaml
│       └── MainWindow.axaml.cs
└── README.md
```

## Setup Instructions

### Server Setup

1. Navigate to the server directory:
   ```
   cd server
   ```

2. Create a virtual environment (optional but recommended):
   ```
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   ```

3. Install dependencies:
   ```
   pip install -r requirements.txt
   ```

4. Run the server:
   ```
   python app.py
   ```
   The server will start on http://localhost:5000

### Desktop Application Setup

1. Navigate to the desktop directory:
   ```
   cd desktop
   ```

2. Restore NuGet packages:
   ```
   dotnet restore
   ```

3. Build and run the application:
   ```
   dotnet run
   ```

## Features

### Server
- RESTful API endpoints for managing clients, sessions, and photos
- SQLite database for data storage
- CORS support for cross-origin requests

### Desktop Application
- Modern GUI built with Avalonia UI
- Three main tabs:
  1. Clients Management
  2. Sessions Management
  3. Photos Management
- Real-time data synchronization with the server
- User-friendly forms for adding new entries
- Tabular view of all data

## Usage

1. Start the server first
2. Then start the desktop application
3. Use the tabs to manage different aspects of the photo studio:
   - Add and view clients
   - Schedule and manage photo sessions
   - Track photos associated with sessions

## Requirements

### Server
- Python 3.8 or higher
- Flask and its dependencies
- SQLite3

### Desktop Application
- .NET 7.0 SDK or higher
- Avalonia UI
- Windows, macOS, or Linux operating system 