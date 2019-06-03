using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vec2 = UnityEngine.Vector2;

namespace bluebean.Box2DLite
{
    [StructLayout(LayoutKind.Explicit, Size =4)]
    public struct FeaturePair
    {
        [FieldOffset(0)]
        public char inEdge1;
        [FieldOffset(1)]
        public char outEdge1;
        [FieldOffset(2)]
        public char inEdge2;
        [FieldOffset(3)]
        public char outEdge2;
        [FieldOffset(0)]
        public int value;
    }

    public struct Contact
    {
        public Vec2 position;
        public Vec2 normal;
        public Vec2 r1, r2;
        public float separation;
        public float pn;
        public float pt;
        public float pnb;
        public float bias;
        public FeaturePair feature;
    }
}
