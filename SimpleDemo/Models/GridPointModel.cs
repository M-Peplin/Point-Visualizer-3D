using PointCloudLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCViz
{
    public class GridPointModel : IPingPoint
    {
        public double X { get ; set ; }
        public double Y { get ; set ; }
        public double Z { get ; set ; }
        public double I { get ; set ; }
    }
}
