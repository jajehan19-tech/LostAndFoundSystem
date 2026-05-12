using Microsoft.EntityFrameworkCore;
using LostAndFoundSystem.Models;

namespace LostAndFoundSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin", RoleDescription = "System Administrator" },
                new Role { Id = 2, RoleName = "User", RoleDescription = "General User" }
            );
        }
    }
}