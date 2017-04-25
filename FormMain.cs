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
        int frameCount=0;

        Grid gridRight = new Grid();
        Boolean useAnnotation = false;


        public FormMain()
        {
            InitializeComponent();
        }

        string applicationTitle = "Algebraic Dynamic Parametric Renderer";
             
        int W,H;
        public int FPS = 16;
        int halfWidth = 160;
        int halfHeight = 90; 
        int AnimLength;
        double ScreenScale;




        //Settings
        public bool realTimeApplying=false;
        public bool isNowVideoSaving = false;
        public string statusFrames = "";
        public string[] arguments;

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

                frames[frameCount] = f;
                frameCount++;
            }


            showFrame = frameCount - 1;
            refreshLeftPicture();
            currentProjectModified = true;
        }


        private Frame lastFrame()
        {
            return frames[frameCount - 1];
        }

       

        void addFrame(Frame f)
        {
            frames[frameCount] = f;
            frameCount++;

            showFrame = frameCount - 1;
            refreshLeftPicture();
            currentProjectModified = true;
        }
         

        void startWork()
        {
            ScreenScale = Math.Max(2.0 * W / pictureBox1.Width, 2.0 * H / pictureBox2.Height);
             
            frames = new Frame[10000];
            frameCount = 0;
             
            //addFrame(curFrame.clone());            
            curFrameChanged = true;
            currentProjectModified = true;

            panel1.BackColor = curFrame.colorMap[(int)numericUpDown10.Value];
            pictureBox1.Image = null;
        }
         
         
 

      


        #region "Animation Settings"

        PointF getMousePosition()  
        {
            Point z = PointToClient(Cursor.Position);

            if (pictureBox2.SizeMode == PictureBoxSizeMode.Normal)
            {
                return new PointF(z.X - pictureBox2.Left, z.Y - pictureBox2.Top);
            } else {
                Size curFrameSize = curFrame.getSize();
                float ZoomScale = Math.Min(1.0F*pictureBox2.Width / curFrameSize.Width, 1.0F*pictureBox2.Height / curFrameSize.Height);

                float realX = z.X - (pictureBox2.Left + 0.5F * pictureBox2.Width);
                float realY = z.Y - (pictureBox2.Top + 0.5F * pictureBox2.Height);

                return new PointF(realX/ZoomScale + 0.5F*curFrameSize.Width, realY/ZoomScale+0.5F*curFrameSize.Height);
            }
        }

 
           
        void resetTransform()
        {
            try
            {
                curFrame.restoreTransform(lastFrame());
            }
            catch (Exception e)
            {
                curFrame.ctr = 0;
                curFrame.scale = 1;
            }
                curFrameChanged = true;
        }

        void resetParams()
        {
            curFrame.param = lastFrame().param.clone();
            curFrameChanged = true;
        }



        void refreshRightPicture()
        { 
             pictureBox2.Image = curFrame.getFrame(halfWidth, halfHeight, gridRight); 
             curFrameChanged = false;
        }

         

        private void pictureBox2_MouseWheel(object sender,MouseEventArgs e)
        {
            double delta = -Math.Sign(e.Delta);
            Complex pole = curFrame.pictureToMath(getMousePosition());
            
            
            if ((Control.ModifierKeys & Keys.Shift) != 0) delta *= 20;


            if ((Control.ModifierKeys & Keys.Control) != 0)
            {
                delta = 2 * 0.017453292 * delta;
                curFrame.rotation += delta; 
                curFrame.ctr = pole + (curFrame.ctr-pole)*Complex.Exp(Complex.ImaginaryOne*delta);
                curFrameChanged = true;
                return;
            }

            double k=Math.Pow(1.05,delta);



            curFrame.ctr = pole - (pole - curFrame.ctr) * k;
            curFrame.scale = curFrame.scale * k;

            curFrameChanged = true;
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            isCursorOverRightArea = true;
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
            isCursorOverRightArea = false;
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

        public int videoWidth = 720;
        public int videoHeight = 540;

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

            p.halfWidth = videoWidth / 2;
            p.halfHeight = videoHeight / 2;

            string name = DateTime.Now.ToString().Replace(":", "");

            p.path = "Output\\Video\\" + name + ".avi";
 

            p.FPS = this.FPS;

            p.frameCount = frameCount;
            p.frames = new Frame[frameCount];
            
            for (int i = 0; i < frameCount; i++)
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

            for (int i = 0; i < frameCount; i++)
            {
                wr.AddFrame(param.frames[i].getFrame(param.halfWidth, param.halfHeight,gridRight));
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
          

        private bool isCursorOverRightArea = false;


        #region "Frame Navigation"

        int showFrame = 0;

        string getShowFrameInfo()
        {
            if (frameCount == 0) return "No frames";
             
            Frame f = frames[showFrame];
            return String.Format("Frame {0}/{1}\n Time {2:0.##}\n"+ 
                "scale {3:e2}\n" + "center ({4:e2};{5:e2})\n{6}",
                showFrame+1, frameCount, showFrame / FPS, f.scale, f.ctr.Real, f.ctr.Imaginary,
                f.getAnnotation()
            );
        }

        
        void refreshLeftPicture()
        {
            hScrollBarFrames.Maximum = frameCount-1;
            hScrollBarFrames.Value = showFrame;
            
            pictureBox1.Image = frames[showFrame].getFrame(halfWidth, halfHeight, gridRight);
            
        }



        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            showFrame = hScrollBarFrames.Value;
            refreshLeftPicture();
        }
         

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (arguments!=null && arguments.Length == 1)
            {
                string fileName = arguments[0];
                arguments = null;
                if (!openProject(fileName))
                {
                    MessageBox.Show("Coluldn't load project");
                } 
            }

            AnimLength = Convert.ToInt32(numericUpDown1.Value);

            labelFrameInfo.Text = getShowFrameInfo();
            hScrollBarFrames.Enabled = frameCount != 0;
            groupBoxAnimation.Enabled = frameCount != 0;

            if(isCursorOverRightArea) 
            {
                Complex pos = curFrame.pictureToMath(getMousePosition());
                labelCurFrameInfo.Text = String.Format("({0:0.##};{1:0.##}) scale={2:0.##}", pos.Real, pos.Imaginary, curFrame.scale);
                 
            }

            if (is_now_tracking) ApplyTracking();

            if (curFrameChanged)
            {
                refreshRightPicture();
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
            remove1FrameToolStripMenuItem.Enabled = (frameCount > 1);           
            remove10ToolStripMenuItem.Enabled = (frameCount > 10);

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
             
            if (this.currentProjectFile!=null){
                this.Text = applicationTitle + " - " + currentProjectFile + (currentProjectModified? "*":"");
            }
            
         }



        private void Form1_Load(object sender, EventArgs e)
        {
            curFrame = new Frame();

            curFrame.colorMap = new Color[Frame.MAX_COLORS];
            for (int i = 0; i < Frame.MAX_COLORS; i++) curFrame.colorMap[i] = Color.White;

             
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
            prepareDialogs();
            startWork();

            
            timer1.Start();
        }
         


        private void removeLastFrames(int count)
        {
            frameCount -= count;
            if (frameCount < 0) frameCount = 0;
            refreshLeftPicture();
            currentProjectModified = true;
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


            refreshRightPicture();
        }
         * 
         * */


        private void setColors(Frame f)
        {
            for (int i = 0; i < f.param.maxUsedColors; i++)
            {
                f.colorMap[i] = curFrame.colorMap[i];
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
            panel1.BackColor = curFrame.colorMap[(int)numericUpDown10.Value];
        } 
        
        private void panel1_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = panel1.BackColor;
            if (cd.ShowDialog() != DialogResult.OK) return;

            setMapColor((int)numericUpDown10.Value, cd.Color);

           // frames[frameCount - 1].colorMap[(int)numericUpDown10.Value] = cd.Color;
           // pictureBox1.Image = frames[frameCount - 1].getFrame(halfWidth, halfHeight);
            curFrameChanged = true;
        }

        private void setMapColor(int index, Color color)
        {
            curFrame.colorMap[index] = color;
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

            int R1 = curFrame.colorMap[c1].R;
            int G1 = curFrame.colorMap[c1].G;
            int B1 = curFrame.colorMap[c1].B;

            int R2 = curFrame.colorMap[c2].R;
            int G2 = curFrame.colorMap[c2].G;
            int B2 = curFrame.colorMap[c2].B;


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
        public bool photoSelectDestination = false;
        public ImageFormat photoImageFormat =   ImageFormat.Jpeg;
        public int photoWidth = 1920;
        public int photoHeight = 1080;
        public string photoLabel = null;
        private SaveFileDialog sfdPhoto = new SaveFileDialog();

        private void makePhoto(object _param)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            photoThreadsCounter++;
            
            photoParameters param = (photoParameters)_param;
            Bitmap bmp = param._frame.getFrame(param._W, param._H,gridRight);

            if (bmp == null) return;

            if (param.label!= null && param.label.Length>0)
            {
                int Frame_Width = 2 * param._W;
                int Frame_Height = 2 * param._H;
                Graphics g = Graphics.FromImage(bmp);
                // g.DrawString(textBox1.Text, new Font("Consolas", 10), Brushes.Gray, new PointF(0, 0));
                // g.DrawString(String.Format("Pos:({0:e5};{1:e5}).Width:{2:e5}", param._frame.ctr.Real, param._frame.ctr.Imaginary, 2 * param._frame.scale), new Font("Consolas", 10), Brushes.Gray, new PointF(0, 11));
            }

            
            bmp.Save(param.path,param.format);

            photoThreadsCounter--;

            statusTimeInfo = sw.ElapsedMilliseconds.ToString() + "ms";
        }

        private String getDialogFilter(ImageFormat format)
        {
            if (format == ImageFormat.Jpeg) return "JPEG files|*.jpg";
            else if (format == ImageFormat.Bmp) return "BMP files|*.bmp";
            else return "All files|*.*";
        }

        private string getFromatExtension(ImageFormat format)
        {
            if (format == ImageFormat.Jpeg) return ".jpg";
            else if (format == ImageFormat.Bmp) return ".bmp";
            else return "";
        }

        private void makePhotoClick()
        { 
            string path="p.jpg";

            if (this.photoSelectDestination)
            { 
                 
                sfdPhoto.Filter = getDialogFilter(photoImageFormat);
                if (sfdPhoto.ShowDialog() == DialogResult.OK)
                {
                    path = sfdPhoto.FileName;
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
                    path = Application.StartupPath+"\\Output\\picture" + n.ToString() + getFromatExtension(photoImageFormat);
                    if (!System.IO.File.Exists(path)) break;
                    n++;
                }
            }

            Frame fr = curFrame.clone();
             

            pictureBox2.Image.Save(path);

            photoParameters param=new photoParameters();
            param._frame= fr;
            param._W= photoWidth/2;
            param._H= photoHeight/2;
            param.path = path;
            param.label = photoLabel;
            param.format = photoImageFormat;


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
            
            Function f = new Function();
            f.setText(textBoxFormula.Text);
            try
            {
                this.Text =  f.eval().ToString();
            }
            catch (Exception ex)
            {
                //
            }
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
            frameCount--;
            addFrame(curFrame.clone());
        }

        private void remove1FrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeLastFrames(1);
        }

        private void remove10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeLastFrames(10);
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

  
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aldyparen " + Application.ProductVersion + "\n" + "© Dmitriy Fedoriaka, 2015-2016");
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You can get manual on the page of the project: http://fedimser.github.io/aldyparen.html");
        }

       

        private void panelAnnotationColor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = gridRight.annotationColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                gridRight.annotationColor = cd.Color;
                panelAnnotationColor.BackColor = cd.Color;
                if (gridRight.annotate)
                {
                    refreshRightPicture();
                }
            }
        }


        #region "Projects"
        SaveFileDialog sfdProject = new SaveFileDialog();
        OpenFileDialog ofdProject = new OpenFileDialog();
        String currentProjectFile = null;
        private bool currentProjectModified = false;


        private bool createNewProject()
        {
            if (!askForSavingCurrent()) return false;
            startWork();
            return true;
        }

        private bool askForSavingCurrent()
        {
            if (frameCount != 0 && currentProjectModified)
            {
                var res = MessageBox.Show("Do you want to save changes to current project?",
                    "", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Cancel) return false;
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!saveProject()) return false;
                }
                return true;
            }
            else return true;
        }

        void prepareDialogs()
        {
            sfdProject.Filter = "Aldyparen Projects|*.adp";
            sfdProject.InitialDirectory = Application.StartupPath;

            ofdProject.Filter = "Aldyparen Projects|*.adp";
            ofdProject.InitialDirectory = Application.StartupPath;

            sfdPhoto.InitialDirectory = Application.StartupPath;
        }

        private bool saveProject()
        {
            if (currentProjectFile == null)
            {
                return saveProjectAs();
            }
            else
            {
                Movie movie = new Movie(frames, frameCount);
                movie.save(currentProjectFile);
                currentProjectModified = false;
                return true;
            }
        }

        private bool saveProjectAs()
        {
            if (frameCount == 0)
            {
                MessageBox.Show("Project is empty");
                return false;
            }

            if(sfdProject.ShowDialog()==DialogResult.OK)
            {
                Movie movie = new Movie(frames, frameCount);
                movie.save(sfdProject.FileName);
                currentProjectFile = sfdProject.FileName;
                currentProjectModified = false;
                return true;
            }
            return false;
        }

        private bool openProject(string fileName)
        {
            timer1.Stop();
            bool OK = false;
            try
            {
                //labelCurFrameInfo.Text = "Loading project...";
                Movie movie = Movie.load(fileName);
                currentProjectFile = fileName;
                frameCount = movie.frames.Length;
                for (int i = 0; i < frameCount; ++i)
                {
                    frames[i] = movie.frames[i];
                }
                showFrame = frameCount - 1;
                refreshLeftPicture();
                curFrame = lastFrame().clone();
                refreshRightPicture();
                if (curFrame.genMode == Frame.GeneratingMode.Formula)
                {
                    textBoxFormula.Text = curFrame.param.genFunc.getText();
                }
                currentProjectModified = false;
                OK = true;
            }
            catch (Exception ex)
            {
                //
            }
            finally
            { 
                timer1.Start();
            }
            return OK;
        }

        private bool openProject()
        {
            if (!askForSavingCurrent()) return false;
            if (ofdProject.ShowDialog() == DialogResult.OK)
            {
                return openProject(ofdProject.FileName);
            }
            return false;
        }
        #endregion

#region "Menu"
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            makePhotoClick();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            makeVideoClick();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNewProject();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openProject();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveProject();
        }

        private void сохранитькакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveProjectAs();
        }


        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!askForSavingCurrent())
            {
                e.Cancel = true;
                return;
            }
            Environment.Exit(0);
        }


        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
            gridRight.useGrid = gridToolStripMenuItem.Checked;
            gridRight.useTicks = ticksToolStripMenuItem.Checked;
            gridRight.useAxes = axesToolStripMenuItem.Checked;
            gridRight.annotate = annotationToolStripMenuItem.Checked;
            refreshRightPicture();
        }
         

#endregion
 
        
    







    }
}
