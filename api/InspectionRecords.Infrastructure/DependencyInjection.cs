using Inspections.Data;
using Inspections.Data.Repositories;
using Inspections.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InspectionRecords.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInspectionPersistence(this IServiceCollection services, string connection)
    {
        services.AddDbContext<InspectionDbContext>(options => options.UseSqlServer(connection,
            sql => sql.MigrationsHistoryTable("__InspectionMigrations")));
        services.AddDbContext<RecordsDbContext>(options => options.UseSqlServer(connection,
            sql => sql.MigrationsHistoryTable("__StateRecordMigrations")));
        services.AddScoped<IInspectionRepository, InspectionRepository>();
        services.AddScoped<IStateRecordRepository, StateRecordRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IChecklistRepository, ChecklistRepository>();
        services.AddScoped<IInspectionImageStore, InspectionImageStore>();
        services.AddScoped<DatabaseInitializer>();
        services.AddHealthChecks().AddCheck<SqlServerHealthCheck>("sql-server");
        return services;
    }
}
