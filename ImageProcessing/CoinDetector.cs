using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImageProcessing
{
    internal class CoinDetector
    {

        private void FindPoints(Bitmap image, int startX, int startY, bool[,] visited, int threshold, List<Point> contour)
        {
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(startX, startY));
            visited[startX, startY] = true;

            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                Color neighborColor;

                contour.Add(p);

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = p.X + dx;
                        int ny = p.Y + dy;

                        if (nx >= 0 && ny >= 0 && nx < image.Width && ny < image.Height && !visited[nx, ny])
                        {
                            neighborColor = image.GetPixel(nx, ny);
                            if (neighborColor.R < threshold)
                            {
                                visited[nx, ny] = true;
                                stack.Push(new Point(nx, ny));
                            }
                        }
                    }
                }
            }
        }



        public Dictionary<string, double> ClassifyPesoCoins(Bitmap coinImage)
        {
            Bitmap processed = CleanImage(coinImage);

            List<List<Point>> objects = new List<List<Point>>();
            bool[,] visited = new bool[processed.Width, processed.Height];
            int blackThreshold = 20;

            for (int y = 0; y < processed.Height; y++)
            {
                for (int x = 0; x < processed.Width; x++)
                {
                    if (processed.GetPixel(x, y).R == 0 && !visited[x, y])
                    {
                        List<Point> points = new List<Point>();
                        FindPoints(processed, x, y, visited, blackThreshold, points);
                        if (points.Count > 0)
                        {
                            objects.Add(points);
                        }
                    }
                }
            }

            Dictionary<string, double> results = new Dictionary<string, double>()
            {
                { "5 Peso", 0 },
                { "1 Peso", 0 },
                { "25 Centavo", 0 },
                { "10 Centavo", 0 },
                { "5 Centavo", 0 },
                { "Value", 0},
            };

            foreach (var coinPoints in objects)
            {
                if (coinPoints.Count > 6500)
                {
                    int points = coinPoints.Count;
                    if (points > 18001)
                    {
                        results["5 Peso"]++;
                        results["Value"] += 5;
                    }
     
                    else if (points > 15001)
                    {
                        results["1 Peso"]++;
                        results["Value"] += 1;
                    }
                        
                    else if (points > 11001)
                    {
                        results["25 Centavo"]++;
                        results["Value"] += 0.25;
                    }
                       
                    else if (points > 8001)
                    {
                        results["10 Centavo"]++;
                        results["Value"] += 0.10;
                    }
                    
                    else if (points > 6500)
                    {
                        results["5 Centavo"]++;
                        results["Value"] += 0.05;
                    }
                        
                    foreach (Point p in coinPoints)
                    {
                        processed.SetPixel(p.X, p.Y, Color.Red);
                    }
                }
            }

            return results;
        }

        public Bitmap CleanImage(Bitmap original)
        {
            Bitmap processed = new Bitmap(original);
            DIP.Threshold(ref original, ref processed, 200);
            return processed;
        }


    }
}