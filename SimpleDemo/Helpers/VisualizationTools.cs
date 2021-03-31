using System.Linq;
using HelixToolkit.Wpf.SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Drawing.Drawing2D;
using PCViz.Models;

namespace PCViz
{
    public static class VisualizationTools
    {
        public static void CreateGrid(LineBuilder gridLines, double gridMaxX, double gridMinX, double gridMaxY, double gridMinY, double gridMinZ)
        {
            for (double i = 1; i < (int)(gridMaxY - gridMinY); i += 50)  // creating grid
            {
                Vector3 p1 = new Vector3(0, (float)i, 0);
                Vector3 p2 = new Vector3((float)(gridMaxX - gridMinX), (float)i, 0);
                gridLines.AddLine(p1, p2);
            }
            for (double i = 1; i < (int)(gridMaxX - gridMinX); i += 50)  // creating grid
            {
                Vector3 p1 = new Vector3((float)i, 0, 0);
                Vector3 p2 = new Vector3((float)i, (float)(gridMaxY - gridMinY), 0);
                gridLines.AddLine(p1, p2);
            }
        }
        public static void CreateAxes(LineBuilder arrows, float gridMaxX, float gridMinX, float gridMaxY, float gridMinY, float gridMaxZ, float gridMinZ)
        {            

            Vector3 p0 = new Vector3(0, 0, 0);
            //x
            Vector3 px1 = new Vector3(gridMaxX - gridMinX, 0, 0);          
            //y
            Vector3 py1 = new Vector3(0, gridMaxY - gridMinY, 0);           
            //z
            var z = (gridMaxX - gridMinX) > (gridMaxY - gridMinY) ? (gridMaxX - gridMinX) : (gridMaxY - gridMinY);
            Vector3 pz1 = new Vector3(0, 0, Math.Abs(z / 5));
            arrows.AddLine(p0, px1);
            arrows.AddLine(p0, py1);          
            arrows.AddLine(p0, pz1);
        }
        public static void ClearView (out PointModel Points,out GridModel Grid,out AxisModel Axis)
        {
            // floor plane grid
            var Maxreset = 100;
            var Minreset = -100;

            Points = new PointModel
            {
                // point positions and color gradient
                PointsGeometry = new PointGeometry3D { Positions = new Vector3Collection(), Indices = new IntCollection(), Colors = new Color4Collection() },
                PointsColor = Colors.White,
                PointsTransform = new Media3D.TranslateTransform3D(0, 0, Minreset)
            };

            LineBuilder gridLines = new LineBuilder();
            VisualizationTools.CreateGrid(gridLines, Maxreset, Minreset, Maxreset, Maxreset, Minreset);
            Grid = new GridModel
            {
                GridGeometry = gridLines.ToLineGeometry3D(),
                GridColor = new Color4(153 / 255.0f, 204 / 255.0f, 255 / 255.0f, (float)0.3).ToColor(),
                GridTransform = new TranslateTransform3D(0, 0, 0)
            };

            // lines
            LineBuilder arrows = new LineBuilder();
            VisualizationTools.CreateAxes(arrows, Maxreset, Minreset, Maxreset, Minreset, Maxreset, Minreset);
            Axis = new AxisModel
            {
                AxisGeometry = arrows.ToLineGeometry3D(),
                AxisColor = new Color4(0, 255 / 255.0f, 255 / 255.0f, (float)0.5).ToColor(),
                AxisTransform = new TranslateTransform3D(0, 0, 0)
            };
        }
        /// <summary>
        /// Get color for given point on scale
        /// </summary>
        /// <param name="min">Min value of given scale</param>
        /// <param name="max">Max value of given scale</param>
        /// <param name="current">Value of current point on scale</param>
        /// <returns></returns>
        /// 
        //Color switcher                       

        public static Color4 RedBlueScale(float min, float max, float current)
        {
            int hrange = 240;
            float H, S = 0.75F, L = 0.45F;
            float[] RGBA = new float[3];

            var dx =  max - min;
            var m =  hrange / dx;
            var bb = 0 - (m * min);


            H = -1 * ( m * current + bb)+ hrange;
            //H = (m * current + bb); //for blue to red
            var a = S * Math.Min(L, 1 - L);

            for (int n = 0; n <= 8 ; n+=4)
            {
                var k = (n + H / 30) % 12;
                var fn = L - a * Math.Max(-1, Math.Min(Math.Min(k - 3, 9 - k), 1));

                if (n == 0) RGBA[0]=fn;
                else if (n == 4) RGBA[2]=fn;
                else if (n == 8) RGBA[1]=fn;
            }

            return new Color4(RGBA[0], RGBA[1], RGBA[2], 1);
        }

        public static Color4 CopperColorScale(float min, float max, float current)
        {
            int lrange = 40;
            float L, S=0.75F, H = 0.35F;
            float[] RGBA = new float[3];

            var dx =  max - min;
            var m =  lrange / dx;
            var bb = 0 - (m * min);

            L = -1 * (m * current + bb) + lrange;            
            var a = S * Math.Min(H, 1 - H);

            for (int n = 0; n <= 8; n += 4)
            {
                var k = (n + L / 30) % 12;
                var fn = H - a * Math.Max(-1, Math.Min(Math.Min(k - 3, 9 - k), 1));

                if (n == 0) RGBA[0] = fn;
                else if (n == 4) RGBA[2] = fn;
                else if (n == 8) RGBA[1] = fn;
            }

            return new Color4(RGBA[0], RGBA[1], RGBA[2], 1);
        }



        public static void AddPointCloud(Dictionary<KeyValuePair<int, int>, double> PointCollection, out PointModel Points,
                                         out GridModel Grid, out AxisModel Axis, out HelixToolkit.Wpf.SharpDX.Camera Camera, bool cameraType=false)
        {
            Camera = null;
            Points = null;
            Grid = null;
            Axis = null;

            var points = new PointGeometry3D();
            var col = new Color4Collection(); // color gradient
            var ptPos = new Vector3Collection(); // point positions
            var ptIdx = new IntCollection(); // point indexes

            if (PointCollection != null && PointCollection.Count() > 1)
            {
                //newdataFromTxt(path, positionX, positionY, positionZ);
                var maxX = PointCollection.Max(x => x.Key.Key); // X
                var minX = PointCollection.Min(x => x.Key.Key); // X
                var maxY = PointCollection.Max(x => x.Key.Value); // Y
                var minY = PointCollection.Min(x => x.Key.Value); // Y
                var maxZ = PointCollection.Max(x => x.Value); // Z
                var minZ = PointCollection.Min(x => x.Value); // Z
                var minYObj = PointCollection.First(x => x.Key.Value == minY);

                Trace.WriteLine($"{maxX} {minX} {maxY} {minY} {maxZ} {minZ}");

                if (PointCollection.Count() <= 0) return;
                foreach (var point in PointCollection)
                {                    
                    var positionToColour = point.Value;

                    ptIdx.Add(ptPos.Count);
                    ptPos.Add(new Vector3((float)point.Key.Key, ((float)point.Key.Value), (float)point.Value));
                    col.Add(new Color4(RedBlueScale((float)minZ,(float)maxZ,(float)positionToColour)));
                }
                Points = new PointModel
                {
                    // indexes and color gradient
                    PointsGeometry = new PointGeometry3D { Positions = ptPos, Indices = ptIdx, Colors = col },
                    PointsColor = Colors.White,
                    PointsTransform = new TranslateTransform3D(-minX, -minY, -minZ)
                };
                // floor plane grid
                LineBuilder gridLines = new LineBuilder();
                VisualizationTools.CreateGrid(gridLines, maxX, minX, maxY, minY, minZ);
                Grid = new GridModel
                {
                    GridGeometry = gridLines.ToLineGeometry3D(),
                    GridColor = new Color4(153 / 255.0f, 204 / 255.0f, 255 / 255.0f, 0.3f).ToColor(),
                    GridTransform = new TranslateTransform3D(0, 0, 0)
                };
                // lines
                LineBuilder arrows = new LineBuilder();
                VisualizationTools.CreateAxes(arrows, (float)maxX, (float)minX, (float)maxY, (float)minY, (float)maxZ, (float)minZ);
                Axis = new AxisModel
                {
                    AxisGeometry = arrows.ToLineGeometry3D(),
                    AxisColor = new Color4(0, 255 / 255.0f, 255 / 255.0f, 0.5f).ToColor(),
                    AxisTransform = new TranslateTransform3D(0, 0, 0)
                };

                if(cameraType == false)
                {
                    Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
                    {
                        Position = new Point3D(0, 0, Math.Abs(minZ * 100)),
                        LookDirection = new Vector3D(((float)maxX - (float)minX) / 2, ((float)maxY - (float)minY) / 2, -Math.Abs(minZ * 100)),//z kamerą jest problem przy przybliżaniu prawdopodobnie przez ten LookDirection albo sposób poruszania kamerą
                        UpDirection = new Vector3D(0, 1, 0),
                        FarPlaneDistance = 5000000
                    };
                }
                else
                {
                    Camera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera
                    {
                        Position = new Point3D(0, 0, Math.Abs(minZ * 100)),
                        LookDirection = new Vector3D(((float)maxX - (float)minX) / 2, ((float)maxY - (float)minY) / 2, -Math.Abs(minZ * 100)),//z kamerą jest problem przy przybliżaniu prawdopodobnie przez ten LookDirection albo sposób poruszania kamerą
                        UpDirection = new Vector3D(0, 1, 0),
                        FarPlaneDistance = 5000000
                    };
                }
                
            }
        }

        public static List<KeyValuePair<KeyValuePair<int, int>, double>> GetPointNeighbours(KeyValuePair<KeyValuePair<int, int>, double> keyValuePair, Dictionary<KeyValuePair<int, int>, double> XYZ)
        {
            try
            {
                int x = keyValuePair.Key.Key;
                int y = keyValuePair.Key.Value;
                var z = keyValuePair.Value;

                var n1 = new KeyValuePair<int, int>(x + 1, y);
                var n2 = new KeyValuePair<int, int>(x - 1, y);
                var n3 = new KeyValuePair<int, int>(x, y - 1);
                var n4 = new KeyValuePair<int, int>(x, y + 1);

                var neigbours = new List<KeyValuePair<KeyValuePair<int, int>, double>>();

                if (XYZ.ContainsKey(n1)) { neigbours.Add(new KeyValuePair<KeyValuePair<int, int>, double>(n1, XYZ[n1])); }
                if (XYZ.ContainsKey(n2)) { neigbours.Add(new KeyValuePair<KeyValuePair<int, int>, double>(n2, XYZ[n2])); }
                if (XYZ.ContainsKey(n3)) { neigbours.Add(new KeyValuePair<KeyValuePair<int, int>, double>(n3, XYZ[n3])); }
                if (XYZ.ContainsKey(n4)) { neigbours.Add(new KeyValuePair<KeyValuePair<int, int>, double>(n4, XYZ[n4])); }

                return neigbours;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static LineBuilder ConnectNeigbours(Dictionary<KeyValuePair<int, int>, double> XYZ)
        {
            var lb = new LineBuilder();


            foreach (var keyValuePair in XYZ)
            {
                int x = keyValuePair.Key.Key;
                int y = keyValuePair.Key.Value;
                var z = keyValuePair.Value;

                var neighboursList = GetPointNeighbours(keyValuePair, XYZ);
                
                foreach (var n in neighboursList)
                {                    
                    if (XYZ.ContainsKey(n.Key)) { lb.AddLine(new Vector3(x, y, (float)z), new Vector3(n.Key.Key, n.Key.Value, (float)n.Value)); }
                }
            }
            return lb;
        }

        public static LineGeometry3D DrawPointGrid(Dictionary<KeyValuePair<int, int>, double> XYZ, double min, double max, Enums.colorScales colorScale)
        {
            var linebuilder = ConnectNeigbours(XYZ);
            var linebuilderGeometry = linebuilder.ToLineGeometry3D();
            var colorCollection = ScaleGridColors(linebuilderGeometry.Lines, min, max, colorScale);

            linebuilderGeometry.Colors = colorCollection;

            return linebuilderGeometry;
        }

        public static Func<float, float, float, Color4> scaleHolder;

        public static void switchColorScale(Enums.colorScales scale)
        {
            switch(scale)
            {
                case Enums.colorScales.RedBlueScale:
                    {
                        scaleHolder = (a, b, c) => RedBlueScale(a, b, c);
                        break; 
                    }
                case (Enums.colorScales.CopperScale):
                    {
                        scaleHolder = (a, b, c) => CopperColorScale(a, b, c);
                        break;
                    }

            }
            
        }

        public static Color4Collection ScalePointColors(Vector3Collection points, double actualMin, double actualMax ,double newMin, double newMax, Enums.colorScales colorScale)
        {
            var temp = new Color4Collection();

            switchColorScale(colorScale);


            foreach (var item in points)
            {
                if (item.Z > (float)newMax) temp.Add(new Color4(VisualizationTools.scaleHolder((float)actualMin, (float)newMax, (float)newMax)));
                else if (item.Z < (float)newMin) temp.Add(new Color4(VisualizationTools.scaleHolder((float)newMin, (float)actualMax, (float)newMin)));
                else temp.Add(new Color4(VisualizationTools.scaleHolder((float)newMin, (float)newMax, item.Z)));
            }
            return temp;
        }

        public static Color4Collection ScaleGridColors(IEnumerable<HelixToolkit.Wpf.SharpDX.Geometry3D.Line> lines, double min, double max, Enums.colorScales colorScale)
        {
            var colorList = new Color4Collection();

            switchColorScale(colorScale);

            foreach (var line in lines)
            {
                var avg = (line.P0.Z + line.P1.Z) / 2;
                if (avg > (float)max)
                {
                    colorList.Add(new Color4(scaleHolder((float)min, (float)max, (float)max)));
                    colorList.Add(new Color4(scaleHolder((float)min, (float)max, (float)max)));
                }
                else if (avg < (float)min)
                {
                    colorList.Add(new Color4(scaleHolder((float)min, (float)max, (float)min)));
                    colorList.Add(new Color4(scaleHolder((float)min, (float)max, (float)min)));
                }
                else
                {
                    colorList.Add(scaleHolder((float)min, (float)max, (float)avg));
                    colorList.Add(scaleHolder((float)min, (float)max, (float)avg));
                }

            }
            return colorList;
        }
    }
}
