using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asciiArtGenerator
{
    internal class tempChars
    {
        static byte[,,] byteChars = new byte[,,]
        {
            {
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
            },
            {
                {0, 0, 0, 255, 255, 0, 0, 0},
                {0, 0, 255, 255, 255, 255, 0, 0},
                {0, 0, 255, 255, 255, 255, 0, 0},
                {0, 0, 0, 255, 255, 0, 0, 0},
                {0, 0, 0, 255, 255, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 255, 255, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
            }};
    }
}
