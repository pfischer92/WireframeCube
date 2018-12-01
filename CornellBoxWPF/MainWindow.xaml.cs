using CornellBoxWPF.BitmapHelper;
using CornellBoxWPF.Helpers;
using System;
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
        // Configs
        private static bool printZbuffer = false;
        private static bool printPositionBuffer = false;
        private static bool printTextureCoordBuffer = false;
        private static bool printNormalsBuffer = false;
        private static bool printDiffuseColorBuffer = false;
        private static bool defferedRendering = false;

        // Globale Attribute
        private readonly Stopwatch stopwatch = new Stopwatch();
        private BitmapTexturing bitmapTexturing = new BitmapTexturing();
        private double frameCounter;
        public static WriteableBitmap image { get; set; }
        public Vector3 color = new Vector3();
        public static Vector3 lightPos = new Vector3(0, 0, 0);
        public static Vector3 lightColor = new Vector3(0.8f, 0.8f, 0.8f);
        public static Vector3 _eye = new Vector3(0, 0, 0);
        public static int _k = 40;

        public static byte[] colourData { get; set; }

        public static byte[] positionBuffer { get; set; }

        public static byte[] textureBuffer { get; set; }

        public static byte[] normalsBuffer { get; set; }

        public static byte[] diffuseColorBuffer { get; set; }
        public static byte[] deferredBuffer { get; set; }
        public static float[] zBuffer { get; set; }

        public static double degree = 0.0;

        public static int bytesPerPixel = 3;
                                                             // Top
        public static Vector3[] cubePoints = new Vector3[] { new Vector3(-1, -1, -1),    // A, 0
                                                             new Vector3(1, -1, -1),     // B, 1
                                                             new Vector3(1, 1, -1),      // C, 2
                                                             new Vector3(-1, 1, -1),     // D, 3
                                                             // Bottom
                                                             new Vector3(-1, -1, 1),     // E, 4
                                                             new Vector3(1, -1, 1),      // F, 5
                                                             new Vector3(1, 1, 1),       // G, 6
                                                             new Vector3(-1, 1, 1)};     // H, 7

        public static Vector2[] textureCoord = {
                new Vector2(1,0),         // A, 0
                new Vector2(0,0),         // B, 1
                new Vector2(1,1),         // C, 2
                new Vector2(0,1),         // D, 3
                new Vector2(1,1),         // E, 4
                new Vector2(0,1),         // F, 5
                new Vector2(1,0),         // G, 6
                new Vector2(1,0)          // H, 7
            };                            

        public static Vector4[] normals = {
            Vector4.UnitZ,          // A, 0
            -Vector4.UnitZ,         // B, 1
            Vector4.UnitX,          // C, 2
            -Vector4.UnitY,         // D, 3
            Vector4.UnitZ,          // E, 4
            -Vector4.UnitX,         // F, 5
            Vector4.UnitX,          // G, 6
            Vector4.UnitY           // H, 7

        };

        public static Vector4[] rotatedNormals;

        public static List<Triangle> triangles = new List<Triangle>() {
                         // Points  (A,B,C) // Fix color        // A color             // B color             // C color                 
            new Triangle(new Vector3(0,1,2),new Vector3(1,0,0), new Vector4(0,1,0,1),  new Vector4(0,0,1,1),  new Vector4(1,0,0,1)), 
            new Triangle(new Vector3(0,2,3),new Vector3(1,0,0), new Vector4(0,1,0,1),  new Vector4(1,0,0,1),  new Vector4(0,0,1,1)), 
            new Triangle(new Vector3(7,6,5),new Vector3(0,1,0), new Vector4(0,1,0,1),  new Vector4(1,0,0,1),  new Vector4(0,0,1,1)), 
            new Triangle(new Vector3(7,5,4),new Vector3(0,1,0), new Vector4(0,1,0,1),  new Vector4(0,0,1,1),  new Vector4(1,0,1,1)), 
            new Triangle(new Vector3(0,3,7),new Vector3(0,0,1), new Vector4(0,1,0,1),  new Vector4(0,0,1,1),  new Vector4(1,0,0,1)), 
            new Triangle(new Vector3(0,7,4),new Vector3(0,0,1), new Vector4(0,1,0,1),  new Vector4(1,0,0,1),  new Vector4(0,0,1,1)), 
            new Triangle(new Vector3(2,1,5),new Vector3(1,1,0), new Vector4(1,0,0,1),  new Vector4(0,0,1,1),  new Vector4(0,0,1,1)), 
            new Triangle(new Vector3(2,5,6),new Vector3(1,1,0), new Vector4(1,0,0,1),  new Vector4(0,0,1,1),  new Vector4(0,1,0,1)), 
            new Triangle(new Vector3(3,2,6),new Vector3(1,0,1), new Vector4(0,0,1,1),  new Vector4(1,0,0,1),  new Vector4(0,1,0,1)), 
            new Triangle(new Vector3(3,6,7),new Vector3(1,0,1), new Vector4(0,0,1,1),  new Vector4(0,1,0,1),  new Vector4(1,0,0,1)), 
            new Triangle(new Vector3(1,0,4),new Vector3(0,1,1), new Vector4(0,0,1,1),  new Vector4(0,1,0,1),  new Vector4(1,0,0,1)), 
            new Triangle(new Vector3(1,4,5),new Vector3(0,1,1), new Vector4(0,0,1,1),  new Vector4(1,0,0,1),  new Vector4(0,1,0,1)) 
        };

        public static Vector3 v1 = new Vector3(0, 0, 5);
        Vector2[] trianglePoints = new Vector2[cubePoints.Length];

        public MainWindow()
        {
            image = new WriteableBitmap(400, 400, 96, 96, PixelFormats.Rgb24, null);
            colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            positionBuffer = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            textureBuffer = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            normalsBuffer = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            diffuseColorBuffer = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            deferredBuffer = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            zBuffer = new float[image.PixelHeight * image.PixelWidth];
            rotatedNormals = new Vector4[normals.Length];

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
            if (frameCounter++ == 0) { stopwatch.Start(); }

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
            Matrix4x4 rotMat = MatrixHelpers.GetXRotationMatrix(degree) *
                MatrixHelpers.GetYRotationMatrix(degree);

            Matrix4x4 invertedMatrix = new Matrix4x4();
            Matrix4x4.Invert(rotMat, out invertedMatrix);
            Matrix4x4 invertTransMatrix = Matrix4x4.Transpose(invertedMatrix);

            // Set and clean up
            Array.Clear(colourData, 0, colourData.Length);

            for (int i = 0; i < zBuffer.Length; i++)
            {
                zBuffer[i] = float.PositiveInfinity;
            }

            Vector3[] points_copy = (Vector3[])cubePoints.Clone(); 

            // Rotate cube by certain degree
            degree += 0.05f;

            for (int i = 0; i < cubePoints.Length; i++)
            {
                // Rotate with given matrix
                points_copy[i] = Vector3.Transform(points_copy[i], rotMat);
                rotatedNormals[i] = Vector4.Transform(normals[i], invertTransMatrix);

                // Translate all points
                points_copy[i] += v1;

                // Calc x' and y'
                trianglePoints[i] = new Vector2((int)(image.Width * points_copy[i].X / points_copy[i].Z + image.Width / 2), (int)(image.Width * points_copy[i].Y / points_copy[i].Z + image.Height / 2));
            }

            foreach (var triangle in triangles)
            {
                // 2D single triangle points
                Vector2 A = trianglePoints[(int)triangle._pointIdx.X];
                Vector2 B = trianglePoints[(int)triangle._pointIdx.Y];
                Vector2 C = trianglePoints[(int)triangle._pointIdx.Z];

                // Start Backface culling
                Vector3 AB = new Vector3(B.X - A.X, B.Y - A.Y, 0);
                Vector3 AC = new Vector3(C.X - A.X, C.Y - A.Y, 0);
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
                            Vector2 AP = new Vector2(x - A.X, y - A.Y);
                            float upper = AP.X * d + AP.Y * (-b);
                            float lower = AP.X * (-c) + AP.Y * a;
                            Vector2 vec = new Vector2(upper * 1 / det, lower * 1 / det);
                            float u = vec.X;
                            float v = vec.Y;

                            if (u >= 0 && v >= 0 && u + v < 1)
                            {
                                // Calc interpolated points and normals
                                Vector3 _a = points_copy[(int)triangle._pointIdx.X];
                                Vector3 _b = points_copy[(int)triangle._pointIdx.Y];
                                Vector3 _c = points_copy[(int)triangle._pointIdx.Z];

                                // Todo: Use coorect world coordinate
                                //Vector3 interpolatedPoint = GetInterpolatedWorldCoordinates(_a, _b, _c, u, v);
                                var ab = _b - _a;
                                var ac = _c - _a;
                                Vector3 interpolatedPoint = _a + u * ab + v * ac;



                                Vector3 interpolatedNormal = GetInterpolatedNormal(triangle, u, v);
                                Vector3 interpolatedTexture = GetInterpolatedTexture(triangle, u, v, interpolatedPoint.Z);
                                

                                //color = triangle._color;
                                //color = GetInterpolatedColor(triangle, u, v, interpolatedPoint.Z);
                                //color = bitmapTexturing.GetBitmapColor(interpolatedTexture);
                                color = bitmapTexturing.GetBitmapColorWithBilinearFiltering(interpolatedTexture);



                                // Specular/Phong
                                //color = GetDiffuseLight(interpolatedPoint, interpolatedNormal, color);
                                //color += GetSpecularLight(interpolatedPoint, interpolatedNormal);

                                // Z buffer
                                if (!float.IsInfinity(interpolatedPoint.Z))
                                {
                                    if (interpolatedPoint.Z < zBuffer[x + y * image.PixelHeight])
                                    {
                                        if (printZbuffer)
                                        {
                                            zBuffer[x + y * image.PixelHeight] = interpolatedPoint.Z;
                                            int Z_MIN = 0;
                                            int Z_MAX = 10;
                                            Vector3 zcolor = (interpolatedPoint.Z - Z_MIN) / (Z_MAX - Z_MIN) * byte.MaxValue * new Vector3(1f / byte.MaxValue, 1f / byte.MaxValue, 1f / byte.MaxValue);
                                            color = zcolor;
                                        }
                                    }
                                }
                                
                                if (printPositionBuffer)
                                {
                                    color = interpolatedPoint;
                                    positionBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                    positionBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                                    positionBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                                    colourData = positionBuffer;
                                }
                                if (printTextureCoordBuffer)
                                {
                                    color = interpolatedTexture;
                                    textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                    textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                                    textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                                    colourData = textureBuffer;
                                }
                                if (printNormalsBuffer)
                                {
                                    color = interpolatedNormal;
                                    normalsBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                    normalsBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                                    normalsBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                                    colourData = normalsBuffer;
                                }
                                if (printDiffuseColorBuffer)
                                {
                                    color = new Vector3(1, 1, 1);
                                    color = GetDiffuseLight(interpolatedPoint, interpolatedNormal, color);
                                    diffuseColorBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                    diffuseColorBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                                    diffuseColorBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                                    colourData = diffuseColorBuffer;
                                }

                                colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                                colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                                colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                            }
                        }
                    }
                }
            }

            // Deferred Rendering
            if (defferedRendering)
            {
                // Calc texture color out of textureBuffer
                for(int x = 0; x <image.PixelWidth; x++)
                {
                    for(int y = 0; y < image.PixelHeight; y++)
                    {
                        var a = textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel];
                        var b = textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1];
                        var c = textureBuffer[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2];

                        Vector3 interplatedTexture = new Vector3(a, b, c);

                        color = bitmapTexturing.GetBitmapColor(interplatedTexture);
                        colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.X);            // Red
                        colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Y);        // Green
                        colourData[x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2] = GammaCorrection.ConvertAndClampAndGammaCorrect(color.Z);        // Blue
                    }
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();
            img.Source = image;
        }

        public static Vector3 GetInterpolatedColor(Triangle t, float u, float v, float z)
        {
            Vector4 color = Vector4.Zero;

            color = t._colorA / z + u * (t._colorB / z - t._colorA / z) + v * (t._colorC / z - t._colorA / z);
            color = color / color.W;

            return new Vector3(color.X, color.Y, color.Z);
        }

        public static Vector3 GetInterpolatedWorldCoordinates(Vector3 a, Vector3 b, Vector3 c, float u, float v)
        {
            Vector3 world = Vector3.Zero;

            world = a + u * (b - a ) + v * (c - a);
            //world = world / world.Z;

            return world;
        }

        public static Vector3 GetInterpolatedTexture(Triangle t, float u, float v, float w)
        {
            Vector3 interpolatedTexture = Vector3.Zero;

            Vector3 vertexIdx = t._pointIdx;

            Vector3 a_texture = new Vector3(textureCoord[(int)vertexIdx.X] /w, 1/w);
            Vector3 b_texture = new Vector3(textureCoord[(int)vertexIdx.Y] /w, 1/w);
            Vector3 c_texture = new Vector3(textureCoord[(int)vertexIdx.Z]/ w, 1/w);

            interpolatedTexture = a_texture + u * (b_texture - a_texture) + v * (c_texture - a_texture);
            interpolatedTexture = interpolatedTexture / interpolatedTexture.Z;

            return interpolatedTexture;
        }

        public static Vector3 GetDiffuseLight(Vector3 interpolatedPoint, Vector3 interpolatedNormal, Vector3 sphereColor)
        {
            Vector3 l = Vector3.Normalize(Vector3.Subtract(lightPos, interpolatedPoint));
            float nL = Vector3.Dot(interpolatedNormal, l);

            Vector3 diffLight = Vector3.Zero;
            if (nL >= 0)
            {
                return diffLight = lightColor * sphereColor * nL;
            }

            return diffLight;
        }

        public static Vector3 GetSpecularLight(Vector3 interpolatedPoint, Vector3 interpolatedNormal)
        {
            Vector3 l = Vector3.Normalize(Vector3.Subtract(lightPos, interpolatedPoint));
            Vector3 s = l - Vector3.Dot(l, interpolatedNormal) * interpolatedNormal;
            Vector3 EH = Vector3.Normalize(Vector3.Subtract(_eye, interpolatedPoint));
            Vector3 r = Vector3.Normalize(l - 2 * s);
            float nL = Vector3.Dot(interpolatedNormal, l);

            Vector3 specularLight = Vector3.Zero;
            if (nL >= 0)
            {
                float phongFactor = Vector3.Dot(r, Vector3.Normalize(interpolatedPoint - _eye));
                specularLight = lightColor * (float)Math.Pow(phongFactor, _k);
            }

            return specularLight;
        }
        public static Vector3 GetInterpolatedNormal(Triangle triangle, float u, float v)
        {
            Vector4 interpolatedNormal = Vector4.Zero;

            Vector3 vertexIdx = triangle._pointIdx;
            Vector4 a_normal = rotatedNormals[(int)vertexIdx.X];
            Vector4 b_normal = rotatedNormals[(int)vertexIdx.Y];
            Vector4 c_normal = rotatedNormals[(int)vertexIdx.Z];

            interpolatedNormal = a_normal + u * (b_normal - a_normal) + v * (c_normal - a_normal);
            interpolatedNormal.W = 0;
            interpolatedNormal = Vector4.Normalize(interpolatedNormal);

            return new Vector3(interpolatedNormal.X, interpolatedNormal.Y, interpolatedNormal.Z);
        }
    }
}



