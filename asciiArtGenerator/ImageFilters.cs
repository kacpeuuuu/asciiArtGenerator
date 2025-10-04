using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace asciiArtGenerator
{
    [SupportedOSPlatform("windows")]
    internal class ImageFilters
    {
        public static void Grayscale(BitmapData bmpData)
        {
            int stride = bmpData.Stride;
            IntPtr scan0 = bmpData.Scan0;
            byte[] buffer = new byte[bmpData.Height * stride];
            Marshal.Copy(scan0, buffer, 0, buffer.Length);

            for (int y = 0; y < bmpData.Height; y++)
            {
                int rowStart = y * stride; // początek wiersza w buffer
                for (int x = 0; x < bmpData.Width; x++)
                {
                    int i = rowStart + x * 3; // dokładny indeks dla R,G,B
                    byte blue = buffer[i];
                    byte green = buffer[i + 1];
                    byte red = buffer[i + 2];

                    byte gray = (byte)((red * 0.299) + (green * 0.587) + (blue * 0.114));
                    buffer[i] = buffer[i + 1] = buffer[i + 2] = gray;
                }
            }

            Marshal.Copy(buffer, 0, scan0, buffer.Length);
        }

        public static void GaussianBlur(BitmapData bmpData)
        {
            int stride = bmpData.Stride;
            IntPtr scan0 = bmpData.Scan0;
            byte[] buffer = new byte[stride * bmpData.Height];
            byte[] resultBuffer = new byte[buffer.Length]; // separate buffer for results
            Marshal.Copy(scan0, buffer, 0, buffer.Length);

            buffer.CopyTo(resultBuffer,0);

            int[,] gaussKernel = new int[,]
            {
                { 1,  4,  6,  4,  1 },
                { 4, 16, 24, 16,  4 },
                { 6, 24, 36, 24,  6 },
                { 4, 16, 24, 16,  4 },
                { 1,  4,  6,  4,  1 }
            };

            int kernelSum = 256; // sum of kernel values

            for (int y = 2; y < bmpData.Height - 2; y++)
            {
                for (int x = 2; x < bmpData.Width - 2; x++)
                {
                    int bSum = 0, gSum = 0, rSum = 0;

                    for (int ky = -2; ky <= 2; ky++)
                    {
                        for (int kx = -2; kx <= 2; kx++)
                        {
                            int pixelIndex = (y + ky) * stride + (x + kx) * 3;
                            int weight = gaussKernel[ky + 2, kx + 2];

                            bSum += buffer[pixelIndex] * weight;
                            gSum += buffer[pixelIndex + 1] * weight;
                            rSum += buffer[pixelIndex + 2] * weight;
                        }
                    }

                    int resultIndex = y * stride + x * 3;
                    resultBuffer[resultIndex] = (byte)Math.Clamp(bSum / kernelSum, 0, 255);
                    resultBuffer[resultIndex + 1] = (byte)Math.Clamp(gSum / kernelSum, 0, 255);
                    resultBuffer[resultIndex + 2] = (byte)Math.Clamp(rSum / kernelSum, 0, 255);
                }
            }


            Marshal.Copy(resultBuffer, 0, bmpData.Scan0, resultBuffer.Length);
        }


        public static Bitmap GetDifference(BitmapData original, BitmapData filtered)
        {
            IntPtr originalScan0 = original.Scan0;
            IntPtr filteredScan0 = filtered.Scan0;
            int stride = original.Stride;

            Bitmap resultBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            BitmapData resultData = resultBitmap.LockBits(
                rect,
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);


            byte[] originalBuffer = new byte[stride * original.Height];
            byte[] filteredBuffer = new byte[stride * original.Height];
            byte[] resultBuffer = new byte[stride * original.Height];

            Marshal.Copy(originalScan0, originalBuffer, 0, originalBuffer.Length);
            Marshal.Copy(filteredScan0, filteredBuffer, 0, filteredBuffer.Length);

            for (int i = 0; i < originalBuffer.Length; i++)
            {
                resultBuffer[i] = (byte)Math.Abs(originalBuffer[i] - filteredBuffer[i]);
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }

        public static void SobelXY(BitmapData bmpData)
        {
            int stride = bmpData.Stride;
            IntPtr scan0 = bmpData.Scan0;
            byte[] buffer = new byte[stride * bmpData.Height];
            byte[] resultBuffer = new byte[buffer.Length]; // separate buffer for results

            Marshal.Copy(scan0, buffer, 0, buffer.Length);
            buffer.CopyTo(resultBuffer, 0);

            int[,] sobelX = new int[,]
{
    { -1, 0, 1 },
    { -1, 0, 1 },
    { -1, 0, 1 }
}; ;

            int[,] sobelY = new int[,]
{
    { -1, -1, -1 },
    {  0,  0,  0 },
    {  1,  1,  1 }
};

//            int[,] sobelX = new int[,]
//{
//    { -5, -4, 0, 4, 5 },
//    { -8, -10, 0, 10, 8 },
//    { -10, -20, 0, 20, 10 },
//    { -8, -10, 0, 10, 8 },
//    { -5, -4, 0, 4, 5 }
//};

//            int[,] sobelY = new int[,]
//{
//    { -5, -8, -10, -8, -5 },
//    { -4, -10, -20, -10, -4 },
//    { 0, 0, 0, 0, 0 },
//    { 4, 10, 20, 10, 4 },
//    { 5, 8, 10, 8, 5 }
//};

            int kernelRadius = sobelX.GetLength(0) / 2;

            for (int y = 2; y < bmpData.Height - 2; y++)
            {
                for (int x = 2; x < bmpData.Width - 2; x++)
                {
                    int sobelXsum = 0;
                    int sobelYsum = 0;

                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                        {
                            int pixelIndex = (y + ky) * stride + (x + kx) * 3;

                            sobelXsum += buffer[pixelIndex] * sobelX[ky + kernelRadius, kx + kernelRadius];
                            sobelYsum += buffer[pixelIndex] * sobelY[ky + kernelRadius, kx + kernelRadius];
                        }
                    }
                    int g = (int)Math.Sqrt(sobelXsum * sobelXsum + sobelYsum * sobelYsum);
                    g = Math.Clamp(g*2, 0, 255);
                    resultBuffer[y * stride + x * 3] = (byte)g;
                    resultBuffer[y * stride + x * 3 + 1] = (byte)g;
                    resultBuffer[y * stride + x * 3 + 2] = (byte)g;
                }
            }


            Marshal.Copy(resultBuffer, 0, bmpData.Scan0, resultBuffer.Length);
        }


        public static void ColorDifference(BitmapData bmpData)
        {
            int stride = bmpData.Stride;
            int height = bmpData.Height;
            int width = bmpData.Width; // make sure you have bmp.Width
            byte[] buffer = new byte[stride * height];
            Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);

            byte[] resultBuffer = new byte[buffer.Length];

            int bytesPerPixel = 3; // assuming 24bpp RGB

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int pos = y * stride + x * bytesPerPixel;

                    // current pixel
                    byte b1 = buffer[pos];
                    byte g1 = buffer[pos + 1];
                    byte r1 = buffer[pos + 2];

                    // pixel to the right
                    int posRight = pos + bytesPerPixel;
                    byte b2 = buffer[posRight];
                    byte g2 = buffer[posRight + 1];
                    byte r2 = buffer[posRight + 2];

                    // compute color differences
                    int dR = r2 - r1;
                    int dG = g2 - g1;
                    int dB = b2 - b1;

                    // magnitude of the color difference vector
                    double diff = Math.Sqrt(dR * dR + dG * dG + dB * dB);

                    // normalize and store as grayscale (optional)
                    byte edge = (byte)Math.Min(255, diff);

                    resultBuffer[pos] = edge;       // B
                    resultBuffer[pos + 1] = edge;   // G
                    resultBuffer[pos + 2] = edge;   // R
                }
            }

            // Copy back to bitmap
            Marshal.Copy(resultBuffer, 0, bmpData.Scan0, resultBuffer.Length);

        }

        public static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }


        //public static void Erosion(BitmapData bmpData, int kernelSize = 3)
        //{
        //    int stride = bmpData.Stride;
        //    int height = bmpData.Height;
        //    int width = bmpData.Width;
        //    int bytesPerPixel = 3;
        //    int radius = kernelSize / 2;

        //    byte[] buffer = new byte[stride * height];
        //    byte[] result = new byte[stride * height];
        //    Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);

        //    for (int y = radius; y < height - radius; y++)
        //    {
        //        for (int x = radius; x < width - radius; x++)
        //        {
        //            bool keep = true;

        //            for (int ky = -radius; ky <= radius && keep; ky++)
        //            {
        //                for (int kx = -radius; kx <= radius; kx++)
        //                {
        //                    int pos = ((y + ky) * stride) + ((x + kx) * bytesPerPixel);
        //                    byte pixel = buffer[pos]; // grayscale – wystarczy 1 kanał

        //                    if (pixel < 128)
        //                    {
        //                        keep = false;
        //                        break;
        //                    }
        //                }
        //            }

        //            int posOut = y * stride + x * bytesPerPixel;
        //            byte val = (byte)(keep ? 255 : 0);
        //            result[posOut] = val;
        //            result[posOut + 1] = val;
        //            result[posOut + 2] = val;
        //        }
        //    }

        //    Marshal.Copy(result, 0, bmpData.Scan0, result.Length);
        //}
    }
}
