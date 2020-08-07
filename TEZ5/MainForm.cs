using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TEZ5
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            string path = Path.GetFullPath("Databas1e.mdf");
            if (File.Exists(path))
            {
                DataController.setConn(path);
                MapController.setConn(path);
                DataController.setData(this.dataGridView);
                DataController.loadSubs(this.substanceComboBox);
                dataGridView.CellValueChanged += CellValueChanged;
                pictureBox.Image = new Bitmap(Properties.Resources.RozaVetrov);
                MapController.LoadMap(gMapControl);
                MapController.Update(this);
                pictureBox1.BackColor = Color.FromArgb(140, 255, 0, 0);
                pictureBox2.BackColor = Color.FromArgb(140, 255, 128, 0);
                pictureBox3.BackColor = Color.FromArgb(140, 255, 255, 0);
                pictureBox4.BackColor = Color.FromArgb(140, 21, 144, 100);
                pictureBox5.BackColor = Color.FromArgb(140, 0, 255, 0);
            }
            else
            {              
                
                MessageBox.Show("База данных не найдена");
            }
        }
       

        private void buttonDel_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> id = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                id.Add(row);
            }
            DataController.delData(id);
            MapController.Update(this);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            DataController.addData();
            MapController.Update(this);
        }
        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataController.changeData(dataGridView, e);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            MapController.SaveMap(gMapControl);
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            if (pointListBox.CheckedItems.Count < 2)
            {
                MessageBox.Show("Выберите не менее двух точек");
                return;
            }
            if (substanceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите элемент");
                return;
            }
            MapController.Calculate(pointListBox, substanceComboBox, gMapControl);
        }
    }
}
