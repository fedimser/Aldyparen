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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
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
            this.numericUpDownVideoHeight = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownVideoWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonBMP = new System.Windows.Forms.RadioButton();
            this.radioButtonJPEG = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxPhotoLabel = new System.Windows.Forms.TextBox();
            this.checkBoxPhotoLabel = new System.Windows.Forms.CheckBox();
            this.checkBoxPhotoSelectDestnation = new System.Windows.Forms.CheckBox();
            this.numericUpDownPhotoWidth = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownPhotoHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoWidth)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPhotoWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPhotoHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(266, 181);
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
            this.groupBox2.Location = new System.Drawing.Point(236, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 163);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video settings";
            // 
            // numericUpDownVideoHeight
            // 
            this.numericUpDownVideoHeight.Location = new System.Drawing.Point(70, 100);
            this.numericUpDownVideoHeight.Maximum = new decimal(new int[] {
            3000,
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
            // numericUpDownVideoWidth
            // 
            this.numericUpDownVideoWidth.Location = new System.Drawing.Point(70, 68);
            this.numericUpDownVideoWidth.Maximum = new decimal(new int[] {
            3000,
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonBMP);
            this.groupBox3.Controls.Add(this.radioButtonJPEG);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textBoxPhotoLabel);
            this.groupBox3.Controls.Add(this.checkBoxPhotoLabel);
            this.groupBox3.Controls.Add(this.checkBoxPhotoSelectDestnation);
            this.groupBox3.Controls.Add(this.numericUpDownPhotoWidth);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numericUpDownPhotoHeight);
            this.groupBox3.Location = new System.Drawing.Point(405, 14);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(247, 161);
            this.groupBox3.TabIndex = 52;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Photo settings";
            // 
            // radioButtonBMP
            // 
            this.radioButtonBMP.AutoSize = true;
            this.radioButtonBMP.Location = new System.Drawing.Point(152, 113);
            this.radioButtonBMP.Name = "radioButtonBMP";
            this.radioButtonBMP.Size = new System.Drawing.Size(68, 24);
            this.radioButtonBMP.TabIndex = 49;
            this.radioButtonBMP.TabStop = true;
            this.radioButtonBMP.Text = "BMP";
            this.radioButtonBMP.UseVisualStyleBackColor = true;
            // 
            // radioButtonJPEG
            // 
            this.radioButtonJPEG.AutoSize = true;
            this.radioButtonJPEG.Location = new System.Drawing.Point(77, 113);
            this.radioButtonJPEG.Name = "radioButtonJPEG";
            this.radioButtonJPEG.Size = new System.Drawing.Size(65, 24);
            this.radioButtonJPEG.TabIndex = 48;
            this.radioButtonJPEG.TabStop = true;
            this.radioButtonJPEG.Text = "JPG";
            this.radioButtonJPEG.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 20);
            this.label10.TabIndex = 47;
            this.label10.Text = "Format";
            // 
            // textBoxPhotoLabel
            // 
            this.textBoxPhotoLabel.Enabled = false;
            this.textBoxPhotoLabel.Location = new System.Drawing.Point(72, 78);
            this.textBoxPhotoLabel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxPhotoLabel.Name = "textBoxPhotoLabel";
            this.textBoxPhotoLabel.Size = new System.Drawing.Size(145, 26);
            this.textBoxPhotoLabel.TabIndex = 45;
            // 
            // checkBoxPhotoLabel
            // 
            this.checkBoxPhotoLabel.AutoSize = true;
            this.checkBoxPhotoLabel.Location = new System.Drawing.Point(8, 78);
            this.checkBoxPhotoLabel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxPhotoLabel.Name = "checkBoxPhotoLabel";
            this.checkBoxPhotoLabel.Size = new System.Drawing.Size(68, 24);
            this.checkBoxPhotoLabel.TabIndex = 44;
            this.checkBoxPhotoLabel.Text = "Title:";
            this.checkBoxPhotoLabel.UseVisualStyleBackColor = true;
            // 
            // checkBoxPhotoSelectDestnation
            // 
            this.checkBoxPhotoSelectDestnation.AutoSize = true;
            this.checkBoxPhotoSelectDestnation.Location = new System.Drawing.Point(8, 53);
            this.checkBoxPhotoSelectDestnation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxPhotoSelectDestnation.Name = "checkBoxPhotoSelectDestnation";
            this.checkBoxPhotoSelectDestnation.Size = new System.Drawing.Size(162, 24);
            this.checkBoxPhotoSelectDestnation.TabIndex = 43;
            this.checkBoxPhotoSelectDestnation.Text = "Select destination";
            this.checkBoxPhotoSelectDestnation.UseVisualStyleBackColor = true;
            // 
            // numericUpDownPhotoWidth
            // 
            this.numericUpDownPhotoWidth.Location = new System.Drawing.Point(55, 22);
            this.numericUpDownPhotoWidth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownPhotoWidth.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownPhotoWidth.Name = "numericUpDownPhotoWidth";
            this.numericUpDownPhotoWidth.Size = new System.Drawing.Size(84, 26);
            this.numericUpDownPhotoWidth.TabIndex = 40;
            this.numericUpDownPhotoWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 24);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 20);
            this.label9.TabIndex = 39;
            this.label9.Text = "Size";
            // 
            // numericUpDownPhotoHeight
            // 
            this.numericUpDownPhotoHeight.Location = new System.Drawing.Point(147, 22);
            this.numericUpDownPhotoHeight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownPhotoHeight.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownPhotoHeight.Name = "numericUpDownPhotoHeight";
            this.numericUpDownPhotoHeight.Size = new System.Drawing.Size(84, 26);
            this.numericUpDownPhotoHeight.TabIndex = 42;
            this.numericUpDownPhotoHeight.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 237);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSettings";
            this.Text = "Aldyparen Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVideoWidth)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPhotoWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPhotoHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxPhotoLabel;
        private System.Windows.Forms.CheckBox checkBoxPhotoLabel;
        private System.Windows.Forms.CheckBox checkBoxPhotoSelectDestnation;
        private System.Windows.Forms.NumericUpDown numericUpDownPhotoWidth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownPhotoHeight;
        private System.Windows.Forms.RadioButton radioButtonBMP;
        private System.Windows.Forms.RadioButton radioButtonJPEG;
        private System.Windows.Forms.Label label10;
    }
}