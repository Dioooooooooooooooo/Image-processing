
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageProcessing
{
    internal unsafe class DIP
    {

        private static int bytesPerPixel, widthInPixels, heightInPixels, widthInBytes, heightInBytes;
        private static byte* PtrFirstPixel, PtrFirstPixelProcessed;
        private static BitmapData loadedBitmapData;
        private static BitmapData processedBitmapData;

        private static void InitializeBitMapProcessing(ref Bitmap a, ref Bitmap b)
        {
            bytesPerPixel = Image.GetPixelFormatSize(a.PixelFormat) / 8;

            loadedBitmapData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), ImageLockMode.ReadOnly, a.PixelFormat);
            processedBitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, a.PixelFormat);

            widthInPixels = loadedBitmapData.Width;
            heightInPixels = loadedBitmapData.Height;

            widthInBytes = widthInPixels * bytesPerPixel;
            heightInBytes = heightInPixels * bytesPerPixel;

            PtrFirstPixel = (byte*)loadedBitmapData.Scan0;
            PtrFirstPixelProcessed = (byte*)processedBitmapData.Scan0;
        }

        public static Bitmap PixelCopy(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    // B -> G -> R -> A
                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = loadedCurrentRow[x + bb];
                        if (bb == 3)
                        {
                            processedCurrentRow[x + bb] = 255;
                        }
                    }
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b;
        }

        public static Bitmap Greyscale(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = (byte)((loadedCurrentRow[x] + loadedCurrentRow[x + 1] + loadedCurrentRow[x + 2]) / 3);
                        if (bb == 3)
                            processedCurrentRow[x + bb] = 255;
                    }
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b;
        }
        public static Bitmap ColorInversion(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = (byte)(255 - loadedCurrentRow[x + bb]);
                        if (bb == 3)
                            processedCurrentRow[x + bb] = 255;
                    }
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b;
        }

        public static Bitmap MirrorHorizontal(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* mirroredRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes / 2; x += bytesPerPixel)
                {
                    int mirroredX = widthInBytes - bytesPerPixel - x;

                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        mirroredRow[x + bb] = loadedCurrentRow[mirroredX + bb];
                        mirroredRow[mirroredX + bb] = loadedCurrentRow[x + bb];
                    }
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b;
        }

        public static Bitmap MirrorVertical(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* mirroredLine = PtrFirstPixelProcessed + ((heightInPixels - 1 - y) * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        mirroredLine[x + bb] = loadedCurrentRow[x + bb];
                    }
                }
            });
            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b; 
        }
        public static Bitmap Histogram(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);
            int[] histdata = new int[256];

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    histdata[loadedCurrentRow[x]]++;
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);

            b = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                int columnHeight = Math.Min(histdata[x] / 5, b.Height - 1);

                for (int y = 0; y < columnHeight; y++)
                {
                    b.SetPixel(x, b.Height - 1 - y, Color.Black);
                }
            }
            return b;
        }

        public static Bitmap Sepia(Bitmap a)
        {
            Bitmap b = new Bitmap(a.Width, a.Height);
            InitializeBitMapProcessing(ref a, ref b);
            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    byte blue = loadedCurrentRow[x];
                    byte green = loadedCurrentRow[x + 1];
                    byte red = loadedCurrentRow[x + 2];

                    processedCurrentRow[x] = (byte)Math.Min((int)(red * 0.272 + green * 0.534 + blue * 0.131), 255);
                    processedCurrentRow[x + 1] = (byte)Math.Min((int)(red * 0.349 + green * 0.686 + blue * 0.168), 255);
                    processedCurrentRow[x + 2] = (byte)Math.Min((int)(red * 0.393 + green * 0.769 + blue * 0.189), 255);
                    processedCurrentRow[x + 2] = (byte)Math.Min((int)(red * 0.393 + green * 0.769 + blue * 0.189), 255);
                    if (bytesPerPixel == 4)
                        processedCurrentRow[x + 3] = 255;
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
            return b;
        }

        public static void Brightness(ref Bitmap a, ref Bitmap b, int value)
        {
            InitializeBitMapProcessing(ref a, ref b);
            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    byte blue = loadedCurrentRow[x];
                    byte green = loadedCurrentRow[x + 1];
                    byte red = loadedCurrentRow[x + 2];

                    if (value > 0)
                    {
                        blue = (byte)Math.Min(blue + value, 255);
                        green = (byte)Math.Min(green + value, 255);
                        red = (byte)Math.Min(red + value, 255);
                    }
                    else
                    {
                        blue = (byte)Math.Max(blue + value, 0);
                        green = (byte)Math.Max(green + value, 0);
                        red = (byte)Math.Max(red + value, 0);
                    }

                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = (bb == 3) ? (byte)255 : (bb == 0) ? blue : (bb == 1) ? green : red;
                    }
                }
            });
            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);

        }

        public static void Contrast(ref Bitmap a, ref Bitmap b, int value)
        {
            int width = a.Width;
            int height = b.Height;
            int numSamples = width * height;
            int[] Ymap = new int[256];
            int[] hist = new int[256];

            InitializeBitMapProcessing(ref a, ref b);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    byte average = 0;
                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = loadedCurrentRow[x + bb];
                        if (bb == 3)
                        {
                            processedCurrentRow[x + bb] = 255;
                            break;
                        }
                        average += (byte)loadedCurrentRow[x + bb];
                    }
                    average /= 3;
                    hist[average]++;
                }
            });

            int histSum = 0;
            for (int i = 0; i < 256; i++)
            {
                histSum += hist[i];
                Ymap[i] = histSum * 255 / numSamples;
            }

            if (value < 100)
            {
                for (int i = 0; i < 256; i++)
                {
                    Ymap[i] = i + (Ymap[i] - i) * value / 100;
                }
            }

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {

                    byte blue = loadedCurrentRow[x];

                    int adjustedGray = Ymap[blue];

                    for (int bb = 0; bb < bytesPerPixel; bb++)
                    {
                        processedCurrentRow[x + bb] = (byte)adjustedGray;
                        if (bb == 3)
                            processedCurrentRow[x + bb] = 255;
                    }
                }
            });
            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
        }

        public static void Rotation(ref Bitmap a, ref Bitmap b, int degree)
        {
            InitializeBitMapProcessing(ref a, ref b);

            float radians = degree * (float)Math.PI / 180;
            int centerX = a.Width / 2;
            int centerY = a.Height / 2;

            float cosA = (float)Math.Cos(radians);
            float sinA = (float)Math.Sin(radians);

            Parallel.For(0, heightInPixels, y =>
            {
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    int x0 = (x / bytesPerPixel) - centerX;
                    int y0 = y - centerY;

                    int xs = (int)(x0 * cosA + y0 * sinA) + centerX;
                    int ys = (int)(-x0 * sinA + y0 * cosA) + centerY;

                    if (xs >= 0 && xs < widthInPixels && ys >= 0 && ys < heightInPixels)
                    {
                        byte* sourcePixel = PtrFirstPixel + ys * loadedBitmapData.Stride + xs * bytesPerPixel;

                        for (int bb = 0; bb < bytesPerPixel; bb++)
                        {
                            processedCurrentRow[x + bb] = sourcePixel[bb];
                            if (bb == 3)
                                processedCurrentRow[x + bb] = 255;
                        }
                    }
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
        }

        public static void Threshold(ref Bitmap a, ref Bitmap b, int value)
        {
            InitializeBitMapProcessing(ref a, ref b);
            byte graydata = 0;

            Parallel.For(0, heightInPixels, y =>
            {
                byte* loadedCurrentRow = PtrFirstPixel + (y * loadedBitmapData.Stride);
                byte* processedCurrentRow = PtrFirstPixelProcessed + (y * processedBitmapData.Stride);

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    byte blue = loadedCurrentRow[x];
                    byte green = loadedCurrentRow[x + 1];
                    byte red = loadedCurrentRow[x + 2];

                    graydata = (byte)((blue + green + red) / 3);
                    if (graydata > value)
                    {
                        processedCurrentRow[x] = 255;
                        processedCurrentRow[x + 1] = 255;
                        processedCurrentRow[x + 2] = 255;
                    }
                    else
                    {
                        processedCurrentRow[x] = 0;
                        processedCurrentRow[x + 1] = 0;
                        processedCurrentRow[x + 2] = 0;
                    }
                    if (bytesPerPixel == 4)
                        processedCurrentRow[x + 3] = 255;
                }
            });

            a.UnlockBits(loadedBitmapData);
            b.UnlockBits(processedBitmapData);
        }

        public static void Subtract(ref Bitmap a, ref Bitmap b, ref Bitmap c)
        {
            Color myGreen = Color.FromArgb(0, 255, 0);
            int greyGreen = (myGreen.R + myGreen.G + myGreen.B) / 3;
            int threshold = 1;

            byte agraydata = 0;
            byte bgraydata = 0;
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    Color adata = a.GetPixel(x, y);
                    Color bdata = b.GetPixel(x, y);

                    agraydata = (byte)((adata.R + adata.G + adata.B) / 3);
                    bgraydata = (byte)((bdata.R + bdata.G + bdata.B) / 3);

                    if (Math.Abs(agraydata - greyGreen) > threshold)
                        c.SetPixel(x, y, adata);
                    else
                        c.SetPixel(x, y, bdata);
                }
            }
        }
    }
}