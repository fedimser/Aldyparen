using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace CyberLyzer
{
    class Fourier
    {
        public static  void fft(ref List<Complex> a, bool invert)
        {
            int n = (int)a.Count();
            if (n == 1) return;

            List<Complex> a0 = new List<Complex>(n / 2);
            List<Complex> a1 = new List<Complex>(n / 2);

            for (int i = 0, j = 0; i < n; i += 2, ++j)
            {
                a0.Add(a[i]);
                a1.Add(a[i + 1]);
            }
            fft(ref a0, invert);
            fft(ref a1, invert);

            double ang = 2 * Math.PI / n * (invert ? -1 : 1);
            Complex w = 1;
            Complex wn = new Complex(Math.Cos(ang), Math.Sin(ang));

            for (int i = 0; i < n / 2; ++i)
            {
                a[i] = a0[i] + w * a1[i];
                a[i + n / 2] = a0[i] - w * a1[i];
                if (invert)
                {
                    a[i] /= 2;
                    a[i + n / 2] /= 2;
                }
                w *= wn;
            }
        }
    }
}
