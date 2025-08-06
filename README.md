# Omnitak Task Management System

A comprehensive, modern task management system built with ASP.NET Core MVC, featuring role-based dashboards, RESTful APIs, and a beautiful Microsoft 365-inspired UI. This application provides enterprise-grade functionality for project management, task tracking, bug reporting, and team collaboration with role-specific access control.

## 🌟 Latest Updates (August 2025)

### Role-Based Dashboard System
- ✨ Implemented role-specific dashboards for different user types
- 🎯 System Admin, Developer, Tester, Team Lead, and Stakeholder views
- 📊 Customized data and functionality based on user permissions
- 🔐 Enhanced security with role-based access control

### Microsoft 365 UI Theme
- 🎨 Modern Microsoft 365-inspired design system
- 📱 Fully responsive layout with clean, professional aesthetics
- 🎭 Consistent color scheme and typography
- ⚡ Improved performance with optimized CSS

### Enhanced Authentication & User Management
- 🔒 Secure BCrypt password hashing
- 👥 User approval system for new registrations
- � Claims-based authentication with role management
- 🔑 System admin controls for user management

### Build & Deployment Improvements
- 🧹 Cleaned up duplicate files and build conflicts
- 🚀 Streamlined build process with error-free compilation
- 📦 Optimized project structure and dependencies
- ✅ Comprehensive migration system

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

- **Role-Based Dashboard System**
  - **System Admin Dashboard**: Complete system oversight, user management, and system statistics
  - **Developer Dashboard**: Assigned tasks, development progress tracking, and code-related activities
  - **Tester Dashboard**: Testing tasks, bug reports, and quality assurance metrics
  - **Team Lead Dashboard**: Team management, project oversight, and performance tracking
  - **Stakeholder Dashboard**: High-level project status, budget tracking, and strategic metrics

- **User Management & Authentication**
  - Secure user registration with admin approval system
  - Role-based access control (SystemAdmin, Developer, Tester, TeamLead, Stakeholder)
  - BCrypt password hashing for enhanced security
  - Claims-based authentication with session management
  - User profile management and status tracking

- **Project Management**
  - Comprehensive project CRUD operations
  - Project status tracking and timeline management
  - Budget allocation and expense tracking
  - Team assignment and project-task relationships
  - Progress monitoring and reporting

- **Task Management**
  - Advanced task CRUD operations with priority levels
  - Task assignment to multiple team members
  - Status workflow management (Todo, InProgress, Completed, OnHold)
  - Due date tracking with overdue notifications
  - Task dependencies and milestone tracking

- **Enhanced Bug Report System**
  - Comprehensive bug tracking with severity levels
  - Bug lifecycle management (Open, InProgress, Resolved, Closed)
  - Assignment workflow and resolution tracking
  - Integration with task management system
  - Detailed bug analytics and reporting

- **Notification & Communication System**
  - Real-time notifications for task updates
  - Role-specific notification filtering
  - Unread notification counters
  - System-wide announcements
  - Email integration capabilities

### API & Integration

- **RESTful APIs**
  - Complete REST API for all entities
  - Swagger/OpenAPI documentation
  - JSON response format
  - CRUD operations for Tasks, Projects, Bug Reports, Users, and Notifications

## 🎨 Modern UI/UX

### Microsoft 365-Inspired Design

- **Professional Theme System**
  - Clean, modern Microsoft 365-inspired interface
  - Consistent color palette with professional aesthetics
  - Modern typography with excellent readability
  - Responsive design that works on all devices
  - Accessibility-first approach with WCAG compliance

- **Role-Specific Dashboards**
  - Customized layouts for each user role
  - Role-appropriate widgets and statistics
  - Contextual navigation and actions
  - Personalized data visualization
  - Intuitive user experience design

- **Component-Based Architecture**
  - Modular CSS with reusable components
  - CSS Grid and Flexbox for modern layouts
  - Custom card designs with consistent styling
  - Interactive elements with smooth animations
  - Responsive tables and data grids

- **Enhanced User Experience**
  - Clean, intuitive navigation structure
  - Context-aware menus and actions
  - Real-time feedback and notifications
  - Progressive disclosure of information
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

- **ASP.NET Core 9** - Latest .NET framework for high-performance web applications
- **Entity Framework Core** - Advanced ORM with comprehensive migration system
- **Microsoft SQL Server** - Enterprise-grade database with full relational support
- **BCrypt.Net** - Industry-standard password hashing for maximum security
- **Claims-Based Authentication** - Modern authentication with role-based authorization

### Frontend Implementation

- **Microsoft 365 Theme**
  - Professional, clean design language
  - Responsive CSS Grid and Flexbox layouts
  - CSS Custom Properties for consistent theming
  - Mobile-first responsive design approach
  - Optimized for performance and accessibility

- **Razor Views & Components**
  - Server-side rendering for optimal SEO
  - Reusable ViewComponents for complex features
  - Partial views for modular development
  - Strongly-typed models for compile-time safety

- **Modern JavaScript**
  - Vanilla JavaScript for essential interactivity
  - Progressive enhancement approach
  - Minimal dependencies for fast load times
  - ES6+ features with broad compatibility

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

### Default System Admin Account

After running database migrations, you can log in using the system admin account:

- **Email**: `uthandocibi@gmail.com`
- **Password**: `password`
- **Role**: System Administrator

**System Admin Capabilities:**
- ✅ Approve/reject new user registrations
- ✅ Manage all users and their roles
- ✅ Access complete system overview dashboard
- ✅ View all projects, tasks, and bug reports
- ✅ System configuration and maintenance
- ✅ Generate system-wide reports and analytics

**Security Note**: Please change the default password after first login for security purposes.

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

### Role-Based Dashboard System (August 2025)

- ✅ Implemented complete role-based dashboard architecture
- ✅ Created role-specific view models and controllers
- ✅ Built customized dashboards for all user roles
- ✅ Enhanced security with role-based access control
- ✅ Integrated claims-based authentication system

### UI/UX Improvements

- ✅ Implemented Microsoft 365-inspired design theme
- ✅ Fixed dashboard card styling and responsiveness
- ✅ Enhanced sidebar navigation with proper alignment
- ✅ Improved overall visual consistency and user experience
- ✅ Optimized CSS for better performance

### System Administration Features

- ✅ Built comprehensive user management system
- ✅ Implemented user approval workflow
- ✅ Created system admin dashboard with full oversight
- ✅ Enhanced security with proper password hashing
- ✅ Added role-based navigation and permissions

### Technical Improvements

- ✅ Cleaned up build errors and duplicate files
- ✅ Streamlined project structure and dependencies
- ✅ Improved database migration system
- ✅ Enhanced error handling and validation
- ✅ Optimized application performance

### Database & Security Enhancements

- ✅ Updated password hashing to BCrypt with proper salt rounds
- ✅ Implemented secure user seeding and migrations
- ✅ Enhanced role-based security throughout the application
- ✅ Improved data validation and entity relationships
- ✅ Added comprehensive audit trails and logging

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

**Latest Updates**: August 2025
- Enhanced role-based dashboard system
- Microsoft 365-inspired UI improvements  
- Comprehensive user management and security
- Streamlined build process and deployment
- Professional-grade task and project management
