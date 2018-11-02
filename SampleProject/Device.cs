using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SampleProject
{
    public class Device
    {
        private byte[] backBuffer;
        private WriteableBitmap image;
        public static int bytesPerPixel = 3;

        public Device(WriteableBitmap bmp)
        {
            this.image = bmp;
            // the back buffer size is equal to the number of pixels to draw
            // on screen (width*height) * 4 (R,G,B & Alpha values). 
            backBuffer = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
        }

        // This method is called to clear the back buffer with a specific color
        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (var index = 0; index < backBuffer.Length; index += 4)
            {
                // BGRA is used by Windows instead by RGBA in HTML5
                backBuffer[index] = b;
                backBuffer[index + 1] = g;
                backBuffer[index + 2] = r;
                backBuffer[index + 3] = a;
            }
        }
        

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 
        public WriteableBitmap Present()
        {
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    backBuffer[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel)] = 255;            // Red
                    backBuffer[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1)] = 255;        // Blue
                    backBuffer[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2)] = 0;        // Green
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), backBuffer, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            return image;
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, System.Windows.Media.Color color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * image.PixelWidth) * 4;

            backBuffer[index] = (byte)(color.B * 255);
            backBuffer[index + 1] = (byte)(color.G * 255);
            backBuffer[index + 2] = (byte)(color.R * 255);
            backBuffer[index + 3] = (byte)(color.A * 255);
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        public Vector2 Project(Vector3 coord, Matrix4x4 transMat)
        {
            // transforming the coordinates
            var point = Vector3.Transform(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * image.PixelWidth + image.PixelWidth / 2.0f;
            var y = -point.Y * image.PixelHeight + image.PixelHeight / 2.0f;
            return (new Vector2(x, y));
        }

        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector2 point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < image.PixelWidth && point.Y < image.PixelHeight)
            {
                // Drawing a yellow point
                PutPixel((int)point.X, (int)point.Y, System.Windows.Media.Color.FromArgb(255, 255, 255, 0));
            }
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            // To understand this part, please read the prerequisites resources
            var viewMatrix = LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            var projectionMatrix = PerspectiveFovRH(0.78f,
                                                           (float)image.PixelWidth / image.PixelHeight,
                                                           0.01f, 1.0f);

            foreach (Mesh mesh in meshes)
            {
                // Beware to apply rotation before translation 
                var worldMatrix = RotationYawPitchRoll(mesh.Rotation.Y,
                                                              mesh.Rotation.X, mesh.Rotation.Z) *
                                  Translation(mesh.Position.X, mesh.Position.Y, mesh.Position.Z);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var vertex in mesh.Vertices)
                {
                    // First, we project the 3D coordinates into the 2D space
                    var point = Project(vertex, transformMatrix);
                    // Then we can draw on screen
                    DrawPoint(point);
                }
            }
        }

        

        /// <summary>
        /// Creates a left-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at matrix.</param>
        public static Matrix4x4 LookAtLH(Vector3 eye, Vector3 target, Vector3 up)
        {
            Matrix4x4 result = new Matrix4x4();
            Vector3 xaxis, yaxis, zaxis;
            zaxis = Vector3.Normalize(Vector3.Subtract(target,  eye));
            xaxis = Vector3.Normalize(Vector3.Cross( up, zaxis)); 

            yaxis = Vector3.Cross( zaxis,  xaxis);

            result = Matrix4x4.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            result.M41 = Vector3.Dot( xaxis,  eye);
            result.M42 = Vector3.Dot( yaxis,  eye);
            result.M43 = Vector3.Dot( zaxis, eye );

            result.M41 = -result.M41;
            result.M42 = -result.M42;
            result.M43 = -result.M43;

            return result;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static Matrix4x4 PerspectiveFovRH(float fov, float aspect, float znear, float zfar)
        {
            Matrix4x4 result = new Matrix4x4();
            float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
            float q = zfar / (znear - zfar);
            
            result.M11 = yScale / aspect;
            result.M22 = yScale;
            result.M33 = q;
            result.M34 = -1.0f;
            result.M43 = q * znear;
            return result;
        }

        /// <summary>
        /// Creates a rotation matrix with a specified yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw around the y-axis, in radians.</param>
        /// <param name="pitch">Pitch around the x-axis, in radians.</param>
        /// <param name="roll">Roll around the z-axis, in radians.</param>
        /// <param name="result">When the method completes, contains the created rotation matrix.</param>
        public static Matrix4x4 RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            Matrix4x4 result = new Matrix4x4();
            Quaternion quaternion = new Quaternion();
            result = RotationQuaternion(quaternion);

            return result;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="rotation">The quaternion to use to build the matrix.</param>
        /// <param name="result">The created rotation matrix.</param>
        public static Matrix4x4 RotationQuaternion( Quaternion rotation)
        {
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            Matrix4x4 result = Matrix4x4.Identity;
            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (zz + xx));
            result.M23 = 2.0f * (yz + xw);
            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (yy + xx));
            return result;
        }

        /// <summary>
        /// Creates a translation matrix using the specified offsets.
        /// </summary>
        /// <param name="x">X-coordinate offset.</param>
        /// <param name="y">Y-coordinate offset.</param>
        /// <param name="z">Z-coordinate offset.</param>
        /// <param name="result">When the method completes, contains the created translation matrix.</param>
        public static Matrix4x4 Translation(float x, float y, float z)
        {
            Matrix4x4 result = Matrix4x4.Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
            return result;
        }

    }
}
