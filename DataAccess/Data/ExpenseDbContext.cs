using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Data
{
    public class ExpenseDbContext:DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(s => s.Groups) // Groups can have in many ApplicationUsers
            //    .WithMany(c => c.ApplicationUsers); // ApplicationUser can belong to  many Groups

            modelBuilder.Entity<ApplicationUser>()
                .Property(b => b.CurrentBalance)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Expense>()
                 .Property(e => e.Amount)
                 .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Expense>()
                .Property(e => e.IndividualAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = 1,
                    Name = "Admin1",
                    Email = "admin1@gmail.com",
                    Password = HashPassword("Admin@123"),
                    Roles = "Admin",
                    GroupsName = ""
                },
                new ApplicationUser
                {
                    Id = 2,
                    Name = "Admin2",
                    Email = "admin2@gmail.com",
                    Password = HashPassword("Admin@123"),
                    Roles = "Admin",
                    GroupsName = ""
                },
                new ApplicationUser
                {
                    Id = 3,
                    Name = "dpcode1",
                    Email = "dpcode1@gmail.com",
                    Password = HashPassword("User@123"),
                    Roles = "User",
                    GroupsName = ""
                },
                new ApplicationUser
                {
                    Id = 4,
                    Name = "dpcode2",
                    Email = "dpcode2@gmail.com",
                    Password = HashPassword("User@123"),
                    Roles = "User",
                    GroupsName = ""
                },
                new ApplicationUser
                {
                    Id = 5,
                    Name = "dpcode3",
                    Email = "dpcode3@gmail.com",
                    Password = HashPassword("User@123"),
                    Roles = "User",
                    GroupsName = ""
                },
                new ApplicationUser
                {
                    Id = 6,
                    Name = "dpcode4",
                    Email = "dpcode4@gmail.com",
                    Password = HashPassword("User@123"),
                    Roles = "User",
                    GroupsName = ""
                }
            );
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
