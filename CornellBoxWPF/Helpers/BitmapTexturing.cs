﻿using CornellBoxWPF.Helpers;
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
        public Vector3 GetBitmapColor(float u, float v, float w, Triangle triangle)
        {
            Vector3 color = Vector3.Zero;
            Vector3 colorA = new Vector3(triangle._sA, triangle._tA, 1) / w;
            Vector3 colorB = new Vector3(triangle._sB, triangle._tB, 1) / w;
            Vector3 colorC = new Vector3(triangle._sC, triangle._tC, 1) / w;

            color = colorA + u * (colorB - colorA) + v * (colorC - colorA) / w;

            // Get Color from bitmap
            int s = (int)(color.X * bitmap.Width) & (bitmap.Width - 1);
            int t = (int)(color.Y * bitmap.Height) & (bitmap.Height - 1);
            
            Color c = bitmap.GetPixel(s, t);
            Vector3 bitMapColor = new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c.B));
            return bitMapColor;
        }
    }
}

