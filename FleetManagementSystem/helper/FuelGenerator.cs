using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class FuelChartGenerator
{
    public static byte[] GenerateFuelBarChart(Dictionary<string, (decimal Quantity, decimal Cost)> monthlyData)
    {
        int width = 1000;
        int height = 450;
        var bitmap = new Bitmap(width, height);
        var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.White);

        var font = new Font("Arial", 10);
        var brush = Brushes.Black;
        var penAxis = new Pen(Color.Black, 1);
        var gridPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dash };

        var months = monthlyData.Keys.ToList();
        var fuelValues = monthlyData.Values.Select(v => v.Quantity).ToList();
        var costValues = monthlyData.Values.Select(v => v.Cost).ToList();

        // Draw axes
        graphics.DrawLine(penAxis, 50, 350, width - 50, 350); // X-axis
        graphics.DrawLine(penAxis, 50, 50, 50, 350);          // Y-axis

        // Draw Y-axis tick marks, labels, and horizontal grid lines
        int yAxisMax = 5000;  // Updated from 2000
        int yStep = 1000;     // Updated from 500

        for (int i = 0; i <= yAxisMax; i += yStep)
        {
            int y = 350 - (i * 300 / yAxisMax);
            graphics.DrawLine(Pens.Gray, 45, y, 50, y); // Tick mark
            graphics.DrawString(i.ToString(), font, brush, 10, y - 7); // Label
            graphics.DrawLine(gridPen, 50, y, width - 50, y); // Grid line
        }


        int chartWidth = width - 100;
        int spacing = chartWidth / months.Count;

        // Draw bars
        for (int i = 0; i < months.Count; i++)
        {
            int xBase = 50 + i * spacing;
            int barWidth = spacing / 3;

            int fuelHeight = (int)((fuelValues[i] / yAxisMax) * 300);
            int costHeight = (int)((costValues[i] / yAxisMax) * 300);

            int yFuel = 350 - fuelHeight;
            int yCost = 350 - costHeight;

            graphics.FillRectangle(Brushes.Blue, xBase, yFuel, barWidth, fuelHeight);
            graphics.FillRectangle(Brushes.Green, xBase + barWidth + 5, yCost, barWidth, costHeight);

            graphics.DrawString(months[i], font, brush, xBase, 360);

            if (fuelHeight > 0)
            {
                string fuelText = Math.Round(fuelValues[i], 0).ToString() + " L";
                SizeF fuelTextSize = graphics.MeasureString(fuelText, font);
                graphics.DrawString(fuelText, font, brush, xBase + (barWidth - fuelTextSize.Width) / 2, yFuel - fuelTextSize.Height - 2);
            }

            if (costHeight > 0)
            {
                string costText = "₹" + Math.Round(costValues[i], 0).ToString();
                SizeF costTextSize = graphics.MeasureString(costText, font);
                graphics.DrawString(costText, font, brush, xBase + barWidth + 5 + (barWidth - costTextSize.Width) / 2, yCost - costTextSize.Height - 2);
            }
        }

        // Draw legend
        int legendX = 850;
        int legendY = 60;

        graphics.FillRectangle(Brushes.Blue, legendX, legendY, 15, 15);
        graphics.DrawString("Fuel Quantity", font, brush, legendX + 20, legendY - 2);

        graphics.FillRectangle(Brushes.Green, legendX, legendY + 25, 15, 15);
        graphics.DrawString("Cost", font, brush, legendX + 20, legendY + 23);

        // Return image
        using (var ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}