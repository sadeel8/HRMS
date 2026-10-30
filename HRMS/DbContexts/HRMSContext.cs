using HRMS.Models;
using Microsoft.EntityFrameworkCore;
namespace HRMS.DbContexts

{
    public class HRMSContext : DbContext
    {
        public HRMSContext(DbContextOptions<HRMSContext> options) : base(options)
        {
            // options
            //sql provider:sql server, oracle,...
            // connection string 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seeding 
            modelBuilder.Entity<Lookup>().HasData(
                //Major Code=0 ----> a value in which I give a name to this category
                //Major Code = 0 ----> Employees Postions
                new Lookup { Id = 1, MajorCode = 0, MinorCode = 0, Name = "Employees Postions" },
                new Lookup { Id = 2, MajorCode = 0, MinorCode = 1, Name = "HR Postions" },
                new Lookup { Id = 3, MajorCode = 0, MinorCode = 2, Name = "Manager" },
                new Lookup { Id = 4, MajorCode = 0, MinorCode = 3, Name = "Developer" },

                // Major Code=1 -----> Departments Types
                new Lookup { Id = 5, MajorCode = 1, MinorCode = 0, Name = "Departments Types" },
                new Lookup { Id = 6, MajorCode = 1, MinorCode = 1, Name = "Finance" },
                new Lookup { Id = 7, MajorCode = 1, MinorCode = 2, Name = "Adminstrative" },
                new Lookup { Id = 8, MajorCode = 1, MinorCode = 3, Name = "Technical" },

                // Major Code=2 -----> Vacation Types
                new Lookup { Id = 9, MajorCode = 2, MinorCode = 0, Name = "Vacation Types" },
                new Lookup { Id = 10, MajorCode = 2, MinorCode = 1, Name = "Annual Vacation" },
                new Lookup { Id = 11, MajorCode = 2, MinorCode = 2, Name = "Sick Vacation" },
                new Lookup { Id = 12, MajorCode = 2, MinorCode = 3, Name = "Unpaid Vacation" }

                );
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Admain", IsAdmin = true, HashedPassword = "$2a$11$673MThCdXOm3VNFrrwpHs./9FkXlpjWYAzTcX55avSC3jkJQS0fn." });

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
               .HasIndex(x => x.UserId)
               .IsUnique();
        }


        // Table ---> DbSet 
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vacation> Vacations { get; set; }





    }
}
