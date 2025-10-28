using EVChargingStation.CARC.Domain.LongLQ;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;

namespace EVChargingStation.CARC.Infrastructure.LongLQ
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FA25_SWD392_SE182594_G6_EvChargingStation _dbContext;

        public UnitOfWork(
            FA25_SWD392_SE182594_G6_EvChargingStation dbContext,
            IGenericRepository<User> userRepository,
            IGenericRepository<Connector> connectorRepository,
            IGenericRepository<Location> locationRepository,
            IGenericRepository<ReservationLongLQ> reservationLongLQRepository,
            IGenericRepository<StationAnhDHV> stationAnhDHVRepository,
            IGenericRepository<VehicleHuyPD> vehicleHuyPDRepository,
            IGenericRepository<Session> sessionRepository
            )
        {
            _dbContext = dbContext;
            Users = userRepository;
            Connectors = connectorRepository;
            Locations = locationRepository;
            ReservationLongLQs = reservationLongLQRepository;
            StationAnhDHVs = stationAnhDHVRepository;
            VehicleHuyPDs = vehicleHuyPDRepository;
            Sessions = sessionRepository;
        }

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Connector> Connectors { get; }
        public IGenericRepository<Location> Locations { get; }
        public IGenericRepository<ReservationLongLQ> ReservationLongLQs { get; }
        public IGenericRepository<StationAnhDHV> StationAnhDHVs { get; }
        public IGenericRepository<VehicleHuyPD> VehicleHuyPDs { get; }
        public IGenericRepository<Session> Sessions { get; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
