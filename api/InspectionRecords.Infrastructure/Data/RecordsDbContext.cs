using Inspections.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data;

public class RecordsDbContext : DbContext
{
    public RecordsDbContext(DbContextOptions<RecordsDbContext> options)
        : base(options)
    {
    }

    public DbSet<StateRecord> StateRecords => Set<StateRecord>();
    public DbSet<StateDefinition> StateDefinitions => Set<StateDefinition>();
    public DbSet<StateMap> StateMaps => Set<StateMap>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StateRecord>(entity =>
        {
            entity.ToTable("STATERECORD");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.StateId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.Points).IsRequired();
            entity.Property(e => e.Visited).IsRequired();
            entity.Property(e => e.Distance).IsRequired();
            entity.Property(e => e.ImageUrl).HasMaxLength(1000).IsRequired(false);
            entity.Property(e => e.ImageFileName).HasMaxLength(500).IsRequired(false);
            entity.Property(e => e.ImageUploadedAt).IsRequired(false);
            entity.HasIndex(e => e.StateId);
        });

        modelBuilder.Entity<StateDefinition>(entity =>
        {
            entity.ToTable("StateDefinitions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Emoji).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PlaceIdsJson).IsRequired();
        });

        modelBuilder.Entity<StateMap>(entity =>
        {
            entity.ToTable("StateMaps");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(50).IsRequired();
            entity.Property(e => e.X).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Y).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Color).HasMaxLength(20).IsRequired();
        });
    }
}
