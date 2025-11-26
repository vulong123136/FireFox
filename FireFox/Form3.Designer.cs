namespace FireFox
{
    partial class Form3
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
            btnRoll = new Button();
            lblDice = new Label();
            rtbLog = new RichTextBox();
            pbDice = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbDice).BeginInit();
            SuspendLayout();
            // 
            // btnRoll
            // 
            btnRoll.BackColor = Color.Lime;
            btnRoll.Font = new Font("Times New Roman", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRoll.ForeColor = SystemColors.ActiveCaptionText;
            btnRoll.Location = new Point(691, 600);
            btnRoll.Name = "btnRoll";
            btnRoll.Size = new Size(237, 125);
            btnRoll.TabIndex = 0;
            btnRoll.Text = "Gieo xúc sắc";
            btnRoll.UseVisualStyleBackColor = false;
            btnRoll.Click += btnRoll_Click;
            // 
            // lblDice
            // 
            lblDice.AutoSize = true;
            lblDice.Font = new Font("Times New Roman", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDice.Location = new Point(788, 365);
            lblDice.Name = "lblDice";
            lblDice.Size = new Size(0, 31);
            lblDice.TabIndex = 1;
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(224, 144);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(336, 581);
            rtbLog.TabIndex = 2;
            rtbLog.Text = "";
            // 
            // pbDice
            // 
            pbDice.Location = new Point(962, 600);
            pbDice.Name = "pbDice";
            pbDice.Size = new Size(123, 125);
            pbDice.SizeMode = PictureBoxSizeMode.Zoom;
            pbDice.TabIndex = 3;
            pbDice.TabStop = false;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1476, 862);
            Controls.Add(pbDice);
            Controls.Add(rtbLog);
            Controls.Add(lblDice);
            Controls.Add(btnRoll);
            Name = "Form3";
            Text = "Cờ Tỷ Phú - Bàn Cờ";
            Load += Form3_Load;
            ((System.ComponentModel.ISupportInitialize)pbDice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRoll;
        private Label lblDice;
        private RichTextBox rtbLog;
        private PictureBox pbDice;
    }
}