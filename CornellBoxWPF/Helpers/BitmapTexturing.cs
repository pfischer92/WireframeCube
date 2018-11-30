using CornellBoxWPF.Helpers;
using System;
using System.Drawing;
using System.Numerics;
using WireframeCube.Helpers;

namespace CornellBoxWPF.BitmapHelper
{
    public class BitmapTexturing
    {
        Bitmap bitmap;
        public BitmapTexturing()
        {
            bitmap = (Bitmap)Image.FromFile("sample-bitmap.bmp");
        }
        public Vector3 GetBitmapColor(Vector3 interpolatedTexture)
        {
            // Get Color from bitmap
            int s = (int)(interpolatedTexture.X * bitmap.Width) & (bitmap.Width - 1);
            int t = (int)(interpolatedTexture.Y * bitmap.Height) & (bitmap.Height - 1);

            Color c = bitmap.GetPixel(s, t);
            Vector3 bitMapColor = new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c.B));
            return bitMapColor;
        }

        public Vector3 GetBitmapColorWithBilinearFiltering(Vector3 interpolatedPoint, float u, float v)
        {
            return Vector3.One;
        }
    }
}

