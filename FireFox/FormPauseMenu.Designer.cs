namespace FireFox
{
    partial class FormPauseMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPauseMenu));
            btnContinue = new Button();
            btnMenu = new Button();
            btnExit = new Button();
            SuspendLayout();
            // 
            // btnContinue
            // 
            btnContinue.BackgroundImage = (Image)resources.GetObject("btnContinue.BackgroundImage");
            btnContinue.BackgroundImageLayout = ImageLayout.Stretch;
            btnContinue.Location = new Point(338, 302);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new Size(150, 147);
            btnContinue.TabIndex = 0;
            btnContinue.UseVisualStyleBackColor = true;
            btnContinue.Click += btnContinue_Click;
            // 
            // btnMenu
            // 
            btnMenu.BackgroundImage = (Image)resources.GetObject("btnMenu.BackgroundImage");
            btnMenu.BackgroundImageLayout = ImageLayout.Stretch;
            btnMenu.Location = new Point(672, 302);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(146, 147);
            btnMenu.TabIndex = 1;
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // btnExit
            // 
            btnExit.BackgroundImage = (Image)resources.GetObject("btnExit.BackgroundImage");
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Location = new Point(1010, 302);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(146, 147);
            btnExit.TabIndex = 2;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // FormPauseMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1509, 818);
            Controls.Add(btnExit);
            Controls.Add(btnMenu);
            Controls.Add(btnContinue);
            DoubleBuffered = true;
            Name = "FormPauseMenu";
            Text = "FormPauseMenu";
            Load += FormPauseMenu_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnContinue;
        private Button btnMenu;
        private Button btnExit;
    }
}