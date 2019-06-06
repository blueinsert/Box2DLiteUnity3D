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

    public class Contact
    {
        public Vec2 m_position;
        public Vec2 m_normal;
        public Vec2 m_r1, m_r2;
        public float m_separation;
        public float m_pn;
        public float m_pt;
        public float m_pnb;
        public float m_bias;
        public float m_massNormal,m_massTangent;
        public FeaturePair m_feature;
    }
}
