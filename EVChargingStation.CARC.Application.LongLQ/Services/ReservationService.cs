using EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Domain.LongLQ.Entities;
using EVChargingStation.CARC.Domain.LongLQ.Enums;
using EVChargingStation.CARC.Infrastructure.LongLQ.Commons;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EVChargingStation.CARC.Application.LongLQ.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IClaimsService _claimsService;

        public ReservationService(IUnitOfWork unitOfWork, ILogger<ReservationService> logger, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _claimsService = claimsService;
        }

        public async Task<Pagination<ReservationDto>> GetReservationsAsync(int pageNumber = 1, int pageSize = 10, Guid? userId = null, ReservationStatus? status = null)
        {
            try
            {
                _logger.LogInformation("Fetching reservations. Page: {Page}, Size: {Size}, UserId: {UserId}, Status: {Status}", pageNumber, pageSize, userId, status);

                var query = _unitOfWork.ReservationLongLQs.GetQueryable();
                if (userId.HasValue) query = query.Where(r => r.UserId == userId.Value);
                if (status.HasValue) query = query.Where(r => r.Status == status.Value);

                var total = await query.CountAsync();
                var reservations = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = new List<ReservationDto>();
                foreach (var r in reservations)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(r.UserId);
                    var station = await _unitOfWork.StationAnhDHVs.GetByIdAsync(r.StationAnhDHVId);
                    var connector = await _unitOfWork.Connectors.GetByIdAsync(r.ConnectorId);
                    items.Add(new ReservationDto
                    {
                        Id = r.LongLQID,
                        User = user.FirstName,
                        StationName = station?.Name ?? string.Empty,
                        ConnectorType = connector?.ConnectorType ?? ConnectorType.Unknown,
                        MinPowerKw = r.MinPowerKw,
                        Status = r.Status,
                        StartTime = r.StartTime,
                        EndTime = r.EndTime,
                        CreatedAt = r.CreatedAt
                    });
                }

                return new Pagination<ReservationDto>(items, total, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations");
                throw;
            }
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching reservation by id: {Id}", id);
                var r = await _unitOfWork.ReservationLongLQs.FirstOrDefaultAsync(x => x.LongLQID == id);
                if (r == null) return null;
                var user = await _unitOfWork.Users.GetByIdAsync(r.UserId);
                var station = await _unitOfWork.StationAnhDHVs.GetByIdAsync(r.StationAnhDHVId);
                var connector = await _unitOfWork.Connectors.GetByIdAsync(r.ConnectorId);
                return new ReservationDto
                {
                    Id = r.LongLQID,
                    User = user.FirstName,
                    StationName = station?.Name ?? string.Empty,
                    ConnectorType = connector?.ConnectorType ?? ConnectorType.Unknown,
                    MinPowerKw = r.MinPowerKw,
                    Status = r.Status,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    CreatedAt = r.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservation by id: {Id}", id);
                throw;
            }
        }

        public async Task<ReservationDto> CreateReservationAsync(ReservationCreateDto dto)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;

                _logger.LogInformation("Creating reservation for userId {UserId} at station {StationId}", userId, dto.StationId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                // Basic validation: station exists
                var station = await _unitOfWork.StationAnhDHVs.GetByIdAsync(dto.StationId);
                if (station == null)
                {
                    _logger.LogWarning("Station not found: {StationId}", dto.StationId);
                    throw new Exception("Station not found.");
                }

                // Validate times
                if (dto.EndTime.HasValue && dto.EndTime <= dto.StartTime)
                {
                    _logger.LogWarning("Invalid times for reservation: Start {Start}, End {End}", dto.StartTime, dto.EndTime);
                    throw new Exception("EndTime must be after StartTime.");
                }

                if (dto.StartTime < DateTime.UtcNow)
                {
                    _logger.LogWarning("StartTime cannot be in the past: {Start}", dto.StartTime);
                    throw new Exception("StartTime cannot be in the past.");
                }

                var connector = await _unitOfWork.Connectors.GetByIdAsync(dto.ConnectorId);

                connector = await _unitOfWork.Connectors.GetByIdAsync(dto.ConnectorId);
                if (connector == null || connector.StationAnhDHVId != dto.StationId)
                {
                    _logger.LogWarning("Connector invalid or not in station. Connector: {ConnectorId}, Station: {StationId}", dto.ConnectorId, dto.StationId);
                    throw new Exception("Connector invalid.");
                }

                var now = DateTime.UtcNow;

                var sessionOverlap = false;
                sessionOverlap = await _unitOfWork.Sessions.GetQueryable()
                    .AnyAsync(r => r.ConnectorId == dto.ConnectorId && r.Status == SessionStatus.Reserved &&
                                   ((r.EndTime == null && dto.EndTime == null) ||
                                    (r.EndTime == null && dto.EndTime > r.StartTime) ||
                                    (dto.EndTime == null && dto.StartTime < (r.EndTime ?? DateTime.MaxValue)) ||
                                    (dto.StartTime < (r.EndTime ?? DateTime.MaxValue) && (dto.EndTime ?? DateTime.MaxValue) > r.StartTime)));

                if (sessionOverlap)
                {
                    _logger.LogWarning("Connector already reserved for the specified time range. Connector: {ConnectorId}", dto.ConnectorId);
                    throw new Exception("Connector already reserved for the specified time range.");
                }
                var reservation = new ReservationLongLQ
                {
                    LongLQID = Guid.NewGuid(),
                    UserId = userId,
                    StationAnhDHVId = dto.StationId,
                    ConnectorId = dto.ConnectorId,
                    PreferredConnectorType = connector.ConnectorType,
                    MinPowerKw = connector.PowerKw,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Status = ReservationStatus.Pending,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreatedBy = userId
                };

                await _unitOfWork.ReservationLongLQs.AddAsync(reservation);
                await _unitOfWork.SaveChangesAsync();
                var session = new Session
                {
                    LongLQID = Guid.NewGuid(),
                    ConnectorId = reservation.ConnectorId,
                    UserId = reservation.UserId,
                    ReservationLongLQId = reservation.LongLQID,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    Status = SessionStatus.Reserved,
                    IsDeleted = false
                };

                await _unitOfWork.Sessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Session created for reservation {ReservationId}: {SessionId}", reservation.LongLQID, session.LongLQID);

                _logger.LogInformation("Reservation created: {ReservationId}", reservation.LongLQID);
                return new ReservationDto
                {
                    Id = reservation.LongLQID,
                    User = user.FirstName,
                    StationName = station.Name,
                    ConnectorType = connector.ConnectorType,
                    MinPowerKw = reservation.MinPowerKw,
                    Status = reservation.Status,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    CreatedAt = reservation.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Cancelling reservation: {Id}", id);
                var reservation = await _unitOfWork.ReservationLongLQs.GetByIdAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reservation not found: {Id}", id);
                    return false;
                }

                if (reservation.Status == ReservationStatus.Canceled || reservation.Status == ReservationStatus.Completed)
                {
                    _logger.LogWarning("Reservation cannot be canceled: {Id}, Status: {Status}", id, reservation.Status);
                    return false;
                }

                reservation.Status = ReservationStatus.Canceled;
                reservation.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.ReservationLongLQs.Update(reservation);
                await _unitOfWork.SaveChangesAsync();

                // If there is an associated session, stop it
                var session = await _unitOfWork.Sessions.FirstOrDefaultAsync(s => s.ReservationLongLQId == reservation.LongLQID);
                if (session != null && session.Status == SessionStatus.Running)
                {
                    session.EndTime = DateTime.UtcNow;
                    session.Status = SessionStatus.Failed;
                    session.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Sessions.Update(session);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Associated session stopped for canceled reservation: {ReservationId}, SessionId: {SessionId}", reservation.LongLQID, session.LongLQID);
                }

                _logger.LogInformation("Reservation canceled: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling reservation: {Id}", id);
                throw;
            }
        }

        public async Task<ReservationDto> UpdateReservationAsync(Guid id, ReservationUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating reservation: {Id}", id);
                var reservation = await _unitOfWork.ReservationLongLQs.GetByIdAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reservation not found: {Id}", id);
                    throw new Exception("Reservation not found.");
                }

                // Validate times
                if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.EndTime <= dto.StartTime)
                {
                    _logger.LogWarning("Invalid times for reservation update: Start {Start}, End {End}", dto.StartTime, dto.EndTime);
                    throw new Exception("EndTime must be after StartTime.");
                }

                if (dto.StartTime.HasValue && dto.StartTime < DateTime.UtcNow)
                {
                    _logger.LogWarning("StartTime cannot be in the past: {Start}", dto.StartTime);
                    throw new Exception("StartTime cannot be in the past.");
                }
                var sessionOverlap = false;
                sessionOverlap = await _unitOfWork.Sessions.GetQueryable()
                    .AnyAsync(r => r.ConnectorId == dto.ConnectorId && r.Status == SessionStatus.Reserved &&
                                   ((r.EndTime == null && dto.EndTime == null) ||
                                    (r.EndTime == null && dto.EndTime > r.StartTime) ||
                                    (dto.EndTime == null && dto.StartTime < (r.EndTime ?? DateTime.MaxValue)) ||
                                    (dto.StartTime < (r.EndTime ?? DateTime.MaxValue) && (dto.EndTime ?? DateTime.MaxValue) > r.StartTime)));

                if (sessionOverlap)
                {
                    _logger.LogWarning("Connector already reserved for the specified time range. Connector: {ConnectorId}", dto.ConnectorId);
                    throw new Exception("Connector already reserved for the specified time range.");
                }

                if (dto.StationId.HasValue && dto.StationId != reservation.StationAnhDHVId)
                {
                    var newStation = await _unitOfWork.StationAnhDHVs.GetByIdAsync(dto.StationId.Value);
                    if (newStation == null)
                    {
                        _logger.LogWarning("Station not found: {StationId}", dto.StationId);
                        throw new Exception("Station not found.");
                    }
                    reservation.StationAnhDHVId = newStation.LongLQID;

                    if (!dto.ConnectorId.HasValue && dto.ConnectorId == reservation.ConnectorId)
                    {
                        _logger.LogWarning("Connector must be specified when changing station: {StationId}", dto.StationId);
                        throw new Exception("Connector must be specified when changing station.");
                    }
                }

                if (dto.ConnectorId.HasValue && dto.ConnectorId != reservation.ConnectorId)
                {
                    var connector = await _unitOfWork.Connectors.GetByIdAsync(dto.ConnectorId.Value);
                    if (connector == null || connector.StationAnhDHVId != reservation.StationAnhDHVId)
                    {
                        _logger.LogWarning("Connector invalid for reservation: {ConnectorId}", dto.ConnectorId);
                        throw new Exception("Connector invalid.");
                    }
                    reservation.ConnectorId = dto.ConnectorId.Value;
                    reservation.PreferredConnectorType = connector.ConnectorType;
                    reservation.MinPowerKw = connector.PowerKw;
                }

                if (dto.StartTime.HasValue)
                    reservation.StartTime = dto.StartTime.Value;

                if (dto.EndTime.HasValue)
                    reservation.EndTime = dto.EndTime.Value;

                if (dto.Status.HasValue)
                    reservation.Status = dto.Status.Value;

                reservation.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.ReservationLongLQs.Update(reservation);
                await _unitOfWork.SaveChangesAsync();

                // Update or create session depending on reservation state
                var session = await _unitOfWork.Sessions.FirstOrDefaultAsync(s => s.ReservationLongLQId == reservation.LongLQID);

                // If reservation is canceled or completed, ensure session is stopped
                if (reservation.Status == ReservationStatus.Canceled || reservation.Status == ReservationStatus.Completed)
                {
                    if (session != null && session.Status == SessionStatus.Running)
                    {
                        session.EndTime = reservation.EndTime ?? DateTime.UtcNow;
                        session.Status = SessionStatus.Failed;
                        session.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.Sessions.Update(session);
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation("Session canceled due to reservation status change: {ReservationId}, SessionId: {SessionId}", reservation.LongLQID, session.LongLQID);
                    }
                }
                else
                {
                    if (dto.ConnectorId.HasValue)
                        session.ConnectorId = dto.ConnectorId.Value;

                    if (dto.StartTime.HasValue)
                        session.StartTime = dto.StartTime.Value;

                    if (dto.EndTime.HasValue)
                    {
                        session.EndTime = dto.EndTime.Value;
                    }

                    session.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Sessions.Update(session);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Session updated for reservation: {ReservationId}, SessionId: {SessionId}", reservation.LongLQID, session.LongLQID);
                }

                _logger.LogInformation("Reservation updated: {Id}", id);

                // Build response similar to CreateReservationAsync/GetReservationByIdAsync
                var user = await _unitOfWork.Users.GetByIdAsync(reservation.UserId);
                var station = await _unitOfWork.StationAnhDHVs.GetByIdAsync(reservation.StationAnhDHVId);
                var resConnector = await _unitOfWork.Connectors.GetByIdAsync(reservation.ConnectorId);

                return new ReservationDto
                {
                    Id = reservation.LongLQID,
                    User = user.FirstName,
                    StationName = station?.Name ?? string.Empty,
                    ConnectorType = resConnector?.ConnectorType ?? ConnectorType.Unknown,
                    MinPowerKw = reservation.MinPowerKw,
                    Status = reservation.Status,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    CreatedAt = reservation.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation: {Id}", id);
                throw;
            }
        }
    }
}
