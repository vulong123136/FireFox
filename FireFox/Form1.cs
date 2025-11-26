namespace FireFox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SoundManager.AttachClickSound(this); // Gắn tiếng click
            SoundManager.PlayBGM(); // Bắt đầu phát nhạc nền ngay khi mở game
        }

        private void btn_choi_Click(object sender, EventArgs e)
        {

            Form2 form2 = new Form2();
            this.Hide();
            form2.ShowDialog();
            if (form2.WantsToExit == true)
            {
                this.Close(); // Đóng Form 1, kết thúc ứng dụng hoàn toàn.
            }
            else
            {
                this.Show(); // Hiện lại Form 1
            }

        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
