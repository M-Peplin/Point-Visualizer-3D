// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using HelixToolkit.Wpf.SharpDX;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using PointCloudLib;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using System;
using PCViz.Models;
using System.Windows.Media.Imaging;
using System.IO;

namespace PCViz
{
    /// <summary>
    /// Application developed by two juniors
    /// at the beginning of their dev career
    /// so don't judge too harshly ;)
    /// Mariej Pepliński & Rafał Kucharski
    /// </summary>
    public class MainViewModel : BaseCameraViewModel
    {
        public LineGeometry3D Lines { get; private set; }
        public PointModel Points
        {
            get { return _points; }
            set { SetValue(ref _points, value); }
        }
        private PointModel _points;
        public AxisModel Axis { get; set; }
        public GridModel Grid { get; set; }
        public GridModel PointGrid { get; set; }
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }
        
        private Dictionary<KeyValuePair<int, int>, double> _gridPoints = new Dictionary<KeyValuePair<int, int>, double>();
        private double[] _maxMinDepth = new double[] { 0.0, 1.0 };

        private double _maxColorScale;
        public double MaxColorScaleZ
        {
            get { return _maxColorScale; }
            set { SetValue(ref _maxColorScale, value); }
        }
        private double _minColorScale;
        public double MinColorScaleZ
        {
            get { return _minColorScale; }
            set { SetValue(ref _minColorScale, value); }
        }

        //---- MPMP Do testow -----

        private Models.Enums.colorScales _colorScales = new Enums.colorScales();

        public Enums.colorScales ColorScales
        {
            get
            {
                return _colorScales;
            }
            set
            {
                SetValue(ref _colorScales, value);
            }
        }

        private bool _cameraTypeSwitch;

        public bool CameraTypeSwitch
        {
            get
            {
                return _cameraTypeSwitch;
            }

            set
            {
                SetValue(ref _cameraTypeSwitch, value);
                ViewPointCloud();
            }
        }


        
        //----------------------------------
        public double[] MaxMinDepth
        {
            get { return _maxMinDepth; }
            set { SetValue(ref _maxMinDepth, value); }
        }
        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            // titles
            Title = "Viewer";
            SubTitle = "Marine Technology";

            // camera setup
            Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(0, 0, 0),
                UpDirection = new Vector3D(2, 1, 0),
                FarPlaneDistance = 0
            };
            // setup lighting            
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White; // light necessary

            DirectionalLightDirection = Camera.LookDirection;
        }
        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(
                    param => ImportDataFromFile(),
                    param => true);
                }
                return _openCommand;
            }
        }
        private ICommand _displayCommand;
        public ICommand DisplayCommand
        {
            get
            {
                if (_displayCommand == null)
                {
                    _displayCommand = new RelayCommand(
                    param => ViewPointCloud(),
                    param => true);
                }
                return _displayCommand;
            }
        }
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand(
                    param => Clear(),
                    param => true);
                }
                return _clearCommand;
            }
        }

        private ICommand _recolorCommand;
        public ICommand RecolorCommand
        {
            get
            {
                if (_recolorCommand == null)
                {                    
                    _recolorCommand = new RelayCommand(
                    param => ScaleColors(MinColorScaleZ, MaxColorScaleZ),
                    param => true);
                }
                return _recolorCommand;
            }
        }
        private ICommand _saveScreenshotCommand;

        public ICommand SaveScreenshotCommand
        {
            get
            {
                if(_saveScreenshotCommand == null)
                {                    
                    _saveScreenshotCommand = new RelayCommand(TakeScreenShot);
                }

                return _saveScreenshotCommand;
            }
        }
                

        private ICommand _testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand(
                    param => test(),
                    param => true);
                }
                return _testCommand;
            }
        }
        private SolidColorBrush _backgroundColorBtn1;
        public SolidColorBrush BackgroundColorBtn1
        {
            get { return _backgroundColorBtn1; }
            set { SetValue(ref _backgroundColorBtn1, value); }
        }

        //-----------------------
        private bool _someConditionalProperty = false;

        public bool SomeConditionalProperty
        {
            get { return _someConditionalProperty; }
            set { SetValue(ref _someConditionalProperty, value); }
        }

        private LineGeometry3D _pointlines;

        public LineGeometry3D PointLines
        {
            get { return _pointlines; }
            set { _pointlines = value; }
        }


        public void test()
        {
            var XYZHashMap = GetPointCloud.XYZMap(Filepath);

            var linebuilderGeometry = VisualizationTools.DrawPointGrid(XYZHashMap, MinColorScaleZ,MaxColorScaleZ, _colorScales);
            PointGrid = new GridModel { GridGeometry = linebuilderGeometry, GridTransform = new TranslateTransform3D(0, 0, 0), GridColor = new Color4(255, 255, 255, 0.5f).ToColor() };
            ScaleColors(MinColorScaleZ, MaxColorScaleZ);
        }
        //----------------------

        private void Clear()
        {
            EffectsManager.Dispose();
            EffectsManager = new DefaultEffectsManager();
            VisualizationTools.ClearView(out PointModel points, out GridModel grid, out AxisModel axis);
            this.Points = points;
            this.Grid = grid;
            this.Axis = axis;
            this.PointGrid = new GridModel(); 
        }

        public string Filepath { get; set; } = "Selected file: None";
        private void ImportDataFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                Filepath = openFileDialog.FileName;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<GridPoint, GridPointModel>());
            var mapper = new Mapper(config);

            try
            {
                _gridPoints = GetPointCloud.XYZMap(Filepath);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

            ViewPointCloud();
        }
        private void ViewPointCloud()
        {
            Clear();
            VisualizationTools.AddPointCloud(_gridPoints, out PointModel points, out GridModel grid, out AxisModel axis,
                                        out HelixToolkit.Wpf.SharpDX.Camera cam, _cameraTypeSwitch);
            this.Camera = cam;
            this.Points = points;
            this.Grid = grid;
            this.Axis = axis;

            GetMinMaxZRange();            
            ScaleColors(MinColorScaleZ, MaxColorScaleZ);
        }
        private void GetMinMaxZRange()
        {
            try
            {
                MaxMinDepth = new double[] { _gridPoints.Max(x => x.Value), _gridPoints.Min(x => x.Value) };
                MaxColorScaleZ = MaxMinDepth[0];
                MinColorScaleZ = MaxMinDepth[1];
            }
            catch (System.Exception)
            {
                //throw;
            }
        }
                

        private void ScaleColors(double min, double max)
        {
            Points.PointsGeometry.Colors = VisualizationTools.ScalePointColors(Points.PointsGeometry.Positions,MaxMinDepth[1],MaxMinDepth[0],min, max, _colorScales);
            if (PointGrid.GridGeometry != null) ScaleGridColors(min, max);
        }

    private void ScaleGridColors(double min, double max)
        {
            PointGrid.GridGeometry.Colors = VisualizationTools.ScaleGridColors(PointGrid.GridGeometry.Lines, min, max, _colorScales);
        }
             

        private void TakeScreenShot(object control)
        {
            FrameworkElement element = control as FrameworkElement;
            if (element == null)
                throw new Exception("Invalid parameter");

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(element);

            string fileName = "ScreenCapture -" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            FileStream stream = new FileStream(fileName, FileMode.Create);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            encoder.Save(stream);       


            using (Bitmap bmp = new Bitmap(stream))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {

                    SaveFileDialog saveFile = new SaveFileDialog(); ;
                    saveFile.Filter = "JPEG|*.jpeg|Bitmap|*.bmp|Gif|*.gif|Png|*.png";

                    if (saveFile.ShowDialog() == true)
                    {
                        bmp.Save(saveFile.FileName);
                    }

                }

            }

        }


       


    }


}
