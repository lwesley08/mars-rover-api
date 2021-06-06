using mars_rover_api.Data;
using mars_rover_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mars_rover_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoverLocationController : ControllerBase
    {
        private readonly ILogger<RoverLocationController> _logger;

        public RoverLocationController(ILogger<RoverLocationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{roverId}")]
        public ActionResult<RoverLocation> GetLastRoverLocation(int roverId)
        {
            try
            {
                _logger.LogInformation($"Retrieving Last Rover Location for Rover # {roverId}.");

                RoverLocation roverLocation = Database.roverLocations?.Where(x => x.RoverId == roverId)?.OrderByDescending(x => x.CreateDate)?.First();

                _logger.LogInformation($"Successfully Last Rover Location for Rover # {roverId}.");

                return roverLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure Last Rover Location for Rover # {roverId}.");
                return new BadRequestResult();
            }
        }
        
        [HttpGet]
        [Route("{roverId}")]
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

        [HttpPost]
        public ActionResult<RoverLocation> SetRoverStartingLocation([FromBody]RoverLocation roverLocation)
        {
            try
            {
                _logger.LogInformation($"Setting Rover Starting Location for Rover # {roverLocation.RoverId}.");

                RoverLocation dbRoverLocation = Database.roverLocations
                                                        ?.Where(x => x.RoverId == roverLocation.RoverId)
                                                        ?.FirstOrDefault();
                
                if (dbRoverLocation != null)
                {
                    return new BadRequestObjectResult(new { Value = $"Rover #{roverLocation.RoverId} already has a location. Try moving rover instead." });
                }

                bool validationResult = ValidateRoverLocation(roverLocation);

                if (validationResult == false)
                {
                    return new BadRequestObjectResult(new { Value = $"Starting Location for Rover #{roverLocation.RoverId} out of bounds or bounds not set." });
                }

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
                    return new BadRequestObjectResult(new { Value = $"Rover #{roverInstructions.RoverId} does not have a location. Try setting starting location first." });
                }

                RoverLocation roverLocation = CalculateNewLocation(currentRoverLocation, roverInstructions.Instructions);

                bool validationResult = ValidateRoverLocation(roverLocation);

                if (validationResult == false)
                {
                    return new BadRequestObjectResult(new { Value = $"Instructions for Rover #{roverInstructions.RoverId} out of bounds or bounds not set." });
                }

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

        private RoverLocation CalculateNewLocation(RoverLocation currentLocation, string instructions)
        {
            char[] instructionsAsCharArray = instructions?.ToCharArray();
            RoverLocation newRoverLocation = new RoverLocation() { 
                RoverId = currentLocation.RoverId, 
                X = currentLocation.X,
                Y = currentLocation.Y,
                Z = currentLocation.Z,
            };

            foreach(char direction in instructionsAsCharArray)
            {
                switch (direction)
                {
                    case 'L':
                        newRoverLocation.Z = TurnLeft(newRoverLocation.Z);
                        break;
                    case 'R':
                        newRoverLocation.Z = TurnRight(newRoverLocation.Z);
                        break;
                    case 'M':
                        newRoverLocation = MoveForward(newRoverLocation);
                        break;
                    default:
                        throw new Exception($"Invalid instruction - { direction }.");
                }
            }

            return newRoverLocation;
        }

        private char TurnLeft(char currentZ)
        {
            int currentDirectionIndex = Database.cardinalDirections.IndexOf(currentZ);
            if (currentDirectionIndex == 0)
            {
                return Database.cardinalDirections.Last();
            }
            else
            {
                return Database.cardinalDirections[currentDirectionIndex - 1];
            }
        }
        
        private char TurnRight(char currentZ)
        {
            int currentDirectionIndex = Database.cardinalDirections.IndexOf(currentZ);
            if (currentDirectionIndex == 4)
            {
                return Database.cardinalDirections.First();
            }
            else
            {
                return Database.cardinalDirections[currentDirectionIndex + 1];
            }
        }
        
        private RoverLocation MoveForward(RoverLocation roverLocation)
        {
            switch (roverLocation.Z)
            {
                case 'N':
                    roverLocation.Y++;
                    break;
                case 'E':
                    roverLocation.X++;
                    break;
                case 'S':
                    roverLocation.Y--;
                    break;
                case 'W':
                    roverLocation.X--;
                    break;
                default:
                    throw new Exception($"Invalid instruction.");
            }
            return roverLocation;
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
