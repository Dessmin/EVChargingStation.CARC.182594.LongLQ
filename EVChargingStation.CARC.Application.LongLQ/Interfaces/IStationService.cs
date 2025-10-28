using EVChargingStation.CARC.Application.LongLQ.DTOs.StationDTOs;

namespace EVChargingStation.CARC.Application.LongLQ.Interfaces
{
    public interface IStationService
    {
        Task<List<StationConnectorsDto>> GetStationsWithConnectorIdsAsync();
    }
}
