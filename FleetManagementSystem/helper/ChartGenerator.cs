using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Linq; // Add this using directive for LINQ methods like .Max()

namespace FleetManagementSystem.Helpers
{
    public static class ChartGenerator
    {
        public static byte[] GenerateAcceptedTripsChart(Dictionary<string, int> monthlyCounts)
        {
            // Set dimensions and margins for better chart scaling
            int width = 1000;
            int height = 450;
            int margin = 60;

            // Bar dimensions will be calculated dynamically
            int totalBars = monthlyCounts.Count;
            if (totalBars == 0)
            {
                // Handle case with no data
                return new byte[0];
            }

            int availableWidth = width - 2 * margin;
            int barWidth = (availableWidth / totalBars) - 20; // Reduce width to provide some spacing
            int spacing = 20;

            // Find the max value to scale the chart's Y-axis
            int maxCount = monthlyCounts.Values.Max();
            if (maxCount == 0)
            {
                maxCount = 10; // Prevent division by zero if all counts are zero
            }

            // Calculate scaling factor for Y-axis
            float scaleFactor = (float)(height - 2 * margin) / maxCount;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", 12))
            using (Font titleFont = new Font("Arial", 16, FontStyle.Bold))
            {
                g.Clear(Color.White);

                // Draw axes
                g.DrawLine(Pens.Black, margin, height - margin, width - margin, height - margin); // X-axis
                g.DrawLine(Pens.Black, margin, margin, margin, height - margin);                 // Y-axis

                

                int xPosition = margin + spacing;
                int i = 0;

                // Draw Y-axis labels and markers
                int yStep = (maxCount > 10) ? (maxCount / 5) : 1;
                for (int yValue = 0; yValue <= maxCount; yValue += yStep)
                {
                    int yPos = height - margin - (int)(yValue * scaleFactor);
                    if (yPos >= margin)
                    {
                        g.DrawString(yValue.ToString(), font, Brushes.Black, new PointF(margin - 40, yPos - 8));
                        g.DrawLine(Pens.LightGray, margin, yPos, width - margin, yPos);
                    }
                }

                // Draw bars and labels for each month
                foreach (var month in monthlyCounts)
                {
                    int barHeight = (int)(month.Value * scaleFactor);
                    int y = height - margin - barHeight;

                    // Get a unique color for the bar
                    Brush barColor = GetColorForMonth(i);

                    // Draw the bar
                    g.FillRectangle(barColor, xPosition, y, barWidth, barHeight);

                    // Draw the value on top of the bar
                    g.DrawString(month.Value.ToString(), font, Brushes.Black, xPosition + barWidth / 2 - 10, y - 20);

                    // Draw the month label
                    g.DrawString(month.Key, font, Brushes.Black, xPosition + barWidth / 2 - (month.Key.Length * 4), height - margin + 5);

                    // Move to the next bar position
                    xPosition += barWidth + spacing;
                    i++;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private static Brush GetColorForMonth(int index)
        {
            Brush[] colors = { Brushes.SkyBlue, Brushes.Orange, Brushes.LightGreen, Brushes.Salmon, Brushes.DarkGray, Brushes.Teal, Brushes.Gold, Brushes.MediumOrchid, Brushes.IndianRed, Brushes.CornflowerBlue, Brushes.ForestGreen, Brushes.Sienna };
            return colors[index % colors.Length];
        }
    }
}