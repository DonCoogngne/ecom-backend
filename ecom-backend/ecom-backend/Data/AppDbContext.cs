using ecom_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.RoleId);
            entity.Property(r => r.RoleName).HasMaxLength(50).IsRequired();
            entity.HasIndex(r => r.RoleName).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(500);
            entity.Property(u => u.Phone).HasMaxLength(30);
            entity.Property(u => u.ProfileImageUrl).HasMaxLength(500);
            entity.Property(u => u.GoogleId).HasMaxLength(128);
            entity.Property(u => u.TwoFactorEnabled).HasDefaultValue(false);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.GoogleId);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BusinessProfile>(entity =>
        {
            entity.ToTable("BusinessProfiles");
            entity.HasKey(b => b.BusinessProfileId);
            entity.Property(b => b.BusinessName).HasMaxLength(150);
            entity.Property(b => b.Category).HasMaxLength(100);
            entity.Property(b => b.Description).HasMaxLength(2000);
            entity.Property(b => b.Website).HasMaxLength(300);
            entity.Property(b => b.LogoUrl).HasMaxLength(500);
            entity.Property(b => b.PrimaryColor).HasMaxLength(20);
            entity.Property(b => b.SecondaryColor).HasMaxLength(20);
            entity.Property(b => b.Location).HasMaxLength(200);
            entity.HasIndex(b => b.UserId).IsUnique();

            entity.HasOne(b => b.User)
                .WithOne()
                .HasForeignKey<BusinessProfile>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("Subscriptions");
            entity.HasKey(s => s.SubscriptionId);
            entity.Property(s => s.PlanName).HasMaxLength(50).IsRequired();
            entity.HasIndex(s => s.UserId).IsUnique();

            entity.HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Subscription>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Invoices)
                .WithOne(i => i.Subscription)
                .HasForeignKey(i => i.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(i => i.InvoiceId);
            entity.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.Property(i => i.Status).HasMaxLength(30).IsRequired();
            entity.Property(i => i.Amount).HasPrecision(10, 2);
            entity.HasIndex(i => i.SubscriptionId);
        });
    }
}
