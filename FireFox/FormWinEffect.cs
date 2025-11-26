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

            // 🔥 FIX 1: KÍCH HOẠT VẼ CUSTOM (CHO PHÉP ONPAINT VÀ CONFETTI HOẠT ĐỘNG)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.winnerName = winnerName;

            // --- Cài đặt Form ---
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(150, 110, 50);
            this.Opacity = 0.0;

            // --- Cài đặt Label (label1) ---
            label1.Text = $"CHÚC MỪNG: {winnerName} ĐÃ CHIẾN THẮNG!";
            label1.Font = new Font("Arial", 50, FontStyle.Bold);
            label1.ForeColor = Color.Gold;
            label1.BackColor = Color.Transparent;
            label1.AutoSize = true;
        }

        private void FormWinEffect_Load(object sender, EventArgs e)
        {
            // 🔥 FIX 2: CĂN GIỮA LABEL VÀ PICTUREBOX (PBFrame)

            // 1. Căn giữa Label
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height - label1.Height) / 2;

            // 2. Căn giữa PictureBox (Khung ảnh)
            pbFrame.Left = (this.ClientSize.Width - pbFrame.Width) / 2;
            pbFrame.Top = (this.ClientSize.Height - pbFrame.Height) / 2;

            // 3. Khởi tạo 200 hạt Confetti (Lúc này Form đã Maximize nên this.Width/Height là đúng)
            for (int i = 0; i < 200; i++)
            {
                confettis.Add(new ConfettiParticle(random, this.Width, this.Height));
            }

            // 4. Bắt đầu hiệu ứng Fade In và Confetti
            fadeInTimer.Interval = 30;
            fadeInTimer.Start();

            confettiTimer.Interval = 20;
            confettiTimer.Start();
        }

        // Timer mờ dần Form (Sự kiện Tick của fadeInTimer)
        private void fadeInTimer_Tick_1(object sender, EventArgs e)
        {
            formOpacity += 0.05;
            if (formOpacity >= 1.0)
            {
                formOpacity = 1.0;
                fadeInTimer.Stop();

                // Dừng và đóng Form sau 5 giây hiển thị rõ
                System.Windows.Forms.Timer closeDelayTimer = new System.Windows.Forms.Timer();
                closeDelayTimer.Interval = 5000;
                closeDelayTimer.Tick += (s, ev) => {
                    closeDelayTimer.Stop();
                    this.Close();
                };
                closeDelayTimer.Start();
            }
            this.Opacity = formOpacity;
        }

        // Timer cập nhật Confetti (Sự kiện Tick của confettiTimer)
        private void confettiTimer_Tick_1(object sender, EventArgs e)
        {
            foreach (var particle in confettis)
            {
                particle.Update();
                if (particle.Y > this.Height)
                {
                    particle.Reset(random, this.Width);
                }
            }
            this.Invalidate(); // Yêu cầu Form vẽ lại (gọi OnPaint)
        }

        // Phương thức vẽ chính (Vẽ confetti)
        protected override void OnPaint(PaintEventArgs e)
        {
            // Bỏ qua base.OnPaint(e) nếu bạn muốn vẽ hoàn toàn tùy chỉnh,
            // nhưng giữ lại base.OnPaint(e) nếu bạn có Control khác (như Label/PictureBox)
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Vẽ từng hạt confetti
            foreach (var particle in confettis)
            {
                using (SolidBrush brush = new SolidBrush(particle.Color))
                {
                    g.FillRectangle(brush, particle.X, particle.Y, particle.Width, particle.Height);
                }
            }
        }
    }

    // Đảm bảo Class ConfettiParticle nằm bên ngoài FormWinEffect
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
            Width = rand.Next(4, 12);
            Height = rand.Next(4, 12);
            speed = rand.Next(3, 8); // Tốc độ rơi ngẫu nhiên
            Reset(rand, maxX);
        }

        public void Reset(Random rand, int maxX)
        {
            // Chọn màu ngẫu nhiên
            Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

            X = rand.Next(0, maxX - Width);
            Y = rand.Next(-300, -20); // Bắt đầu từ phía trên màn hình
        }

        public void Update()
        {
            Y += speed;
            // Thêm hiệu ứng lắc lư ngang nhẹ
            X += random.Next(-1, 2);
        }
    }
}