using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace CyberLyzer
{
   public  class CLFrame
    { 


        public delegate int Get_Pixel(Complex pnt, CLFrameParams p); 
         

      public  double rotation;
        public  double scale;
        public Complex ctr;
       public Get_Pixel dlgt;

        public Color[] colorMap; 

        


        public CLFrame()
        {
            rotation = 0;
            scale = 1;
            ctr = new Complex(0,0);
            colorMap = new Color[1000];
        }

        
       public CLFrameParams  param; 

       public  CLFrame(double _rot, double _scale, Complex _ctr, Get_Pixel _dlgt, CLFrameParams _params)
        {
            rotation = _rot;
            scale = _scale;
            ctr = _ctr;
            dlgt = _dlgt;
            param = _params.clone();
            colorMap = new Color[1000];
        }


            
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

       unsafe public Bitmap getFrame(int W, int H)
       {
           int W2 = W * 2;
           int H2 =H* 2;
           Bitmap bmp = new Bitmap(W2,   H2,PixelFormat.Format24bppRgb);

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


       public CLFrame clone( )
       {
           var ret = new CLFrame(this.rotation, this.scale, this.ctr, this.dlgt, this.param);
           ret.colorMap = new Color[ param.maxUsedColors];
           for (int i=0;i<param.maxUsedColors;i++)
           {
                ret.colorMap[i]=this.colorMap[i];
           }
           return ret;
       }

       public CLFrame getMove(CLFrame newFrame, double step)
       {
           var ret = this.clone();

           ret.rotation = rotation+  step*(newFrame.rotation-rotation);
           ret.scale=     scale + step * (newFrame.scale - scale);
           ret.ctr=     ctr+  step*(newFrame.ctr-ctr);
           ret.param = this.param.getMove(newFrame.param, step);

           return ret;
              
       }

       public void restoreTransform(CLFrame oldFrame)
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
