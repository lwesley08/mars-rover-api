using mars_rover_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mars_rover_api.Data
{
    public class Database
    {
        public static GridBoundaries gridBoundaries = null;
        public static List<RoverLocation> roverLocations = new List<RoverLocation>();
        public static List<Rover> rovers = new List<Rover>() { 
            new Rover() { RoverId = 1, RoverName = "Rover One" },
            new Rover() { RoverId = 2, RoverName = "Rover Two" }
        };
        public static List<char> cardinalDirections = new List<char>() { 'N', 'E', 'S', 'W' };
    }
}
