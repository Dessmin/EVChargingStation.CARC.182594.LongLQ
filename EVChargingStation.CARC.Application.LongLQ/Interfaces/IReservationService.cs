using EVChargingStation.CARC.Infrastructure.LongLQ.Commons;
using EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs;
using EVChargingStation.CARC.Domain.LongLQ.Enums;

namespace EVChargingStation.CARC.Application.LongLQ.Interfaces
{
    public interface IReservationService
    {
        Task<Pagination<ReservationDto>> GetReservationsAsync(int pageNumber =1, int pageSize =10, Guid? userId = null, ReservationStatus? status = null);

        Task<ReservationDto?> GetReservationByIdAsync(Guid id);

        Task<ReservationDto> CreateReservationAsync(ReservationCreateDto dto);

        Task<bool> CancelReservationAsync(Guid id);

        Task<ReservationDto> UpdateReservationAsync(Guid id, ReservationUpdateDto dto);
    }
}
