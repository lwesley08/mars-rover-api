using mars_rover_api.Data;
using mars_rover_api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace mars_rover_api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("GridBoundaries")]
    [EnableCors("AllowAll")]
    public class GridBoundariesController : ControllerBase
    {
        private readonly ILogger<GridBoundariesController> _logger;

        public GridBoundariesController(ILogger<GridBoundariesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<GridBoundaries> SetGridBoundaries([FromBody]GridBoundaries gridBoundaries)
        {
            try
            {
                _logger.LogInformation("Setting Grid Boundaries.");

                Database.gridBoundaries = gridBoundaries;

                _logger.LogInformation("Successfully Set Grid Boundaries.");

                return gridBoundaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure Setting Grid Boundaries");
                return new BadRequestResult();
            }
        }

    }
}
