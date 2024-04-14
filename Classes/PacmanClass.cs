using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Classes
{
    public class PacmanClass
    {
        public PacmanClass()
        {
            PositionX = 20;
            PositionY = 20;
            Direction = Direction.Right;
            NextDirection = Direction.Right;
        }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public Direction Direction { get; set; }

        public Direction NextDirection { get; set; }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}

