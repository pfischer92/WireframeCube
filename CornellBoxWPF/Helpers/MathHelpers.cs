using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.Helpers
{
    public class MathHelpers
    {
        public static int Highest(params int[] inputs)
        {
            return inputs.Max();
        }

        public static int Lowest(params int[] inputs)
        {
            return inputs.Min();
        }

        // Copied from java Math library
        public static int floorDiv(int x, int y)
        {
            int r = x / y;
            // if the signs are different and modulo not zero, round down
            if ((x ^ y) < 0 && (r * y != x))
            {
                r--;
            }
            return r;
        }

        public static int floorMod(int x, int y)
        {
            return x - floorDiv(x, y) * y;
        }
    }
}
