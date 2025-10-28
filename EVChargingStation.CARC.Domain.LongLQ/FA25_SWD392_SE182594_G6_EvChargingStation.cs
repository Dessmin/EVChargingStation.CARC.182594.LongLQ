using EVChargingStation.CARC.Domain.LongLQ.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace EVChargingStation.CARC.Domain.LongLQ
{
    public class FA25_SWD392_SE182594_G6_EvChargingStation : DbContext
    {
        public FA25_SWD392_SE182594_G6_EvChargingStation() { }

        public FA25_SWD392_SE182594_G6_EvChargingStation(DbContextOptions<FA25_SWD392_SE182594_G6_EvChargingStation> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<VehicleHuyPD> VehicleHuyPD { get; set; }
        public DbSet<StationAnhDHV> StationAnhDHV { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Connector> Connectors { get; set; }
        public DbSet<ReservationLongLQ> ReservationLongLQ { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<InvoiceTruongNN> InvoiceTruongNN { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserPlanHoaHTT> UserPlanHoaHTT { get; set; }
        public DbSet<StaffStation> StaffStations { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Location ↔ Station (one-to-many)
            modelBuilder.Entity<StationAnhDHV>()
                .HasOne(s => s.Location)
                .WithMany(l => l.StationAnhDHV)
                .HasForeignKey(s => s.LocationId);

            // User ↔ Vehicle (one-to-many)
            modelBuilder.Entity<VehicleHuyPD>()
                .HasOne(v => v.User)
                .WithMany(u => u.VehicleHuyPD)
                .HasForeignKey(v => v.UserId);

            // Station ↔ Connector (one-to-many)
            modelBuilder.Entity<Connector>()
                .HasOne(c => c.StationAnhDHV)
                .WithMany(s => s.Connectors)
                .HasForeignKey(c => c.StationAnhDHVId);

            // Reservation ↔ User (many-to-one)
            modelBuilder.Entity<ReservationLongLQ>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReservationLongLQ)
                .HasForeignKey(r => r.UserId);

            // Reservation ↔ Station (many-to-one)
            modelBuilder.Entity<ReservationLongLQ>()
                .HasOne(r => r.StationAnhDHV)
                .WithMany(s => s.ReservationLongLQ)
                .HasForeignKey(r => r.StationAnhDHVId);

            // Reservation ↔ Connector (many-to-one, optional)
            modelBuilder.Entity<ReservationLongLQ>()
                .HasOne(r => r.Connector)
                .WithMany(c => c.ReservationLongLQ)
                .HasForeignKey(r => r.ConnectorId)
                .IsRequired(false);

            // Session ↔ Reservation (one-to-one, optional)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.ReservationLongLQ)
                .WithOne(r => r.Session)
                .HasForeignKey<Session>(s => s.ReservationLongLQId)
                .IsRequired(false);

            // Session ↔ Connector (many-to-one)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Connector)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.ConnectorId);

            // Session ↔ User (many-to-one)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId);

            // Session ↔ Invoice (many-to-one, optional)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.InvoiceTruongNN)
                .WithMany(i => i.Sessions)
                .HasForeignKey(s => s.InvoiceTruongNNId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Invoice ↔ User (many-to-one)
            modelBuilder.Entity<InvoiceTruongNN>()
                .HasOne(i => i.User)
                .WithMany(u => u.InvoiceTruongNN)
                .HasForeignKey(i => i.UserId);

            // Payment ↔ Invoice (many-to-one, optional)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.InvoiceTruongNN)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceTruongNNId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Payment ↔ Session (many-to-one, optional)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Session)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SessionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Payment ↔ User (many-to-one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId);

            // UserPlan ↔ User (many-to-one)
            modelBuilder.Entity<UserPlanHoaHTT>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPlanHoaHTT)
                .HasForeignKey(up => up.UserId);

            // UserPlan ↔ Plan (many-to-one)
            modelBuilder.Entity<UserPlanHoaHTT>()
                .HasOne(up => up.Plan)
                .WithMany(p => p.UserPlanHoaHTT)
                .HasForeignKey(up => up.PlanId);

            // StaffStation ↔ User (many-to-one)
            modelBuilder.Entity<StaffStation>()
                .HasOne(ss => ss.StaffUser)
                .WithMany(u => u.StaffStations)
                .HasForeignKey(ss => ss.StaffUserId);

            // StaffStation ↔ Station (many-to-one)
            modelBuilder.Entity<StaffStation>()
                .HasOne(ss => ss.StationAnhDHV)
                .WithMany(s => s.StaffStations)
                .HasForeignKey(ss => ss.StationAnhDHVId);

            // Report ↔ Staff (User) (many-to-one, optional)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Recommendation ↔ User (many-to-one)
            modelBuilder.Entity<Recommendation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recommendations)
                .HasForeignKey(r => r.UserId);

            // Recommendation ↔ Station (many-to-one)
            modelBuilder.Entity<Recommendation>()
                .HasOne(r => r.StationAnhDHV)
                .WithMany(s => s.Recommendations)
                .HasForeignKey(r => r.StationAnhDHVId);

            // Recommendation ↔ Connector (optional, many-to-one)
            modelBuilder.Entity<Recommendation>()
                .HasOne(r => r.Connector)
                .WithMany(c => c.Recommendations)
                .HasForeignKey(r => r.ConnectorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This is a fallback configuration in case the options aren't provided
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseNpgsql(connectionString);
                }
            }
        }
    }
}
