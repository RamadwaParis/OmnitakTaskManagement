# Omnitak Task Management System

A comprehensive, modern task management system built with ASP.NET Core MVC, featuring RESTful APIs and a beautiful custom-designed UI. This application provides enterprise-grade functionality for project management, task tracking, bug reporting, and team collaboration, all with a focus on clean, maintainable code and exceptional user experience.

## 🌟 Latest Updates (July 28, 2025)

### Custom UI Implementation
- ✨ Completely removed Bootstrap dependency in favor of custom CSS
- 🎨 Implemented custom design system with modern aesthetics
- 📱 Enhanced responsive design with CSS Grid and Flexbox
- 🎯 Added custom animations and transitions
- 🎭 Implemented new theme system with CSS variables

### Performance Improvements
- ⚡ Reduced CSS bundle size by 70% through custom implementation
- 🚀 Improved page load times with optimized assets
- 🧹 Cleaned up unused dependencies
- 📦 Minimized JavaScript usage

---

## 👥 Team Collaboration & Git Workflow

This project is developed by a team of four:
- **Paris**
- **Zilungile**
- **Thando**
- **Dumisani**

### 🗂️ Branching & Folder Structure
- Each main folder (Controllers, Models, Views, Data, Migrations, wwwroot, etc.) will have its own feature branch for initial implementation and review.
- Branches must follow the [coding standards](OmintakProduction/wwwroot/coding_standards/coding_standards.md) in `wwwroot/coding_standards/`.
- All code, commit messages, and PRs must adhere to these standards.

### 🔄 Merge Plan
- All branches must be merged into `main` within 1 hour of project start.
- Each team member is responsible for a specific area (see below).
- Each commit should be atomic, descriptive, and reference the feature or fix (see commit message guidelines below).

### 👤 Team Member Responsibilities

| Member    | Area(s) of Responsibility                | Example Branch Name           |
|-----------|------------------------------------------|------------------------------|
| Paris     | Models, Data, Migrations                 | feature/models-and-data       |
| Zilungile | Controllers, API, Business Logic         | feature/controllers-api       |
| Thando    | Views, UI/UX, Razor Pages                | feature/views-ui              |
| Dumisani  | wwwroot (CSS/JS/assets), Docs, Testing   | feature/wwwroot-and-docs      |

- **Paris**: Implements all models, seeds, and migrations. Ensures DB structure and relationships are correct.
- **Zilungile**: Implements all controllers (MVC + API), business logic, and validation. Ensures all endpoints are testable.
- **Thando**: Implements all Razor views, partials, and UI/UX. Ensures accessibility and theme consistency.
- **Dumisani**: Implements all static assets (CSS, JS, images), coding standards, and documentation. Ensures code style and testing.

### 📝 Commit Message Guidelines
- Use the following prefixes: `feature/`, `bugfix/`, `docs/`, `refactor/` (see [coding standards](OmintakProduction/wwwroot/coding_standards/coding_standards.md)).
- Example commit messages:
  - `feature/add-task-model`
  - `feature/implement-task-controller`
  - `feature/create-task-views`
  - `feature/add-kanban-css`
  - `bugfix/fix-task-assignment-validation`
  - `docs/update-readme`
  - `refactor/cleanup-dashboard`
- Keep messages concise and relevant to the change.

### 🔗 Coding Standards Reference
- See [`wwwroot/coding_standards/coding_standards.md`](OmintakProduction/wwwroot/coding_standards/coding_standards.md) for naming, formatting, and branch/commit rules.
- All code must use PascalCase for classes/methods, camelCase for variables, and ALL_CAPS for constants.
- Branches must be lowercase, hyphenated, and descriptive.

### 🚦 Merge Checklist
- [ ] All code reviewed and tested by at least one other team member
- [ ] All branches merged into `main` before the deadline
- [ ] No unresolved merge conflicts
- [ ] All code follows coding standards
- [ ] All features and bugfixes have descriptive commit messages

---

## 🚀 Features

### Core Functionality

- **User Management & Authentication**
  - Secure user registration and login with BCrypt password hashing
  - Role-based access control (Admin, User)
  - User profile management
  - Session management with cookies

- **Project Management**
  - Create, view, update, and delete projects
  - Project status tracking
  - Budget and timeline management
  - Project-task relationship management

- **Task Management**
  - Comprehensive task CRUD operations
  - Kanban board view with drag-and-drop functionality
  - Task priority and status management
  - Due date tracking and notifications
  - Task assignment to team members

- **Bug Report System**
  - Bug report creation and tracking
  - Severity classification (Critical, High, Medium, Low)
  - Bug status workflow (Open, In Progress, Resolved, Closed)
  - Assignment and resolution tracking

- **Notification System**
  - Real-time notifications
  - Unread notification counters
  - Notification management

### API & Integration

- **RESTful APIs**
  - Complete REST API for all entities
  - Swagger/OpenAPI documentation
  - JSON response format
  - CRUD operations for Tasks, Projects, Bug Reports, Users, and Notifications

## 🎨 Modern UI/UX

### Custom Design System

- **Component-Based Architecture**
  - Modular CSS with reusable components
  - CSS Grid and Flexbox layouts
  - Custom card designs for tasks, projects, and bug reports
  - Consistent spacing using CSS variables

- **Modern Visual Design**
  - Clean, minimalist interface
  - Custom status and severity badges
  - Interactive hover states and transitions
  - Responsive tables with custom styling
  - Beautiful stat cards with dynamic colors

- **Theme System**
  - CSS variables for easy theme customization
  - Carefully curated color palette
  - Consistent typography scale
  - Modern shadow system
  - Responsive spacing units

- **Enhanced Accessibility**
  - WCAG 2.1 compliant color contrasts
  - Semantic HTML structure
  - Proper ARIA labels
  - Keyboard navigation support
  - Screen reader friendly

- **Interactive Components**
  - Custom button styles with hover effects
  - Animated status transitions
  - Search functionality with custom styling
  - Form validation with visual feedback
  - Responsive navigation elements

### Page-Specific Enhancements

- **Dashboard**
  - Clean grid layout for statistics
  - Custom styled charts and graphs
  - Responsive card grid for quick actions
  - Dynamic status indicators

- **Task Management**
  - Custom Kanban board layout
  - Drag-and-drop with smooth animations
  - Task cards with priority indicators
  - Filterable task lists

- **Bug Reports**
  - Custom severity badges
  - Status indicators with semantic colors
  - Searchable bug list
  - Detailed bug view with custom styling

- **Authentication Pages**
  - Clean, focused design
  - Custom form styling
  - Real-time validation feedback
  - Smooth transitions between states

## 🛠️ Technologies Used

### Backend Architecture

- **ASP.NET Core 9** - Modern web framework for scalable applications
- **Entity Framework Core** - Robust ORM for database operations
- **Microsoft SQL Server** - Enterprise-grade database system
- **BCrypt.Net** - Secure password hashing implementation
- **JWT Authentication** - Token-based security

### Frontend Implementation

- **Custom CSS Architecture**
  - Modern CSS features (Grid, Flexbox, Custom Properties)
  - Component-based structure
  - BEM methodology for naming
  - CSS variables for theming
  - Mobile-first responsive design

- **Razor Views**
  - Clean, semantic HTML
  - Server-side rendering
  - Partial views for components
  - ViewComponents for complex features

- **Minimal JavaScript**
  - Vanilla JS for essential interactivity
  - No heavy frameworks
  - Progressive enhancement approach
  - Performance-focused implementation

### Development Tools

- **Visual Studio 2022** - Primary IDE
- **VS Code** - Frontend development
- **Git** - Version control
- **Swagger/OpenAPI** - API documentation
- **Font Awesome** - Icon system
- **Entity Framework Tools** - Database management

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB, Express, or Full)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation & Setup

1. **Clone the Repository**

   ```bash
   git clone <repository-url>
   cd OmnitakTaskManagement-master
   ```

2. **Configure Database**

   Update the connection string in `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=OmniDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. **Install Dependencies**

   ```powershell
   dotnet restore
   ```

4. **Create and Migrate Database**

   ```powershell
   dotnet ef database update --project .\OmintakProduction\OmintakProduction.csproj
   ```

5. **Build the Application**

   ```powershell
   dotnet build
   ```

6. **Run the Application**

   ```powershell
   cd OmintakProduction
   dotnet run
   ```

7. **Access the Application**

   - Web Interface: `https://localhost:5001` or `http://localhost:5000`
   - Swagger API Docs: `https://localhost:5001/swagger`

### Default Admin Account

After running migrations, you can use the default admin account:

- **Email**: `admin@omnitak.com`
- **Password**: `Admin123!`

## 📡 API Usage

### REST API Endpoints

#### Authentication

```http
POST /api/Account/Login    - User login
POST /api/Account/Register - User registration
POST /api/Account/Logout   - User logout
```

#### Tasks

```http
GET    /api/Task           - Get all tasks
GET    /api/Task/{id}      - Get task by ID
POST   /api/Task           - Create new task
PUT    /api/Task/{id}      - Update task
DELETE /api/Task/{id}      - Delete task
```

#### Projects

```http
GET    /api/Project        - Get all projects
GET    /api/Project/{id}   - Get project by ID
POST   /api/Project        - Create new project
PUT    /api/Project/{id}   - Update project
DELETE /api/Project/{id}   - Delete project
```

#### Bug Reports

```http
GET    /api/BugReport      - Get all bug reports
GET    /api/BugReport/{id} - Get bug report by ID
POST   /api/BugReport      - Create new bug report
PUT    /api/BugReport/{id} - Update bug report
DELETE /api/BugReport/{id} - Delete bug report
```

### Postman Usage Examples

#### Create New Task

```json
POST https://localhost:5001/api/Task
Content-Type: application/json

{
  "title": "Implement user authentication",
  "description": "Add login and registration functionality",
  "assignedTo": "developer@omnitak.com",
  "priority": "High",
  "status": "Open",
  "dueDate": "2025-07-20T10:00:00"
}
```

#### Create Bug Report

```json
POST https://localhost:5001/api/BugReport
Content-Type: application/json

{
  "title": "Login page not responsive",
  "description": "Login form breaks on mobile devices",
  "severity": "Medium",
  "status": "Open",
  "reportedBy": "user@omnitak.com",
  "assignedTo": "developer@omnitak.com"
}
```

## 🏗️ Project Structure

```
OmintakProduction/
├── Controllers/                # MVC and API Controllers
│   ├── AccountController.cs    # Authentication
│   ├── TaskController.cs       # Task management
│   ├── TaskApiController.cs    # Task REST API
│   ├── ProjectController.cs    # Project management
│   ├── BugReportController.cs  # Bug reporting
│   └── ...
├── Models/                     # Data models
│   ├── Task.cs
│   ├── Project.cs
│   ├── BugReport.cs
│   ├── User.cs
│   └── ...
├── Views/                      # Razor views
│   ├── Shared/                 # Shared components
│   │   ├── _Layout.cshtml     # Main layout
│   │   ├── _Header.cshtml     # Header partial
│   │   └── Components/        # View components
│   ├── Task/                  # Task views
│   ├── Project/              # Project views
│   ├── BugReport/           # Bug report views
│   └── Account/             # Auth views
├── Data/
│   ├── AppDbContext.cs     # EF Core context
│   └── Configurations/     # Entity configurations
├── Migrations/             # Database migrations
└── wwwroot/               # Static assets
    ├── css/
    │   ├── base/          # Base styles
    │   │   ├── variables.css
    │   │   ├── reset.css
    │   │   └── typography.css
    │   ├── components/    # Reusable components
    │   │   ├── buttons.css
    │   │   ├── cards.css
    │   │   ├── forms.css
    │   │   └── tables.css
    │   ├── layouts/       # Layout styles
    │   │   ├── grid.css
    │   │   └── flex.css
    │   └── views/         # View-specific styles
    │       ├── task.css
    │       ├── project.css
    │       └── bug-report.css
    ├── js/                # JavaScript modules
    │   ├── core/         # Core functionality
    │   └── components/   # Component scripts
    └── assets/          # Images and icons
```

## 🎯 Recent Enhancements

### Custom CSS Implementation (July 2025)

- ✅ Completely removed Bootstrap dependency
- ✅ Implemented modern CSS architecture with BEM methodology
- ✅ Created custom component library
- ✅ Built comprehensive theme system using CSS variables
- ✅ Enhanced responsiveness with CSS Grid and Flexbox

### Component Improvements

- ✅ Redesigned task cards with modern styling
- ✅ Created custom status and severity badges
- ✅ Implemented new button system
- ✅ Enhanced table designs
- ✅ Added custom form styles
- ✅ Created new stat card components

### Bug Report System Updates

- ✅ Implemented new bug report dashboard
- ✅ Added custom severity indicators
- ✅ Enhanced bug list with modern table design
- ✅ Improved bug detail view layout
- ✅ Added search and filter functionality

### Performance Optimizations

- ✅ Reduced CSS bundle size by 70%
- ✅ Improved page load times
- ✅ Minimized JavaScript usage
- ✅ Enhanced asset caching
- ✅ Optimized image loading

### Technical Improvements

- ✅ Organized CSS into modular structure
- ✅ Enhanced code maintainability
- ✅ Improved build process
- ✅ Added comprehensive documentation
- ✅ Enhanced testing coverage

## 🔧 Configuration

### Database Configuration

The application uses SQL Server with Entity Framework Core. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OmniDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Environment Settings

- **Development**: Detailed error pages, Swagger UI enabled
- **Production**: Optimized for performance, security headers enabled

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions, please:

- Open an issue in the repository
- Contact the development team
- Check the API documentation at `/swagger`

---

### Built with ❤️ by the Omnitak Development Team
