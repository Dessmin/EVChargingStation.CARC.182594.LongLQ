using EVChargingStation.CARC.Domain.LongLQ.Entities;

namespace EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Connector> Connectors { get; }
        IGenericRepository<Location> Locations { get; }
        IGenericRepository<ReservationLongLQ> ReservationLongLQs { get; }
        IGenericRepository<StationAnhDHV> StationAnhDHVs { get; }
        IGenericRepository<VehicleHuyPD> VehicleHuyPDs { get; }
        IGenericRepository<Session> Sessions { get; }
        Task<int> SaveChangesAsync();
    }
}
