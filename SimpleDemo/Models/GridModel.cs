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
    public class GridModel
    {
        public LineGeometry3D GridGeometry { get;  set; }
        public Transform3D GridTransform { get;  set; }
        public Color GridColor { get;  set; }
    }
}
