using mars_rover_api;
using mars_rover_api.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace mars_rover_api_test
{
    public class Tests
    {
        readonly List<RoverLocation> roverStartingLocations = new List<RoverLocation>() { 
            new RoverLocation() { RoverId = 1, X = 1, Y = 2, Z = 'N' },
            new RoverLocation() { RoverId = 2, X = 3, Y = 3, Z = 'E' }
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase('N', 'W')]
        [TestCase('E', 'N')]
        [TestCase('S', 'E')]
        [TestCase('W', 'S')]
        public void TurnLeft(char currentZ, char expectedResult)
        {
            var result = LocationUtility.TurnLeft(currentZ);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        
        [Test]
        [TestCase('N', 'E')]
        [TestCase('E', 'S')]
        [TestCase('S', 'W')]
        [TestCase('W', 'N')]
        public void TurnRight(char currentZ, char expectedResult)
        {
            var result = LocationUtility.TurnRight(currentZ);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(1, 2, 'N')]
        public void MoveForward_IncrementsXIfFacingNorth(int X, int Y, char Z)
        {
            var result = LocationUtility.MoveForward(new RoverLocation() { X = X, Y = Y, Z = Z });
            Assert.That(result.X, Is.EqualTo(X++));
        }
        
        [Test]
        [TestCase(1, 2, 'S')]
        public void MoveForward_DecrementsXIfFacingSouth(int X, int Y, char Z)
        {
            var result = LocationUtility.MoveForward(new RoverLocation() { X = X, Y = Y, Z = Z });
            Assert.That(result.X, Is.EqualTo(X--));
        }
        
        [Test]
        [TestCase(1, 2, 'E')]
        public void MoveForward_IncrementsYIfFacingEast(int X, int Y, char Z)
        {
            var result = LocationUtility.MoveForward(new RoverLocation() { X = X, Y = Y, Z = Z });
            Assert.That(result.Y, Is.EqualTo(Y++));
        }
        
        [Test]
        [TestCase(1, 2, 'W')]
        public void MoveForward_DecrementsYIfFacingWest(int X, int Y, char Z)
        {
            var result = LocationUtility.MoveForward(new RoverLocation() { X = X, Y = Y, Z = Z });
            Assert.That(result.Y, Is.EqualTo(Y--));
        }

        [Test]
        [TestCase(1, "LMLMLMLMM", 1, 3, 'N')]
        [TestCase(2, "MMRMMRMRRM", 5, 1, 'E')]
        public void CalculateNewLocation(int roverId, string instructions, int expectedX, int expectedY, char expectedZ)
        {
            var startingLocation = roverStartingLocations.Where(x => x.RoverId == roverId).FirstOrDefault();
            var result = LocationUtility.CalculateNewLocation(startingLocation, instructions);
            Assert.That(result.X, Is.EqualTo(expectedX));
            Assert.That(result.Y, Is.EqualTo(expectedY));
            Assert.That(result.Z, Is.EqualTo(expectedZ));
        }
    }
}