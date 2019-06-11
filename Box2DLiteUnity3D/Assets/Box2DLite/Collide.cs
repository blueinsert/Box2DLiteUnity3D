using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    /// <summary>
    /// 分离轴
    /// </summary>
    public enum Axis
    {
        FACE_A_X,
        FACE_A_Y,
        FACE_B_X,
        FACE_B_Y,
    }
    
    /// <summary>
    /// 边编号
    /// </summary>
    public enum EdgeNumbers
    {
        NO_EDGE = 0,
        EDGE1,
        EDGE2,
        EDGE3,
        EDGE4,
    }

    /// <summary>
    /// 裁剪顶点
    /// </summary>
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
        /// 将incidentBox上的碰撞边数据根据ReferenceBox sideNormal上两条边进行裁剪
        /// </summary>
        /// <param name="vOut">输出的碰撞边数据</param>
        /// <param name="vIn">输入的碰撞边数据</param>
        /// <param name="normal">sideNormal</param>
        /// <param name="offset">裁剪依据边上的点在sideNormal上投影长度</param>
        /// <param name="clipEdge">裁剪依据边编号</param>
        /// <returns></returns>
        static int ClipSegmentToLine(ClipVertex[] vOut, ClipVertex[] vIn, Vec2 normal, float offset, EdgeNumbers clipEdge)
        {
            // Start with no output points
            int numOut = 0;

            // Calculate the distance of end points to the line
            // 如果是正数，说明碰撞边顶点越过了裁剪依据边，需要被裁剪
            float distance0 = Vec2.Dot(normal, vIn[0].v) - offset;
            float distance1 = Vec2.Dot(normal, vIn[1].v) - offset;

            // If the points are behind the plane
            if (distance0 <= 0.0f) vOut[numOut++] = vIn[0];
            if (distance1 <= 0.0f) vOut[numOut++] = vIn[1];

            // If the points are on different sides of the plane
            if (distance0 * distance1 < 0.0f)
            {
                // Find intersection point of edge and plane
                float interp = distance0 / (distance0 - distance1);
                vOut[numOut].v = vIn[0].v +(vIn[1].v - vIn[0].v)* interp;
                //重新设置碰撞点的特征数据
                //特征数据是一个四元组(in1,out1,in2,out2),被裁剪的顶点由两个Box的两条边形成
                //todo 感觉有bug，顶点顺序保证是逆时针时，判断之后设置特征数据的操作才有意义
                //另一方面，实际不会发生碰撞边被两条sideNormal的边同时裁剪的情况，最多一条
                //在这种情况下，两个裁剪操作中，一个保持原状输出
                //另一个进行了裁剪，顶点顺序可能会发生改变，但这些顶点不会再次进入这个函数
                //所以实际不会出现有bug的情况
                if (distance0 > 0.0f)
                {
                    //（in1,_,_,out2)
                    vOut[numOut].feature = vIn[0].feature;
                    vOut[numOut].feature.inEdge1 = (char)clipEdge;
                    vOut[numOut].feature.inEdge2 = (char)EdgeNumbers.NO_EDGE;//清空
                }
                else
                {
                    //(_,out1,in2,_)
                    vOut[numOut].feature = vIn[1].feature;
                    vOut[numOut].feature.outEdge1 = (char)clipEdge;
                    vOut[numOut].feature.outEdge2 = (char)EdgeNumbers.NO_EDGE;//清空
                }
                ++numOut;
            }
            return numOut;
        }

        /// <summary>
        /// 返回incident box的碰撞边的顶点,顶点设置顺序保证是逆时针
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
            Vec2 n = -(RotT * normal);
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
        
        /// <summary>
        /// 通过轴分离算法得到bodyA和bodyB的碰撞点
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="bodyA"></param>
        /// <param name="bodyB"></param>
        /// <returns></returns>
        public static int CollideTest(Contact[] contacts, Body bodyA, Body bodyB)
        {
            // Setup
            Vec2 hA = bodyA.m_size * 0.5f;//half size
            Vec2 hB = bodyB.m_size * 0.5f;

            Vec2 posA = bodyA.m_position;
            Vec2 posB = bodyB.m_position;

            Mat22 RotA = new Mat22(bodyA.m_rotation);
            Mat22 RotB = new Mat22(bodyB.m_rotation);

            //对于二维旋转矩阵，矩阵的转置等于矩阵的逆
            Mat22 RotAT = RotA.Transpose();
            Mat22 RotBT = RotB.Transpose();

            Vec2 dp = posB - posA;//全局坐标系中，由BodyA指向BodyB的向量
            Vec2 dA = RotAT * dp;//BodyA的本地坐标系中，由BodyA指向BodyB的向量
            Vec2 dB = RotBT * dp;//BodyB的本地坐标系中，由BodyA指向BodyB的向量

            Mat22 C = RotAT * RotB;
            Mat22 absC = C.Abs();
            Mat22 absCT = absC.Transpose();

            //确定不相交，提前退出的情形
            // Box A faces
            //absC*hB会得到B在A的本地坐标系中的轴对齐外接矩形的halfSize
            //DebugDraw.Instance.DrawBox(bodyB.m_position, absC * hB * 2, 0, Color.gray);
            Vec2 faceA =  dA.Abs() - hA - absC * hB;
            if (faceA.x > 0.0f || faceA.y > 0.0f)
                return 0;

            // Box B faces
            //absCT * hA会得到A在B的本地坐标系中的轴对齐外接矩形的halfSize
            Vec2 faceB = dB.Abs() - absCT * hA - hB;
            if (faceB.x > 0.0f || faceB.y > 0.0f)
                return 0;
            //Step 1
            // Find best axis
            //寻找最小分离轴
            Axis axis;
            float separation;
            Vec2 normal;

            // Box A faces
            axis = Axis.FACE_A_X;
            separation = faceA.x;
            //B在A的右方，分离法线是A的X轴方向，由A指向B
            //或者B在A的左方，分离法线是A的-X轴方向，由A指向B
            normal = dA.x > 0.0f ? RotA.col1 : -RotA.col1;

            //const float relativeTol = 0.95f;
            //const float absoluteTol = 0.01f;

            if (faceA.y > separation)
            //if (faceA.y > relativeTol * separation + absoluteTol * hA.y)
            {
                axis = Axis.FACE_A_Y;
                separation = faceA.y;
                //B在A的上方，分离法线是A的Y轴方向，由A指向B
                //或者B在A的下方，分离法线是A的-Y轴方向，由A指向B
                normal = dA.y > 0.0f ? RotA.col2 : -RotA.col2;
            }

            // Box B faces
            if (faceB.x > separation)
            //if (faceB.x > relativeTol * separation + absoluteTol * hB.x)
            {
                axis = Axis.FACE_B_X;
                separation = faceB.x;
                //B在A的右方，分离法线是B的X轴方向，由A指向B
                //或者B在A的左方，分离法线是A的-X轴方向，由A指向B
                normal = dB.x > 0.0f ? RotB.col1 : -RotB.col1;
            }

            if (faceB.y > separation)
            //if (faceB.y > relativeTol * separation + absoluteTol * hB.y)
            {
                axis =  Axis.FACE_B_Y;
                separation = faceB.y;
                //B在A的上方，分离法线是B的Y轴方向，由A指向B
                //或者B在A的下方，分离法线是B的-Y轴方向，由A指向B
                normal = dB.y > 0.0f ? RotB.col2 : -RotB.col2;
            }
            // Step 2
            // Setup clipping plane data based on the separating axis
            //根据最小分离轴，决定了一个是referenceBox,另一个是incidentBox
            //获取incident上的碰撞边的数据
            //由referenceBox指向incidnetBox的法线
            Vec2 frontNormal = new Vec2();
            //垂直于frontNormal的方向
            Vec2 sideNormal = new Vec2();
            //incident上的碰撞边数据
            ClipVertex[] incidentEdge = new ClipVertex[2];
            //为了支持将IncidentBox的碰撞边根据referenceBox sideNormal方向上的两条边裁剪的数据
            float front = 0; //refenceBox的frontNormal方向上的边上的点在frontNormal方向上的投影长度
            float negSide = 0;//refenceBox的sideNormal方向上的边上的点在sideNormal方向上的投影长度
            float posSide = 0;//refenceBox的-sideNormal方向上的边上的点在-sideNormal方向上的投影长度
            //ReferenceBox的两条裁剪边编号
            EdgeNumbers negEdge = EdgeNumbers.NO_EDGE;
            EdgeNumbers posEdge = EdgeNumbers.NO_EDGE;

            // Compute the clipping lines and the line segment to be clipped.
            switch (axis)
            {
                case Axis.FACE_A_X:
                    {
                        frontNormal = normal;
                        front = Vec2.Dot(posA, frontNormal) + hA.x;
                        sideNormal = RotA.col2;
                        float side = Vec2.Dot(posA, sideNormal);
                        negSide = -side + hA.y;
                        posSide = side + hA.y;
                        negEdge = EdgeNumbers.EDGE3;
                        posEdge = EdgeNumbers.EDGE1;
                        ComputeIncidentEdge(out incidentEdge, hB, posB, RotB, frontNormal);
                    }
                    break;

                case Axis.FACE_A_Y:
                    {
                        frontNormal = normal;
                        front = Vec2.Dot(posA, frontNormal) + hA.y;
                        sideNormal = RotA.col1;
                        float side = Vec2.Dot(posA, sideNormal);
                        negSide = -side + hA.x;
                        posSide = side + hA.x;
                        negEdge = EdgeNumbers.EDGE2;
                        posEdge = EdgeNumbers.EDGE4;
                        ComputeIncidentEdge(out incidentEdge, hB, posB, RotB, frontNormal);
                    }
                    break;

                case  Axis.FACE_B_X:
                    {
                        frontNormal = -normal;
                        front = Vec2.Dot(posB, frontNormal) + hB.x;
                        sideNormal = RotB.col2;
                        float side = Vec2.Dot(posB, sideNormal);
                        negSide = -side + hB.y;
                        posSide = side + hB.y;
                        negEdge = EdgeNumbers.EDGE3;
                        posEdge = EdgeNumbers.EDGE1;
                        ComputeIncidentEdge(out incidentEdge, hA, posA, RotA, frontNormal);
                    }
                    break;

                case Axis.FACE_B_Y:
                    {
                        frontNormal = -normal;
                        front = Vec2.Dot(posB, frontNormal) + hB.y;
                        sideNormal = RotB.col1;
                        float side = Vec2.Dot(posB, sideNormal);
                        negSide = -side + hB.x;
                        posSide = side + hB.x;
                        negEdge = EdgeNumbers.EDGE2;
                        posEdge = EdgeNumbers.EDGE4;
                        ComputeIncidentEdge(out incidentEdge, hA, posA, RotA, frontNormal);
                    }
                    break;
            }
            foreach (var clipVertex in incidentEdge)
            {
               //DebugDraw.Instance.DrawPoint(clipVertex.v, new Color(1,0,0,0.1f));
            }
            //Step 3
            // clip other face with 5 box planes (1 face plane, 4 edge planes)
            //将incidentBox碰撞边数据根据referenceBox的两个裁剪边进行裁剪
            ClipVertex[] clipPoints1 = new ClipVertex[2];
            ClipVertex[] clipPoints2 = new ClipVertex[2];
            int np;

            // Clip to negative box side 1
            np = ClipSegmentToLine(clipPoints1, incidentEdge, -sideNormal, negSide, negEdge);

            if (np < 2)
                return 0;

            // Clip to positive box side 1
            np = ClipSegmentToLine(clipPoints2, clipPoints1, sideNormal, posSide, posEdge);

            if (np < 2)
                return 0;
            foreach (var clipVertex in clipPoints2)
            {
                //DebugDraw.Instance.DrawPoint(clipVertex.v, new Color(1, 0, 0, 0.5f));
            }
            // Now clipPoints2 contains the clipping points.
            // Due to roundoff, it is possible that clipping removes all points.

            int numContacts = 0;
            for (int i = 0; i < 2; ++i)
            {
                 separation = Vec2.Dot(frontNormal, clipPoints2[i].v) - front;

                if (separation <= 0)
                {
                    var contact = contacts[numContacts];
                    contact.m_separation = separation;//是一个负数
                    contact.m_normal = normal;//由BodyA指向BodyB
                    // slide contact point onto reference face (easy to cull)
                    //接触点位于参考Box的表面
                    contact.m_position = clipPoints2[i].v - frontNormal * separation;
                    contact.m_feature = clipPoints2[i].feature;
                    if (axis == Axis.FACE_B_X || axis == Axis.FACE_B_Y)
                        Flip(ref contact.m_feature);
                    contacts[numContacts] = contact;
                    ++numContacts;
                }
            }
            return numContacts;
        }

    }

}
