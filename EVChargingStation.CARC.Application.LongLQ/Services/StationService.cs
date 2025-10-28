using EVChargingStation.CARC.Application.LongLQ.DTOs.StationDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EVChargingStation.CARC.Application.LongLQ.Services
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public StationService(IUnitOfWork unitOfWork, ILogger<StationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<StationConnectorsDto>> GetStationsWithConnectorIdsAsync()
        {
            try
            {
                // Load stations with their connectors (include connector properties)
                var stations = await _unitOfWork.StationAnhDHVs.GetAllAsync(null, s => s.Connectors);

                var result = stations.Select(s => new StationConnectorsDto
                {
                    StationId = s.LongLQID,
                    Name = s.Name,
                    Connectors = s.Connectors.Select(c => new ConnectorInfoDto
                    {
                        ConnectorId = c.LongLQID,
                        ConnectorType = c.ConnectorType,
                        PowerKw = c.PowerKw
                    }).ToList()
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stations with connectors");
                throw;
            }
        }
    }
}
