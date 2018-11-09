using CornellBoxWPF.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WireframeCube.Helpers;

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
        public Vector3 color = new Vector3();
        public Vector3 light = new Vector3(0, 0, 0);
        public Vector3 camera = new Vector3(0, 0, 0);

        public static byte[] colourData { get; set; }

        public static double degree = 0.0;

        public static int bytesPerPixel = 3;
                                                                      // top
        public static List<Vector3> cubePoints = new List<Vector3>() {new Vector3(-1, -1, -1),    // A, 0
                                                                      new Vector3(1,-1,-1),       // B, 1
                                                                      new Vector3(1,1,-1),        // C, 2
                                                                      new Vector3(-1,1,-1),       // D, 3
                                                                     // bottom
                                                                     new Vector3(-1,-1,1),        // E, 4
                                                                     new Vector3(1,-1,1),         // F, 5
                                                                     new Vector3(1,1,1),          // G, 6
                                                                     new Vector3(-1,1,1)};        // H, 7



        public static List<Triangle> triangles = new List<Triangle>() {
            new Triangle(new Vector3(0,1,2),new Vector3(1,0,0), new Vector4(0,1,0,0), new Vector4(0,0,1,0), new Vector4(1,0,0,0)), 
            new Triangle(new Vector3(0,2,3),new Vector3(1,0,0), new Vector4(0,1,0,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0)),
            new Triangle(new Vector3(7,6,5),new Vector3(0,1,0), new Vector4(0,1,0,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0)),
            new Triangle(new Vector3(7,5,4),new Vector3(0,1,0) ,new Vector4(0,1,0,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0)), 
            new Triangle(new Vector3(0,3,7),new Vector3(0,0,1), new Vector4(0,1,0,0), new Vector4(0,0,1,0), new Vector4(1,0,0,0)), 
            new Triangle(new Vector3(0,7,4),new Vector3(0,0,1), new Vector4(0,1,0,0), new Vector4(0,0,1,0), new Vector4(1,0,0,0)),
            new Triangle(new Vector3(2,1,5),new Vector3(1,1,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0), new Vector4(0,0,1,0)),
            new Triangle(new Vector3(2,5,6),new Vector3(1,1,0), new Vector4(1,0,0,0), new Vector4(0,0,1,0), new Vector4(0,1,0,0)),
            new Triangle(new Vector3(3,2,6),new Vector3(1,0,1), new Vector4(0,0,1,0), new Vector4(1,0,0,0), new Vector4(0,1,0,0)),
            new Triangle(new Vector3(3,6,7),new Vector3(1,0,1) ,new Vector4(0,0,1,0), new Vector4(0,1,0,0), new Vector4(1,0,0,0)),
            new Triangle(new Vector3(1,0,4),new Vector3(0,1,1), new Vector4(0,0,1,0), new Vector4(0,1,0,0), new Vector4(1,0,0,0)),
            new Triangle(new Vector3(1,4,5),new Vector3(0,1,1), new Vector4(0,0,1,0), new Vector4(1,0,0,0), new Vector4(0,1,0,0))
        };

        public static Vector3 v1 = new Vector3(0, 0, 5);
        Point[] trianglePoints = new Point[cubePoints.Count];

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

            // Set and clean up
            colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            List<Vector3> points_copy = new List<Vector3>(cubePoints);

            // Rotate cube by certain degree
            degree += 0.05f;

            for (int i = 0; i < cubePoints.Count; i++)
            {
                // Rotate with given matrix
                points_copy[i] = Vector3.Transform(points_copy[i], MatrixHelpers.GetXRotationMatrix(degree)*MatrixHelpers.GetYRotationMatrix(degree));

                // Translate all points
                points_copy[i] += v1;

                // Calc x' and y'
                trianglePoints[i] = new Point((int)(image.Width * points_copy[i].X / points_copy[i].Z + image.Width / 2), (int)(image.Width * points_copy[i].Y / points_copy[i].Z + image.Height / 2));
            }

            foreach (var triangle in triangles)
            {
                // 2D single triangle points
                Point A = trianglePoints[(int)triangle._pointIdx.X];
                Point B = trianglePoints[(int)triangle._pointIdx.Y];
                Point C = trianglePoints[(int)triangle._pointIdx.Z];
                
                // Start Backface culling
                Vector3 AB = new Vector3((float)(B.X - A.X), (float)(B.Y - A.Y), 0);
                Vector3 AC = new Vector3((float)(C.X - A.X), (float)(C.Y - A.Y), 0);
                Vector3 n = Vector3.Normalize(Vector3.Cross(AB, AC));

                // Optimization(from inner to outer loop) -> constants for u,v calc
                float a = AB.X;
                float b = AC.X;
                float c = AB.Y;
                float d = AC.Y;
                float det = a * d - b * c;

                // Get bounding box
                int min_x = MathHelpers.Lowest(new int[] { (int)A.X, (int)B.X, (int)C.X });
                int max_x = MathHelpers.Highest(new int[] { (int)A.X, (int)B.X, (int)C.X });
                int min_y = MathHelpers.Lowest(new int[] { (int)A.Y, (int)B.Y, (int)C.Y });
                int max_y = MathHelpers.Highest(new int[] { (int)A.Y, (int)B.Y, (int)C.Y });
                // End Optimization

                if (n.Z > 0.0f) // Triangle is in front -> Render it
                {   // End Backface culling
                    for (int x = min_x; x < max_x; x++) // iterate only from min_x to max_x in bounding box
                    {
                        for (int y = min_y; y < max_y; y++) // iterate only from min_x to max_x in bounding box
                        {
                           // Calc u and v
                           Vector2 AP = new Vector2(x - (float)A.X, y - (float)A.Y);
                           float upper = AP.X * d + AP.Y * (-b);
                           float lower = AP.X * (-c) + AP.Y * a;
                           Vector2 vec = new Vector2(upper * 1 / det, lower * 1 / det);
                           float u = vec.X;
                           float v = vec.Y;

                           if (u >= 0 && v >= 0 && u + v < 1)
                           {
                               //color = triangle._color;
                               color = GetInterpolatedColor(triangle, u, v);
                               colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                               colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Blue
                               colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Green
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

        public static Vector3 GetInterpolatedColor(Triangle t, float u, float v)
        {
            Vector3 color = Vector3.Zero;
            Vector3 colorA = new Vector3(t._colorA.X, t._colorA.Y, t._colorA.Z);
            Vector3 colorB = new Vector3(t._colorB.X, t._colorB.Y, t._colorB.Z);
            Vector3 colorC = new Vector3(t._colorC.X, t._colorC.Y, t._colorC.Z);

            color = colorA + u * (colorB - colorA) + v * (colorC - colorA);

            return color;
        }

    }
}



