using Inspections.Data;
using Inspections.Data.Repositories;
using Inspections.Models.Entities;
using Inspections.Models.Entities.Templates;
using Inspections.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("Inspections")
    ?? @"Server=(localdb)\MSSQLLocalDB;Database=InspectionRecordsDemo;Integrated Security=True;TrustServerCertificate=True";

builder.Services.AddDbContext<InspectionDbContext>(options => options.UseSqlServer(connection,
    sql => sql.MigrationsHistoryTable("__InspectionMigrations")));
builder.Services.AddDbContext<RecordsDbContext>(options => options.UseSqlServer(connection,
    sql => sql.MigrationsHistoryTable("__StateRecordMigrations")));
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IStateRecordRepository, StateRecordRepository>();
builder.Services.AddScoped<IStateRecordService, StateRecordService>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();
app.MapGet("/api/health", async (InspectionDbContext db, CancellationToken token) =>
    await db.Database.CanConnectAsync(token) ? Results.Ok("Healthy") : Results.StatusCode(503));

if (args.Contains("--initialize"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var inspections = scope.ServiceProvider.GetRequiredService<InspectionDbContext>();
    var records = scope.ServiceProvider.GetRequiredService<RecordsDbContext>();
    await inspections.Database.MigrateAsync();
    await records.Database.MigrateAsync();

    if (!await inspections.Templates.AnyAsync())
    {
        inspections.Templates.Add(new Template
        {
            Name = "Facility inspection",
            Description = "Example template for a fictional facility.",
            Versions = [new TemplateVersion { VersionNumber = 1 }]
        });
        await inspections.SaveChangesAsync();
    }

    if (!await records.StateDefinitions.AnyAsync())
    {
        records.StateDefinitions.Add(new StateDefinition
        {
            Id = "sample-region", Name = "Sample region", Emoji = "📍", PlaceIdsJson = "[]"
        });
        await records.SaveChangesAsync();
    }
    return;
}

await app.RunAsync("http://localhost:5080");
