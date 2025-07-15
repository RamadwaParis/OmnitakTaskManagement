
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using OmintakProduction.Models;
// ...existing code...

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
        public DbSet<Team> Team { get; set; } // Add DbSet for Team

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Cascade delete: Deleting a project deletes its tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Cascade delete: Deleting a project deletes its tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete: Deleting a ticket deletes its tasks
            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Ticket)
                .WithMany(ti => ti.Tasks)
                .HasForeignKey(t => t.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskHistory>()
                .HasOne<Models.Task>()
                .WithMany()
                .HasForeignKey(th => th.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskHistory>()
                .HasOne<User>()
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
                new User { UserId = 1, RoleId = 1, UserName = "AdminUser", Email = "Admin1.seededData@omnitak.com", Password = "Admin@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 2, RoleId = 1, UserName = "AdminUser2", Email = "Admin2.seededData@omnitak.com", Password = "Admin@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 3, RoleId = 2, UserName = "EngineerUser1", Email = "Engineer1.seededData@omnitak.com", Password = "Engineer@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 4, RoleId = 2, UserName = "EngineerUser2", Email = "Engineer2.seededData@omnitak.com", Password = "Engineer@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 5, RoleId = 3, UserName = "TesterUser1", Email = "Tester1.seededData@omnitak.com", Password = "Tester@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 6, RoleId = 3, UserName = "TesterUser2", Email = "Tester2.seededData@omnitak.com", Password = "Tester@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26) },
                new User { UserId = 7, RoleId = 2, UserName = "Dreamer1", Email = "dreamer1@omnitak.com", Password = "Dream@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 1 },
                new User { UserId = 8, RoleId = 2, UserName = "Dreamer2", Email = "dreamer2@omnitak.com", Password = "Dream@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 1 },
                new User { UserId = 9, RoleId = 2, UserName = "Found1", Email = "found1@omnitak.com", Password = "Found@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 2 },
                new User { UserId = 10, RoleId = 2, UserName = "Found2", Email = "found2@omnitak.com", Password = "Found@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 2 },
                new User { UserId = 11, RoleId = 2, UserName = "Genty1", Email = "genty1@omnitak.com", Password = "Genty@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 3 },
                new User { UserId = 12, RoleId = 2, UserName = "Genty2", Email = "genty2@omnitak.com", Password = "Genty@123", isActive = true, CreatedDate = new DateOnly(2025, 06, 26), TeamId = 3 }
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
            new Role
            {
                RoleId = 1,
                UserId = 1,
                RoleName = "SystemAdmin"

            });
            modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 2,
                UserId = 2,
                RoleName = "Engineer"

            });
                        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 3,
                UserId = 3,
                RoleName = "Software Tester"

            });

            modelBuilder.Entity<Team>()
                .HasMany(t => t.TeamMembers)
                .WithOne(u => u.Team)
                .HasForeignKey(u => u.TeamId);

            modelBuilder.Entity<Team>().HasData(
                new Team { TeamId = 1, TeamName = "Dreamspace" },
                new Team { TeamId = 2, TeamName = "404Found" },
                new Team { TeamId = 3, TeamName = "Genty" }
            );
        }
    }
}
