using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireFox
{
    public partial class FormPlayerEliminated : Form
    {
        public FormPlayerEliminated(string name)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.DarkRed;
            this.Opacity = 0;
            label1.Text = $"{name} đã bị loại!";
            fade.Start();
        }

        int a = 0;
        private void fade_Tick(object sender, EventArgs e)
        {
            if (a < 100)
            {
                a += 10;
                this.Opacity = a / 100.0;
            }
            else fade.Stop();
        }
      

        private void FormPlayerEliminated_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height - label1.Height) / 2;

        }
    }

}
