namespace FireFox
{
    partial class FormWinEffect
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            fadeInTimer = new System.Windows.Forms.Timer(components);
            confettiTimer = new System.Windows.Forms.Timer(components);
            pbFrame = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbFrame).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(138, 153);
            label1.Name = "label1";
            label1.Size = new Size(522, 140);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // fadeInTimer
            // 
            fadeInTimer.Interval = 30;
            fadeInTimer.Tick += fadeInTimer_Tick_1;
            // 
            // confettiTimer
            // 
            confettiTimer.Interval = 30;
            confettiTimer.Tick += confettiTimer_Tick_1;
            // 
            // pbFrame
            // 
            pbFrame.BackColor = Color.Transparent;
            pbFrame.BackgroundImageLayout = ImageLayout.None;
            pbFrame.Image = Properties.Resources.GoldenFrame;
            pbFrame.Location = new Point(2, -5);
            pbFrame.Name = "pbFrame";
            pbFrame.Size = new Size(795, 457);
            pbFrame.SizeMode = PictureBoxSizeMode.StretchImage;
            pbFrame.TabIndex = 1;
            pbFrame.TabStop = false;
            // 
            // FormWinEffect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(pbFrame);
            Name = "FormWinEffect";
            Text = "FormWinEffect";
            WindowState = FormWindowState.Maximized;
            Load += FormWinEffect_Load;
            ((System.ComponentModel.ISupportInitialize)pbFrame).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.Timer confettiTimer;
        private PictureBox pbFrame;
    }
}