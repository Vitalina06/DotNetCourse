using Microsoft.EntityFrameworkCore;
using UserAuthApp.Models;

namespace UserAuthApp.Data
{
    class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userauthdb;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Login).HasColumnName("login").IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired().HasMaxLength(100);
                entity.Property(u => u.CreatedDate).HasColumnName("created_date");
                entity.Property(u => u.UpdatedDate).HasColumnName("updated_date");
                entity.HasIndex(u => u.Login).IsUnique();
            });
        }
    }
}