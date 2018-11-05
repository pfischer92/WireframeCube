using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WireframeCube.Helpers;

namespace CornellBoxWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Vector3 camera = new Vector3(0, 0, 0);
        private readonly Stopwatch stopwatch = new Stopwatch();
        private double frameCounter;
        public static WriteableBitmap image { get; set; }
        public Vector3 color = new Vector3();

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
        public static List<Triangle> triangles = new List<Triangle>() {
            new Triangle(new Vector3(0,1,2), new Vector3(1,0,0)),
            new Triangle(new Vector3(0,2,3),new Vector3(1,0,0)),
            new Triangle(new Vector3(7,6,5),new Vector3(0,1,0)),
            new Triangle(new Vector3(7,5,4),new Vector3(0,1,0)),
            new Triangle(new Vector3(0,3,7),new Vector3(0,0,1)),
            new Triangle(new Vector3(0,7,4),new Vector3(0,0,1)),
            new Triangle(new Vector3(2,1,5),new Vector3(1,1,0)),
            new Triangle(new Vector3(2,5,6),new Vector3(1,1,0)),
            new Triangle(new Vector3(3,2,6),new Vector3(1,0,1)),
            new Triangle(new Vector3(3,6,7),new Vector3(1,0,1)),
            new Triangle(new Vector3(1,0,4),new Vector3(0,1,1)),
            new Triangle(new Vector3(1,4,5),new Vector3(0,1,1))
        };


        public MainWindow()
        {
            image = new WriteableBitmap(400, 400, 96, 96, PixelFormats.Rgb24, null);
            colourData = new byte[image.PixelHeight * image.PixelWidth * 3];
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
            colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            Vector3 v1 = new Vector3(0, 0, 5);
            Point[] trianglePoints = new Point[points.Count];
            List<Vector3> copy = new List<Vector3>(points);

            // Rotate cube by certain degree
            degree += 0.05f;

            for (int i = 0; i < points.Count; i++)
            {
                // Rotate with given matrix
                copy[i] = Vector3.Transform(copy[i], MatrixHelpers.GetXRotationMatrix(degree) * MatrixHelpers.GetYRotationMatrix(degree));

                // Translate all points
                copy[i] += v1;

                // Calc x' and y'
                trianglePoints[i] = new Point((int)(image.Width * copy[i].X / copy[i].Z + image.Width / 2), (int)(image.Width * copy[i].Y / copy[i].Z + image.Height / 2));
            }

            foreach (var triangle in triangles)
            {
                // 2D single triangle points
                Point A = trianglePoints[(int)triangle._pointIdx.X];
                Point B = trianglePoints[(int)triangle._pointIdx.Y];
                Point C = trianglePoints[(int)triangle._pointIdx.Z];

                // Backface culling here
                Vector3 AB = new Vector3((float)(B.X - A.X), (float)(B.Y - A.Y), 0);
                Vector3 AC = new Vector3((float)(C.X - A.X), (float)(C.Y - A.Y), 0);
                Vector3 n = Vector3.Normalize(Vector3.Cross(AB, AC));
                
                if (n.Z > 0.0f) // Triangle is in front
                {
                    for (int x = 0; x < image.PixelWidth; x++)
                    {
                        for (int y = 0; y < image.PixelHeight; y++)
                        {
                            // Calc u and v
                            Vector2 AP = new Vector2(x - (float)A.X, y - (float)A.Y);
                            float a = AB.X;
                            float b = AC.X;
                            float c = AB.Y;
                            float d = AC.Y;
                            float det = a * d - b * c;

                            float upper = AP.X * d + AP.Y * (-b);
                            float lower = AP.X * (-c) + AP.Y * a;
                            Vector2 vec = new Vector2(upper * 1 /det, lower * 1/det);
                            float u = vec.X;
                            float v = vec.Y;

                            if (u >= 0 && v >= 0 && u + v < 1)
                            {
                                color = triangle._color;
                                colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Blue
                                colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2)] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Green
                            }
                        }
                    }
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            img.Source = image;
        }
    }
}



