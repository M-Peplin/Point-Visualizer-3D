using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace PointCloudLib
{
    public class GetPointCloud
    {
        static public List<GridPoint> FromTxt(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);                

                string dataFirstLine = sr.ReadLine();
                dataFirstLine.ToLower();

                string[] firstLineInfo = dataFirstLine.Split(',');

                string data = sr.ReadLine();
                var pointCollection = new List<GridPoint>();
                while (data != null)
                {

                    GridPoint gridPointToAdd = new GridPoint(0,0,0,0);

                    string[] values = data.Split(',');

                    if (values.Length <= 1) values = data.Split(' ');
                    int[] ValueIndex = new int[4];


                    for (int i = 0; i < values.Length; i++)
                    {


                        switch (firstLineInfo[i])
                        {
                            case "utmx":
                                {                                    
                                    ValueIndex[0] = i;
                                    break;
                                }

                            case "utmy":
                                {                                    
                                    ValueIndex[1] = i;
                                    break;
                                }
                            case "z":
                                {                                    
                                    ValueIndex[2] = i;
                                    break;
                                }
                            case "ampl":
                                {                                    
                                    ValueIndex[3] = i;
                                    break;
                                }

                        }
                    }


                    gridPointToAdd.X = double.Parse(values[ValueIndex[0]], System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                    gridPointToAdd.Y = double.Parse(values[ValueIndex[1]], System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                    gridPointToAdd.Z = double.Parse(values[ValueIndex[2]], System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                    gridPointToAdd.I = double.Parse(values[ValueIndex[3]], System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

                    pointCollection.Add(gridPointToAdd);

                    data = sr.ReadLine();
                }
                return pointCollection;
            }
            catch (Exception e)
            {
                //throw e;
                MessageBox.Show(e.Message);
                return null;
            }

        }
        static public Dictionary<KeyValuePair<int,int>,double> XYZMap (string path)
        {
            var pointsCollection = FromTxt(path);

            var xydict = new Dictionary<KeyValuePair<int, int>, double>();

            foreach (var item in pointsCollection)
            {
                var xy = new KeyValuePair<int, int>((int)item.X, (int)item.Y);
                xydict[xy] = item.Z;
            }
            return xydict;
        }

        static public Dictionary<int,int> XYMap (string path)
        {
            var pointsCollection = FromTxt(path);

            var xydict = new Dictionary<int, int>();

            foreach (var item in pointsCollection)
            {
                xydict[(int)item.X] = (int)item.Y;
            }
            return xydict;
        }

        static public Dictionary<int, double> XZMap(string path)
        {
            var pointsCollection = FromTxt(path);

            var xzdict = new Dictionary<int, double>();

            foreach (var item in pointsCollection)
            {
                xzdict[(int)item.X] = item.Z;
            }
            return xzdict;
        }
        static public Dictionary<int, int> YXMap(string path)
        {
            var pointsCollection = FromTxt(path);

            var xydict = new Dictionary<int, int>();

            foreach (var item in pointsCollection)
            {
                xydict[(int)item.Y] = (int)item.X;
            }
            return xydict;
        }

    }
}
