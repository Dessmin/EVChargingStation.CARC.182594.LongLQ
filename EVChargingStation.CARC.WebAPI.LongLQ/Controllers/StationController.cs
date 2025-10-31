using EVChargingStation.CARC.Application.LongLQ.DTOs.StationDTOs;
using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Application.LongLQ.Utils;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Controllers
{
    [Route("api/stations")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;
        private readonly IClaimsService _claimsService;

        public StationController(IStationService stationService, IClaimsService claimsService)
        {
            _stationService = stationService;
            _claimsService = claimsService; // fixed field name
        }

        [HttpGet("connectors")]
        [SwaggerOperation(Summary = "List stations and connector ids", Description = "Returns all stations and the ids of their connectors.")]
        [ProducesResponseType(typeof(ApiResult<List<StationConnectorsDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<IActionResult> GetStationsWithConnectors()
        {
            try
            {
                var result = await _stationService.GetStationsWithConnectorIdsAsync();
                return Ok(ApiResult<List<StationConnectorsDto>>.Success(result, "200", "Fetched stations with connectors."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<StationConnectorsDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
