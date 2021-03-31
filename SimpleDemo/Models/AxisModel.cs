using HelixToolkit.Wpf.SharpDX;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PCViz
{
    public class AxisModel
    {
        public LineGeometry3D AxisGeometry { get; set; }
        public Transform3D AxisTransform { get; set; }
        public Color AxisColor { get; set; }
    }
}
