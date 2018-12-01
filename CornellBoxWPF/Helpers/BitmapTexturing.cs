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

        public Vector3 GetBitmapColorWithBilinearFiltering(Vector3 interpolatedTexture)
        {
            // Get Color from bitmap
            int h = bitmap.Height;
            int w = bitmap.Width;
            float u = interpolatedTexture.X;
            float v = interpolatedTexture.Y;

            if (u < 0)
            {
                u = u + 1;
            }
            if (v < 0)
            {
                v = v + 1;
            }

            float u_scaled = u * w;
            float v_scaled = v * h;


            /*   c1   c2   
                 c3   c4
                            */
            int s1 = MathHelpers.floorMod((int)u_scaled, w);
            int t1 = MathHelpers.floorMod((int)v_scaled + 1, h);
            int s2 = MathHelpers.floorMod((int)u_scaled + 1, w);
            int t2 = MathHelpers.floorMod((int)v_scaled, h);

            Color c1 = bitmap.GetPixel(s1, t1);
            Color c2 = bitmap.GetPixel(s2, t1);
            Color c3 = bitmap.GetPixel(s1, t2);
            Color c4 = bitmap.GetPixel(s2, t2);


            Vector3 left_interpolation = Vector3.Lerp(
                 new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c1.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c1.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c1.B)),
                 new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c3.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c3.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c3.B)),
                 u_scaled - (int)u_scaled);

            Vector3 right_interpolation = Vector3.Lerp(
                    new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c2.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c2.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c2.B)),
                    new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c4.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c4.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c4.B)),
                    u_scaled - (int)u_scaled);

            Vector3 final_interpolation = Vector3.Lerp(left_interpolation, right_interpolation,
                    v_scaled - (int)v_scaled);

            return final_interpolation;
            
        }
    }
}

