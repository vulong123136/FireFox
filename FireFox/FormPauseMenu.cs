using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireFox
{
    public partial class FormPauseMenu : Form
    {
        public int Option = 0; // 1=continue, 2=menu, 3=exit

        public FormPauseMenu()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Option = 1;
            this.Close();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            Option = 2;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Option = 3;
            this.Close();
        }

        private void FormPauseMenu_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                c.Left = (this.ClientSize.Width - c.Width) / 2;
            }

            btnContinue.Top = this.ClientSize.Height / 2 - 200;
            btnMenu.Top = this.ClientSize.Height / 2;
            btnExit.Top = this.ClientSize.Height / 2 + 200;

        }
    }

}
