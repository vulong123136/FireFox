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
            // Đảm bảo luôn có một lựa chọn được chọn sẵn để tránh lỗi
            radioButton1.Checked = true;
        }

        // --- ĐÂY LÀ HÀM MÀ MÁY BÁO LỖI KHÔNG TÌM THẤY (LỖI 2) ---
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Xác định số người chơi dựa trên RadioButton
            int soNguoi = 2; // Mặc định là 2

            if (radioButton2.Checked == true)
            {
                soNguoi = 3;
            }
            else if (radioButton3.Checked == true)
            {
                soNguoi = 4;
            }
            Form3 form3 = new Form3(soNguoi);

            this.Hide();        
            form3.ShowDialog(); 
            if (form3.WantsToExit == true)
            {
                WantsToExit = true; // Báo hiệu cho Form1 biết là cần thoát
                this.Close();
            }
            else
            {
                this.Show();
            };       
        }

        // Các hàm sự kiện thừa (nếu có lỡ tạo thì để trống như này để không bị lỗi)
        private void label1_Click(object sender, EventArgs e) { }
        private void Form2_Load(object sender, EventArgs e) 
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;

            radioButton1.Left = this.ClientSize.Width / 2 - 300;
            radioButton2.Left = this.ClientSize.Width / 2 - 100;
            radioButton3.Left = this.ClientSize.Width / 2 + 100;

            button1.Left = (this.ClientSize.Width - button1.Width) / 2;
        }
    }
}