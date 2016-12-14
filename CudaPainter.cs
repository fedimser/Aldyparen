﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using Cudafy.Types;

using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
 



namespace Aldyparen
{
    class CudaPainter
    {
         

        private static GPGPU gpu;
        public static bool enabled = false;
        public static bool corrupted = false;
        public static bool rowScan = false;
 
        public static String errorMessage;

        public static bool cudaEnable()
        {
            try
            {
                CudafyModule km = CudafyTranslator.Cudafy();
                Console.WriteLine("Translator OK");
                gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
                Console.WriteLine("GPU OK");
                gpu.LoadModule(km);
                enabled = true;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false; 
            } 
        }

        public static void cudaDisable()
        {
            enabled = false;
        }

        public static bool isCudaAvailable()
        {
            return (getProperties() != null);
        }

        public static GPGPUProperties getProperties()
        { 
            foreach (GPGPUProperties prop in CudafyHost.GetDeviceProperties(CudafyModes.Target))
            {
                return prop;
            }
            return null;
        }

        public static String getPropertiesString()
        {
            GPGPUProperties prop = getProperties();
            if(prop==null) return "N/A";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("   --- General Information for device {0} ---\n", 0);
            sb.AppendFormat("Name:  {0}\n", prop.Name);
            sb.AppendFormat("Platform Name:  {0}\n", prop.PlatformName);
            sb.AppendFormat("Device Id:  {0}\n", prop.DeviceId);
            sb.AppendFormat("Compute capability:  {0}.{1}\n", prop.Capability.Major, prop.Capability.Minor);
            sb.AppendFormat("Clock rate: {0}\n", prop.ClockRate);
            sb.AppendFormat("Simulated: {0}\n", prop.IsSimulated);
            sb.AppendFormat("\n");

            sb.AppendFormat("   --- Memory Information for device {0} ---\n", 0);
            sb.AppendFormat("Total global mem:  {0}\n", prop.TotalMemory);
            sb.AppendFormat("Total constant Mem:  {0}\n", prop.TotalConstantMemory);
            sb.AppendFormat("Max mem pitch:  {0}\n", prop.MemoryPitch);
            sb.AppendFormat("Texture Alignment:  {0}\n", prop.TextureAlignment);
            sb.AppendFormat("\n");

            sb.AppendFormat("   --- MP Information for device {0} ---", 0);
            sb.AppendFormat("Shared mem per mp: {0}\n", prop.SharedMemoryPerBlock);
            sb.AppendFormat("Registers per mp:  {0}\n", prop.RegistersPerBlock);
            sb.AppendFormat("Threads in warp:  {0}\n", prop.WarpSize);
            sb.AppendFormat("Max threads per block:  {0}\n", prop.MaxThreadsPerBlock);
            sb.AppendFormat("Max thread dimensions:  ({0}, {1}, {2})", prop.MaxThreadsSize.x,
                              prop.MaxThreadsSize.y, prop.MaxThreadsSize.z);
            sb.AppendFormat("Max grid dimensions:  ({0}, {1}, {2})", prop.MaxGridSize.x, prop.MaxGridSize.y,
                              prop.MaxGridSize.z);

            return sb.ToString();
        }

        public static bool canRender(Frame frame)
        {
            return (frame.genMode==Frame.GeneratingMode.Formula);
        }

        unsafe public static Bitmap render(int W, int H, Frame frame)
        {
            if (frame.genMode != Frame.GeneratingMode.Formula)
            {
                throw new Exception("Cannot render frame with CUDA. Frame generated by formla needed.");
            }

            if (corrupted)
            {
                throw new Exception("You must restart application id you want to use CUDA!");
            }

            try
            {
                if (!gpu.IsCurrentContext)
                {
                    gpu.SetCurrentContext();
                }


                byte[] b = new byte[12 * W * H];



                //Allocating memory for answer
                byte[] dev_b = gpu.Allocate<byte>(12 * W * H);
                gpu.CopyToDevice(b, dev_b);




                //Forming and passing color map
                int colorCount = frame.colorMap.Length;
                byte[] colorMap = new byte[3 * colorCount];
                byte[] dev_colorMap = gpu.Allocate<byte>(3 * colorCount);
                for (int i = 0; i < colorCount; i++)
                {
                    colorMap[3 * i] = frame.colorMap[i].B;
                    colorMap[3 * i + 1] = frame.colorMap[i].G;
                    colorMap[3 * i + 2] = frame.colorMap[i].R;
                }
                gpu.CopyToDevice(colorMap, dev_colorMap);



                //Forming and passing RPN for frame's formula
                byte[] dev_rpnFormula = gpu.Allocate<byte>(frame.param.genFunc.rpnFormula.Length);
                gpu.CopyToDevice(frame.param.genFunc.rpnFormula, dev_rpnFormula);

                int cnt = frame.param.genFunc.rpnKoef.Length;
                ComplexD[] rpnKoef = new ComplexD[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    rpnKoef[i] = makeComplexDFromNet(frame.param.genFunc.rpnKoef[i]);
                }

                if (cnt == 0)
                {
                    cnt = 1;
                    rpnKoef = new ComplexD[1] { new ComplexD(0, 0) };
                }

                ComplexD[] dev_rpnKoef = gpu.Allocate<ComplexD>(cnt);

                gpu.CopyToDevice(rpnKoef, dev_rpnKoef);




                //Allocating memory for stacks
                ComplexD[] dev_stackMem = gpu.Allocate<ComplexD>(4 * W * H * Function.MAX_STACK_SIZE);


                 
                //Calling main routine
                int N = frame.param.genFunc.rpnFormula.Length;
                dim3 gs = new dim3(2 * W, 2 * H);

                if (rowScan)
                {
                    gs = new dim3(2*W, 1);
                    for (int y = 0; y < 2*H; y++)
                    {
                        gpu.Launch(gs, 1).setPixel(W, H, N, dev_b, dev_colorMap, dev_stackMem, dev_rpnFormula, dev_rpnKoef, (double)frame.rotation, (double)frame.scale, new ComplexD((double)frame.ctr.Real, (double)frame.ctr.Imaginary), new ComplexD((double)frame.param.genInit.Real, (double)frame.param.genInit.Imaginary), (double)frame.param.genInfty, frame.param.genSteps, y);
                    }
                }
                else
                {
                    gpu.Launch(gs, 1).setPixel(W, H, N, dev_b, dev_colorMap, dev_stackMem, dev_rpnFormula, dev_rpnKoef, (double)frame.rotation, (double)frame.scale, new ComplexD((double)frame.ctr.Real, (double)frame.ctr.Imaginary), new ComplexD((double)frame.param.genInit.Real, (double)frame.param.genInit.Imaginary), (double)frame.param.genInfty, frame.param.genSteps, 0);
                }
                gpu.CopyFromDevice(dev_b, b);



                //Freeing memory
                gpu.Free(dev_b);
                gpu.Free(dev_colorMap);
                gpu.Free(dev_rpnFormula);
                gpu.Free(dev_rpnKoef);
                gpu.Free(dev_stackMem);


                //saving picture
                int W2 = W * 2;
                int H2 = H * 2;
                Bitmap bmp = new Bitmap(W2, H2, PixelFormat.Format24bppRgb);
                BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W2, H2), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                byte* curpos = ((byte*)bd.Scan0);


                for (int y = 0; y < H2; y++)
                {
                    curpos = ((byte*)bd.Scan0) + y * bd.Stride;
                    int offset = y * 3 * W2;
                    for (int x = 0; x < 3 * W2; x++)
                    {
                        *(curpos++) = b[x + offset];
                    }
                }


                bmp.UnlockBits(bd);
                return bmp;
            }
            catch (Exception ex)
            {
                corrupted = true;
                return null;
            }      
          

            
        }
         

        [Cudafy]
        private static int Mandelbrot(ComplexD c)
        {
            //DEBUG = MANDELBROT
            double r =  Cudafy.GMath.Sqrt((float)((c.x - 0.25F) * (c.x - 0.25F) + c.y * c.y));
            double t = Cudafy.GMath.Atan2((float)c.y, (float)c.x);
            if (r <= 0.5 * (1 - Cudafy.GMath.Cos((float)t))) return 0;
             

            ComplexD z = new ComplexD(0,0);
            for (int i = 0; i < 200; i++)
            {
                z = ComplexD.Add(ComplexD.Multiply( z ,z), c);
                if (z.x * z.x + z.y * z.y > 4) return 1;
            }
            return 0;
        }


        [Cudafy]
        private static void setPixel(GThread thread, int W, int H,int rpnLength,byte[] bmp, byte[] colorMap,ComplexD[] stackMem,
            byte[] rpnFormula, ComplexD[] rpnKoef ,double rotation, 
            double scale, ComplexD ctr,ComplexD init,double infty,int steps,
            int offsetY)
        {
            int x = thread.blockIdx.x;
            int y = thread.blockIdx.y+offsetY;
            int idx = x + 2 * W * y;

            ComplexD c1 = new ComplexD(Cudafy.GMath.Cos((float)rotation), Cudafy.GMath.Sin((float)rotation)); 
            ComplexD c2 = new ComplexD((x - W) * (scale / W), (y - H) * (scale / W)) ;
            ComplexD c = ComplexD.Add(  ComplexD.Multiply(c1,c2), ctr);


            int clr = getSequenceDivergence(c, rpnLength, rpnFormula, rpnKoef, init, infty, steps, stackMem, Function.MAX_STACK_SIZE * idx);
           


            bmp[3 * idx] = colorMap[3 * clr];
            bmp[3 * idx + 1] = colorMap[3 * clr+1];
            bmp[3 * idx + 2] = colorMap[3 * clr+2];      
        }
 



        private static ComplexD makeComplexDFromNet(Complex x)
        {
            return new ComplexD((double)x.Real, (double)x.Imaginary);
        }

         
        [Cudafy]
        private static int getSequenceDivergence(ComplexD c, int rpnLength, byte[] rpnFormula, ComplexD[] rpnKoef,
            ComplexD init, double infty, int steps, ComplexD[] stackMem, int stackOffset)
        { 
            ComplexD z = init; 
            for (int i = steps - 1; i >= 1; i--)
            { 
                z = eval(rpnLength, rpnFormula, rpnKoef, c, z, stackMem, stackOffset);
                if (abs(z) > infty) return i;
            }
            return 0;
        }



        #region "Math"

        


        [Cudafy]
        private static double abs(ComplexD c)
        {
            return Cudafy.GMath.Sqrt((float)(c.x * c.x + c.y * c.y));
        }

        [Cudafy]
        private static double arg(ComplexD c)
        {
            return Cudafy.GMath.Atan2((float)c.y, (float)c.x);
        }

        [Cudafy]
        private static ComplexD makeComplexD(double _abs, double _arg)
        {
            return new ComplexD(_abs * Cudafy.GMath.Cos((float)_arg), _abs * Cudafy.GMath.Sin((float)_arg));
        }

        [Cudafy]
        private static ComplexD eIPhi(double phi)
        {
            return new ComplexD(Cudafy.GMath.Cos((float)phi), Cudafy.GMath.Sin((float)phi));
        }

        [Cudafy]
        private static ComplexD exp(ComplexD c)
        {
            return makeComplexD(Cudafy.GMath.Exp((float)c.x), c.y);
        }


        [Cudafy]
        private static ComplexD log(ComplexD c)
        {
            return new ComplexD(Cudafy.GMath.Log((float)abs(c)), arg(c));
        }

        [Cudafy]
        private static ComplexD sin(ComplexD c)
        {
            return new ComplexD(Cudafy.GMath.Cosh((float)c.y) * Cudafy.GMath.Sin((float)c.x),
                Cudafy.GMath.Sinh((float)c.y) * Cudafy.GMath.Cos((float)c.x));
        }

        [Cudafy]
        private static ComplexD cos(ComplexD c)
        {
            return new ComplexD(Cudafy.GMath.Cosh((float)c.y) * Cudafy.GMath.Cos((float)c.x),
                -Cudafy.GMath.Sinh((float)c.y) * Cudafy.GMath.Sin((float)c.x));
        }

        [Cudafy]
        private static ComplexD tg(ComplexD c)
        {
            ComplexD sn = sin(c);
            ComplexD cs = cos(c);

            if (cs.x == 0 && cs.y == 0) return new ComplexD(1e37F, 0);
            return ComplexD.Divide(sn, cs);
        }

        [Cudafy]
        private static ComplexD ctg(ComplexD c)
        {
            ComplexD sn = sin(c);
            ComplexD cs = cos(c);

            if (sn.x == 0 && sn.y == 0) return new ComplexD(double.MaxValue, 0);
            return ComplexD.Divide(cs, sn);
        }

        [Cudafy]
        private static ComplexD th(ComplexD c)
        {
            ComplexD sn = sh(c);
            ComplexD cs = ch(c);

            if (cs.x == 0 && cs.y == 0) return new ComplexD(double.MaxValue, 0);
            return ComplexD.Divide(sn, cs);
        }

        [Cudafy]
        private static ComplexD cth(ComplexD c)
        {
            ComplexD sn = sh(c);
            ComplexD cs = ch(c);

            if (sn.x == 0 && sn.y == 0) return new ComplexD(double.MaxValue, 0);
            return ComplexD.Divide(cs, sn);
        }

        [Cudafy]
        private static ComplexD sqrt(ComplexD c)
        {
            return makeComplexD(Cudafy.GMath.Sqrt((float)abs(c)), arg(c) / 2);
        }

        [Cudafy]
        private static ComplexD sh(ComplexD c)
        {
            ComplexD t = ComplexD.Subtract(exp(c), exp(new ComplexD(-c.x, -c.y)));
            return new ComplexD(t.x / 2, t.y / 2);
        }

        [Cudafy]
        private static ComplexD ch(ComplexD c)
        {
            ComplexD t = ComplexD.Add(exp(c), exp(new ComplexD(-c.x, -c.y)));
            return new ComplexD(t.x / 2, t.y / 2);
        }

        [Cudafy]
        private static ComplexD pow(ComplexD c1, ComplexD c2)
        {
            double a = abs(c1);
            double b = arg(c1);
            double c =   c2.x;
            double d =   c2.y;

            if (a == 0) a = 1e-38F;

            return makeComplexD(Cudafy.GMath.Pow((float)a, (float)c) * Cudafy.GMath.Exp((float)(-b * d)), Cudafy.GMath.Log((float)a) * d + b * c);
             
        }


        [Cudafy]
        private static ComplexD arcsin(ComplexD c)
        {
            ComplexD x1 = new ComplexD(-c.y, c.x);
            ComplexD one = new ComplexD(1, 0);
            ComplexD x2 = sqrt(ComplexD.Subtract(one, ComplexD.Multiply(c, c)));
             
            return ComplexD.Multiply(new ComplexD(0, -1), log(ComplexD.Add(x1, x2)));
        }

        [Cudafy]
        private static ComplexD arccos(ComplexD c)
        {
            ComplexD t = arcsin(c);
            return new ComplexD(0.5 *3.14159265358979323846 - t.x, -t.y);
        }

        [Cudafy]
        private static ComplexD arctg(ComplexD c)
        { 
            ComplexD x = ComplexD.Divide(new ComplexD(1-c.y, c.x), new ComplexD(1+c.y,-c.x));
            return ComplexD.Multiply(new ComplexD(0, -0.5), log(x));
        }

        [Cudafy]
        private static ComplexD arcctg(ComplexD c)
        {
            ComplexD t = arctg(c);
            return new ComplexD( 3.14159265358979323846 - t.x, -t.y);
        }


        [Cudafy]
        private static ComplexD eval(int rpnLength, byte[] rpnFormula, ComplexD[] rpnKoef, ComplexD c, ComplexD z, ComplexD[] stack, int stackOffset)
        {
            //return ComplexD.Add( ComplexD.Multiply(ComplexD.Multiply(z,z),z),c);


            int sPtr = stackOffset - 1;
            int vPtr = 0;

            for (int i = 0; i < rpnLength; i++)
            {
                byte v = rpnFormula[i];
                if (v == 0)
                {
                    stack[++sPtr] = rpnKoef[vPtr++];
                }
                else if (v <= 5)
                {
                    if (v == 1) stack[sPtr - 1] = ComplexD.Add(stack[sPtr - 1], stack[sPtr]);
                    else if (v == 2) stack[sPtr - 1] = ComplexD.Subtract(stack[sPtr - 1], stack[sPtr]);
                    else if (v == 3) stack[sPtr - 1] = ComplexD.Multiply(stack[sPtr - 1], stack[sPtr]);
                    else if (v == 4) stack[sPtr - 1] = ComplexD.Divide(stack[sPtr - 1], stack[sPtr]);
                    else if (v == 5) stack[sPtr - 1] = pow(stack[sPtr - 1], stack[sPtr]);

                    sPtr--;
                }
                else if (v <= 10)
                {
                    if (v == 6) stack[sPtr] = log(stack[sPtr]);
                    else if (v == 7) stack[sPtr] = exp(stack[sPtr]);
                    else if (v == 8) stack[sPtr] = sin(stack[sPtr]);
                    else if (v == 9) stack[sPtr] = cos(stack[sPtr]);
                    else if (v == 10) stack[sPtr] = tg(stack[sPtr]);
                }
                else if (v <= 19)
                {
                    if (v == 11) stack[sPtr] = ctg(stack[sPtr]);
                    else if (v == 12) stack[sPtr] = arcsin(stack[sPtr]);
                    else if (v == 13) stack[sPtr] = arccos(stack[sPtr]);
                    else if (v == 14) stack[sPtr] = arctg(stack[sPtr]);
                    else if (v == 15) stack[sPtr] = arcctg(stack[sPtr]);
                    else if (v == 16) stack[sPtr] = sh(stack[sPtr]);
                    else if (v == 17) stack[sPtr] = ch(stack[sPtr]);
                    else if (v == 18) stack[sPtr] = th(stack[sPtr]);
                    else if (v == 19) stack[sPtr] = cth(stack[sPtr]);
                }
                else if (v <= 25)
                {
                    if (v == 20) stack[sPtr] = new ComplexD(abs(stack[sPtr]), 0);
                    if (v == 21) stack[sPtr] = new ComplexD(stack[sPtr].x, 0);
                    if (v == 22) stack[sPtr] = new ComplexD(stack[sPtr].y, 0);
                    if (v == 23) stack[sPtr] = new ComplexD(arg(stack[sPtr]), 0);
                    if (v == 24) stack[sPtr] = sqrt(stack[sPtr]);
                    if (v == 25) stack[sPtr] = new ComplexD(-stack[sPtr].x, -stack[sPtr].y);
                }
                else if (v == 64) stack[++sPtr] = c;
                else if (v == 65) stack[++sPtr] = z;
            }

            return stack[stackOffset];
        }


        #endregion
       


    }
}
