using binaryToInt;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace asciiArtGenerator
{
    [SupportedOSPlatform("windows")]

    internal class CharacterMatching
    {
        private readonly int _charsHorizontal;
        private readonly int _charsVertical;
        internal string _log;
        public CharacterMatching(BitmapData colorBmpData, BitmapData edgeBmpData)
        {

            _charsHorizontal = colorBmpData.Width / 8;
            _charsVertical = colorBmpData.Height / 8;
            
            //color
            int stride = colorBmpData.Stride;

            byte[] colorBuffer = new byte[colorBmpData.Height * stride];
            Marshal.Copy(colorBmpData.Scan0, colorBuffer, 0, colorBmpData.Stride*colorBmpData.Height);

            //edges
            byte[] edgeBuffer = new byte[colorBmpData.Height * stride];
            Marshal.Copy(edgeBmpData.Scan0, edgeBuffer, 0, edgeBmpData.Stride * edgeBmpData.Height);

            // here we slice the pic into 8x8 grids and give them to the pattern matcher
            for (int y = 0; y < _charsVertical; y++)
            {
                for (int x = 0; x < _charsHorizontal; x++)
                {
                    int blockIndex = (y * 8) * stride + (x * 8) * 3;
                    byte[] edgeGrid8x8 = new byte[64];
                    byte[] colorGrid8x8 = new byte[64*3];
                    for (int ky = 0; ky < 8; ky++)
                    {
                        for (int kx = 0; kx < 8; kx++)
                        {
                            int pixelIndex = blockIndex + ky*stride + kx * 3;
                            int localGridIndex = ky * 8 + kx;
                            edgeGrid8x8[localGridIndex] = edgeBuffer[pixelIndex];

                            colorGrid8x8[localGridIndex*3] = colorBuffer[pixelIndex];
                            colorGrid8x8[localGridIndex*3 + 1] = colorBuffer[pixelIndex+1];
                            colorGrid8x8[localGridIndex*3 + 2] = colorBuffer[pixelIndex+2];
                        }
                    }
                    char bestChar = MatchChar(edgeGrid8x8);
                    int[] colors = GetAverageColor(colorGrid8x8);
                    WriteCharacter(colors[0], colors[1], colors[2], bestChar);
                }
                Console.WriteLine();
            }
            
        }

        static void WriteCharacter(int blue, int green, int red, char character)
        {
            Console.Write($"\u001b[38;2;{red};{green};{blue}m{character}");
        }

        static int[] GetAverageColor(byte[] grid)
        {
            int sumBlue = 0;
            int sumGreen = 0;
            int sumRed = 0;

            for(int i = 0; i < 64; i++)
            {
                sumBlue += grid[i*3];
                sumGreen += grid[i*3+1];
                sumRed += grid[i*3+2];
            }
            return new int[] { sumBlue / 64, sumGreen / 64, sumRed / 64 };
        }

        static char MatchChar(byte[] grid)
        {
            byte[,,] byteChars = byteValues.byteChars;
            char[] asciiChars = byteValues.asciiChars;

            int bestScore = int.MaxValue;
            int bestIndex = 0;

            int charCount = asciiChars.Length;

            for (int c = 0; c < charCount; c++)
            {
                int score = 0;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int gridIndex = y * 8 + x;
                        int diff = grid[gridIndex] - byteChars[c, y, x];
                        score += diff * diff;
                    }
                }

                //Console.WriteLine($"score: {score} character: {asciiChars[c]}");
                



                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = c;
                }
            }

            return asciiChars[bestIndex];
        }

    }
}
