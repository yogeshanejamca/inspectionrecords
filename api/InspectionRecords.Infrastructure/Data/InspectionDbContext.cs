using Inspections.Models.Entities;
using Inspections.Models.Entities.Templates;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data;

public class InspectionDbContext : DbContext
{
    public InspectionDbContext(DbContextOptions<InspectionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<InspectionPause> InspectionPauses => Set<InspectionPause>();
    public DbSet<InspectionImage> InspectionImages => Set<InspectionImage>();

    public DbSet<Template> Templates=> Set<Template>();


 public DbSet<TemplateVersion> TemplateVersions { get; set; }
 public DbSet<TemplateField> TemplateFields { get; set; }

 public DbSet<Checklist> Checklists { get; set; }
 public DbSet<ChecklistFieldValue> ChecklistFieldValues { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Template>().WithMany().HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.Id);

        });





 modelBuilder.Entity<TemplateVersion>()
.HasOne(x => x.Template)
.WithMany(x => x.Versions)
.HasForeignKey(x => x.TemplateId);

 modelBuilder.Entity<TemplateField>()
.HasOne(x => x.TemplateVersion)
.WithMany(x => x.Fields)
.HasForeignKey(x => x.TemplateVersionId);



modelBuilder.Entity<Checklist>()
.HasOne(x => x.TemplateVersion)
.WithMany()
.HasForeignKey(x => x.TemplateVersionId);

 modelBuilder.Entity<ChecklistFieldValue>()
.HasOne(x => x.Checklist)
.WithMany(x => x.FieldValues)
.HasForeignKey(x => x.ChecklistId);

 modelBuilder.Entity<ChecklistFieldValue>()
.HasOne(x => x.TemplateField)
.WithMany()
.HasForeignKey(x => x.TemplateFieldId)
.OnDelete(DeleteBehavior.NoAction);



        // Inspection image mapping
        modelBuilder.Entity<InspectionImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BlobName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.HasOne(e => e.Inspection)
                .WithMany()
                .HasForeignKey(e => e.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InspectionPause>()
            .HasOne<Inspection>()
            .WithMany()
            .HasForeignKey(pause => pause.InspectionId);
    }
}
