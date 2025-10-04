using asciiArtGenerator;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace asciiArtGenerator
{
    internal class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {

        }
    }
}


//{
//    string inputPath = @"C:\Users\kacpeu\Desktop\mela.jpg";
//    string outputGrayPath = @"C:\Users\kacpeu\Desktop\output1_grayscale.png";
//    string outputBlurPath = @"C:\Users\kacpeu\Desktop\output1_gauss.png";
//    string outputDiffPath = @"C:\Users\kacpeu\Desktop\output1_diff.png";
//    string outputEdgePath = @"C:\Users\kacpeu\Desktop\edge.png";
//    string outputAllEdgePath = @"C:\Users\kacpeu\Desktop\allEdges.png";

//    Stopwatch sw = new Stopwatch();

//    using (Bitmap original = new Bitmap(inputPath))
//    {
//        Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);

//        // === 1️⃣ Grayscale ===
//        using (Bitmap gray = (Bitmap)original.Clone())
//        {
//            sw.Restart();
//            BitmapData grayData = gray.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
//            ImageFilters.Grayscale(grayData);
//            gray.UnlockBits(grayData);
//            gray.Save(outputGrayPath, ImageFormat.Png);
//            sw.Stop();

//            Console.WriteLine($"Grayscale saved → {outputGrayPath} ({sw.ElapsedMilliseconds} ms)");

//            // === 2️⃣ Gaussian Blur ===
//            using (Bitmap blurred = (Bitmap)gray.Clone())
//            {
//                sw.Restart();
//                BitmapData blurData = blurred.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
//                ImageFilters.GaussianBlur(blurData);
//                blurred.UnlockBits(blurData);
//                blurred.Save(outputBlurPath, ImageFormat.Png);
//                sw.Stop();

//                Console.WriteLine($"Gaussian blur saved → {outputBlurPath} ({sw.ElapsedMilliseconds} ms)");

//                // === 3️⃣ Difference (gray - blur) ===
//                sw.Restart();
//                BitmapData grayDataRO = gray.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
//                BitmapData blurDataRO = blurred.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
//                Bitmap diff = ImageFilters.GetDifference(grayDataRO, blurDataRO);
//                gray.UnlockBits(grayDataRO);
//                blurred.UnlockBits(blurDataRO);
//                diff.Save(outputDiffPath, ImageFormat.Png);
//                sw.Stop();

//                Console.WriteLine($"Difference saved → {outputDiffPath} ({sw.ElapsedMilliseconds} ms)");
//            }
//        }

//        // === 4️⃣ Sobel edge detection ===
//        using (Bitmap sobelEdges = (Bitmap)original.Clone())
//        {
//            sw.Restart();
//            BitmapData sobelData = sobelEdges.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
//            ImageFilters.Grayscale(sobelData);
//            ImageFilters.GaussianBlur(sobelData);
//            ImageFilters.SobelXY(sobelData);
//            sobelEdges.UnlockBits(sobelData);
//            sobelEdges.Save(outputEdgePath, ImageFormat.Png);
//            sw.Stop();

//            Console.WriteLine($"Sobel edges saved → {outputEdgePath} ({sw.ElapsedMilliseconds} ms)");
//        }
//    }
//    Console.WriteLine("\n✅ All filters applied successfully!");
//}