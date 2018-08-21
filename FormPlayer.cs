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
    public partial class FormPlayer : Form
    {
        private bool needClose = false;

        public FormPlayer()
        {
            InitializeComponent();
        }

        public void setImage(Bitmap image) { 
            pictureBox1.Image = image;
        }

        public void safeClose()
        {
            needClose = true;
        } 

        private void FormPlayer_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (needClose)
            {
                this.Close();
            }
        }
         
    }
}
