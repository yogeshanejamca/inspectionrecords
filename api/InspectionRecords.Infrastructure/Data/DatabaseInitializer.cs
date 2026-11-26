using Inspections.Models.Entities;
using Inspections.Models.Entities.Templates;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data;

public sealed class DatabaseInitializer(InspectionDbContext inspections, RecordsDbContext records)
{
    public async Task InitializeAsync(bool seedDemo, CancellationToken token = default)
    {
        await inspections.Database.MigrateAsync(token);
        await records.Database.MigrateAsync(token);

        if (seedDemo)
        {
            await SeedInspectionsAsync(token);
            await SeedStateRecordsAsync(token);
        }
    }

    private async Task SeedInspectionsAsync(CancellationToken token)
    {
        await using var transaction = await inspections.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, token);

        var template = await inspections.Templates.FirstOrDefaultAsync(
            item => item.Name == "Facility inspection", token);
        if (template is null)
        {
            template = new Template
            {
                Name = "Facility inspection",
                Description = "Example template for a fictional facility.",
                Versions = [new TemplateVersion { VersionNumber = 1 }]
            };
            inspections.Templates.Add(template);
            await inspections.SaveChangesAsync(token);
        }

        if (!await inspections.Inspections.AnyAsync(token))
        {
            var date = DateTime.UtcNow.Date.AddDays(1);
            inspections.Inspections.AddRange(
                new Inspection
                {
                    Title = "Loading dock safety check",
                    Description = "Review signage and pedestrian routes at a fictional warehouse.",
                    Status = InspectionStatus.Scheduled,
                    ScheduledAt = date.AddHours(14),
                    TemplateId = template.Id
                },
                new Inspection
                {
                    Title = "Emergency exit walkthrough",
                    Description = "Check that the marked exits are clear.",
                    Status = InspectionStatus.Completed,
                    ScheduledAt = date.AddDays(-3).AddHours(15),
                    TemplateId = template.Id
                });
            await inspections.SaveChangesAsync(token);
        }

        await transaction.CommitAsync(token);
    }

    private async Task SeedStateRecordsAsync(CancellationToken token)
    {
        await using var transaction = await records.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, token);

        const string region = "sample-region";
        if (!await records.StateDefinitions.AnyAsync(item => item.Id == region, token))
        {
            records.StateDefinitions.Add(new StateDefinition
            {
                Id = region, Name = "Sample region", Emoji = "📍", PlaceIdsJson = "[]"
            });
        }

        var sampleId = Guid.Parse("c48567af-f81c-4f54-8aa9-4d4ae767af57");
        if (!await records.StateRecords.AnyAsync(item => item.StateId == region, token))
        {
            records.StateRecords.Add(new StateRecord
            {
                Id = sampleId, StateId = region, Name = "Riverside trail", Icon = "📍",
                Description = "Fictional site used for the local demo.", Points = 5, Distance = 1.5
            });
        }

        await records.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}
