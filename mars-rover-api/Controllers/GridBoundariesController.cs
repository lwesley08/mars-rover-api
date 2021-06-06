using mars_rover_api.Data;
using mars_rover_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace mars_rover_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GridBoundariesController : ControllerBase
    {
        private readonly ILogger<GridBoundariesController> _logger;

        public GridBoundariesController(ILogger<GridBoundariesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<GridBoundaries> GetGridBoundaries()
        {
            try
            {
                _logger.LogInformation("Retrieving Grid Boundaries.");

                GridBoundaries gridBoundaries = Database.gridBoundaries;

                _logger.LogInformation("Successfully Retrieved Grid Boundaries.");

                return gridBoundaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure Retriving Grid Boundaries");
                return new BadRequestResult();
            }
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
