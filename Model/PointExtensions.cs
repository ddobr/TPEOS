using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    static class PointExtensions
    {
        public static Point Sum(this Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point Diff(this Point minuend, Point subtrahend)
        {
            return new Point(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);
        }

        public static List<Point> IncidentPoints(this Point point)
        {
            var result = new List<Point>
            {
                point + new Size(0, -1), point + new Size(0, 1), point + new Size(-1, 0), point + new Size(1, 0)
            };
            return result;
        }
    }
}
