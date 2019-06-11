using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vec2 = UnityEngine.Vector2;

namespace bluebean.Box2DLite
{
    /// <summary>
    /// 顶点特征数据是一个四元组(in1,out1,in2,out2),
    /// 由形成顶点的两条边来表征
    /// 没被裁剪的顶点的特征数据是（in1,out1,_,_)或者（_,_,in2,out2)
    /// 被裁剪的顶点由两个Box的两条边形成
    /// 内容为(in1,_,_,out2)或者(_,out1,in2,_)
    /// </summary>
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

    /// <summary>
    /// 接触类，由Arbiter类持有
    /// </summary>
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
