using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using OmintakProduction.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                
                new User
                {
                    UserId = 1,
                    RoleId = 1,
                    UserName = "AdminUser",
                    Email = "Admin1.seededData@omnitak.com",
                    Password = "Admin@123",
                    isActive = true,
                    CreatedDate = new DateOnly(2025, 06, 26)
                }
             );

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    UserId = 2,
                    RoleId = 1,
                    UserName = "AdminUser2",
                    Email = "Admin2.seededData@omnitak.com",
                    Password = "Admin@123",
                    isActive = true,
                    CreatedDate = new DateOnly(2025, 06, 26)
                }
             );

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    UserId = 3,
                    RoleId =2,
                    UserName = "EngineerUser1",
                    Email = "Engineer1.seededData@omnitak.com",
                    Password = "Engineer@123",
                    isActive = true,
                    CreatedDate = new DateOnly(2025, 06, 26)
                }
             );

            modelBuilder.Entity<User>().HasData(

               new User
               {
                   UserId = 4,
                   RoleId = 2,
                   UserName = "EngineerUser2",
                   Email = "Engineer2.seededData@omnitak.com",
                   Password = "Engineer@123",
                   isActive = true,
                   CreatedDate = new DateOnly(2025, 06, 26)
               }
            );


            modelBuilder.Entity<User>().HasData(

                new User
                {
                    UserId = 5,
                    RoleId =3,
                    UserName = "TesterUser1",
                    Email = "Tester1.seededData@omnitak.com",
                    Password = "Tester@123",
                    isActive = true,
                    CreatedDate = new DateOnly(2025, 06, 26)
                }
             );

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    UserId = 6,
                    RoleId = 3,
                    UserName = "TesterUser2",
                    Email = "Tester2.seededData@omnitak.com",
                    Password = "Tester@123",
                    isActive = true,
                    CreatedDate = new DateOnly(2025, 06, 26)
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

        }
    }
}
