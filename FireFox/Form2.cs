using System;
using System.Windows.Forms;

namespace FireFox
{
    public partial class Form2 : Form
    {
        public bool WantsToExit = false;
        public Form2()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int soNguoi = 2;
            if (radioButton2.Checked) soNguoi = 3;
            else if (radioButton3.Checked) soNguoi = 4;

            Form3 form3 = new Form3(soNguoi);
            this.Hide();
            form3.ShowDialog();

            if (form3.WantsToExit)
            {
                WantsToExit = true;
                this.Close();
            }
            else
            {
                this.Show();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Căn chỉnh vị trí (giữ nguyên code cũ của bạn)
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            radioButton1.Left = this.ClientSize.Width / 2 - 300;
            radioButton2.Left = this.ClientSize.Width / 2 - 100;
            radioButton3.Left = this.ClientSize.Width / 2 + 100;
            button1.Left = (this.ClientSize.Width - button1.Width) / 2;

            // Gắn âm thanh
            SoundManager.AttachClickSound(this);
        }

        // --- THÊM HÀM NÀY ĐỂ SỬA LỖI ---
        private void label1_Click(object sender, EventArgs e)
        {
            // Để trống, không làm gì cả
        }
    }
}