using EVChargingStation.CARC.Domain.LongLQ;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Domain.LongLQ.Enums;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.Infrastructure.LongLQ.SeedData
{
    public static class ReservationSeedData
    {
        public static async Task SeedAsync(FA25_SWD392_SE182594_G6_EvChargingStation context)
        {
            if (context == null) return;

            // Seed Locations
            if (!await context.Locations.AnyAsync())
            {
                var locations = new List<Location>
 {
 new Location
 {
 LongLQID = Guid.Parse("44444444-4444-4444-4444-444444444444"),
 Name = "HCMC Central Station",
 Address = "100 Dong Khoi, District1, HCMC",
 Latitude =10.7769m,
 Longitude =106.7009m,
 City = "Ho Chi Minh City",
 StateProvince = "Ho Chi Minh",
 Country = "Vietnam",
 Timezone = "Asia/Ho_Chi_Minh",
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 },
 new Location
 {
 LongLQID = Guid.Parse("55555555-5555-5555-5555-555555555555"),
 Name = "Binh Thanh District Station",
 Address = "200 Xo Viet Nghe Tinh, Binh Thanh, HCMC",
 Latitude =10.8142m,
 Longitude =106.7072m,
 City = "Ho Chi Minh City",
 StateProvince = "Ho Chi Minh",
 Country = "Vietnam",
 Timezone = "Asia/Ho_Chi_Minh",
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 }
 };

                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
            }

            // Seed Stations
            if (!await context.StationAnhDHV.AnyAsync())
            {
                var stations = new List<StationAnhDHV>
 {
 new StationAnhDHV
 {
 LongLQID = Guid.Parse("66666666-6666-6666-6666-666666666666"),
 Name = "Central Fast Charging Station",
 LocationId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
 Status = StationStatus.Online,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 },
 new StationAnhDHV
 {
 LongLQID = Guid.Parse("77777777-7777-7777-7777-777777777777"),
 Name = "Binh Thanh Charging Hub",
 LocationId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
 Status = StationStatus.Online,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 }
 };

                await context.StationAnhDHV.AddRangeAsync(stations);
                await context.SaveChangesAsync();
            }

            // Seed Connectors
            if (!await context.Connectors.AnyAsync())
            {
                var connectors = new List<Connector>
 {
 new Connector
 {
 LongLQID = Guid.Parse("88888888-8888-8888-8888-888888888888"),
 StationAnhDHVId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
 ConnectorType = ConnectorType.CCS,
 PowerKw =150m,
 Status = ConnectorStatus.Free,
 PricePerKwh =5000m,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 },
 new Connector
 {
 LongLQID = Guid.Parse("99999999-9999-9999-9999-999999999999"),
 StationAnhDHVId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
 ConnectorType = ConnectorType.CHAdeMO,
 PowerKw =100m,
 Status = ConnectorStatus.Free,
 PricePerKwh =4500m,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 },
 new Connector
 {
 LongLQID = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
 StationAnhDHVId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
 ConnectorType = ConnectorType.AC,
 PowerKw =22m,
 Status = ConnectorStatus.Free,
 PricePerKwh =3500m,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 },
 new Connector
 {
 LongLQID = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
 StationAnhDHVId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
 ConnectorType = ConnectorType.CCS,
 PowerKw =200m,
 Status = ConnectorStatus.Occupied,
 PricePerKwh =5500m,
 IsDeleted = false,
 CreatedAt = DateTime.UtcNow,
 CreatedBy = Guid.Empty
 }
 };

                await context.Connectors.AddRangeAsync(connectors);
                await context.SaveChangesAsync();
            }
            return;
        }
    }
}
