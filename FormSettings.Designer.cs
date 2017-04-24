namespace Aldyparen
{
    partial class FormSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxRealTime = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxSteps = new System.Windows.Forms.TextBox();
            this.textBoxInfty = new System.Windows.Forms.TextBox();
            this.textBoxInitY = new System.Windows.Forms.TextBox();
            this.textBoxInitX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownVideoWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownVideoHeight = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxRealTime
            // 
            this.checkBoxRealTime.AutoSize = true;
            this.checkBoxRealTime.Location = new System.Drawing.Point(12, 181);
            this.checkBoxRealTime.Name = "checkBoxRealTime";
            this.checkBoxRealTime.Size = new System.Drawing.Size(165, 24);
            this.checkBoxRealTime.TabIndex = 0;
            this.checkBoxRealTime.Text = "Real-time applying";
            this.checkBoxRealTime.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(214, 232);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 48);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "init.X";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxSteps);
            this.groupBox1.Controls.Add(this.textBoxInfty);
            this.groupBox1.Controls.Add(this.textBoxInitY);
            this.groupBox1.Controls.Add(this.textBoxInitX);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 163);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced parameters";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(146, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Careful!";
            // 
            // textBoxSteps
            // 
            this.textBoxSteps.Location = new System.Drawing.Point(56, 123);
            this.textBoxSteps.Name = "textBoxSteps";
            this.textBoxSteps.Size = new System.Drawing.Size(84, 26);
            this.textBoxSteps.TabIndex = 9;
            // 
            // textBoxInfty
            // 
            this.textBoxInfty.Location = new System.Drawing.Point(56, 91);
            this.textBoxInfty.Name = "textBoxInfty";
            this.textBoxInfty.Size = new System.Drawing.Size(84, 26);
            this.textBoxInfty.TabIndex = 8;
            // 
            // textBoxInitY
            // 
            this.textBoxInitY.Location = new System.Drawing.Point(56, 59);
            this.textBoxInitY.Name = "textBoxInitY";
            this.textBoxInitY.Size = new System.Drawing.Size(84, 26);
            this.textBoxInitY.TabIndex = 7;
            // 
            // textBoxInitX
            // 
            this.textBoxInitX.Location = new System.Drawing.Point(56, 27);
            this.textBoxInitX.Name = "textBoxInitX";
            this.textBoxInitX.Size = new System.Drawing.Size(84, 26);
            this.textBoxInitX.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "init.Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "steps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "infty";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "FPS";
            // 
            // numericUpDownFPS
            // 
            this.numericUpDownFPS.Location = new System.Drawing.Point(70, 25);
            this.numericUpDownFPS.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownFPS.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.Size = new System.Drawing.Size(66, 26);
            this.numericUpDownFPS.TabIndex = 5;
            this.numericUpDownFPS.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownVideoHeight);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDownVideoWidth);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownFPS);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(252, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 163);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video";
            // 
            // numericUpDownVideoWidth
            // 
            this.numericUpDownVideoWidth.Location = new System.Drawing.Point(70, 68);
            this.numericUpDownVideoWidth.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownVideoWidth.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownVideoWidth.Name = "numericUpDownVideoWidth";
            this.numericUpDownVideoWidth.Size = new System.Drawing.Size(66, 26);
            this.numericUpDownVideoWidth.TabIndex = 7;
            this.numericUpDownVideoWidth.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Width";
            // 
            // numericUpDownVideoHeight
            // 
            this.numericUpDownVideoHeight.Location = new System.Drawing.Point(70, 100);
            this.numericUpDownVideoHeight.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownVideoHeight.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownVideoHeight.Name = "numericUpDownVideoHeight";
            this.numericUpDownVideoHeight.Size = new System.Drawing.Size(66, 26);
            this.numericUpDownVideoHeight.TabIndex = 9;
            this.numericUpDownVideoHeight.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Height";
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 295);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBoxRealTime);
            this.Name = "FormSettings";
            this.Text = "FormSettings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxRealTime;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSteps;
        private System.Windows.Forms.TextBox textBoxInfty;
        private System.Windows.Forms.TextBox textBoxInitY;
        private System.Windows.Forms.TextBox textBoxInitX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericUpDownVideoHeight;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownVideoWidth;
        private System.Windows.Forms.Label label7;
    }
}