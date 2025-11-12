using System;
using System.Windows.Forms;

namespace FireFox
{
    public partial class Form2 : Form
    {
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

            // 2. SỬA LỖI 1: Truyền biến 'soNguoi' vào trong ngoặc
            // Lúc này Form3 sẽ nhận được số 2, 3, hoặc 4
            Form3 form3 = new Form3(soNguoi);

            this.Hide();        // Ẩn Form 2
            form3.ShowDialog(); // Hiện Form 3
            this.Show();        // Khi Form 3 đóng thì hiện lại Form 2
        }

        // Các hàm sự kiện thừa (nếu có lỡ tạo thì để trống như này để không bị lỗi)
        private void label1_Click(object sender, EventArgs e) { }
        private void Form2_Load(object sender, EventArgs e) { }
    }
}