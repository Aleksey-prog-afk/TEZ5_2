using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TEZ5
{
    static class DataController
    {
        private static string path;
        private static SqlConnection connection;
        private static SqlDataAdapter adapter;
        private static DataSet ds;
        private static double xist = 55.00515;
        private static double yist = 83.06187;
        public static void setConn(string path)
        {
           
            connection = new SqlConnection($@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = {path};Integrated Security=True");

        }
        public static void setData(System.Windows.Forms.DataGridView gridView)
        {
            connection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Points Join Results ON Points.Id = Results.Id", connection);
            ds = new DataSet();
            adapter.Fill(ds);
            ds.Tables[0].Columns.Remove("Id1");
            ds.Tables[0].Columns.Remove("distance");
            gridView.DataSource = ds.Tables[0];
            gridView.Columns[0].HeaderText = "Точка отбора";
            gridView.Columns[1].HeaderText = "Широта";
            gridView.Columns[2].HeaderText = "Долгота";
            gridView.Columns["Id"].ReadOnly = true;
            connection.Close();


        }
        public static void changeData(System.Windows.Forms.DataGridView gridView, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(gridView.Rows[e.RowIndex].Cells[0].Value);
            if (gridView.Columns[e.ColumnIndex].Name == "lat" || gridView.Columns[e.ColumnIndex].Name == "lng")
            {
                double x = Convert.ToDouble(gridView.Rows[e.RowIndex].Cells[1].Value);
                double y = Convert.ToDouble(gridView.Rows[e.RowIndex].Cells[2].Value);
                double distance = distCalculate(x, y);
                SqlCommand command = new SqlCommand(@"UPDATE POINTS SET lat = @x, lng = @y, distance = @dist WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@x", x);
                command.Parameters.AddWithValue("@y", y);
                command.Parameters.AddWithValue("@dist", distance);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            else
            {
                string column = gridView.Columns[e.ColumnIndex].Name;
                double value = Convert.ToDouble(gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                SqlCommand command = new SqlCommand($@"UPDATE RESULTS SET {column} = @value  WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@value", value);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void addData()
        {
            DataRow row = ds.Tables[0].NewRow();
            for (int i = 1; i < row.Table.Columns.Count; i++)
            {
                row[i] = 0;
            }
            SqlCommand command = new SqlCommand("SELECT ident_current('Points')", connection);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            int id = Convert.ToInt32(reader.GetValue(0));
            row[0] = id + 1;
            reader.Close();
            ds.Tables[0].Rows.Add(row);
            adapter.InsertCommand = new SqlCommand("INSERT INTO Points VALUES (0,0,0); INSERT INTO Results VALUES (" + row[0] + "," +
                "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)", connection);
            adapter.Update(ds);
            connection.Close();
        }
        public static void delData(List<System.Windows.Forms.DataGridViewRow> id)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            foreach (System.Windows.Forms.DataGridViewRow i in id)
            {
                builder.Append(i.Cells["Id"].Value + ",");
                ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[i.Index]);
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(")");
            SqlCommand DeleteCommand = new SqlCommand("DELETE FROM Points WHERE Points.Id IN " + builder + " DELETE FROM Results WHERE Results.Id IN " + builder, connection);
            connection.Open();
            DeleteCommand.ExecuteNonQuery();
            connection.Close();
        }
        private static double distCalculate(double x, double y)
        {
            double distance = Math.Sin(xist * Math.PI / 180) * Math.Sin(x * Math.PI / 180) +
                Math.Cos(xist * Math.PI / 180) * Math.Cos(x * Math.PI / 180) * Math.Cos(yist * Math.PI / 180 - y * Math.PI / 180);
            distance = Math.Acos(distance) * 6371210;
            return distance;

        }
        public static void loadSubs(ComboBox box)
        {
            for (int i = 3; i < ds.Tables[0].Columns.Count; i++)
            {
                box.Items.Add(ds.Tables[0].Columns[i].ColumnName);
            }
        }
    }

}

