using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aldyparen
{
    public partial class FormCudaSettings : Form
    {
        public FormCudaSettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
         
        private void FormCudaSettings_Load(object sender, EventArgs e)
        {
            if (CudaPainter.isCudaAvailable())
            {
                if (CudaPainter.corrupted)
                {
                    label1.Text = "There was error in CUDA. Restart application.";
                    labelProperties.Text = CudaPainter.errorMessage;
                    groupBox1.Enabled = false;
                }
                else
                {
                    label1.Text = "CUDA is available!";
                    groupBox1.Enabled = true;

                    if (CudaPainter.enabled)
                    {
                        checkBox1.Checked = true;
                        labelProperties.Text = CudaPainter.getPropertiesString();
                    }
                    else
                    {
                        checkBox1.Checked = false;
                        labelProperties.Text = "";
                    }
                }
                
            }
            else
            {
                label1.Text = "CUDA is unavailable. You must have Nvidia GPU and CUDA driver installed.";
                groupBox1.Enabled = false;
            }

            

            checkBox2.Checked = CudaPainter.rowScan; 
        }
 

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && !CudaPainter.enabled)
            {
                CudaPainter.cudaEnable();
                if (!CudaPainter.enabled)
                {
                    labelProperties.Text = "Error while enabling CUDA:\n" + CudaPainter.errorMessage;

                }
                else
                {
                    labelProperties.Text = CudaPainter.getPropertiesString();
                }
            }
            else
            {
                CudaPainter.cudaDisable();
                labelProperties.Text = "";
            }

            checkBox1.Checked = CudaPainter.enabled;
             
        }

        private void FormCudaSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            CudaPainter.rowScan = checkBox2.Checked;
        }
         
    }
}
