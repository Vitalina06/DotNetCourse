using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthService.Models;

namespace AuthService.Data
{
    class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Config.Settings.GetConnectionString("Postgres"));
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
                entity.Property(u => u.CreatedDate).HasColumnName("created_date").HasColumnType("timestamp without time zone");
                entity.Property(u => u.UpdatedDate).HasColumnName("updated_date").HasColumnType("timestamp without time zone");
                entity.HasIndex(u => u.Login).IsUnique();
            });
        }
    }
}