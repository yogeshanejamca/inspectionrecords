using InspectionRecords.Infrastructure;
using Inspections.Data;
using Inspections.Service;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("Inspections")
    ?? @"Server=(localdb)\MSSQLLocalDB;Database=InspectionRecordsDemo;Integrated Security=True;TrustServerCertificate=True";

builder.Services.AddInspectionPersistence(connection);
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IStateRecordService, StateRecordService>();
builder.Services.AddScoped<TemplateService>();
builder.Services.AddScoped<ChecklistService>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/api/health");

if (args.Contains("--initialize"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync(seedDemo: !args.Contains("--schema-only"));
    return;
}

await app.RunAsync("http://localhost:5080");
