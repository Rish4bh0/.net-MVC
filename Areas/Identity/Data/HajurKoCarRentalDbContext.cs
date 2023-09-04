using Rentalmandu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Rentalmandu.Areas.Identity.Data;

public class HajurKoCarRentalDbContext : IdentityDbContext<HajurKoCarRentalUser>
{
    public HajurKoCarRentalDbContext(DbContextOptions<HajurKoCarRentalDbContext> options)
        : base(options)
    {
        
    }

    public HajurKoCarRentalDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<Notification>()
       .HasOne(n => n.User)
       .WithMany()
       .HasForeignKey(n => n.UserID)
       .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<CarInfo> CarInfo { get; set; }

    public DbSet<Rental> Rental { get; set; }
    public DbSet<HajurKoCarRentalUser> User { get; set; }

    public DbSet<CarDamage> CarDamage { get; set; }
    
    public DbSet<Offer> Offer { get; set; }

    public DbSet<Notification> Notification { get; set; }

    



}
