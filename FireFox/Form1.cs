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
            lb_name.Left = (this.ClientSize.Width - lb_name.Width) / 2;
            btn_choi.Left = (this.ClientSize.Width / 2) - btn_choi.Width - 20;
            btn_thoat.Left = (this.ClientSize.Width / 2) + 20;
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
