using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PCViz
{
    public class PointModel : ObservableObject
    {
        public PointGeometry3D PointsGeometry { get;  set; }
        public Transform3D PointsTransform { get;  set; }
        public Color PointsColor { get;  set; }
    }
}
