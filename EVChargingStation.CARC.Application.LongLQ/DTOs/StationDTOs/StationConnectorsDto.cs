using System;
using System.Collections.Generic;
using EVChargingStation.CARC.Domain.LongLQ.Enums;

namespace EVChargingStation.CARC.Application.LongLQ.DTOs.StationDTOs
{
 public class StationConnectorsDto
 {
 public Guid StationId { get; set; }
 public string Name { get; set; } = string.Empty;
 public List<ConnectorInfoDto> Connectors { get; set; } = new List<ConnectorInfoDto>();
 }

 public class ConnectorInfoDto
 {
 public Guid ConnectorId { get; set; }
 public ConnectorType ConnectorType { get; set; }
 public decimal PowerKw { get; set; }
 }
}
