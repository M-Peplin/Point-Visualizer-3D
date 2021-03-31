using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudLib
{
    public class GridPoint : IPingPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double I { get; set; }

        public GridPoint(double x, double y, double z, double i)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.I = i;
        }
    }
}
