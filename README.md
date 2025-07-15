# OmintakProduction
# Task Management System

A robust and user-friendly Task Management System built with ASP.NET Core MVC. This application provides streamlined functionality for user authentication, project tracking, task/ticket assignment, and user management, aimed at improving productivity and team collaboration.

## Features

- **User Management**
  - User registration and login
  - View a list of all users
  - Update or delete user information

- **Project Management**
  - Create new projects
  - View, update, and delete existing projects

- **Ticket Management**
  - Create, view, update, and delete task tickets
  - Assign tickets to users
  - Track task progress within projects

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- C#
- HTML, CSS (custom, no framework)

## Modern UI

- Fully custom, responsive CSS (see `wwwroot/css/site.css`)
- No CSS frameworks required
- Modern cards, navigation, buttons, and tables
- Mobile-friendly and accessible

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or VS Code

### Build and Run

1. Open a terminal in the project root (where `OmintakProduction.sln` is located).
2. Build the solution:
   ```powershell
   dotnet build OmintakProduction.sln
   ```
3. Run the application:
   ```powershell
   dotnet run --project .\OmintakProduction\OmintakProduction.csproj
   ```
4. Open the displayed local URL (e.g., `http://localhost:5000`) in your browser.

### Database
- Update the connection string in `appsettings.json` if needed.
- To apply migrations and create the database, use:
  ```powershell
  dotnet ef database update --project .\OmintakProduction\OmintakProduction.csproj
  ```

---

For more details, see the code and comments in each controller and view.
