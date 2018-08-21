using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Aldyparen
{
    public class FpsUtil
    {
        private const int SLIDING_WINDOW = 10;
        private DateTime time0 = DateTime.Now;
        private List<long> timeMarks = new List<long>();

        private Font font = new Font("", 10);
        private Brush brush = Brushes.Black;

        private long curTimeMilliseconds()
        {
            long timeMs = (long)DateTime.Now.Subtract(time0).TotalMilliseconds;
            Console.WriteLine(timeMs);
            return timeMs;
        }

        public void frame()
        {
            timeMarks.Add(curTimeMilliseconds());
        }

        public float fps()
        {
            if (timeMarks.Count <= SLIDING_WINDOW)
            {
                return 0;
            }
            long timeMs = timeMarks[timeMarks.Count - 1] - timeMarks[timeMarks.Count - 1 - SLIDING_WINDOW];
            return (float)(1000 * SLIDING_WINDOW) / timeMs;
        }

        public void drawFpsOnImage(Bitmap img)
        { 
            Graphics g = Graphics.FromImage(img);
            String fpsMessage = String.Format("{0:0.0} FPS", fps());
            g.DrawString(fpsMessage, font, brush, new Point(10, 10));
        }
    }
}
