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

namespace CyberLyzer
{
    public partial class Form1 : Form
    {

        CLFrame curFrame;

        CLFrame[] frames;
        Color[] colors;
        int frame_counter=0; 


        public Form1()
        {
            InitializeComponent();
        }
             
        int W,H;
        int W1 = 80;
        int H1 = 60;
        int FPS;
        int AnimLength;
        double ScreenScale;


        //Make animation
        private void button2_Click(object sender, EventArgs e)
        {
            CLFrame f = lastFrame();
            int S = (int)numericUpDownDiv.Value;


            for (int i = 1; i <= AnimLength; i++)
            {
                f =  f.getMove(curFrame, 1.0 / ( AnimLength-i+1));
                //f.moveColors(S);

                frames[frame_counter] = f;
                frame_counter++;
            }

            pictureBox1.Image = lastFrame().getFrame(W1, H1);

            curFrame = lastFrame().clone();
            curFrameChanged = true;
            
        }


        private CLFrame lastFrame()
        {
            return frames[frame_counter - 1];
        }

       

        void addFrame(CLFrame f)
        {
            frames[frame_counter] = f;
            frame_counter++;

            showLastFrame();
        }



        void initWork()
        {
            curFrame = new CLFrame();

            colors = new Color[1000];
            for(int i=0;i<1000;i++)colors[i]=Color.White ;
        }

        void startWork()
        {
            ScreenScale = Math.Max(2.0 * W / pictureBox1.Width, 2.0 * H / pictureBox2.Height);


            frames = new CLFrame[10000];
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

 
          

        private void button3_Click(object sender, EventArgs e)
        {
            resetTransform();
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

        bool curFrameChanged = false;
         

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


        private void button1_Click(object sender, EventArgs e)
        {
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
            p.frames = new CLFrame[frame_counter];
            
            for (int i = 0; i < frame_counter; i++)
            {
                p.frames[i] = frames[i];
            }

            Thread t = new Thread(makeVideo);
            t.Start(p);         
        }


        public class VideoParameters
        {
            public CLFrame[] frames;
            public int frameCount;
            public int halfWidth;
            public int halfHeight;
            public string path;
            public int FPS;  

        }

        private void makeVideo(object _param)
        {
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
            }

            wr.Close();

            photoThreadsCounter--;
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

            LabelThreads.Text = "Threads: " + photoThreadsCounter.ToString();

            button5.Enabled = (frame_counter > 1);
         }

        private void Form1_Load(object sender, EventArgs e)
        {
            initParamsMenu(); 
            
            initWork();
            comboBox1.SelectedIndex = 0;
            startWork();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            startWork();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (frame_counter <=1  ) return;
            frame_counter--;

            showLastFrame();
        }

        private void showLastFrame()
        {
            pictureBox1.Image = lastFrame().getFrame(W1, H1); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            curFrame = new CLFrame();

            if (comboBox1.SelectedIndex == 0)
            { 
                curFrame.dlgt = new CLFrame.Get_Pixel( FractalPainters.MandelbrotPainter);

                curFrame.param = new CLFrameParams(0, 2);
                
                setMapColor(0, Color.Black);
                setMapColor(1,  Color.White);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.CubeRootPainter); 
                curFrame.param = new CLFrameParams(0,3);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.TetraRootPainter);
              
                curFrame.param = new CLFrameParams(0,40);
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.DebugPainter);
                curFrame.param = new CLFrameParams(4, 100);
            }
            if (comboBox1.SelectedIndex == 4)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.Mandelbrot2Painter);
                curFrame.param = new CLFrameParams(0, 256);
                colors[255] = Color.Black;
                for (int i = 0; i < 255; i++)
                {
                     setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.MandelBrotCubePainter);
                curFrame.param = new CLFrameParams(0, 256);
                colors[255] = Color.Black;
                for (int i = 0; i < 255; i++)
                {
                    setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.GenericMandelbrotPainter);
                curFrame.param = new CLFrameParams(1, 256);
                
               setMapColor(curFrame.param.maxUsedColors - 1, Color.Blue);

                setMapColor(0,Color.Black);
                setMapColor(curFrame.param.maxUsedColors - 2, Color.Yellow);
                makeGradient(0, curFrame.param.maxUsedColors - 2);

            }
            else if (comboBox1.SelectedIndex == 7)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.GenericMandelbrotPainter);
                curFrame.param = new CLFrameParams(1, 20);
                setMapColor(curFrame.param.maxUsedColors - 1,Color.Blue);

                setMapColor(0, Color.Black);
                setMapColor(curFrame.param.maxUsedColors - 2, Color.Yellow);
                makeGradient(0, curFrame.param.maxUsedColors - 2);

            } 
            else if (comboBox1.SelectedIndex == 8)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.MandelBrotTetraPainter);
                curFrame.param = new CLFrameParams(0, 256);
                setMapColor(255, Color.Black);
                for (int i = 0; i < 255; i++)
                {
                 setMapColor(i, Color.FromArgb(255, i, i, 0));
                }
            }
            else if (comboBox1.SelectedIndex == 9)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.DzetaPainter);
                curFrame.param = new CLFrameParams(0, 50);
            }
             else if (comboBox1.SelectedIndex == 10)
            {
                curFrame.dlgt = new CLFrame.Get_Pixel(FractalPainters.PNL_Painter);
                curFrame.param = new CLFrameParams(7, 50);
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


        private void setColors(CLFrame f)
        {
            for (int i = 0; i < f.param.maxUsedColors; i++)
            {
                f.colorMap[i] = colors[i];
            }

            curFrameChanged = true;
        }


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
           public  CLFrame _frame;
           public int _W;
           public int _H;
           public string path;
           public string label;
           public ImageFormat format = ImageFormat.Jpeg;
             
        }

        private int photoThreadsCounter = 0;

        private void makePhoto(object _param)
        {
            photoThreadsCounter++;
            
            photoParameters param = (photoParameters)_param;
            Bitmap bmp = param._frame.getFrame(param._W, param._H);

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
        }

        private void button9_Click(object sender, EventArgs e)
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

            CLFrame fr = curFrame.clone();
            

            string label = "";
            if (checkBox3.Checked)
            {
                label = String.Format("Pos:({0:e5};{1:e5}).Width:{2:e5}");
            }

            pictureBox2.Image.Save(path);

            photoParameters param=new photoParameters();
            param._frame= fr;
            param._W=(int)numericUpDown4.Value;
            param._H= (int)numericUpDown3.Value;
            param.path = path;
            param.label=label;

            if (checkBoxBMP.Checked)
            {
                param.format = ImageFormat.Bmp;
            }

            Thread t = new Thread(makePhoto);
            t.Start(param);
             
             
        }

        #endregion

         
        //show another window
        private void button10_Click(object sender, EventArgs e)
        {
            var f2 = new Form2();


            CLFrame fr = curFrame.clone();

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

        private void button13_Click(object sender, EventArgs e)
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



        //append
        private void button8_Click(object sender, EventArgs e)
        {
            addFrame(curFrame.clone());
        }


        //replace    
        private void button11_Click_1(object sender, EventArgs e)
        {
            frame_counter--;
            addFrame(curFrame.clone());
        }
         










    }
}
