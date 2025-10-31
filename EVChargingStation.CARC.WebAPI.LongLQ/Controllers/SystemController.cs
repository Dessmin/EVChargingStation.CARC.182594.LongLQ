using EVChargingStation.CARC.Application.LongLQ.Utils;
using EVChargingStation.CARC.Domain.LongLQ;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Domain.LongLQ.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly FA25_SWD392_SE182594_G6_EvChargingStation _context;
        private readonly ILogger<SystemController> _logger;

        public SystemController(FA25_SWD392_SE182594_G6_EvChargingStation context, ILogger<SystemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("seed-all-data")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await ClearDatabase(_context);
                await SeedUserAsync();
                await SeedVehicleAsync();
                await SeedPlanAsync();
                await SeedUserPlanAsync();
                await SeedLocationAsync();
                await SeedStationAsync();
                await SeedConnectorAsync();
                await SeedSessionAsync();
                await SeedReservationAsync();

                return Ok(ApiResult<object>.Success(new
                {
                    Message = "Data seeded successfully."
                }));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError("Database update error during data seeding: {Message}", dbEx.Message);
                return StatusCode(500, "Database update error occurred while seeding data.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during data seeding: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while seeding data.");
            }
        }

        private async Task ClearDatabase(FA25_SWD392_SE182594_G6_EvChargingStation context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                //Delete data in order to avoid foreign key constraint issues
                await context.InvoiceTruongNN.ExecuteDeleteAsync();
                await context.Sessions.ExecuteDeleteAsync();
                await context.ReservationLongLQ.ExecuteDeleteAsync();
                await context.Connectors.ExecuteDeleteAsync();
                await context.StationAnhDHV.ExecuteDeleteAsync();
                await context.Locations.ExecuteDeleteAsync();
                await context.UserPlanHoaHTT.ExecuteDeleteAsync();
                await context.Plans.ExecuteDeleteAsync();
                await context.VehicleHuyPD.ExecuteDeleteAsync();
                await context.Users.ExecuteDeleteAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Deleted data in database successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error clearing database: {Message}", ex.Message);
                throw;
            }
        }

        private async Task SeedUserAsync()
        {
            _logger.LogInformation("Seeding users with roles...");

            var passwordHasher = new PasswordHasher();

            //Seed User
            var users = new List<User>
            {
                new()
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Gender = Gender.Male,
                    Email = "Admin@gmail.com",
                    Phone = "1234567890",
                    Address = "123 Admin St, City, Country",
                    Role = RoleType.Admin,
                    Status = UserStatus.Active
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} users successfully.", users.Count);
        }

        private async Task SeedVehicleAsync()
        {
            _logger.LogInformation("Seeding vehicles...");

            //Get Admin User
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");

            if (adminUser == null)
            {
                _logger.LogError("Admin user not found for vehicle seeding.");
                return;
            }

            //Create vehicles for Admin
            var vehicles = new List<VehicleHuyPD>
            {
                new()
                {
                    Make = "Tesla",
                    Model = "Model3",
                    Year = 2023,
                    LicensePlate = "30A-1345",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.LongLQID
                },
                new()
                {
                    Make = "VinFast",
                    Model = "VF e34",
                    Year = 2024,
                    LicensePlate = "29B-67890",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.LongLQID
                },
                new()
                {
                    Make = "BMW",
                    Model = "i4",
                    Year = 2023,
                    LicensePlate = "31C-54321",
                    ConnectorType = ConnectorType.AC,
                    UserId = adminUser.LongLQID
                }
            };

            await _context.VehicleHuyPD.AddRangeAsync(vehicles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} vehicles successfully.", vehicles.Count);
        }

        private async Task SeedPlanAsync()
        {
            _logger.LogInformation("Seeding plans...");

            //Create plans with different types
            var plans = new List<Plan>
            {
                new()
                {
                    Name = "Basic Prepaid",
                    Description = "Pay as you go - Top up your account and charge at standard rates",
                    Type = PlanType.Prepaid,
                    Price =0m, //No monthly fee
                    MaxDailyKwh =50m
                },
                new()
                {
                    Name = "Standard Postpaid",
                    Description ="Monthly billing with competitive rates for regular users",
                    Type = PlanType.Postpaid,
                    Price =99000m, //Monthly fee per moth
                    MaxDailyKwh =100m
                },
                new()
                {
                    Name = "Premium Postpaid",
                    Description = "Enhanced postpaid plan with higher daily limits and priority support",
                    Type = PlanType.Postpaid,
                    Price =199000m, //199,000 VND per month
                    MaxDailyKwh =200m
                },
                new()
                {
                    Name = "VIP Unlimited",
                    Description = "Exclusive VIP plan with unlimited charging, priority access, and premium benefits",
                    Type = PlanType.VIP,
                    Price =499000m, //499,000 VND per month
                    MaxDailyKwh = null // Unlimited
                }
            };

            await _context.Plans.AddRangeAsync(plans);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} plans successfully.", plans.Count);
        }

        private async Task SeedUserPlanAsync()
        {
            _logger.LogInformation("Seeding user plans...");
            //Get Admin User
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");

            if (adminUser == null)
            {
                _logger.LogError("Admin user not found for user plan seeding.");
                return;
            }

            //Get VIP Plan
            var vipPlan = await _context.Plans
                .FirstOrDefaultAsync(p => p.Type == PlanType.VIP);

            if (vipPlan == null)
            {
                _logger.LogError("VIP plan not found for user plan seeding.");
                return;
            }

            //Assign VIP plan to Admin user
            var userPlans = new List<UserPlanHoaHTT>
            {
                new()
                {
                UserId = adminUser.LongLQID,
                PlanId = vipPlan.LongLQID,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1) //1 year validity
                }
            };

            await _context.UserPlanHoaHTT.AddRangeAsync(userPlans);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} user plans successfully.", userPlans.Count);
        }

        private async Task SeedLocationAsync()
        {
            _logger.LogInformation("Seeding locations...");

            var locations = new List<Location>
            {
                new()
                {
                    Name = "FPT University HCM",
                    Address = "Lô E2a-7, Đường D1, Khu Công nghệ cao, P.Long Thạnh Mỹ, TP. Thủ Đức, TP.HCM",
                    Latitude =10.8411m,
                    Longitude =106.8098m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Vincom Center Landmark81",
                    Address = "720A Điện Biên Phủ, Vinhomes Tân Cảng, Bình Thạnh, TP.HCM",
                    Latitude =10.7946m,
                    Longitude =106.7218m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Saigon Centre",
                    Address = "65 Lê Lợi, Bến Nghé, Quận1, TP.HCM",
                    Latitude =10.7769m,
                    Longitude =106.7009m,
                    City = "Ho Chi Minh City",
                    StateProvince = "Ho Chi Minh",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Hanoi Times Tower",
                    Address = "57 Lieu Giai, Ba Dinh, Hanoi",
                    Latitude =21.0285m,
                    Longitude =105.8542m,
                    City = "Hanoi",
                    StateProvince = "Hanoi",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Dragon Bridge Area",
                    Address = "An Hai Bac, Son Tra, Da Nang",
                    Latitude =16.0544m,
                    Longitude =108.2022m,
                    City = "Da Nang",
                    StateProvince = "Da Nang",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                },
                new()
                {
                    Name = "Nha Trang Central",
                    Address = "Tran Phu, Loc Tho, Nha Trang",
                    Latitude =12.2388m,
                    Longitude =109.1967m,
                    City = "Nha Trang",
                    StateProvince = "Khanh Hoa",
                    Country = "Vietnam",
                    Timezone = "Asia/Ho_Chi_Minh"
                }
            };

            await _context.Locations.AddRangeAsync(locations);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} locations successfully.", locations.Count);
        }

        private async Task SeedStationAsync()
        {
            _logger.LogInformation("Seeding stations...");

            //Get Locations
            var locations = await _context.Locations.ToListAsync();

            if (!locations.Any())
            {
                _logger.LogError("No locations found for station seeding.");
                return;
            }

            var stations = new List<StationAnhDHV>
            {
                new()
                {
                    Name = "FPT Fast Charging Hub",
                    LocationId = locations.First(l => l.Name != null && l.Name.Contains("FPT")).LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Landmark81 EV Station",
                    LocationId = locations.First(l => l.Name != null && l.Name.Contains("Landmark")).LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Saigon Centre Power Point",
                    LocationId = locations.First(l => l.Name != null && l.Name.Contains("Saigon Centre")).LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Hanoi Central Charging",
                    LocationId = locations.First(l => l.City == "Hanoi").LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Dragon Bridge Supercharger",
                    LocationId = locations.First(l => l.City == "Da Nang").LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "Nha Trang Seaside Station",
                    LocationId = locations.First(l => l.City == "Nha Trang").LongLQID,
                    Status = StationStatus.Online
                },
                // Add a few secondary stations for larger cities
                new()
                {
                    Name = "HCMC West Charging Point",
                    LocationId = locations.First(l => l.City == "Ho Chi Minh City").LongLQID,
                    Status = StationStatus.Online
                },
                new()
                {
                    Name = "HCMC East QuickCharge",
                    LocationId = locations.First(l => l.City == "Ho Chi Minh City").LongLQID,
                    Status = StationStatus.Offline
                }
            };

            await _context.StationAnhDHV.AddRangeAsync(stations);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} stations successfully.", stations.Count);
        }

        private async Task SeedConnectorAsync()
        {
            _logger.LogInformation("Seeding connectors...");

            var stations = await _context.StationAnhDHV.ToListAsync();

            if (!stations.Any())
            {
                _logger.LogError("No stations found for connector seeding.");
                return;
            }

            var connectors = new List<Connector>();

            // Add multiple connectors for each station
            foreach (var station in stations)
            {
                connectors.AddRange(new List<Connector>
                {
                    new()
                    {
                        StationAnhDHVId = station.LongLQID,
                        ConnectorType = ConnectorType.CCS,
                        PowerKw =150m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh =4500m
                    },
                    new()
                    {
                        StationAnhDHVId = station.LongLQID,
                        ConnectorType = ConnectorType.CHAdeMO,
                        PowerKw =100m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh =4000m
                    },
                    new()
                    {
                        StationAnhDHVId = station.LongLQID,
                        ConnectorType = ConnectorType.AC,
                        PowerKw =22m,
                        Status = ConnectorStatus.Free,
                        PricePerKwh =3000m
                    }
                });
            }

            await _context.Connectors.AddRangeAsync(connectors);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} connectors successfully.", connectors.Count);
        }

        private async Task SeedSessionAsync()
        {
            _logger.LogInformation("Seeding sessions...");

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");
            var connectors = await _context.Connectors.ToListAsync();

            if (adminUser == null || !connectors.Any())
            {
                _logger.LogError("Admin user or connectors not found for session seeding.");
                return;
            }

            var sessions = new List<Session>
            {
                new()
                {
                    ConnectorId = connectors[0].LongLQID,
                    UserId = adminUser.LongLQID,
                    StartTime = DateTime.UtcNow.AddDays(-2),
                    EndTime = DateTime.UtcNow.AddDays(-2).AddHours(2),
                    Status = SessionStatus.Stopped,
                    SocStart = 20m,
                    SocEnd = 80m,
                    EnergyKwh = 45m,
                    Cost = 202500m
                },
                new()
                {
                    ConnectorId = connectors[1].LongLQID,
                    UserId = adminUser.LongLQID,
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    EndTime = DateTime.UtcNow.AddDays(-1).AddHours(1.5),
                    Status = SessionStatus.Stopped,
                    SocStart = 30m,
                    SocEnd = 85m,
                    EnergyKwh = 35m,
                    Cost = 140000m
                },
                new()
                {
                    ConnectorId = connectors[2].LongLQID,
                    UserId = adminUser.LongLQID,
                    StartTime = DateTime.UtcNow.AddHours(-4),
                    EndTime = DateTime.UtcNow.AddHours(-1),
                    Status = SessionStatus.Stopped,
                    SocStart = 15m,
                    SocEnd = 90m,
                    EnergyKwh = 50m,
                    Cost = 150000m //50 kWh *3,000 VND
                }
            };

            await _context.Sessions.AddRangeAsync(sessions);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} sessions successfully.", sessions.Count);
        }

        private async Task SeedReservationAsync()
        {
            _logger.LogInformation("Seeding reservations...");

            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");
            var stations = await _context.StationAnhDHV.ToListAsync();
            var connectors = await _context.Connectors.ToListAsync();

            if (adminUser == null || !stations.Any() || !connectors.Any())
            {
                _logger.LogError("Required data not found for reservation seeding.");
                return;
            }

            var fptStation = stations.FirstOrDefault(s => s.Name == "FPT Fast Charging Hub");
            var landmarkStation = stations.FirstOrDefault(s => s.Name == "Landmark81 EV Station");

            if (fptStation == null || landmarkStation == null)
            {
                _logger.LogError("Required stations not found for reservation seeding.");
                return;
            }

            // Get connectors for each type at FPT station
            var fptCcsConnector = connectors.FirstOrDefault(c =>
                c.StationAnhDHVId == fptStation.LongLQID && c.ConnectorType == ConnectorType.CCS);
            var fptChaDemoConnector = connectors.FirstOrDefault(c =>
                c.StationAnhDHVId == fptStation.LongLQID && c.ConnectorType == ConnectorType.CHAdeMO);
            var landmarkAcConnector = connectors.FirstOrDefault(c =>
                c.StationAnhDHVId == landmarkStation.LongLQID && c.ConnectorType == ConnectorType.AC);

            if (fptCcsConnector == null || landmarkAcConnector == null || fptChaDemoConnector == null)
            {
                _logger.LogError("Required connectors not found for reservation seeding.");
                return;
            }

            var reservations = new List<ReservationLongLQ>
            {
                new()
                {
                    LongLQID = Guid.NewGuid(),
                    UserId = adminUser.LongLQID,
                    StationAnhDHVId = fptStation.LongLQID,
                    ConnectorId = fptCcsConnector.LongLQID,
                    PreferredConnectorType = ConnectorType.CCS,
                    MinPowerKw = 100m,
                    PriceType = PriceType.PrePaid,
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                    Status = ReservationStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminUser.LongLQID,
                    IsDeleted = false
                },
                new()
                {
                    LongLQID = Guid.NewGuid(),
                    UserId = adminUser.LongLQID,
                    StationAnhDHVId = landmarkStation.LongLQID,
                    ConnectorId = landmarkAcConnector.LongLQID,
                    PreferredConnectorType = ConnectorType.AC,
                    MinPowerKw = 22m,
                    PriceType = PriceType.Free,
                    StartTime = DateTime.UtcNow.AddDays(2),
                    EndTime = DateTime.UtcNow.AddDays(2).AddHours(3),
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminUser.LongLQID,
                    IsDeleted = false
                },
                new()
                {
                    LongLQID = Guid.NewGuid(),
                    UserId = adminUser.LongLQID,
                    StationAnhDHVId = fptStation.LongLQID,
                    ConnectorId = fptChaDemoConnector.LongLQID,  // Assign CHAdeMO connector instead of null
                    PreferredConnectorType = ConnectorType.CHAdeMO,
                    MinPowerKw = 50m,
                    PriceType = PriceType.PrePaid,
                    StartTime = DateTime.UtcNow.AddDays(3),
                    EndTime = DateTime.UtcNow.AddDays(3).AddHours(1),
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminUser.LongLQID,
                    IsDeleted = false
                }
            };

            await _context.ReservationLongLQ.AddRangeAsync(reservations);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} reservations successfully.", reservations.Count);
        }
    }
}
