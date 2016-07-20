﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Windows.Forms; 

namespace Aldyparen
{

     
   public  class Frame
    {

        public enum GeneratingMode
        {
            Delegate,
            Formula
        }

       
        public delegate int Get_Pixel(Complex pnt, FrameParams p);


        public GeneratingMode genMode;


      


        public  double rotation;
        public  double scale;
        public Complex ctr;
        public Get_Pixel dlgt;

        public Color[] colorMap;
        public const int MAX_COLORS = 256;
        


        public Frame()
        {
            genMode = GeneratingMode.Formula;
            rotation = 0;
            scale = 1;
            ctr = new Complex(0,0);
            colorMap = new Color[MAX_COLORS];
        }

        
       public FrameParams  param; 

       /*
       //Constructor for deleagate generating mode
       public  Frame(double _rot, double _scale, Complex _ctr, Get_Pixel _dlgt, FrameParams _params)
        {
            genMode = GeneratingMode.Delegate;
            rotation = _rot;
            scale = _scale;
            ctr = _ctr;
            dlgt = _dlgt;
            param = _params.clone();
            colorMap = new Color[MAX_COLORS];
        }

       //Constructor for formula generating mode
       public Frame(double _rot, double _scale, Complex _ctr,FrameParams _params)
       {
           genMode = GeneratingMode.Formula;
           rotation = _rot;
           scale = _scale;
           ctr = _ctr;
           param = _params.clone();
           colorMap = new Color[MAX_COLORS];
       }*/
 

            /*
       //Deprecated
       public  Bitmap getFrameSafe  (int W, int H)
        {
            Bitmap bmp = new Bitmap(2 *W, 2 * H);

            for (int x = 0; x < 2 * W; x++)
                for (int y = 0; y < 2 * H; y++)
                {
                    Complex v = new Complex((x - W) * (scale / W), (y - H) * (scale / W)) * Complex.Exp(rotation * Complex.ImaginaryOne) + ctr;
                    int color_num = dlgt(v,  param);
                    if (color_num <  param.maxUsedColors && color_num >= 0)
                    { 
                        bmp.SetPixel(x, y,colorMap[color_num] );
                    }
                }

            return bmp;
        }
             * */


       unsafe private Bitmap getFrameDelegate(int W, int H)
       {
           int W2 = W * 2;
           int H2 = H * 2;
           Bitmap bmp = new Bitmap(W2, H2, PixelFormat.Format24bppRgb);

           BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W2, H2), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

           try
           {
               byte* curpos;
               for (int y = 0; y < H2; y++)
               {
                   curpos = ((byte*)bd.Scan0) + y * bd.Stride;
                   for (int x = 0; x < W2; x++)
                   {
                       Complex v = new Complex((x - W) * (scale / W), (y - H) * (scale / W)) * Complex.Exp(rotation * Complex.ImaginaryOne) + ctr;
                       int color_num = dlgt(v, param);

                       *(curpos++) = colorMap[color_num].B;
                       *(curpos++) = colorMap[color_num].G;
                       *(curpos++) = colorMap[color_num].R;
                   }
               }
           }
           finally
           {
               bmp.UnlockBits(bd);
           }


           return bmp;
       }


       private int getSequenceDivergence(Complex c, Function func, Complex init,double infty, int steps)
       { 
           Complex z = init;
           Dictionary<String, Complex> d = new Dictionary<String,Complex>();
           d["c"] = c;
           for (int i = steps - 1; i >= 1; i--)
           {
               d["z"] = z;
               z = func.eval(d);
               if (z.Magnitude > infty) return i;
           }
           return 0;
       }

       unsafe private Bitmap getFrameFormula(int W, int H)
       {
           int W2 = W * 2;
           int H2 = H * 2;
           Bitmap bmp = new Bitmap(W2, H2, PixelFormat.Format24bppRgb);

           BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W2, H2), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

           try
           {
               byte* curpos;
               for (int y = 0; y < H2; y++)
               {
                   curpos = ((byte*)bd.Scan0) + y * bd.Stride;
                   for (int x = 0; x < W2; x++)
                   {
                       Complex v = new Complex((x - W) * (scale / W), (y - H) * (scale / W)) * Complex.Exp(rotation * Complex.ImaginaryOne) + ctr;

                       int clr = getSequenceDivergence(v, param.genFunc, param.genInit, param.genInfty, param.genSteps);


                       *(curpos++) = colorMap[clr].B;
                       *(curpos++) = colorMap[clr].G;
                       *(curpos++) = colorMap[clr].R;
                   }
               }
           }
           finally
           {
               bmp.UnlockBits(bd);
           }


           return bmp;
       }

        

       //Real size is twice more!
       unsafe public Bitmap getFrame(int W, int H)
       {
           if (CudaPainter.enabled && CudaPainter.canRender(this))
           {
               try
               {
                   return CudaPainter.render(W, H, this);
               }
               catch (Exception ex)
               {
                   MessageBox.Show("Error while rendering picture on GPU.\n" + ex.Message + "\n\nRecommendations:\n0.Restart application immidiately.\n1.Increase timeout in registry (HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\\...)\n2.Request smaller frame\n3.Don't use CUDA.");
                   return null;
               }
           }
           else
           {
                if(genMode==GeneratingMode.Delegate)   return getFrameDelegate(W, H);
                else return getFrameFormula(W, H);
           }
       }


       public Frame clone()
       {
           var ret = new Frame();

           ret.genMode = this.genMode;
           ret.rotation = this.rotation;
           ret.scale=this.scale;
           ret.ctr = this.ctr;
           ret.dlgt = this.dlgt;
           ret.param = this.param.clone(); 
           ret.colorMap = new Color[ param.maxUsedColors];
           for (int i=0;i<param.maxUsedColors;i++)
           {
                ret.colorMap[i]=this.colorMap[i];
           }
           return ret;
       }

       public Frame getMove(Frame newFrame, double step)
       {
           var ret = this.clone();

           ret.rotation = rotation+  step*(newFrame.rotation-rotation);
           ret.scale = scale + step * (newFrame.scale - scale);
           ret.ctr = ctr+  step*(newFrame.ctr-ctr);
           ret.param = this.param.getMove(newFrame.param, step);

           return ret;
              
       }

       public void restoreTransform(Frame oldFrame)
       {
           this.rotation = oldFrame.rotation;
           this.scale = oldFrame.scale;
           this.ctr = oldFrame.ctr;
       }


       public void moveColors(int S)
       {
           Random r = new Random();
           for (int i = 0; i < param.maxUsedColors; i++)
           {
               colorMap[i] = MoveColor(colorMap[i], S,r);
           }
       }

       private static Color MoveColor(Color clr, int S,Random r)
       {
           int R = clr.R - S + r.Next(2 * S);
           int G = clr.G - S + r.Next(2 * S);
           int B = clr.B - S + r.Next(2 * S);
           if (R < 0) R = 0;
           if (R > 255) R = 255;
           if (G < 0) G = 0;
           if (G > 255) G = 255;
           if (B < 0) B = 0;
           if (B > 255) B = 255;
           return Color.FromArgb(255, R, G, B);
       }
        
    }
}