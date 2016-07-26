using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using AForge.Video.VFW;
using System.Drawing.Imaging;

using System.Threading;
using System.IO;

namespace Aldyparen
{
    public partial class FormMain : Form
    {

        public Frame curFrame;
        public bool curFrameChanged = false;

        Frame[] frames;
        Color[] colors;
        int frame_counter=0; 


        public FormMain()
        {
            InitializeComponent();
        }
             
        int W,H;
        int W1 = 160;
        int H1 = 90;
        int FPS;
        int AnimLength;
        double ScreenScale;



        //Settings
        public bool realTimeApplying=false;
        public bool isNowVideoSaving = false;
        public string statusFrames = "";


        //Make animation
        private void button2_Click(object sender, EventArgs e)
        {

            makeAnimation();
        }

        private void makeAnimation()
        {
            Frame f = lastFrame().clone();
            int S = (int)numericUpDownDiv.Value;


            for (int i = 1; i <= AnimLength; i++)
            {
                f = f.getMove(curFrame, 1.0 / (AnimLength - i + 1));
                f.moveColors(S);

                frames[frame_counter] = f;
                frame_counter++;
            }

            pictureBox1.Image = lastFrame().getFrame(W1, H1);

            //curFrame = lastFrame().clone();
            //curFrameChanged = true;
        }


        private Frame lastFrame()
        {
            return frames[frame_counter - 1];
        }

       

        void addFrame(Frame f)
        {
            frames[frame_counter] = f;
            frame_counter++;

            showLastFrame();
        }



        void initWork()
        {
            curFrame = new Frame();

            colors = new Color[Frame.MAX_COLORS];
            for (int i = 0; i < Frame.MAX_COLORS; i++) colors[i] = Color.White;
        }

        void startWork()
        {
            ScreenScale = Math.Max(2.0 * W / pictureBox1.Width, 2.0 * H / pictureBox2.Height);


            frames = new Frame[10000];
            frame_counter = 0;
             
            addFrame(curFrame.clone());            
            curFrameChanged = true;

            panel1.BackColor = curFrame.colorMap[(int)numericUpDown10.Value];
        }
         
         
 

      


        #region "Animation Settings"

        Complex get_Mouse_Position()    //In Relative Screen coord
        {
            Point z = Cursor.Position;
            z = PointToClient(z);
            return new Complex(z.X - pictureBox2.Left - pictureBox2.Width / 2, z.Y - pictureBox2.Top - pictureBox2.Height / 2) / (0.5 * pictureBox2.Width);

        }

 
           
        void resetTransform()
        {  
            curFrame.restoreTransform(lastFrame());
            curFrameChanged = true;
        }

        void resetParams()
        {
            curFrame.param = lastFrame().param.clone();
            curFrameChanged = true;
        }
         

        void refreshFrame()
        { 
             pictureBox2.Image = curFrame.getFrame(W1, H1); 
            curFrameChanged = false;
        }

         

        private void pictureBox2_MouseWheel(object sender,MouseEventArgs e)
        {
            double delta = -Math.Sign(e.Delta);
            if ((Control.ModifierKeys & Keys.Shift) != 0) delta *= 20;


            if ((Control.ModifierKeys & Keys.Control) != 0)
            { 
                curFrame.rotation += 2*0.017453292*delta; 
                curFrameChanged = true;
                return;
            }

            double k=Math.Pow(1.05,delta);

           

            Complex p1 = curFrame.ctr + curFrame.scale * get_Mouse_Position();
            curFrame.ctr = p1 - (p1 - curFrame.ctr) * k;
            curFrame.scale = curFrame.scale * k;

            curFrameChanged = true;
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            pictureBox2.Focus();
        }

        bool is_now_tracking = false;
        Point last_tracling_cursor_position;

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {  
            is_now_tracking = true;
            last_tracling_cursor_position = Cursor.Position;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (is_now_tracking) ApplyTracking();
            is_now_tracking = false;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            if (is_now_tracking) ApplyTracking();
            is_now_tracking = false;
        }

        void ApplyTracking()
        {
            Point p = Cursor.Position;

            Complex dx = new Complex(p.X - last_tracling_cursor_position.X, p.Y - last_tracling_cursor_position.Y);
            curFrame.ctr -= (2.0 / pictureBox2.Width) * Complex.Exp(Complex.ImaginaryOne*curFrame.rotation)*curFrame.scale * dx;

            last_tracling_cursor_position = p;
            curFrameChanged = true;
        }


        #endregion


        #region "Video"


        private void makeVideoClick()
        {
            if (isNowVideoSaving)
            {
                MessageBox.Show("One video rendering in progress. Please wait.");
                return;
            }

            if (!Directory.Exists("Output\\Video"))
            {
                Directory.CreateDirectory("Output\\Video");
            }

            var p= new VideoParameters();

            p.halfWidth =   (int)numericUpDown6.Value / 2;
            p.halfHeight =   (int)numericUpDown7.Value / 2;

            string name = DateTime.Now.ToString().Replace(":", "");

            p.path = "Output\\Video\\" + name + ".avi";
 

            p.FPS = (int) numericUpDown5.Value;

            p.frameCount = frame_counter;
            p.frames = new Frame[frame_counter];
            
            for (int i = 0; i < frame_counter; i++)
            {
                p.frames[i] = frames[i];
            }

            Thread t = new Thread(makeVideo);
            t.Start(p);         
        }


        public class VideoParameters
        {
            public Frame[] frames;
            public int frameCount;
            public int halfWidth;
            public int halfHeight;
            public string path;
            public int FPS;  

        }

        private void makeVideo(object _param)
        {
            isNowVideoSaving = true;
            photoThreadsCounter++;

            VideoParameters param = (VideoParameters)_param;

            AForge.Video.VFW.AVIWriter wr;
            wr = new AVIWriter();

            wr.FrameRate = Convert.ToInt32(param.FPS);
            wr.Open(param.path, 2*param.halfWidth,2* param.halfHeight);

            for (int i = 0; i < frame_counter; i++)
            {
                wr.AddFrame(param.frames[i].getFrame(param.halfWidth, param.halfHeight));
                Console.WriteLine("Frame {0} out of {1} is done!", i + 1, param.frameCount);
                statusFrames = String.Format("{0}/{1}", i + 1, param.frameCount);
            }

            wr.Close();

            photoThreadsCounter--; 
            isNowVideoSaving = false;
            statusFrames = "";
        }

        #endregion




        #region "Mapping"
        static Complex[] values;
        static int values_cnt = 0;

        private void reset_values_list()
        {
            values = new Complex[curFrame.param.maxUsedColors];
            values_cnt = 0;
        }

        private int get_value_number(Complex x)
        {
            for (int i = 0; i < values_cnt; i++) if (Math.Abs(x.Real - values[i].Real) + Math.Abs(x.Imaginary - values[i].Imaginary) < 1e-8) return i;

            if (values_cnt == curFrame.param.maxUsedColors) return values_cnt - 1;

            values[values_cnt] = x;
            values_cnt++;
            return values_cnt - 1;
        }


        #endregion 
          

        private void timer1_Tick(object sender, EventArgs e)
        {   
            FPS = (int)numericUpDown5.Value;
            W = (int)numericUpDown6.Value;
            H = (int)numericUpDown7.Value;
            AnimLength = Convert.ToInt32(numericUpDown1.Value);

            labelOF.Text = String.Format("scl={0:e2} ctr=({1:e2};{2:e2}) Frame={3} Time={4:0.##}", lastFrame().scale, lastFrame().ctr.Real, lastFrame().ctr.Imaginary, frame_counter, (double)frame_counter / FPS);
            labelNF.Text = String.Format("scl={0:e2} ctr=({1:e2};{2:e2}) Frame={3} Time={4:0.##}", curFrame.scale, curFrame.ctr.Real, curFrame.ctr.Imaginary, frame_counter + AnimLength, (double)(frame_counter + AnimLength) / FPS);

            if (is_now_tracking) ApplyTracking();

            if (curFrameChanged)
            {
                refreshFrame();
            }

            labelThreads.Text = "Threads: " + photoThreadsCounter.ToString();

            if (CudaPainter.enabled)
            {
                if (CudaPainter.canRender(curFrame)  )
                    LabelCuda.Text = "CUDA Enabled";
                else
                    LabelCuda.Text = "CUDA Enabled but cannot be used";
            }
            else
            {
                LabelCuda.Text = "CUDA Disabled";
            }
            remove1FrameToolStripMenuItem.Enabled = (frame_counter > 1);           
            remove10ToolStripMenuItem.Enabled = (frame_counter > 10);

            if(curFrame.genMode == Frame.GeneratingMode.Formula)
            {
                labelActiveFormula.Text = curFrame.param.genFunc.getText();
            }
            else
            {
                labelActiveFormula.Text = "";
            }

            labelFrames.Text = statusFrames;
            labelStopwatch.Text = statusTimeInfo;
         }



        private void Form1_Load(object sender, EventArgs e)
        { 
            
            initWork();



            curFrame = new Frame();
            curFrame.genMode = Frame.GeneratingMode.Formula;
            curFrame.param = new FrameParams(0, 100);
            curFrame.param.genFunc = new Function(new String[2] { "c", "z" });
            curFrame.param.genFunc.setText(textBoxFormula.Text);
            curFrame.param.genInfty = 2;
            curFrame.param.genSteps = 20;
            curFrame.param.genInit = new Complex(0, 0);

            setMapColor(0, Color.Black);
            setMapColor(1, Color.White);


            reset_values_list();


             


            ResetColorCounters(); 
            setColors(curFrame);

            /*
            if (frames == null || (checkBox1.Checked == false)) startWork();
            else
            {
                addFrame(curFrame.clone());
                showLastFrame();
            }*/


            refreshFrame();


            startWork();
        }
         

        private void removeLastFrame()
        {
            if (frame_counter <=1  ) return;
            frame_counter--;
            showLastFrame();
        }

        private void removeLastTenFrames()
        {
            frame_counter-=10;
            if (frame_counter < 1) frame_counter = 1;
            showLastFrame();
        }

        private void showLastFrame()
        {
            pictureBox1.Image = lastFrame().getFrame(W1, H1); 
        }

       

        /*
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           

            if (comboBox1.SelectedIndex == 0)
            {
                curFrame = new Frame();
                curFrame.genMode = Frame.GeneratingMode.Formula;
                curFrame.param = new FrameParams(0, 20);
                curFrame.param.genFunc = getFunction();
                curFrame.param.genInfty = 2;
                curFrame.param.genSteps = 20;
                curFrame.param.genInit = new Complex(0, 0);




                setMapColor(0, Color.Black);
                setMapColor(1, Color.White);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.CubeRootPainter); 
                curFrame.param = new FrameParams(0,3);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.TetraRootPainter);
              
                curFrame.param = new FrameParams(0,40);
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.DebugPainter);
                curFrame.param = new FrameParams(4, 100);
            }
            if (comboBox1.SelectedIndex == 4)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.Mandelbrot2Painter);
                curFrame.param = new FrameParams(0, 256);
                colors[255] = Color.Black;
                for (int i = 0; i < 255; i++)
                {
                     setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.MandelBrotCubePainter);
                curFrame.param = new FrameParams(0, 256);
                colors[255] = Color.Black;
                for (int i = 0; i < 255; i++)
                {
                    setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.GenericMandelbrotPainter);
                curFrame.param = new FrameParams(1, 256);
                
               setMapColor(curFrame.param.maxUsedColors - 1, Color.Blue);

                setMapColor(0,Color.Black);
                setMapColor(curFrame.param.maxUsedColors - 2, Color.Yellow);
                makeGradient(0, curFrame.param.maxUsedColors - 2);

            }
            else if (comboBox1.SelectedIndex == 7)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.GenericMandelbrotPainter);
                curFrame.param = new FrameParams(1, 20);
                setMapColor(curFrame.param.maxUsedColors - 1,Color.Blue);

                setMapColor(0, Color.Black);
                setMapColor(curFrame.param.maxUsedColors - 2, Color.Yellow);
                makeGradient(0, curFrame.param.maxUsedColors - 2);

            } 
            else if (comboBox1.SelectedIndex == 8)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.MandelBrotTetraPainter);
                curFrame.param = new FrameParams(0, 256);
                setMapColor(255, Color.Black);
                for (int i = 0; i < 255; i++)
                {
                 setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 9)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.DzetaPainter);
                curFrame.param = new FrameParams(0, 50);
            }
            else if (comboBox1.SelectedIndex == 10)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.PolynomPainter2);
                curFrame.param = new FrameParams(6, 50);
                makeGeoGradient(0, 49);
            }
            else if (comboBox1.SelectedIndex == 11)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.PolynomPainter3);
                curFrame.param = new FrameParams(10, 50);
                makeGeoGradient(0, 49);
            }

            else if (comboBox1.SelectedIndex == 12)
            {
                curFrame.genMode = Frame.GeneratingMode.Delegate;
                curFrame.dlgt = new Frame.Get_Pixel(FractalPainters.PolynomPainterAlt);
                curFrame.param = new FrameParams(10, 50);
                makeGeoGradient(0, 49);
            }

            reset_values_list();



            resetParamsMenu();
            

            ResetColorCounters();
            textBox1.Text = comboBox1.Text;
            setColors(curFrame);


            if (frames == null || (checkBox1.Checked == false)) startWork();
            else
            {
                addFrame(curFrame.clone());
                showLastFrame();
            }


            refreshFrame();
        }
         * 
         * */


        private void setColors(Frame f)
        {
            for (int i = 0; i < f.param.maxUsedColors; i++)
            {
                f.colorMap[i] = colors[i];
            }

            curFrameChanged = true;
        }

        /*
        #region "Params menu"


        Label[] labelsParams;
        NumericUpDown[] inputParams;
        int maxParams = 20;

        void initParamsMenu()
        {
            labelsParams = new Label[maxParams];
            inputParams = new NumericUpDown[maxParams]; 
            
            for (int i = 0; i < maxParams; i++)
            {
                labelsParams[i] =  new Label();
                inputParams[i] = new NumericUpDown();

                groupBoxParams.Controls.Add(labelsParams[i]);
                groupBoxParams.Controls.Add(inputParams[i]);

                 

                labelsParams[i].Location = new Point(5, 20+ 25 * i);
                labelsParams[i].Text="k" + i.ToString();
                labelsParams[i].AutoSize = true;
                

                inputParams[i].Location = new Point(30, 20+25 * i);
                inputParams[i].Maximum = 1000;
                inputParams[i].Minimum = -1000;
                inputParams[i].Increment = (decimal)0.001;
                inputParams[i].DecimalPlaces = 3;
                inputParams[i].Width = 70;
                inputParams[i].Tag = i.ToString();
                inputParams[i].Value = 1;
                inputParams[i].ValueChanged += new System.EventHandler(this.inputParams_ValueChanged);
                inputParams[i].Click += new System.EventHandler(this.inputParams_Click);
            }
        }

        void resetParamsMenu()
        { 


            //if (labelsParams == null) return;
            
            for (int i = 0; i < maxParams; i++)
            {
                labelsParams[i].Hide();
                inputParams[i].Hide();
            }

            for (int i = 0; i <  curFrame.param.N; i++)
            {
                labelsParams[i].Show();
                inputParams[i].Show();
                curFrame.param.k[i] = (double)inputParams[i].Value;
            }

        }

        private void inputParams_ValueChanged(object sender, EventArgs e)
        {
            var sndr = (NumericUpDown)sender;
            int id = Convert.ToInt32(sndr.Tag);
            curFrame.param.k[id] = (double)sndr.Value;

            curFrameChanged = true;
        } 

        private void      inputParams_Click(object sender, EventArgs e)
        {
            var sndr = (NumericUpDown)sender;
            sndr.Select(0,100);
        }

        #endregion
        */


        #region "Color map"
        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            panel1.BackColor = colors[(int)numericUpDown10.Value];
        } 
        
        private void panel1_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = panel1.BackColor;
            if (cd.ShowDialog() != DialogResult.OK) return;

            setMapColor((int)numericUpDown10.Value, cd.Color);

           // frames[frame_counter - 1].colorMap[(int)numericUpDown10.Value] = cd.Color;
           // pictureBox1.Image = frames[frame_counter - 1].getFrame(W1, H1);
            curFrameChanged = true;
        }

        private void setMapColor(int index, Color color)
        {
            colors[index] = color;
            if (index < curFrame.param.maxUsedColors)
            {
                curFrame.colorMap[index] = color;
                curFrameChanged = true;
            }

            if ((int)numericUpDown10.Value == index)
            {
                panel1.BackColor = color;
            }
       }


        
        private void button4_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < curFrame.param.maxUsedColors; i++)
            {
                setMapColor(i,   Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255)));
                setColors(lastFrame());
            }
            curFrameChanged = true;
 
        }

        //Gradiet
        private void button7_Click(object sender, EventArgs e)
        {
            int c1 = (int)numericUpDown11.Value;
            int c2 = (int)numericUpDown12.Value;

            if (checkBoxGeo.Checked)
            {
                makeGeoGradient(c1, c2);
            }
            else
            {
                makeGradient(c1, c2);
            }

        }

        void makeGeoGradient(int c1, int c3)
        { 
            int c2 = (c1 + c3) / 2;

            if (c1 == c2 || c2 == c3)
            {
                MessageBox.Show("Small borders");
                return;
            }

            setMapColor(c1, Color.Green);
            setMapColor(c2, Color.Yellow);
            setMapColor(c3, Color.Red);

            makeGradient(c1, c2);
            makeGradient(c2, c3);
        }

        void makeGradient(int c1, int c2)
        {

            int R1 = colors[c1].R;
            int G1 = colors[c1].G;
            int B1 = colors[c1].B;

            int R2 = colors[c2].R;
            int G2 = colors[c2].G;
            int B2 = colors[c2].B;


            for (int i = 0; i <= c2-c1; i++)
            {
              setMapColor(c1+i, Color.FromArgb(255, R1 + i * (R2 - R1) / (c2 - c1), G1 + i * (G2 - G1) / (c2 - c1), B1 + i * (B2 - B1) / (c2 - c1)));
            }  
        }

        void ResetColorCounters()
        {

            numericUpDown10.Maximum = curFrame.param.maxUsedColors- 1;
            numericUpDown11.Maximum = curFrame.param.maxUsedColors - 1;
            numericUpDown12.Maximum = curFrame.param.maxUsedColors - 1;

            numericUpDown11.Value=0;
            numericUpDown12.Value = curFrame.param.maxUsedColors - 1;


        }


#endregion



        #region "PhotoTaking"

        public class photoParameters
        { 
           public  Frame _frame;
           public int _W;
           public int _H;
           public string path;
           public string label;
           public ImageFormat format = ImageFormat.Jpeg;
             
        }

        private int photoThreadsCounter = 0;
        private string statusTimeInfo = "";

        private void makePhoto(object _param)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            photoThreadsCounter++;
            
            photoParameters param = (photoParameters)_param;
            Bitmap bmp = param._frame.getFrame(param._W, param._H);

            if (bmp == null) return;

            if (param.label.Length > 0)
            {
                int Frame_Width = 2 * param._W;
                int Frame_Height = 2 * param._H;
                Graphics g = Graphics.FromImage(bmp);
                g.DrawString(textBox1.Text, new Font("Consolas", 10), Brushes.Gray, new PointF(0, 0));
                g.DrawString(String.Format("Pos:({0:e5};{1:e5}).Width:{2:e5}", param._frame.ctr.Real, param._frame.ctr.Imaginary, 2 * param._frame.scale), new Font("Consolas", 10), Brushes.Gray, new PointF(0, 11));
            }

            
            bmp.Save(param.path,param.format);

            photoThreadsCounter--;

            statusTimeInfo = sw.ElapsedMilliseconds.ToString() + "ms";
        }

        private void makePhotoClick()
        { 
            string path="p.jpg";

            if (checkBox2.Checked)
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = "JPG Files|*.jpg";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    path = sfd.FileName;
                }
            }
            else
            {
                if(!System.IO.Directory.Exists(Application.StartupPath+"\\Output"))
                {
                    System.IO.Directory.CreateDirectory(Application.StartupPath+"\\Output");
                }


                int n = 1;
                while (true)
                {
                    path = Application.StartupPath+"\\Output\\picture" + n.ToString() + ".jpg";

                    if (checkBoxBMP.Checked)
                    {
                        path = Application.StartupPath + "\\Output\\picture" + n.ToString() + ".bmp";
                    }

                    if (!System.IO.File.Exists(path)) break;
                    n++;
                }
            }

            Frame fr = curFrame.clone();
            

            string label = "";
            if (checkBox3.Checked)
            {
                label = String.Format("Pos:({0:e5};{1:e5}).Width:{2:e5}",fr.ctr.Real,fr.ctr.Imaginary,fr);
            }

            pictureBox2.Image.Save(path);

            photoParameters param=new photoParameters();
            param._frame= fr;
            param._W=(int)numericUpDown4.Value/2;
            param._H= (int)numericUpDown3.Value/2;
            param.path = path;
            param.label=label;

            if (checkBoxBMP.Checked)
            {
                param.format = ImageFormat.Bmp;
            }


            bool separThread = true;

            if (!separThread)
            {
                makePhoto(param);
            }
            else
            {
                Thread t = new Thread(makePhoto);
                t.Start(param);
            }
             
        }

        #endregion

         
        //show another window
        private void button10_Click(object sender, EventArgs e)
        {
            var f2 = new Form2();


            Frame fr = curFrame.clone();

            Bitmap tb  = fr.getFrame(640/2, 480/2);
            f2.MultVector = new byte[640 * 480];

            int p = 0;
            for(int y=0;y<480;y++)
                for (int x = 0; x < 640; x++)
                {
                    Color c = tb.GetPixel(x, y);
                    byte b = (byte)((c.R+c.G+c.B)/3);
                    f2.MultVector[p] = b;
                    p++;
                }
            
            f2.ShowDialog();
        }

       
 

  

        
         

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        { 
            textBox1.Enabled = checkBox3.Checked;
        }

        private void setArea()
        {
            double _ctr1=0, _ctr2=0, _scl=0;

            try
            {
                Utilty.InputBox("", "Center.Real", ref _ctr1);
                Utilty.InputBox("", "Center.Imaginary", ref _ctr2);
                Utilty.InputBox("", "Scale", ref _scl);
            }
            catch
            {
                return;
            }
            curFrame.scale = _scl;
            curFrame.ctr = new Complex(_ctr1, _ctr2);
            curFrameChanged = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            resetParams();
        }


 

 

        private void cUDASettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormCudaSettings();
            f.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Function f = new Function();
            f.addVariable("x");

            Dictionary<String, Complex> vars = new Dictionary<string, Complex>();
            vars["x"] = new Complex(14, 88);

            f.setText(textBoxFormula.Text);

            if (f.isCorrect())
            {
                labelActiveFormula.Text = f.eval(vars).ToString();
            }
            else
            {
                labelActiveFormula.Text = f.errorMessage;
            }
        }


        public void paintText(RichTextBox tb, Color clr, int start, int end)
        { 
            int cp = tb.SelectionStart;
            tb.Select(start, end - start + 1);
            tb.SelectionColor = clr;
            tb.DeselectAll();
            tb.SelectionStart = cp;
        }

        public void paintText(RichTextBox tb, Color clr)
        {
            int cp = tb.SelectionStart;
            tb.SelectAll();
            tb.SelectionColor = clr;
            tb.DeselectAll();
            tb.SelectionStart = cp;
        }


        public void refreshFormula()
        {
            Function f = new Function(new String[2] { "c", "z" });
            f.setText(textBoxFormula.Text);
             

            if (f.isCorrect())
            {
                int cp = textBoxFormula.SelectionStart;
                labelFormulaError.Text = "";
                curFrame.param.genFunc = f;
                curFrameChanged = true;
                textBoxFormula.Text = f.getText();
                textBoxFormula.SelectionStart = cp;
            }
            else
            {
                labelFormulaError.Text = f.errorMessage;                 
                paintText(textBoxFormula, Color.Red, f.errorIdx1,f.errorIdx2);
            } 
        }

        private void textBoxFormula_TextChanged(object sender, EventArgs e)
        {
            paintText(textBoxFormula, Color.Black);
            if (realTimeApplying) refreshFormula();             
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            refreshFormula();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = new FormSettings();
            f.formMain = this;
            f.ShowDialog();
        }

        private void classicMandelbrotSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxFormula.Text = "z*z+c";
            refreshFormula();
        }

        const String gen_pn_2 = "1*z*z+1*z*c+1*c*c+1*z+1*c+1";
        const String gen_pn_3 = "1*z*z*z+1*z*z*c+1*z*c*c+1*c*c*c*c+1*z*z+1*z*c+1*c*c+1*z+1*c+1";


        private void polynom2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxFormula.Text = gen_pn_2;
            refreshFormula();
        }

        private void polynom3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxFormula.Text = gen_pn_3;
            refreshFormula();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startWork();
        }

        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addFrame(curFrame.clone());
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frame_counter--;
            addFrame(curFrame.clone());
        }

        private void remove1FrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeLastFrame();
        }

        private void remove10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeLastTenFrames();
        }

        private void makeAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makeAnimation();
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setArea();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetTransform();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            makeVideoClick();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            makePhotoClick();
        }

        private void makeVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makeVideoClick();
        }

        private void makePhotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makePhotoClick();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aldyparen " + Application.ProductVersion + "\n" + "© Dmitriy Fedoriaka, 2015-2016");
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You can get manual on the page of the project: http://fedimser.github.io/aldyparen.html");
        }
         
         

    }
}
