﻿using mars_rover_api.Data;
using mars_rover_api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mars_rover_api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("RoverLocation")]
    [EnableCors("AllowAll")]
    public class RoverLocationController : ControllerBase
    {
        private readonly ILogger<RoverLocationController> _logger;

        public RoverLocationController(ILogger<RoverLocationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("SetStartingLocation")]
        public ActionResult<RoverLocation> SetRoverStartingLocation([FromBody]RoverLocation roverLocation)
        {
            try
            {
                _logger.LogInformation($"Setting Rover Starting Location for Rover # {roverLocation.RoverId}.");
                
                bool validationResult = ValidateRoverLocation(roverLocation);

                if (validationResult == false)
                {
                    throw new Exception($"Starting Location for Rover #{roverLocation.RoverId} out of bounds or bounds not set.");
                }

                roverLocation.CreateDate = DateTime.Now;

                Database.roverLocations.Add(roverLocation);

                _logger.LogInformation($"Sucessfully Set Rover Starting Location for Rover # {roverLocation.RoverId}.");

                return roverLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure Setting Rover Starting Location for Rover # {roverLocation.RoverId}.");
                return new BadRequestResult();
            }
        }
        
        [HttpPost]
        [Route("Instruct")]
        public ActionResult<RoverLocation> InstructRover([FromBody]RoverInstructions roverInstructions)
        {
            try
            {
                _logger.LogInformation($"Instructing Rover #{roverInstructions.RoverId}.");

                RoverLocation currentRoverLocation = Database.roverLocations?.Where(x => x.RoverId == roverInstructions.RoverId)
                                                                            ?.OrderByDescending(x => x.CreateDate)
                                                                            ?.FirstOrDefault();
                
                if (currentRoverLocation == null)
                {
                    throw new Exception($"Rover #{roverInstructions.RoverId} does not have a location. Try setting starting location first.");
                }

                RoverLocation roverLocation = LocationUtility.CalculateNewLocation(currentRoverLocation, roverInstructions.Instructions);

                bool validationResult = ValidateRoverLocation(roverLocation);

                if (validationResult == false)
                {
                    throw new Exception("Instructions for Rover #{roverInstructions.RoverId} out of bounds or bounds not set.");
                }

                roverLocation.CreateDate = DateTime.Now;

                Database.roverLocations.Add(roverLocation);

                _logger.LogInformation($"Sucessfully Set Rover Starting Location for Rover # {roverLocation.RoverId}.");

                return roverLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure Setting Rover Starting Location for Rover # {roverInstructions.RoverId}.");
                return new BadRequestResult();
            }
        }

        [HttpGet]
        [Route("GetLocationHistory/{roverId}")]
        public ActionResult<List<RoverLocation>> GetRoverLocationHistory(int roverId)
        {
            try
            {
                _logger.LogInformation($"Retrieving Rover Location History for Rover # {roverId}.");

                List<RoverLocation> roverLocation = Database.roverLocations?.Where(x => x.RoverId == roverId)
                                                                            ?.OrderByDescending(x => x.CreateDate)
                                                                            ?.ToList();

                _logger.LogInformation($"Successfully Rover Location History for Rover # {roverId}.");

                return roverLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure Rover Location History for Rover # {roverId}.");
                return new BadRequestResult();
            }
        }

        private bool ValidateRoverLocation(RoverLocation roverLocation)
        {
            try
            {
                if (roverLocation.X < 0 || roverLocation.Y < 0) { return false; }

                if (!Database.cardinalDirections.Contains(roverLocation.Z)) { return false; }

                GridBoundaries gridBoundaries = Database.gridBoundaries;

                if (gridBoundaries == null) { return false; }

                if (roverLocation.X > gridBoundaries.X || roverLocation.Y > gridBoundaries.Y) { return false; }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
