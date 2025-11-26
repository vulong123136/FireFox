using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FireFox
{
    public partial class FormWinEffect : Form
    {
        private string winnerName;
        private double formOpacity = 0.0;
        private List<ConfettiParticle> confettis = new List<ConfettiParticle>();
        private Random random = new Random();

        public FormWinEffect(string winnerName)
        {
            InitializeComponent();

            // Kích hoạt chế độ vẽ mượt mà (quan trọng cho hiệu ứng pháo giấy)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.winnerName = winnerName;

            // Cấu hình Form Full màn hình
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(150, 110, 50); // Nền màu vàng nâu
            this.Opacity = 0.0;

            // Cấu hình chữ
            label1.Text = $"CHÚC MỪNG\n{winnerName.ToUpper()}\nVÔ ĐỊCH!";
            label1.ForeColor = Color.Gold;
            label1.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void FormWinEffect_Load(object sender, EventArgs e)
        {
            // 1. Căn giữa dòng chữ vào màn hình
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height - label1.Height) / 2;

            // 2. Tạo 200 hạt pháo giấy ngẫu nhiên
            for (int i = 0; i < 200; i++)
            {
                confettis.Add(new ConfettiParticle(random, this.Width, this.Height));
            }

            // 3. Phát nhạc chiến thắng
            // (Đảm bảo bạn đã add file win.wav vào Resources như hướng dẫn trước)
            SoundManager.PlaySound(Properties.Resources.win);

            // 4. Bắt đầu hiệu ứng
            fadeInTimer.Start();
            confettiTimer.Start();
        }

        // Timer làm mờ Form hiện dần lên
        private void fadeInTimer_Tick(object sender, EventArgs e)
        {
            formOpacity += 0.05;
            if (formOpacity >= 1.0)
            {
                formOpacity = 1.0;
                fadeInTimer.Stop();

                // Tự động đóng sau 8 giây
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                t.Interval = 8000;
                t.Tick += (s, ev) => { t.Stop(); this.Close(); };
                t.Start();
            }
            this.Opacity = formOpacity;
        }

        // Timer cập nhật vị trí pháo giấy
        private void confettiTimer_Tick(object sender, EventArgs e)
        {
            foreach (var particle in confettis)
            {
                particle.Update();
                // Nếu rơi quá màn hình thì reset lại lên trên
                if (particle.Y > this.Height) particle.Reset(random, this.Width);
            }
            this.Invalidate(); // Vẽ lại Form
        }

        // Hàm vẽ đồ họa (GDI+)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Vẽ pháo giấy
            foreach (var particle in confettis)
            {
                using (SolidBrush brush = new SolidBrush(particle.Color))
                {
                    g.FillRectangle(brush, particle.X, particle.Y, particle.Width, particle.Height);
                }
            }

            // (Tùy chọn) Vẽ thêm một khung viền đơn giản bằng code thay cho ảnh
            using (Pen pen = new Pen(Color.Gold, 10))
            {
                int margin = 50;
                g.DrawRectangle(pen, margin, margin, this.Width - margin * 2, this.Height - margin * 2);
            }
        }
    }

    // Class hạt pháo giấy (Không đổi)
    public class ConfettiParticle
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color Color { get; private set; }
        private int speed;
        private Random random;

        public ConfettiParticle(Random rand, int maxX, int maxY)
        {
            this.random = rand;
            Width = rand.Next(8, 20);
            Height = rand.Next(8, 20);
            speed = rand.Next(5, 15);
            Reset(rand, maxX);
        }

        public void Reset(Random rand, int maxX)
        {
            Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            X = rand.Next(0, maxX);
            Y = rand.Next(-200, -50);
        }

        public void Update()
        {
            Y += speed;
            X += random.Next(-2, 3); // Lắc lư nhẹ sang hai bên
        }
    }
}