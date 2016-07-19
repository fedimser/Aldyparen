using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            checkBoxRealTime.Checked = formMain.realTimeApplying;
            textBoxInfty.Text = formMain.curFrame.param.genInfty.ToString();
            textBoxSteps.Text = formMain.curFrame.param.genSteps.ToString();
            textBoxInitX.Text = formMain.curFrame.param.genInit.Real.ToString();
            textBoxInitY.Text = formMain.curFrame.param.genInit.Imaginary.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                formMain.realTimeApplying = checkBoxRealTime.Checked;
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


                formMain.curFrame.param.genInit = v1;
                formMain.curFrame.param.genInfty = v2;
                formMain.curFrame.param.genSteps = v3;


                formMain.curFrameChanged = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving parameters.\n"+ ex.Message);
            }
            this.Close();
        }
    }
}
