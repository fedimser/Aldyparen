using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;

namespace Aldyparen
{
    public partial class Form2 : Form
    {
        Bitmap target; 
        int r2=40000;

        public byte[] MultVector ;
        


        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                target = (Bitmap) Bitmap.FromFile(ofd.FileName);
                pictureBox1.Image = target;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        { 
            init_work();
            StartCapturing();
        }

        VideoCaptureDevice videoSource;

        void init_work()
        { 
             
            // enumerate video devices
            var videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
            // create video source
              videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
            // set NewFrame event handler
            videoSource.NewFrame += new   NewFrameEventHandler( video_NewFrame );
             


        }

        void StartCapturing()
        {
            videoSource.Start();
        }

        void StopCapturing()
        {
            videoSource.SignalToStop();
        }

        private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
        {
            target = (Bitmap)eventArgs.Frame.Clone();//(new Rectangle(0, 0, 100, 100), System.Drawing.Imaging.PixelFormat.DontCare);
            target.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Bitmap_To_Matrices();

            if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked || checkBox4.Checked)
            {
                if (checkBox1.Checked)
                {
                    MakeNegative();
                }
                if (checkBox2.Checked)
                {
                    MakeMirror();
                }
                else if (checkBox3.Checked)
                {

                    MakeInversion();
                }
                else if (checkBox4.Checked)
                {
                    MakeFractal();
                }
            }

            Matrices_To_Bitmap();

            pictureBox1.Image =target;
             
        }

       const  int W = 640;
        const int H = 480;

        byte[,] target_R = new byte[480, 640];
        byte[,] target_G = new byte[480, 640];
        byte[,] target_B = new byte[480, 640];

        byte[,] target_new_R = new byte[480, 640];
        byte[,] target_new_G = new byte[480, 640];
        byte[,] target_new_B = new byte[480, 640]; 

        unsafe void Bitmap_To_Matrices()
        {  
            BitmapData bd = target.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly,PixelFormat.Format24bppRgb);

            try
            {
                    byte* curpos;  
                    for (int h = 0; h < H; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < W; w++)
                        {
                             target_R[h,w]=  *(curpos++);
                             target_G[h, w] = *(curpos++);
                             target_B[h, w] = *(curpos++);  
                        }
                    } 
            }
            finally
            {
                target.UnlockBits(bd);
            }
        }

        

        unsafe void Matrices_To_Bitmap()
        {
            BitmapData bd = target.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                byte* curpos;
                for (int h = 0; h < H; h++)
                {
                    curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                    for (int w = 0; w < W; w++)
                    {
                        *(curpos++) = target_R[h, w];
                        *(curpos++) = target_G[h, w];
                        *(curpos++) = target_B[h, w]; 
                    }
                }
            }
            finally
            {
                target.UnlockBits(bd);
            }
        }

         
        void MakeInversion ()
        {  

            for(int y=0;y<H;y++)
                for (int x = 0; x < W; x++)
                { 
                    int dx=x-W/2;
                    int dy=y-H/2;

                    if (dx == 0 && dy == 0)
                    {
                        target_new_R[x, y] = target_new_R[x, y];
                        target_new_G[x, y] = target_new_G[x, y];
                        target_new_B[x, y] = target_new_B[x, y]; 
                        continue;
                    }

                    int nx = W / 2 + (r2 * dx) / (dx * dx + dy * dy);
                    int ny = H / 2 + (r2 * dy) / (dx * dx + dy * dy);

                    if (nx < 0 || nx >= W || ny < 0 || ny >= H) 
                    { 
                        target_new_R[y, x] = target_R[y, x];
                        target_new_G[y, x] = target_G[y, x];
                        target_new_B[y, x] = target_B[y, x]; 
                        continue;
                    } 

                    target_new_R[y, x] = target_R[ny,nx];
                    target_new_G[y, x] = target_G[ny, nx];
                    target_new_B[y, x] = target_B[ny, nx];
                }

            for(int y=0;y<H;y++)
                for (int x = 0; x < W; x++)
                {
                    target_R[y, x] = target_new_R[y, x];
                    target_G[y, x] = target_new_G[y, x];
                    target_B[y, x] = target_new_B[y, x]; 
                }
        }

          void MakeNegative()
        {

            for (int h = 0; h < H; h++)
            { 
                for (int w = 0; w < W; w++)
                {
                    target_R[h,w]^= (byte)255; 
                }
            }

            for (int h = 0; h < H; h++)
            {
                for (int w = 0; w < W; w++)
                {
                    target_G[h, w] ^= (byte)255;
                }
            }

            for (int h = 0; h < H; h++)
            {
                for (int w = 0; w < W; w++)
                {
                    target_B[h, w] ^= (byte)255;
                }
            }
           
        }

        unsafe void MakeFractal()
        {   
                int i = 0;
                for (int h = 0; h < H; h++)                
                    for (int w = 0; w < W; w++)
                    { target_R[h, w] = (byte)((target_R[h, w] * MultVector[i]) / 255); i++; }

                i=0;
                for (int h = 0; h < H; h++)
                    for (int w = 0; w < W; w++)
                    {target_G[h, w] = (byte)((target_G[h, w] * MultVector[i]) / 255);i++;}

                i = 0;
                for (int h = 0; h < H; h++)
                    for (int w = 0; w < W; w++)
                    {target_B[h, w] = (byte)((target_B[h, w] * MultVector[i]) / 255);i++;}
                

        }

        unsafe void MakeMirror()
        {  
            int p1,p2;
                for (int h = 0; h < H; h++)
                {
                    p1 = 0;p2=W-1; 
                    while(p1<p2)
                    { 
                        target_R[h,p1]=target_R[h,p2]= (byte)((target_R[h,p1]+target_R[h,p2])/2);
                         target_G[h,p1]=target_G[h,p2]= (byte)((target_G[h,p1]+target_G[h,p2])/2);
                        target_B[h,p1]=target_B[h,p2]= (byte)((target_B[h,p1]+target_B[h,p2])/2);
                        p1++; p2--;
                    }
                }
          
        }

        Color avgColor(Color Color1, Color Color2)
        {
            return Color.FromArgb(255, Color1.R / 2 + Color2.R / 2, Color1.G / 2 + Color2.G / 2, Color1.B / 2 + Color2.B / 2);
        }


        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapturing();
        }
         
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int r = (int)numericUpDown1.Value;
            r2 = r * r;
        }
         
    }
}
