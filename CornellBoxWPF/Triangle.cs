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
        public Vector4 _normal { get; set; }
        
        public float _sA { get; set; }
        public float _tA { get; set; }
        public float _sB { get; set; }
        public float _tB { get; set; }
        public float _sC { get; set; }
        public float _tC { get; set; }

        public Triangle(Vector3 pointIdx, Vector3 fixColor, Vector4 colorA, float sA, float tA, Vector4 colorB, float sB, float tB, Vector4 colorC, float sC, float tC, Vector4 normal)
        {
            _pointIdx = pointIdx;
            _color = fixColor;
            _colorA = colorA;
            _sA = sA;
            _tA = tA;
            _colorB = colorB;
            _sB = sB;
            _tB = tB;
            _colorC = colorC;
            _sC = sC;
            _tC = tC;
            _normal = normal;
        }
    }
}
