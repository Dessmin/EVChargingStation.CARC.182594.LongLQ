using EVChargingStation.CARC.Domain.LongLQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly FA25_SWD392_SE182594_G6_EvChargingStation _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminController> _logger;

        public AdminController(FA25_SWD392_SE182594_G6_EvChargingStation dbContext,
        IWebHostEnvironment env,
        IConfiguration config,
        ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Drop the application's database. Allowed when running in Development environment
        /// or when the correct secret is provided via the "X-Admin-Secret" header.
        /// WARNING: This deletes all data.
        /// </summary>
        [HttpDelete("drop-database")]
        public async Task<IActionResult> DropDatabase()
        {
            // Check environment
            var allowedInDev = _env.IsDevelopment();

            var secret = _config["Admin:DropSecret"];
            var provided = Request.Headers.TryGetValue("X-Admin-Secret", out var providedSecret) ? providedSecret.ToString() : string.Empty;
            var secretMatches = !string.IsNullOrEmpty(secret) && provided == secret;

            if (!allowedInDev && !secretMatches)
            {
                _logger.LogWarning("Unauthorized attempt to drop database. Environment: {Env}", _env.EnvironmentName);
                return Forbid();
            }

            try
            {
                _logger.LogWarning("Dropping database initiated by {Source}.", allowedInDev ? "Development" : "AdminSecret");
                var deleted = await _dbContext.Database.EnsureDeletedAsync();
                if (deleted)
                {
                    _logger.LogWarning("Database dropped successfully.");
                    return Ok(new { message = "Database dropped." });
                }

                _logger.LogInformation("Database did not exist or could not be dropped.");
                return Ok(new { message = "Database did not exist or was not dropped." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while attempting to drop the database.");
                return StatusCode(500, new { message = "Error while attempting to drop the database.", detail = ex.Message });
            }
        }
    }
}
