using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FleetManagementSystem.Helpers
{
    public static class ChartGenerator
    {
        public static byte[] GenerateAcceptedTripsChart(int septemberCount, int octoberCount)
        {
            int width = 500;
            int height = 350;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", 12))
            {
                g.Clear(Color.White);

                // Draw axes
                g.DrawLine(Pens.Black, 50, 300, 450, 300); // X-axis
                g.DrawLine(Pens.Black, 50, 50, 50, 300);   // Y-axis

                // Bar values
                int[] values = { septemberCount, octoberCount };
                string[] labels = { "September", "October" };
                Brush[] colors = { Brushes.SkyBlue, Brushes.Orange };

                for (int i = 0; i < values.Length; i++)
                {
                    int barHeight = values[i] * 20;
                    int x = 100 + i * 150;
                    int y = 300 - barHeight;

                    g.FillRectangle(colors[i], x, y, 80, barHeight);
                    g.DrawString(values[i].ToString(), font, Brushes.Black, x + 25, y - 20);
                    g.DrawString(labels[i], font, Brushes.Black, x + 10, 310);
                }

                g.DrawString("Accepted Trips Comparison", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, 120, 20);

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
