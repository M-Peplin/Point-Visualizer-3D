// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainView.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf.SharpDX.Utilities;
using System;
using System.Windows;

namespace PCViz
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {

            //this.InitializeComponent();
            InitializeComponent();
            Closed += (s, e) =>
            {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();

                }
            };

                        
        }
    }
}