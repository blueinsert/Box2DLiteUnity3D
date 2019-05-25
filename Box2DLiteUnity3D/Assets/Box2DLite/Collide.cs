using System.Collections;
using System.Collections.Generic;
using Vec2 = UnityEngine.Vector2;

namespace bluebean.Box2DLite
{
  
    // Box vertex and edge numbering:
    //
    //        ^ y
    //        |
    //        e1
    //   v2 ------ v1
    //    |        |
    // e2 |        | e4  --> x
    //    |        |
    //   v3 ------ v4
    //        e3

    public enum Axis
    {
        FACE_A_X,
        FACE_A_Y,
        FACE_B_X,
        FACE_B_Y,
    }

    public enum EdgeNumbers
    {
        NO_EDGE = 0,
        EDGE1,
        EDGE2,
        EDGE3,
        EDGE4,
    }

    public struct ClipVertex
    {
        public Vec2 v;
        public FeaturePair feature;
    }

    public static class Collide
    {
        public static void Flip(ref FeaturePair fp)
        {
            MathUtils.Swap<char>(ref fp.inEdge1, ref fp.inEdge2);
            MathUtils.Swap<char>(ref fp.outEdge1, ref fp.outEdge2);
        }

        /// <summary>
        /// 返回incident box的碰撞边的顶点
        /// </summary>
        /// <param name="clipVertices">返回的incident box的碰撞边的顶点</param>
        /// <param name="size">incident box的尺寸</param>
        /// <param name="pos">incident box的位置</param>
        /// <param name="rot">incident box的旋转</param>
        /// <param name="normal">指向incident box的碰撞法线</param>
        static void ComputeIncidentEdge(out ClipVertex[] clipVertices, Vec2 size, Vec2 pos, Mat22 rot, Vec2 normal)
        {
            clipVertices = new ClipVertex[2];
            //将碰撞法线转换到incident box的本地坐标系
            Mat22 RotT = rot.Transpose();
            Vec2 n = -(rot * normal);
            Vec2 nAbs = n.Abs();
            //todo 貌似有bug,如果不是正方形
            if(nAbs.x > nAbs.y)
            {
                if (MathUtils.Sign(n.x) > 0)
                {
                    //边4
                    clipVertices[0].v.Set(size.x, -size.y);
                    clipVertices[0].feature.inEdge2 = (char)EdgeNumbers.EDGE3;
                    clipVertices[0].feature.outEdge2 = (char)EdgeNumbers.EDGE4;
                    clipVertices[1].v.Set(size.x, size.y);
                    clipVertices[1].feature.inEdge2 = (char)EdgeNumbers.EDGE4;
                    clipVertices[1].feature.outEdge2 = (char)EdgeNumbers.EDGE1;
                }
                else
                {
                    //边2
                    clipVertices[0].v.Set(-size.x, size.y);
                    clipVertices[0].feature.inEdge2 = (char)EdgeNumbers.EDGE1;
                    clipVertices[0].feature.outEdge2 = (char)EdgeNumbers.EDGE2;
                    clipVertices[1].v.Set(-size.x, -size.y);
                    clipVertices[1].feature.inEdge2 = (char)EdgeNumbers.EDGE2;
                    clipVertices[1].feature.outEdge2 = (char)EdgeNumbers.EDGE3;
                }
            }
            else
            {
                if (MathUtils.Sign(n.y) > 0)
                {
                    //边1
                    clipVertices[0].v.Set(size.x, size.y);
                    clipVertices[0].feature.inEdge2 = (char)EdgeNumbers.EDGE4;
                    clipVertices[0].feature.outEdge2 = (char)EdgeNumbers.EDGE1;
                    clipVertices[1].v.Set(-size.x, size.y);
                    clipVertices[1].feature.inEdge2 = (char)EdgeNumbers.EDGE1;
                    clipVertices[1].feature.outEdge2 = (char)EdgeNumbers.EDGE2;
                }
                else
                {
                    //边3
                    clipVertices[0].v.Set(-size.x, -size.y);
                    clipVertices[0].feature.inEdge2 = (char)EdgeNumbers.EDGE2;
                    clipVertices[0].feature.outEdge2 = (char)EdgeNumbers.EDGE3;
                    clipVertices[1].v.Set(size.x, -size.y);
                    clipVertices[1].feature.inEdge2 = (char)EdgeNumbers.EDGE3;
                    clipVertices[1].feature.outEdge2 = (char)EdgeNumbers.EDGE4;
                }
            }
            //转换到世界坐标系
            clipVertices[0].v = pos + rot * clipVertices[0].v;
            clipVertices[1].v = pos + rot * clipVertices[1].v;
        }
    }
}
