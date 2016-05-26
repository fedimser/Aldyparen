using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace CyberLyzer
{
    public static class FractalPainters
    {

        const double eps = 1e-9;  


       public static int CubeRootPainter(Complex z0,  CLFrameParams p)
        {
            Complex z = z0;
            for (int i = 0; i < 100; i++)
            {
                Complex z1 = (2.0 / 3.0) * z + 1 / (3 * z * z);


                if ((z - z1).Magnitude < 1e-9) { break; }
                z = z1;
            }

            if (z.Imaginary < 1e-5 && z.Imaginary > -1e-5) return 0;
            if (z.Imaginary > 0) return 1;
            if (z.Imaginary < 0) return 2;
            return 0;
        }

       public static int TetraRootPainter(Complex z0,  CLFrameParams p)
        {
            Complex z = z0;
            for (int i = 0; i < 100; i++)
            {
                Complex z1 = 0.75 * z + 0.25 / (z * z * z);
                //Complex z1 = z -( Complex.Pow(z, _k1)-1) / ((_k1 - 1) * Complex.Pow(z, _k1 - 1));

                if ((z - z1).Magnitude < 1e-9) { break; }
                z = z1;
            }

            if (z.Imaginary < 1e-5 && z.Imaginary > -1e-5)
            {
                if (z.Real > 0) return 0;
                if (z.Real < 0) return 1;
            }
            if (z.Imaginary > 0) return 2;
            if (z.Imaginary < 0) return 3;
            return 0;
        }

       public static int MandelbrotPainter(Complex c, CLFrameParams p)
        {
            //Check Cardioid
            double r = Math.Sqrt((c.Real - 0.25) * (c.Real - 0.25) + c.Imaginary * c.Imaginary);
            double t = Math.Atan2(c.Imaginary, c.Real);
            if (r <= 0.5 * (1 - Math.Cos(t))) return 0;

            Complex z = 0;
            for (int i = 0; i < 200; i++)
            {
                z = z * z + c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 4) return 1;
            }
            return 0;
        }

       public static int Mandelbrot2Painter(Complex c,  CLFrameParams p)
        {
            //Check Cardioid
            double r = Math.Sqrt((c.Real - 0.25) * (c.Real - 0.25) + c.Imaginary * c.Imaginary);
            double t = Math.Atan2(c.Imaginary, c.Real);
            if (r <= 0.5 * (1 - Math.Cos(t))) return 255;

            Complex z = 0;
            for (int i = 0; i < 255; i++)
            {
                z = z * z + c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 4) return i;
            }
            return 255;
        }

       public static int MandelBrotCubePainter(Complex c,  CLFrameParams p)
        {

            Complex z = 0;
            for (int i = 0; i < 255; i++)
            {
                z = z * z * z + c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 100) return i;
            }
            return 255;
        }



       public static int MandelBrotTetraPainter(Complex c,  CLFrameParams p)
        {

            Complex z = 0;
            for (int i = 0; i < 255; i++)
            {
                z = z * z * z * z + c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 16) return i;
            }
            return 255;
        }


       public static int GenericMandelbrotPainter(Complex c,  CLFrameParams p)
        {
            int M = p.maxUsedColors - 1;

            Complex z = 0;
            for (int i = 0; i < M; i++)
            {
                z = Complex.Pow(z, p.k[0]) + c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 4) return i;
            }
            return M;
        }

       public static int DebugPainter(Complex c,  CLFrameParams p)
        {

            Complex z = c;
            for (int i = 0; i < 50; i++)
            {
                z = p.k[1] * z * z + p.k[2] * c * z + p.k[3] * c * c;
                if (z.Magnitude > 100) return i;
            }
            return 50;

        }

       public static int DzetaPainter(Complex c,  CLFrameParams p)
        {

            return 0;

        }

       public static int PNL_Painter(Complex c, CLFrameParams p)
        {
            int M = p.maxUsedColors - 1;

            Complex z = 0;
            for (int i = 0; i < M; i++)
            {
                z = p.k[1] + p.k[2] * z + p.k[3] * c + p.k[4] * z * z + p.k[5] * z * c + p.k[6] * c * c;
                if (z.Real * z.Real + z.Imaginary * z.Imaginary > 1000) return i;
            }
            return M;
        }



    }
}
