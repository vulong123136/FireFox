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

        }

        private void btn_choi_Click(object sender, EventArgs e)
        {

            Form2 form2 = new Form2();
            this.Hide();
            form2.ShowDialog();
            this.Show();

        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
