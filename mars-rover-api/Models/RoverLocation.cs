using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mars_rover_api.Models
{
    public class RoverLocation
    {
        public int RoverId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public char Z { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
