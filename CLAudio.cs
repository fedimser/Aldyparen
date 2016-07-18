using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using NAudio.Wave;
using System.Drawing.Imaging;

namespace Aldyparen
{
    class CLAudio
    {

        int[] Picture_Histogramm;

        private WaveOut waveOut;
        private CLWaveProvider32 waveProv = new CLWaveProvider32();

        private void button12_Click(object sender, EventArgs e)
        {
            StartStopSineWave();

        }

        private void StartStopSineWave()
        {
            if (waveOut == null)
            {
                waveOut = new WaveOut();
                waveOut.Init(waveProv);
                waveOut.Play();
            }
            else
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        public class CLWaveProvider32 : WaveProvider32
        {
            int sample = 0;

            public float Frequency = 20;
            public float Amplitude = 1;

            public override int Read(float[] buffer, int offset, int sampleCount)
            {
                Console.WriteLine(sampleCount.ToString());
                int sampleRate = WaveFormat.SampleRate;
                for (int n = 0; n < sampleCount; n++)
                {
                    buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                    sample++;
                    if (sample >= sampleRate) sample = 0;
                }
                return sampleCount;
            }
        }


        unsafe private void Build_Picture_Histogram(ref   Bitmap target, int W, int H)
        {
            BitmapData bd = target.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Picture_Histogramm = new int[3 * 256];

            try
            {
                byte* curpos;
                int br;
                for (int h = 0; h < H; h++)
                {
                    curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                    for (int w = 0; w < W; w++)
                    {
                        br = *(curpos++) + *(curpos++) + *(curpos++);
                        Picture_Histogramm[br]++;
                    }
                }
            }
            finally
            {
                target.UnlockBits(bd);
            }
        }
    }
}
