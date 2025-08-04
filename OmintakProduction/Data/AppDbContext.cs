
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using OmintakProduction.Models;
using BCrypt.Net;

namespace OmintakProduction.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectReport> ProjectReport { get; set; }
        public DbSet<TestReport> TestReport { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignment { get; set; }
        public DbSet<TaskComment> TaskComment { get; set; }
        public DbSet<TaskHistory> TaskHistory { get; set; }
        public DbSet<BugReport> BugReports { get; set; }
        public DbSet<Team> Team { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Cascade delete: Deleting a project deletes its tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Ticket.AssignedToUser relationship
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Ticket.DeletedByUser relationship for soft delete audit trail
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.DeletedByUser)
                .WithMany()
                .HasForeignKey(t => t.DeletedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cascade delete: Deleting a ticket deletes its tasks
            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Ticket)
                .WithMany(ti => ti.Tasks)
                .HasForeignKey(t => t.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-many relationship for AssignedUsers in Task
            modelBuilder.Entity<Models.Task>()
                .HasMany(t => t.AssignedUsers)
                .WithMany()
                .UsingEntity(j => j.ToTable("TaskAssignedUsers"));

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Task.DeletedByUser relationship for soft delete audit trail
            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.DeletedByUser)
                .WithMany()
                .HasForeignKey(t => t.DeletedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // TaskHistory configuration - use NoAction to prevent cascade cycles
            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.Task)
                .WithMany()
                .HasForeignKey(th => th.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.User)
                .WithMany()
                .HasForeignKey(th => th.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BugReport>()
                .HasOne(b => b.ReportedByUser)
                .WithMany()
                .HasForeignKey(b => b.ReportedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BugReport>()
                .HasOne(b => b.AssignedToUser)
                .WithMany()
                .HasForeignKey(b => b.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BugReport>()
                .HasOne(b => b.Task)
                .WithMany()
                .HasForeignKey(b => b.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskComment>()
                .HasOne<Models.Task>()
                .WithMany()
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskComment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>().HasData(
                // System Administrator - Password: password - Only user remaining
                new User {
                    UserId = 1,
                    RoleId = 1,
                    UserName = "Tee",
                    Email = "uthandocibi@gmail.com",
                    FirstName = "Thando",
                    LastName = "Cibi",
                    Password = "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", // password
                    isActive = true,
                    IsApproved = true,
                    CreatedDate = new DateOnly(2025, 01, 01),
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 1,
                    Title = "Sample Ticket",
                    Description = "This is a sample ticket description.",
                    Status = "ToDo",
                    DueDate = new DateOnly(2025, 06, 26),
                    CreatedAt = new DateOnly(2025, 06, 26),
                    CompletedAt = new DateOnly(2025, 06, 26),
                });

            modelBuilder.Entity<Ticket>().HasData(
               new Ticket
               {
                   Id = 2,
                   Title = "Sample Ticket",
                   Description = "This is a sample ticket description.",
                   Status = "In_Progress",
                   DueDate = new DateOnly(2025, 06, 26),
                   CreatedAt = new DateOnly(2025, 06, 26),
                   CompletedAt = new DateOnly(2025, 06, 26),
               });

            modelBuilder.Entity<Ticket>().HasData(
   new Ticket
   {
       Id = 3,
       Title = "Sample Ticket",
       Description = "This is a sample ticket description.",
       Status = "In_Review",
       DueDate = new DateOnly(2025, 06, 26),
       CreatedAt = new DateOnly(2025, 06, 26),
       CompletedAt = new DateOnly(2025, 06, 26),
   });

            modelBuilder.Entity<Ticket>().HasData(
           new Ticket
           {
               Id = 4,
               Title = "Sample Ticket",
               Description = "This is a sample ticket description.",
               Status = "Done",
               DueDate = new DateOnly(2025, 06, 26),
               CreatedAt = new DateOnly(2025, 06, 26),
               CompletedAt = new DateOnly(2025, 06, 26),
           });

            modelBuilder.Entity<Project>().HasData(
           new Project
           {
               ProjectId = 4,
               ProjectName = "Sample Ticket",
               Description = "This is a sample ticket description.",
               Status = "Done",
               DueDate = new DateOnly(2025, 06, 26),

           });

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "SystemAdmin" },
                new Role { RoleId = 2, RoleName = "Developer" },
                new Role { RoleId = 3, RoleName = "Tester" },
                new Role { RoleId = 4, RoleName = "Stakeholder" },
                new Role { RoleId = 5, RoleName = "TeamLead" }
            );

            modelBuilder.Entity<Team>()
                .HasMany(t => t.TeamMembers)
                .WithOne(u => u.Team)
                .HasForeignKey(u => u.TeamId);

            modelBuilder.Entity<Team>().HasData(
                new Team { TeamId = 1, TeamName = "Dreamspace" },
                new Team { TeamId = 2, TeamName = "404Found" },
                new Team { TeamId = 3, TeamName = "Genty" }
            );

            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamLead) // Navigation property in Team
                .WithMany() // No inverse navigation in User
                .HasForeignKey(t => t.TeamLeadId) // Foreign key in Team
                .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascading deletes

            base.OnModelCreating(modelBuilder);
        }
    }
}
