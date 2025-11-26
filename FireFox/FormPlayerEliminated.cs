using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireFox
{
    public partial class FormPlayerEliminated : Form
    {
        public FormPlayerEliminated(string name)
        {
            InitializeComponent();

            // --- Cấu hình giao diện Form ---
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen; // Căn giữa màn hình
            this.BackColor = Color.DarkRed; // Màu nền đỏ cảnh báo
            this.Opacity = 0; // Bắt đầu ở trạng thái trong suốt
            this.TopMost = true; // Luôn nổi lên trên cùng
            this.Size = new Size(800, 400); // Kích thước Form

            // --- Cấu hình Label thông báo ---
            // Đảm bảo label1 có Properties: AutoSize = false, Dock = Fill trong Designer hoặc code này sẽ ghi đè
            label1.Text = $"{name.ToUpper()}\nĐÃ BỊ LOẠI!";
            label1.ForeColor = Color.White;
            label1.Font = new Font("Arial", 40, FontStyle.Bold);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Dock = DockStyle.Fill; // Label lấp đầy Form để chữ luôn ở giữa

            // --- Bắt đầu hiệu ứng hiện dần ---
            fade.Interval = 25; // Tốc độ mượt
            fade.Start();
        }

        private void fade_Tick(object sender, EventArgs e)
        {
            // Tăng độ đậm dần dần
            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.05;
            }
            else
            {
                // Khi đã hiện rõ hoàn toàn thì dừng timer và chuẩn bị đóng
                fade.Stop();
                AutoCloseAsync();
            }
        }

        // Hàm đợi 2 giây rồi đóng Form
        private async void AutoCloseAsync()
        {
            await Task.Delay(2000); // Chờ 2000ms (2 giây)
            this.Close();
        }

        private void FormPlayerEliminated_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }
    }
}