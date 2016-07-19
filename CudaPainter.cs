using System;
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

        //public static int 

        private static GPGPU gpu;
        public static bool enabled = false; 
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

        unsafe public static Bitmap getFrame(int W, int H, CLFrame frame)
        {
            if (!gpu.IsCurrentContext)
            {
                gpu.SetCurrentContext();
            }


            byte[] b = new byte[12 * W * H];
             

            byte[] dev_b = gpu.Allocate<byte>(12*W*H);
            //gpu.CopyToDevice(b,dev_b);


            //Forming and passing color map
            int colorCount = frame.colorMap.Length;
            byte[] colorMap = new byte[3 * colorCount];
            byte[] dev_colorMap = gpu.Allocate<byte>(3 * colorCount);
            for (int i = 0; i < colorCount; i++)
            {
                colorMap[3 * i] = frame.colorMap[i].B;
                colorMap[3 * i+1] = frame.colorMap[i].G;
                colorMap[3 * i+2] = frame.colorMap[i].R;
            }
            gpu.CopyToDevice(colorMap, dev_colorMap);


            dim3 gs = new dim3(2 * W, 2 * H);

            gpu.Launch(gs,1).setPixel(W, H, (float)frame.rotation, (float)frame.scale, (float)frame.ctr.Real, (float)frame.ctr.Imaginary, dev_b,dev_colorMap); 
            gpu.CopyFromDevice(dev_b, b);
            gpu.Free(dev_b);
            gpu.Free(dev_colorMap);


            int W2 = W * 2;
            int H2 = H * 2;
            Bitmap bmp = new Bitmap(W2, H2, PixelFormat.Format24bppRgb);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W2, H2), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte* curpos = ((byte*)bd.Scan0);


            for (int y = 0; y < H2; y++)
            {
                curpos = ((byte*)bd.Scan0) + y * bd.Stride;
                int offset = y*3*W2;
                for (int x = 0; x < 3*W2; x++)
                {
                    *(curpos++) = b[x+offset];
                }
            }
            

            bmp.UnlockBits(bd);
            return bmp;
        }


        [Cudafy]
        private static ComplexF eval(ComplexF[] vars, int[] formula, float[] koef)
        {
            ComplexF[] stack = new ComplexF[formula.Length];

            return stack[0];
        }

        [Cudafy]
        private static int Mandelbrot(ComplexF  c)
        {
          //DEBUG = MANDELBROT
            float r = Cudafy.GMath.Sqrt((c.x - 0.25F) * (c.x - 0.25F) + c.y * c.y);
            float t = Cudafy.GMath.Atan2(c.y, c.x);
            if (r <= 0.5 * (1 - Cudafy.GMath.Cos(t))) return 0;
            
            ComplexF z = new ComplexF(0,0);
            for (int i = 0; i < 200; i++)
            {
                z = ComplexF.Add(ComplexF.Multiply( z ,z), c);
                if (z.x * z.x + z.y * z.y > 4) return 1;
            }
            return 0;
        }

        [Cudafy]
        private static void setPixel(GThread thread, int W, int H, float rotation, float scale, float ctr_x, float ctr_y, byte[] bmp, byte[] colorMap)
        {
             
            int x = thread.blockIdx.x;
            int y = thread.blockIdx.y;
            int idx = x + 2 * W * y;

            ComplexF ctr = new ComplexF(ctr_x, ctr_y);
            ComplexF c1 = new ComplexF(Cudafy.GMath.Cos( rotation), Cudafy.GMath.Sin( rotation)); 
            ComplexF c2 = new ComplexF((x - W) * (scale / W), (y - H) * (scale / W)) ;
            ComplexF c = ComplexF.Add(  ComplexF.Multiply(c1,c2), ctr);
            int clr =  Mandelbrot(c);
             
             
            bmp[3 * idx] = colorMap[3 * clr];            
            bmp[3 * idx+1] = colorMap[3 * clr+1];            
            bmp[3 * idx+2] = colorMap[3 * clr+2];
                  
           
        }



    }
}
