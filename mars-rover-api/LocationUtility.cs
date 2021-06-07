using mars_rover_api.Data;
using mars_rover_api.Models;
using System;
using System.Linq;

namespace mars_rover_api
{
    public class LocationUtility
    {
        public static RoverLocation CalculateNewLocation(RoverLocation currentLocation, string instructions)
        {
            char[] instructionsAsCharArray = instructions?.ToCharArray();
            RoverLocation newRoverLocation = new RoverLocation()
            {
                RoverId = currentLocation.RoverId,
                X = currentLocation.X,
                Y = currentLocation.Y,
                Z = currentLocation.Z,
            };

            foreach (char direction in instructionsAsCharArray)
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

        public static char TurnLeft(char currentZ)
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

        public static char TurnRight(char currentZ)
        {
            int currentDirectionIndex = Database.cardinalDirections.IndexOf(currentZ);
            if (currentDirectionIndex == 3)
            {
                return Database.cardinalDirections.First();
            }
            else
            {
                return Database.cardinalDirections[currentDirectionIndex + 1];
            }
        }

        public static RoverLocation MoveForward(RoverLocation roverLocation)
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
    }
}
