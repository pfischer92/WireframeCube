using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class MatrixHelpers
    {
        public static Matrix4x4 GetXRotationMatrix(double degree)
        {
            Matrix4x4 rotMatX = new Matrix4x4();
            rotMatX.M11 = 1;
            rotMatX.M22 = (float)Math.Cos(degree);
            rotMatX.M23 = -(float)Math.Sin(degree);
            rotMatX.M32 = (float)Math.Sin(degree);
            rotMatX.M33 = (float)Math.Cos(degree);
            rotMatX.M44 = 1;
            return rotMatX;
        }

        public static Matrix4x4 GetYRotationMatrix(double degree)
        {
            Matrix4x4 rotMatY = new Matrix4x4();
            rotMatY.M11 = (float)Math.Cos(degree);
            rotMatY.M13 = (float)Math.Sin(degree);
            rotMatY.M22 = 1;
            rotMatY.M31 = -(float)Math.Sin(degree);
            rotMatY.M33 = (float)Math.Cos(degree);
            rotMatY.M44 = 1;
            return rotMatY;
        }
        public static Matrix4x4 GetZRotationMatrix(double degree)
        {
            Matrix4x4 rotMatZ = new Matrix4x4();
            rotMatZ.M11 = (float)Math.Cos(degree);
            rotMatZ.M12 = -(float)Math.Sin(degree);
            rotMatZ.M21 = (float)Math.Sin(degree);
            rotMatZ.M22 = -(float)Math.Cos(degree);
            rotMatZ.M33 = 1;
            rotMatZ.M44 = 1;
            return rotMatZ;
        }
    }
}
