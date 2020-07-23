using GMap.NET;
using System.Drawing;

namespace TEZ5
{
    public class GMapPoint : GMap.NET.WindowsForms.GMapMarker
    {
        private PointLatLng point_;
        private float size_ = 2;
        private Brush brush;
        public PointLatLng Point
        {
            get
            {
                return point_;
            }
            set
            {
                point_ = value;
            }
        }
        public GMapPoint(PointLatLng p, double q)
            : base(p)
        {
            point_ = p;
            if (q > 7)
            {
                brush = new SolidBrush(Color.FromArgb(80, 255, 0, 0));
            }
            else if (q > 5)
            {
                brush = new SolidBrush(Color.FromArgb(80, 255, 128, 0));
            }
            else if (q > 3)
            {
                brush = new SolidBrush(Color.FromArgb(80, 255, 255, 0));
            }
            else if (q > 2)
            {
                brush = new SolidBrush(Color.FromArgb(80, 21, 144, 100));
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(80, 0, 255, 0));
            }
        }

        public override void OnRender(Graphics g)
        {

            g.FillRectangle(brush, LocalPosition.X, LocalPosition.Y, size_, size_);



        }
    }
}
