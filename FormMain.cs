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
        public Frame workFrame;
        public bool workFrameChanged = false;
        Movie movie;

        Grid grid = new Grid(); 

        public FormMain()
        {
            InitializeComponent();
        }

        string applicationTitle = "Algebraic Dynamic Parametric Renderer";
              
        public int FPS = 16;
        int halfWidth = 160;
        int halfHeight = 90;   




        //Settings
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


            int AnimLength = Convert.ToInt32(numericUpDown1.Value);

            Frame f = movie[-1].clone();
            int S = (int)numericUpDownDiv.Value;


            for (int i = 1; i <= AnimLength; i++)
            {
                f = f.getMove(workFrame, 1.0 / (AnimLength - i + 1));
                f.moveColors(S);
                movie.appendFrame(f);
            }
             
            showFrame = movie.frameCount() - 1;
            refreshLeftPicture();
            currentProjectModified = true;
            refreshWindow();
        } 
       

        void addFrame(Frame f)
        {
            movie.appendFrame(f);

            showFrame = movie.frameCount() - 1;
            refreshLeftPicture();
            currentProjectModified = true;
            refreshWindow();
        }

        void removeFrames(int start, int end)
        {
            movie.removeFrames(start, end);
            if (showFrame >= movie.frameCount()) showFrame = movie.frameCount() - 1;

            refreshLeftPicture();
            currentProjectModified = true;
            refreshWindow();
        }
         

        bool createNewProject()
        {
            if (movie != null && !askForSavingCurrent()) return false;

            movie = new Movie();
                       
            workFrameChanged = true;
            currentProjectModified = false;
            currentProjectFile = null;
            
            
            pictureBox1.Image = null;

            refreshWindow();
            return true;
        }
         
         
 

      


        #region "Animation Settings"

        PointF getMousePosition()  
        {
            Point z = PointToClient(Cursor.Position);

            if (pictureBox2.SizeMode == PictureBoxSizeMode.Normal)
            {
                return new PointF(z.X - pictureBox2.Left, z.Y - pictureBox2.Top);
            } else {
                Size workFrameSize = workFrame.getSize();
                float ZoomScale = Math.Min(1.0F*pictureBox2.Width / workFrameSize.Width, 1.0F*pictureBox2.Height / workFrameSize.Height);

                float realX = z.X - (pictureBox2.Left + 0.5F * pictureBox2.Width);
                float realY = z.Y - (pictureBox2.Top + 0.5F * pictureBox2.Height);

                return new PointF(realX/ZoomScale + 0.5F*workFrameSize.Width, realY/ZoomScale+0.5F*workFrameSize.Height);
            }
        }

 
           
        void resetTransform()
        {
            try
            {
                workFrame.restoreTransform(movie[-1]);
            }
            catch (Exception e)
            {
                workFrame.ctr = 0;
                workFrame.scale = 1;
            }
            workFrameChanged = true;
        }

        void resetParams()
        {
            workFrame.param = movie[-1].param.clone();
            workFrameChanged = true;
        }



        void refreshRightPicture()
        { 
             pictureBox2.Image = workFrame.getFrame(halfWidth, halfHeight, grid); 
             workFrameChanged = false;
        }

         

        private void pictureBox2_MouseWheel(object sender,MouseEventArgs e)
        {
            double delta = -Math.Sign(e.Delta);
            Complex pole = workFrame.pictureToMath(getMousePosition());
            
            
            if ((Control.ModifierKeys & Keys.Shift) != 0) delta *= 20;


            if ((Control.ModifierKeys & Keys.Control) != 0)
            {
                delta = 2 * 0.017453292 * delta;
                workFrame.rotation += delta; 
                workFrame.ctr = pole + (workFrame.ctr-pole)*Complex.Exp(Complex.ImaginaryOne*delta);
                workFrameChanged = true;
                return;
            }

            double k=Math.Pow(1.05,delta);



            workFrame.ctr = pole - (pole - workFrame.ctr) * k;
            workFrame.scale = workFrame.scale * k;

            workFrameChanged = true;
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
            workFrame.ctr -= (2.0 / pictureBox2.Width) * Complex.Exp(Complex.ImaginaryOne*workFrame.rotation)*workFrame.scale * dx;

            last_tracling_cursor_position = p;
            workFrameChanged = true;
        }


        #endregion


        #region "Video"

        SaveFileDialog sfdVideo = new SaveFileDialog();

        public int videoWidth = 720;
        public int videoHeight = 540;

        private void makeVideoClick()
        {
            if (isNowVideoSaving)
            {
                MessageBox.Show("One video rendering in progress. Please wait.");
                return;
            }

            if (sfdVideo.ShowDialog() != DialogResult.OK) return;
           

            var p = new VideoParameters();
            p.halfWidth = videoWidth / 2;
            p.halfHeight = videoHeight / 2;
            p.path = sfdVideo.FileName;
            p.FPS = this.FPS;
            p.movie = movie;

            Thread t = new Thread(makeVideo);
            t.Start(p);         
        }


        public class VideoParameters
        {
            public Movie movie; 
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
            wr.Open(param.path, 2 * param.halfWidth, 2 * param.halfHeight);

            int frameCount = param.movie.frameCount();
            for (int i = 0; i < frameCount; i++)
            {
                wr.AddFrame(param.movie[i].getFrame(param.halfWidth, param.halfHeight,grid));
                Console.WriteLine("Frame {0} out of {1} is done!", i + 1, frameCount);
                statusFrames = String.Format("{0}/{1}", i + 1, frameCount);
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
            values = new Complex[workFrame.param.maxUsedColors];
            values_cnt = 0;
        }

        private int get_value_number(Complex x)
        {
            for (int i = 0; i < values_cnt; i++) if (Math.Abs(x.Real - values[i].Real) + Math.Abs(x.Imaginary - values[i].Imaginary) < 1e-8) return i;

            if (values_cnt == workFrame.param.maxUsedColors) return values_cnt - 1;

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
            if (movie.frameCount()==0) return "No frames";
             
            Frame f = movie[showFrame];
            return String.Format("Frame {0}/{1}\n Time {2:0.##}\n"+ 
                "scale {3:e2}\n" + "center ({4:e2};{5:e2})\n{6}",
                showFrame+1, movie.frameCount(), showFrame / FPS, f.scale, f.ctr.Real, f.ctr.Imaginary,
                f.getAnnotation()
            );
        }

        
        void refreshLeftPicture()
        {
            if (movie.frameCount() == 0)
            {
                pictureBox1.Image = null;
                hScrollBarFrames.Hide();
            }
            else
            {
                hScrollBarFrames.Show();
                hScrollBarFrames.Maximum = movie.frameCount() - 1;
                hScrollBarFrames.Value = showFrame;
                pictureBox1.Image = movie[showFrame].getFrame(halfWidth, halfHeight, grid);
            }
        }



        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            showFrame = hScrollBarFrames.Value;
            refreshLeftPicture();
            refreshWindow();
        }
         

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
             

            if(isCursorOverRightArea) 
            {
                Complex pos = workFrame.pictureToMath(getMousePosition());
                labelCurFrameInfo.Text = String.Format("({0:0.##};{1:0.##}) scale={2:0.##}", pos.Real, pos.Imaginary, workFrame.scale);
                 
            }

            if (is_now_tracking) ApplyTracking();

            if (workFrameChanged)
            {
                refreshRightPicture();
            }


            labelThreads.Text = "Threads: " + photoThreadsCounter.ToString();
            labelFrames.Text = statusFrames;
            labelStopwatch.Text = statusTimeInfo;
            checkFormulaChanged();
        }

        void refreshWindow()
        {
            int frameCount = movie.frameCount();

            labelFrameInfo.Text = getShowFrameInfo();
            hScrollBarFrames.Enabled = frameCount != 0;
            tabPageAnimation.Enabled = frameCount != 0;
            removeFrameToolStripMenuItem.Enabled = frameCount != 0;
            cloneToWorkingAreaToolStripMenuItem.Enabled = frameCount != 0;
            makeAnimationToolStripMenuItem.Enabled = frameCount != 0;
            remove1FrameToolStripMenuItem.Enabled = (frameCount > 1);
            remove10ToolStripMenuItem.Enabled = (frameCount > 10);
            replaceToolStripMenuItem.Enabled = (frameCount != 0);
            clearToolStripMenuItem.Enabled = (frameCount != 0);


            this.Text = applicationTitle + " - " +
                ((currentProjectFile==null)? "New project": currentProjectFile)
                + (currentProjectModified ? "*" : "");

            panel1.BackColor = workFrame.colorMap[(int)numericUpDown10.Value];
            panelAnnotationColor.BackColor = grid.annotationColor;

            

            if (CudaPainter.enabled)
            {
                if (CudaPainter.canRender(workFrame))
                    LabelCuda.Text = "CUDA Enabled";
                else
                    LabelCuda.Text = "CUDA Enabled but cannot be used";
            }
            else
            {
                LabelCuda.Text = "CUDA Disabled";
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            workFrame = new Frame();

            workFrame.colorMap = new Color[Frame.MAX_COLORS];
            for (int i = 0; i < Frame.MAX_COLORS; i++) workFrame.colorMap[i] = Color.White;

             
            workFrame.genMode = Frame.GeneratingMode.Formula;
            workFrame.param = new FrameParams(0, 100);
            workFrame.param.genFunc = new Function(new String[2] { "c", "z" });
            workFrame.param.genFunc.setText(textBoxFormula.Text);
            workFrame.param.genInfty = 2;
            workFrame.param.genSteps = 20;
            workFrame.param.genInit = new Complex(0, 0);

            setMapColor(0, Color.Black);
            setMapColor(1, Color.White);


            reset_values_list();

             
            ResetColorCounters(); 
            setColors(workFrame);
            prepareDialogs();
            
            
            if (arguments != null && arguments.Length == 1)
            {
                string fileName = arguments[0];
                arguments = null;
                if (!openProject(fileName))
                {
                    MessageBox.Show("Coluldn't load project");
                }
            }
            else
            {
                createNewProject();
            }

            refreshFormula();
            refreshLeftPicture();

            timer1.Start();
        }
         


         


        private void setColors(Frame f)
        {
            for (int i = 0; i < f.param.maxUsedColors; i++)
            {
                f.colorMap[i] = workFrame.colorMap[i];
            }

            workFrameChanged = true;
        }
         



        #region "Color map"
        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            panel1.BackColor = workFrame.colorMap[(int)numericUpDown10.Value];
        } 
        
        private void panel1_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = panel1.BackColor;
            if (cd.ShowDialog() != DialogResult.OK) return;

            setMapColor((int)numericUpDown10.Value, cd.Color);

           // frames[frameCount - 1].colorMap[(int)numericUpDown10.Value] = cd.Color;
           // pictureBox1.Image = frames[frameCount - 1].getFrame(halfWidth, halfHeight);
            workFrameChanged = true;
        }

        private void setMapColor(int index, Color color)
        {
            workFrame.colorMap[index] = color;
            if (index < workFrame.param.maxUsedColors)
            {
                workFrame.colorMap[index] = color;
                workFrameChanged = true;
            }

            if ((int)numericUpDown10.Value == index)
            {
                panel1.BackColor = color;
            }
       }


        
        private void button4_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < workFrame.param.maxUsedColors; i++)
            {
                setMapColor(i,   Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255)));
                setColors(movie[-1]);
            }
            workFrameChanged = true;
 
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

            int R1 = workFrame.colorMap[c1].R;
            int G1 = workFrame.colorMap[c1].G;
            int B1 = workFrame.colorMap[c1].B;

            int R2 = workFrame.colorMap[c2].R;
            int G2 = workFrame.colorMap[c2].G;
            int B2 = workFrame.colorMap[c2].B;


            for (int i = 0; i <= c2-c1; i++)
            {
              setMapColor(c1+i, Color.FromArgb(255, R1 + i * (R2 - R1) / (c2 - c1), G1 + i * (G2 - G1) / (c2 - c1), B1 + i * (B2 - B1) / (c2 - c1)));
            }  
        }

        void ResetColorCounters()
        {

            numericUpDown10.Maximum = workFrame.param.maxUsedColors- 1;
            numericUpDown11.Maximum = workFrame.param.maxUsedColors - 1;
            numericUpDown12.Maximum = workFrame.param.maxUsedColors - 1;

            numericUpDown11.Value=0;
            numericUpDown12.Value = workFrame.param.maxUsedColors - 1;


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
            Bitmap bmp = param._frame.getFrame(param._W, param._H,grid);

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

            Frame fr = workFrame.clone();
             

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
            workFrame.scale = _scl;
            workFrame.ctr = new Complex(_ctr1, _ctr2);
            workFrameChanged = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            resetParams();
        }


 

 

        private void cUDASettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormCudaSettings();
            f.ShowDialog();
            refreshWindow();
        }


        #region "Formula"

        private bool formulaChanged = false;

        private void paintText(RichTextBox tb, Color clr, int start, int end)
        { 
            int cp = tb.SelectionStart;
            tb.Select(start, end - start + 1);
            tb.SelectionColor = clr;
            tb.DeselectAll();
            tb.SelectionStart = cp;
        }


        private void paintText(RichTextBox tb, Color clr)
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
                workFrame.param.genFunc = f;
                workFrameChanged = true;
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
            formulaChanged = true;
        }

        private void checkFormulaChanged()
        {
            if (!formulaChanged) return;

            paintText(textBoxFormula, Color.Black);
            refreshFormula();

            Function f = new Function();
            f.setText(textBoxFormula.Text);
            try
            {
                this.Text = f.eval().ToString();
            }
            catch (Exception ex)
            {
                //
            }
            formulaChanged = false;
        }

        #endregion




        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetTransform();
        }

  
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aldyparen " + Application.ProductVersion + "\n" + "© Dmitriy Fedoriaka, 2015-2017");
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You can get manual on the page of the project: http://fedimser.github.io/aldyparen.html");
        }

       

        private void panelAnnotationColor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = grid.annotationColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                grid.annotationColor = cd.Color;
                panelAnnotationColor.BackColor = cd.Color;
                if (grid.annotate)
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

         

        private bool askForSavingCurrent()
        {
            if (movie.frameCount() != 0 && currentProjectModified)
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
            
            
            if (!Directory.Exists("Output\\Video"))
            {
                Directory.CreateDirectory("Output\\Video");
            }
            sfdVideo.InitialDirectory = Application.StartupPath + "\\Output\\Video";
            sfdVideo.Filter = "AVI video|*.avi";
        }

        private bool saveProject()
        {
            if (currentProjectFile == null)
            {
                return saveProjectAs();
            }
            else
            {
                movie.save(currentProjectFile);
                currentProjectModified = false;
                refreshWindow();
                return true;
            }
        }

        private bool saveProjectAs()
        {
            if (movie.frameCount() == 0)
            {
                MessageBox.Show("Project is empty");
                return false;
            }

            if(sfdProject.ShowDialog()==DialogResult.OK)
            {
                movie.save(sfdProject.FileName);
                currentProjectFile = sfdProject.FileName;
                currentProjectModified = false;
                refreshWindow();
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
                movie = Movie.load(fileName);
                currentProjectFile = fileName;
                showFrame = movie.frameCount() - 1;
                refreshLeftPicture();
                workFrame = movie[-1].clone();
                refreshFormula();
                refreshRightPicture();
                if (workFrame.genMode == Frame.GeneratingMode.Formula)
                {
                    textBoxFormula.Text = workFrame.param.genFunc.getText();
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
            refreshWindow();
            return OK;
        }

        private bool openProject()
        {
            if (!askForSavingCurrent()) return false;
            if (ofdProject.ShowDialog() == DialogResult.OK)
            {
                if (openProject(ofdProject.FileName))
                {
                    return true;
                }
                else 
                {
                    MessageBox.Show("Couldn't open file " + ofdProject.FileName);
                    return false;
                }
            }
            return false;
        }
        #endregion

#region "Menu"
         
        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = new FormSettings();
            f.formMain = this;
            f.ShowDialog();
            refreshWindow();
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
            movie.removeFrames(0, movie.frameCount());
        }

        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addFrame(workFrame.clone());
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeFrames(movie.frameCount() - 1, movie.frameCount());
            addFrame(workFrame.clone());
        }

        private void remove1FrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeFrames(movie.frameCount() - 1, movie.frameCount());
        }

        private void remove10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeFrames(movie.frameCount() - 10, movie.frameCount());
        }

        private void makeAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makeAnimation();
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setArea();
        }


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
            grid.useGrid = gridToolStripMenuItem.Checked;
            grid.useTicks = ticksToolStripMenuItem.Checked;
            grid.useAxes = axesToolStripMenuItem.Checked;
            grid.annotate = annotationToolStripMenuItem.Checked;
            refreshRightPicture();
        }


        private void removeFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeFrames(showFrame, showFrame + 1);
        }


        private void cloneToWorkingAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            workFrame = movie[showFrame];
            refreshRightPicture();
        }


#endregion

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (movie.frameCount() == 0) {
                MessageBox.Show("Nothing to play.");
                return;
            }
            PreviewPlayer player = new PreviewPlayer();
            player.setDownScale((double)numericUpDownDownscale.Value);
            player.setShowFps(checkBoxShowFps.Checked);
            player.play(movie);
        }
         
    }
}
