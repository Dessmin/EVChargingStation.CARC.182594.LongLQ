using EVChargingStation.CARC.Application.LongLQ.DTOs.ReservationDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Application.LongLQ.Utils;
using EVChargingStation.CARC.Domain.LongLQ.Enums;
using EVChargingStation.CARC.Infrastructure.LongLQ.Commons;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IClaimsService _claimsService;

        public ReservationController(IReservationService reservationService, IClaimsService claimsService)
        {
            _reservationService = reservationService;
            _claimsService = claimsService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get reservations", Description = "Get paginated reservations with optional filters.")]
        [ProducesResponseType(typeof(ApiResult<Pagination<ReservationDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<IActionResult> GetReservations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? userId = null, [FromQuery] ReservationStatus? status = null)
        {
            try
            {
                var result = await _reservationService.GetReservationsAsync(pageNumber, pageSize, userId, status);
                return Ok(ApiResult<Pagination<ReservationDto>>.Success(result, "200", "Fetched reservations."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<Pagination<ReservationDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get reservation by id", Description = "Get a reservation by its id.")]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 400)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var result = await _reservationService.GetReservationByIdAsync(id);
                return Ok(ApiResult<ReservationDto>.Success(result!, "200", "Fetched reservation."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReservationDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create reservation", Description = "Create a new reservation.")]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 400)]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(dto);
                return Ok(ApiResult<ReservationDto>.Success(result, "200", "Reservation created."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReservationDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update reservation", Description = "Update an existing reservation.")]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<ReservationDto>), 400)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ReservationUpdateDto dto)
        {
            try
            {
                var result = await _reservationService.UpdateReservationAsync(id, dto);
                return Ok(ApiResult<ReservationDto>.Success(result, "200", "Reservation updated."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReservationDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost("{id}/cancel")]
        [SwaggerOperation(Summary = "Cancel reservation", Description = "Cancel a reservation by id.")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<bool>), 400)]
        public async Task<IActionResult> Cancel([FromRoute] Guid id)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(id);
                return Ok(ApiResult<bool>.Success(result, "200", "Reservation canceled."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<bool>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
