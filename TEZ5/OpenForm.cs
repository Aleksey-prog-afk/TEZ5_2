using System;
using System.Drawing;
using System.Windows.Forms;

namespace TEZ5
{
    public partial class OpenForm : Form
    {
        private bool change = false;
        public OpenForm()
        {
            InitializeComponent();
            this.Width = Properties.Resources._1.Width;
            this.Height = Properties.Resources._1.Height;
            pictureBox1.Width = Properties.Resources._1.Width;
            pictureBox1.Height = Properties.Resources._1.Height;
            pictureBox1.Image = new Bitmap(Properties.Resources._1);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!change)
            {
                this.Width = Properties.Resources._2.Width;
                this.Height = Properties.Resources._2.Height;
                pictureBox1.Width = Properties.Resources._2.Width;
                pictureBox1.Height = Properties.Resources._2.Height;
                pictureBox1.Image = new Bitmap(Properties.Resources._2);
                change = true;
            }
            else
            {


                this.Close();
            }
        }
    }
}
