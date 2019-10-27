using System;
using System.Linq;
using System.Collections.Generic;

namespace GameEngine
{
    public class CheckLine
    {
        public readonly int X;
        public readonly int Y;
        public readonly string Title;
        public readonly int[] Surroundings = Enumerable.Repeat<int>(1, 4).ToArray();
        public CheckLine(int pY, int pX, string pTitle)
        {
            Title = pTitle;
            Y = pY;
            X = pX;
        }
    }
}

// This class was created to add properties to array of elements on the board. []surroundings will track the power(number of consecutive elements)
// of every element. Aside from this class we need only 2d"directions" array and one small function.