using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class Triangle
    {
        public Vector3 _pointIdx { get; set; }
        public Vector3 _color { get; set; }

        public Triangle(Vector3 pointIdx, Vector3 color)
        {
            _pointIdx = pointIdx;
            _color = color;
        }
    }
}
