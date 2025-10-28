using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EVChargingStation.CARC.Domain.LongLQ
{
    public class FA25_SWD392_SE182594_G6_EvChargingStationFactory : IDesignTimeDbContextFactory<FA25_SWD392_SE182594_G6_EvChargingStation>
    {
        public FA25_SWD392_SE182594_G6_EvChargingStation CreateDbContext(string[] args)
        {
            // For migrations and other design-time operations
            var optionsBuilder = new DbContextOptionsBuilder<FA25_SWD392_SE182594_G6_EvChargingStation>();

            // Set up configuration to read connection string
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Get the connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // For PostgreSQL: Npgsql.EntityFrameworkCore.PostgreSQL
            optionsBuilder.UseNpgsql(connectionString,
                npgsqlOptionsAction => npgsqlOptionsAction.MigrationsAssembly(typeof(FA25_SWD392_SE182594_G6_EvChargingStation).Assembly.FullName));

            return new FA25_SWD392_SE182594_G6_EvChargingStation(optionsBuilder.Options);
        }
    }
}
