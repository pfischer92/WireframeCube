using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.Helpers
{
    public class GammaCorrection
    {
        public static float GAMMA = 2.2f;
        public static byte ConvertAndClampAndGammaCorrect(float color)
        {
            float gamma_corrected = (float)Math.Pow(color, 1f / GAMMA);
            int x = (int)Math.Round(255 * gamma_corrected, 0);

            if (x > 255) return 255;
            if (x < 0) return 0;
            return (byte)x;
        }
    }
}
