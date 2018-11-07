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
    }
}
