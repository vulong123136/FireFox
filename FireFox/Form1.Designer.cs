namespace FireFox
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lb_name = new Label();
            btn_choi = new Button();
            btn_thoat = new Button();
            SuspendLayout();
            // 
            // lb_name
            // 
            lb_name.BackColor = Color.Transparent;
            lb_name.Font = new Font("Jokerman", 99.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lb_name.ForeColor = Color.Yellow;
            lb_name.Location = new Point(360, 200);
            lb_name.Name = "lb_name";
            lb_name.Size = new Size(830, 230);
            lb_name.TabIndex = 0;
            lb_name.Text = "Cờ Tỷ Phú";
            lb_name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_choi
            // 
            btn_choi.BackColor = Color.Yellow;
            btn_choi.Font = new Font("Showcard Gothic", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_choi.Location = new Point(376, 496);
            btn_choi.Name = "btn_choi";
            btn_choi.Size = new Size(243, 115);
            btn_choi.TabIndex = 1;
            btn_choi.Text = "Chơi ngay";
            btn_choi.UseVisualStyleBackColor = false;
            btn_choi.Click += btn_choi_Click;
            // 
            // btn_thoat
            // 
            btn_thoat.BackColor = Color.Yellow;
            btn_thoat.Font = new Font("Showcard Gothic", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_thoat.Location = new Point(875, 496);
            btn_thoat.Name = "btn_thoat";
            btn_thoat.Size = new Size(240, 115);
            btn_thoat.TabIndex = 2;
            btn_thoat.Text = "Thoát";
            btn_thoat.UseVisualStyleBackColor = false;
            btn_thoat.Click += btn_thoat_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1478, 862);
            Controls.Add(btn_thoat);
            Controls.Add(btn_choi);
            Controls.Add(lb_name);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Label lb_name;
        private Button btn_choi;
        private Button btn_thoat;
    }
}
