
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CornellBoxWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WriteableBitmap image { get; set; }

        public static float GAMMA = 2.2f;
        public static int bytesPerPixel = 3;
        public static int antiAliasing = 5;
        // top
        public static List<Vector3> points = new List<Vector3>() { new Vector3(-1, -1, -1),
                                                                   new Vector3(1,-1,-1),
                                                                   new Vector3(1,1,-1),
                                                                   new Vector3(-1,1,-1),
                                                                    // bottom
                                                                   new Vector3(-1,-1,1),
                                                                   new Vector3(1,-1,1),
                                                                   new Vector3(1,1,1),
                                                                   new Vector3(-1,1,1)};
        public static List<Vector3> triangleIdx = new List<Vector3>() {
            new Vector3(0,1,2),
            new Vector3(0,2,3),
            new Vector3(7,6,5),
            new Vector3(7,5,4),
            new Vector3(0,3,7),
            new Vector3(0,7,4),
            new Vector3(2,1,5),
            new Vector3(2,5,6),
            new Vector3(3,2,6),
            new Vector3(3,6,7),
            new Vector3(1,0,4),
            new Vector3(1,4,5)};


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Registering to the XAML rendering loop
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        // Rendering loop handler
        void CompositionTarget_Rendering(object sender, object e)
        {
            Polygon p = new Polygon();
            p.Stroke = Brushes.Black;
            p.Points.Add(new Point(0, 0));
            p.Points.Add(new Point(100, 0));
            p.Points.Add(new Point(0, 100));
            Main_Canvas.Children.Add(p);
        }
    }
}



