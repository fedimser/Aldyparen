using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Numerics;

namespace Aldyparen
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        public FormMain formMain;

        private void FormSettings_Load(object sender, EventArgs e)
        { 
            textBoxInfty.Text = formMain.workFrame.param.genInfty.ToString();
            textBoxSteps.Text = formMain.workFrame.param.genSteps.ToString();
            textBoxInitX.Text = formMain.workFrame.param.genInit.Real.ToString();
            textBoxInitY.Text = formMain.workFrame.param.genInit.Imaginary.ToString();


            // Video Settings
            numericUpDownFPS.Value = formMain.FPS;
            numericUpDownVideoHeight.Value = formMain.videoHeight;
            numericUpDownVideoWidth.Value = formMain.videoWidth;

            //Photo settings
            numericUpDownPhotoHeight.Value = formMain.photoHeight;
            numericUpDownPhotoWidth.Value = formMain.photoWidth;
            checkBoxPhotoLabel.Checked = (formMain.photoLabel != null);
            textBoxPhotoLabel.Text = formMain.photoLabel;
            checkBoxPhotoSelectDestnation.Checked = formMain.photoSelectDestination;
            radioButtonJPEG.Checked = (formMain.photoImageFormat == ImageFormat.Jpeg);
            radioButtonBMP.Checked = (formMain.photoImageFormat == ImageFormat.Bmp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Save Photo Settings
                formMain.photoHeight = (int)numericUpDownPhotoHeight.Value;
                formMain.photoWidth = (int)numericUpDownPhotoWidth.Value;
                if (checkBoxPhotoLabel.Checked)
                    formMain.photoLabel = textBoxPhotoLabel.Text;
                else
                    formMain.photoLabel = null;
                formMain.photoSelectDestination = checkBoxPhotoSelectDestnation.Checked;
                if (radioButtonBMP.Checked) formMain.photoImageFormat = ImageFormat.Bmp;
                if (radioButtonJPEG.Checked) formMain.photoImageFormat = ImageFormat.Jpeg;

                //Save Video Settings
                formMain.FPS = (int)numericUpDownFPS.Value;
                formMain.videoHeight = (int)numericUpDownVideoHeight.Value;
                formMain.videoWidth = (int)numericUpDownVideoWidth.Value;
                 
                


                var v1  = new Complex(Convert.ToDouble(textBoxInitX.Text), Convert.ToDouble(textBoxInitY.Text));
                var v2 = Convert.ToDouble(textBoxInfty.Text);
                var v3 = Convert.ToInt32(textBoxSteps.Text);

                if (v3 < 1 || v3> 100)
                {
                    throw new ArgumentOutOfRangeException("steps must be between 1 and 100");
                }

                if (v2<0)
                {
                    throw new ArgumentOutOfRangeException("infty must be positve.");
                }


                formMain.workFrame.param.genInit = v1;
                formMain.workFrame.param.genInfty = v2;
                formMain.workFrame.param.genSteps = v3;


                formMain.workFrameChanged = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving parameters.\n"+ ex.Message);
            }
            this.Close();
        }
    }
}
