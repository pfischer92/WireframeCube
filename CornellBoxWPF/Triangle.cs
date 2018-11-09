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
        public Vector4 _colorA { get; set; }
        public Vector4 _colorB { get; set; }
        public Vector4 _colorC { get; set; }
        public Triangle(Vector3 pointIdx, Vector3 fixColor, Vector4 colorA, Vector4 colorB, Vector4 colorC)
        {
            _pointIdx = pointIdx;
            _color = fixColor;
            _colorA = colorA;
            _colorB = colorB;
            _colorC = colorC;
        }

    }
}
