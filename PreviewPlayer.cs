using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace Aldyparen
{
    class PreviewPlayer
    {
        private Movie movie;
        private FormPlayer formPlayer;
        private double downScale = 1;
        private bool showFps = false;
        private FpsUtil fpsUtil = new FpsUtil();

        public void play(Movie movie_)
        {
            this.movie = movie_;
            formPlayer = new FormPlayer();
            formPlayer.Show();
            formPlayer.WindowState = FormWindowState.Maximized;

            Thread t = new Thread(playInternal);
            t.Start();
        }

        public void setDownScale(double downScale_) {
            this.downScale = downScale_;
        }

        public void setShowFps(bool showFps_) {
            this.showFps = showFps_;
        }

        private void playInternal()
        {
            Grid emptyGrid = new Grid();
            int halfWidth = (int)(formPlayer.Width * 0.5 * downScale);
            int halfHeight = (int)(formPlayer.Height * 0.5 * downScale);

            for (int frameNumber = 0; frameNumber < movie.frameCount(); frameNumber++)
            {
                if (!formPlayer.Visible) break;
                Frame frame = movie[frameNumber];
                Bitmap img = frame.getFrame(halfWidth, halfHeight, emptyGrid);

                if (showFps)
                {
                    fpsUtil.frame();
                    fpsUtil.drawFpsOnImage(img);
                }

                formPlayer.setImage(img);
            }
            formPlayer.safeClose();
        }

        private static Bitmap getBitmap(int w, int h, int i) {
            Bitmap img = new Bitmap(w, h);
            Graphics graphics = Graphics.FromImage(img);
            Byte z = (Byte)(i % 255);
            Color c = Color.FromArgb(255, z, z, z);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    img.SetPixel(x, y, c);
            return img;
        }    
    }
}
