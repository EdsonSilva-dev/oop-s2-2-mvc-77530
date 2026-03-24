using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafety.MVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Premises> Premises { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
    public DbSet<FollowUp> FollowUps { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigurePremises(builder);
        ConfigureInspection(builder);
        ConfigureFollowUp(builder);
    }

    private static void ConfigurePremises(ModelBuilder builder)
    {
        builder.Entity<Premises>(entity =>
        {
            entity.ToTable("Premises");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(p => p.Town)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.RiskRating)
                .IsRequired()
                .HasConversion<string>();

            entity.HasMany(p => p.Inspections)
                .WithOne(i => i.Premises)
                .HasForeignKey(i => i.PremisesId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureInspection(ModelBuilder builder)
    {
        builder.Entity<Inspection>(entity =>
        {
            entity.ToTable("Inspections");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.InspectionDate)
                .IsRequired();

            entity.Property(i => i.Score)
                .IsRequired();

            entity.Property(i => i.Outcome)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(i => i.Notes)
                .HasMaxLength(1000);

            entity.HasOne(i => i.Premises)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.PremisesId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(i => i.FollowUps)
                .WithOne(f => f.Inspection)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasCheckConstraint("CK_Inspection_Score", "[Score] >= 0 AND [Score] <= 100");
        });
    }

    private static void ConfigureFollowUp(ModelBuilder builder)
    {
        builder.Entity<FollowUp>(entity =>
        {
            entity.ToTable("FollowUps");

            entity.HasKey(f => f.Id);

            entity.Property(f => f.DueDate)
                .IsRequired();

            entity.Property(f => f.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(f => f.ClosedDate)
                .IsRequired(false);

            entity.HasOne(f => f.Inspection)
                .WithMany(i => i.FollowUps)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}