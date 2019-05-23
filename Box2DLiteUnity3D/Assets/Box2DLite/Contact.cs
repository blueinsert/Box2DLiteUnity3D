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
        Vec2 position;
        Vec2 normal;
        Vec2 r1, r2;
        float separation;
        float pn;
        float pt;
        float pnb;
        float bias;
        FeaturePair feature;
    }
}
