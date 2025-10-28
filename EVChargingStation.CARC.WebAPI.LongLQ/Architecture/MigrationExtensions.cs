using EVChargingStation.CARC.Domain.LongLQ;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Architecture;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, ILogger _logger)
    {
        try
        {
            _logger.LogInformation("Applying migrations...");
            using var scope = app.ApplicationServices.CreateScope();

            using var dbContext =
                scope.ServiceProvider.GetRequiredService<FA25_SWD392_SE182594_G6_EvChargingStation>();

            dbContext.Database.Migrate();
            _logger.LogInformation("Migrations applied successfully!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An problem occurred during migration!");
        }
    }
}