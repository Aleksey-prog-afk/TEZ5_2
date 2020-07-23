using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TEZ5
{

    public static class MapController
    {
        private static double xist = 55.00515;
        private static double yist = 83.06187;
        private static SqlConnection connection;
        private static GMapOverlay pointsOverlay = new GMapOverlay();
        private static GMapOverlay rezOverlay = new GMapOverlay("Результаты");
        private static void PointsLoad(GMapControl map)
        {
            string id;
            double x, y;
            pointsOverlay.Clear();
            GMarkerGoogle marker;
            marker = new GMarkerGoogle(new PointLatLng(xist, yist), GMarkerGoogleType.red_small);
            marker.ToolTip = new GMapRoundedToolTip(marker);
            marker.ToolTipText = "ТЭЦ-5";
            pointsOverlay.Markers.Add(marker);
            SqlCommand command = new SqlCommand("Select * FROM Points", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                x = (double)reader["lat"];
                y = (double)reader["lng"];
                id = reader["Id"].ToString();
                marker = new GMarkerGoogle(new PointLatLng(x, y), GMarkerGoogleType.black_small);
                marker.ToolTip = new GMapToolTip(marker);
                marker.ToolTipText = id;
                pointsOverlay.Markers.Add(marker);
            }
            connection.Close();
            map.Overlays.Add(pointsOverlay);
        }
        public static void setConn(string ugollename)
        {
            string path = Path.GetFullPath(ugollename);
            path.Replace(@"\\", @"\");
            connection = new SqlConnection($@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = {path};Integrated Security=True");

        }
        public static void LoadMap(GMapControl map)
        {
            map.Bearing = 0;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Left;
            map.GrayScaleMode = true;
            map.MaxZoom = 18;
            map.MinZoom = 5;
            map.PolygonsEnabled = true;
            map.MarkersEnabled = true;
            map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            map.NegativeMode = false;
            map.ShowTileGridLines = false;
            map.Zoom = 11;
            map.Dock = DockStyle.None;
            map.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            map.Position = new PointLatLng(xist, yist);
            map.ShowCenter = false;
            map.Overlays.Add(rezOverlay);
            map.MapScaleInfoEnabled = true;

        }
        public static void Update(MainForm form)
        {
            CheckedListBoxUpdate(form.pointListBox);
            form.gMapControl.Overlays.Clear();
            form.gMapControl.Overlays.Clear();
            PointsLoad(form.gMapControl);
        }
        private static void CheckedListBoxUpdate(CheckedListBox listBox)
        {
            listBox.Items.Clear();
            SqlCommand command = new SqlCommand("Select Id FROM Points", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listBox.Items.Add(reader[0]);
            }
            connection.Close();
        }
        public static void SaveMap(GMapControl map)
        {
            try
            {

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "PNG (*.png)|*.png";
                    dialog.FileName = "GMap.NET image";
                    Image image = map.ToImage();
                    if (image != null)
                    {
                        using (image)
                        {

                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                //Заносим в переменную имя файла введенное
                                //в диалоговом окне.
                                string fileName = dialog.FileName;


                                if (!fileName.EndsWith(".png",
                                StringComparison.OrdinalIgnoreCase))
                                {
                                    fileName += ".png";
                                }

                                image.Save(fileName);


                                MessageBox.Show("Карта успешно сохранена в директории： "
                                + Environment.NewLine
                                + dialog.FileName, "GMap.NET",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка при сохранении карты： "
                + Environment.NewLine
                + exception.Message,
                "GMap.NET",
                MessageBoxButtons.OK,
                MessageBoxIcon.Hand);
            }
        }
        public static void Calculate(CheckedListBox listBox, ComboBox box, GMapControl map)
        {
            string subname = box.SelectedItem.ToString();
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            for (int i = 0; i < listBox.CheckedItems.Count; i++)
            {
                builder.Append(listBox.CheckedItems[i].ToString() + ",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(")");
            double x = 0, y = 0, rez;
            double[] qt = new double[listBox.CheckedItems.Count];
            double[] windt = new double[listBox.CheckedItems.Count];
            double[] rt = new double[listBox.CheckedItems.Count];
            double angle;
            int counter = 0;
            SqlCommand command = new SqlCommand($"Select lat, lng, {subname} FROM Points, Results WHERE Points.Id IN " + builder + " AND Results.Id = Points.Id ", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                x = (double)reader["lat"];
                y = (double)reader["lng"];
                rez = (double)reader[subname];
                qt[counter] = rez;
                windt[counter] = Wind(x, y);
                angle = Math.Sin(xist * Math.PI / 180) * Math.Sin(x * Math.PI / 180) + Math.Cos(xist * Math.PI / 180) * Math.Cos(x * Math.PI / 180) * Math.Cos(yist * Math.PI / 180 - y * Math.PI / 180);
                rt[counter] = Math.Acos(angle) * 6371;
                counter++;
            }
            connection.Close();
            double s1 = 0, s2 = 0, temp1, temp2;
            double rm = 3.5;
            double[] tempQt = new double[listBox.CheckedItems.Count];
            double mnk = Double.MaxValue, mnk1 = 0;
            for (temp1 = 1; temp1 <= 10000; temp1 += 1)
            {

                for (temp2 = -2; temp2 < 2; temp2 += 0.01)
                {
                    for (int i = 0; i < listBox.CheckedItems.Count; i++)
                    {
                        tempQt[i] = windt[i] * temp1 * Math.Pow(rt[i], temp2) * Math.Exp(-2 * rm / rt[i]);
                        mnk1 += Math.Pow(tempQt[i] - qt[i], 2);
                    }
                    if (mnk1 < mnk)
                    {
                        mnk = mnk1;
                        s1 = temp1;
                        s2 = temp2;
                    }
                    mnk1 = 0;
                }
            }
            rezOverlay.Clear();
            rezOverlay.Markers.Clear();
            for (double i = xist - 0.11; i < xist + 0.11; i += 0.001)
            {
                for (double j = yist - 0.2; j < yist + 0.2; j += 0.001)
                {
                    Qpole(i, j, s1, s2);
                }
            }
            if (map.Overlays.Contains(rezOverlay))
            {
                map.Overlays.Clear();

            }
            map.Overlays.Add(rezOverlay);
            PointsLoad(map);


        }
        private static double Wind(double x, double y)
        {

            double ugol = 180 / Math.PI * Math.Atan((x - xist) / (y - yist));
            double[] roza = { 12.4, 4.7, 0.9, 3.1, 10.4, 37.1, 10.8, 7.8 };
            double wind;

            if (y - yist <= 0)
            {
                ugol += 180;
            }
            if (ugol < 0)
            {
                ugol += 360;
            }
            if (ugol >= 0 & ugol <= 45)
                wind = roza[0] + (roza[1] - roza[0]) * (ugol) / 45;
            else if (ugol >= 45 & ugol <= 90)
                wind = roza[1] + (roza[2] - roza[1]) * (ugol - 45) / 45;
            else if (ugol >= 90 & ugol < 135)
                wind = roza[2] + (roza[3] - roza[2]) * (ugol - 90) / 45;
            else if (ugol >= 135 & ugol <= 180)
                wind = roza[3] + (roza[4] - roza[3]) * (ugol - 135) / 45;
            else if (ugol >= 180 & ugol <= 225)
                wind = roza[4] + (roza[5] - roza[4]) * (ugol - 180) / 45;
            else if (ugol >= 225 & ugol <= 270)
                wind = roza[5] + (roza[6] - roza[5]) * (ugol - 225) / 45;
            else if (ugol >= 270 & ugol <= 315)
                wind = roza[6] + (roza[7] - roza[6]) * (ugol - 270) / 45;
            else
                wind = roza[7] + (roza[0] - roza[7]) * (ugol - 315) / 45;
            return wind;

        }
        private static void Qpole(double x, double y, double s1, double s2)
        {
            double windt = Wind(x, y);
            double angle = Math.Sin(xist * Math.PI / 180) * Math.Sin(x * Math.PI / 180) + Math.Cos(xist * Math.PI / 180) * Math.Cos(x * Math.PI / 180) * Math.Cos(yist * Math.PI / 180 - y * Math.PI / 180);
            double rt = Math.Acos(angle) * 6371;
            double qt = windt * s1 * Math.Pow(rt, s2) * Math.Exp(-2 * 1.5 / rt);
            GMapPoint point = new GMapPoint(new PointLatLng(x, y), qt);
            rezOverlay.Markers.Add(point);
        }

    }
}
