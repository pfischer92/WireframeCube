﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Windows;
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
        private readonly Stopwatch stopwatch = new Stopwatch();
        private double frameCounter;
        public static WriteableBitmap image { get; set; }

        public static byte[] colourData { get; set; }

        public static double degree = 0.0;

        public static int bytesPerPixel = 3;
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
            new Vector3(5,4,1)
        };


        public MainWindow()
        {
            image = new WriteableBitmap(400, 400, 96, 96, PixelFormats.Rgb24, null);
            colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Registering to the XAML rendering loop
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CalcFrameRate()
        {
            if (frameCounter++ == 0){ stopwatch.Start(); }
            
            var frameRate = (long)(frameCounter / stopwatch.Elapsed.TotalSeconds);
            if (frameRate > 0)
            {
                frame_rate.Content = frameRate.ToString() + " fps";
            }
        }

        // Rendering loop handler
        void CompositionTarget_Rendering(object sender, object e)
        {
            CalcFrameRate();
            canv.Children.Clear();
            Polygon p = new Polygon();
            p.Stroke = Brushes.Black;
            Vector3 v1 = new Vector3(0, 0, 5);
            Point[] trianglePoints = new Point[points.Count];
            List<Vector3> copy = new List<Vector3>(points);

            // Rotate cube by certain degree
            degree += 0.01f;

            for (int i = 0; i < points.Count; i++)
            {
                // Rotate with given matrix
                copy[i] = Vector3.Transform(copy[i], MatrixHelpers.GetXRotationMatrix(degree) * MatrixHelpers.GetYRotationMatrix(degree));

                // Translate all points
                copy[i] += v1;

                // Calc x' and y'
                trianglePoints[i] = new Point((int)(canv.Width * copy[i].X / copy[i].Z + canv.Width / 2), (int)(canv.Width * copy[i].Y / copy[i].Z + canv.Height / 2));
            }

            foreach (var triangle in triangleIdx)
            {
                Point p1 = trianglePoints[(int)triangle.X];
                Point p2 = trianglePoints[(int)triangle.Y];
                Point p3 = trianglePoints[(int)triangle.Z];

                p.Points.Add(p1);
                p.Points.Add(p2);
                p.Points.Add(p3);
            }
            
            canv.Children.Add(p);


            //for (int x = 0; x < image.PixelWidth; x++)
            //{
            //    for (int y = 0; y < image.PixelHeight; y++)
            //    {

            //        // Calc u and v
            //        for (int i = 0; i < points.Count; i++)
            //        {

            //        }

            // if(u >= 0 && v >= 0 && u + v < 1){
            // drawPixel(x,y, color);
            //}


            //colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
            //colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Blue
            //colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Green
        }
            //}

            //image.Lock();
            //image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            //image.Unlock();

            //img.Source = image;

        //}
    }
}



