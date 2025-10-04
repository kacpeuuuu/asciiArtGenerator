using asciiArtGenerator;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace asciiArtGenerator
{
    [SupportedOSPlatform("windows")]
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string imagePath = @"C:\Users\kacpeu\Desktop\nature1.jpeg";  // ← podaj swoją ścieżkę do obrazu
            using Bitmap fullImage = new Bitmap(imagePath);
            Console.ReadLine();
            //int newWidth = fullImage.Width / 2;
            //int newHeight = (int)(fullImage.Height / 2 * 0.65) ;

            int newWidth = fullImage.Width;
            int newHeight = (int)(fullImage.Height * 0.65);

            using Bitmap colorBitmap = new Bitmap(fullImage, new Size(newWidth, newHeight));


            // Tworzymy kopię do obróbki krawędziowej
            using Bitmap edgeBitmap = new Bitmap(colorBitmap);

            // Lock obu bitmap, aby pracować bezpośrednio na danych
            Rectangle rect = new Rectangle(0, 0, colorBitmap.Width, colorBitmap.Height);
            BitmapData colorData = colorBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData edgeData = edgeBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            try
            {
                // 1️⃣ Najpierw grayscale
                ImageFilters.Grayscale(edgeData);

                // 2️⃣ Potem wykrywanie krawędzi (np. Sobel)
                //ImageFilters.SobelXY(edgeData);
                //ImageFilters.GaussianBlur(edgeData);
                //ImageFilters.GaussianBlur(edgeData);

                // 3️⃣ ASCII art generator
                new CharacterMatching(colorData, edgeData);
            }
            finally
            {
                // Odblokuj pamięć po zakończeniu
                colorBitmap.UnlockBits(colorData);
                edgeBitmap.UnlockBits(edgeData);
            }

            stopwatch.Stop();
            // Reset koloru terminala
            Console.WriteLine("\u001b[0m");
            Console.WriteLine($"\nElapsed Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}



        //{
        //    string inputPath = @"C:\Users\kacpeu\Desktop\jampi3.jpg";
        //    string outputGrayPath = @"C:\Users\kacpeu\Desktop\output1_grayscale.png";
        //    string outputBlurPath = @"C:\Users\kacpeu\Desktop\output1_gauss.png";
        //    string outputDiffPath = @"C:\Users\kacpeu\Desktop\output1_diff.png";
        //    string outputEdgePath = @"C:\Users\kacpeu\Desktop\edge.png";
        //    string colorVector = @"C:\Users\kacpeu\Desktop\colorVector.png";
        //    string cielab = @"C:\Users\kacpeu\Desktop\cielab.png";

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

        //        // === 5️⃣ Color Difference Detection ===
        //        using (Bitmap colorDiff = (Bitmap)original.Clone())
        //        {
        //            sw.Restart();
        //            Console.WriteLine($"Loaded image pixel format: {colorDiff.PixelFormat}");

        //            // ✅ ZAPISZ KOPIĘ ORYGINALNEGO obrazu (przed filtrem)
        //            using (Bitmap originalCopy = (Bitmap)colorDiff.Clone())
        //            {
        //                originalCopy.Save(colorVector.Replace(".png", "_original.png"), ImageFormat.Png);
        //            }

        //            // 🧠 Teraz możesz spokojnie modyfikować colorDiff
        //            BitmapData diffData = colorDiff.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        //            ImageFilters.ColorDifference(diffData);
        //            colorDiff.UnlockBits(diffData);

        //            // ✅ Zapisz wynik po filtrze
        //            colorDiff.Save(colorVector, ImageFormat.Png);

        //            sw.Stop();
        //            Console.WriteLine($"Color difference edges saved → {colorVector} ({sw.ElapsedMilliseconds} ms)");
        //        }


        //        using (Bitmap morph = (Bitmap)original.Clone())
        //        {
        //            BitmapData morphData = morph.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        //            ImageFilters.Grayscale(morphData);
        //            ImageFilters.Erosion(morphData, 3);
        //            morph.UnlockBits(morphData);
        //            morph.Save(@"C:\Users\kacpeu\Desktop\erosion.png", ImageFormat.Png);
        //        }
        //    }
        //    Console.WriteLine("\n✅ All filters applied successfully!");
        //}