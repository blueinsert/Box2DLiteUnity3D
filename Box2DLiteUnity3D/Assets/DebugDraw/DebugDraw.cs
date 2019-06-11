using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vec2 = bluebean.Box2DLite.Vec2;

namespace bluebean.Box2DLite
{
    public struct DDVertex
    {
        public Color m_color;
        public Vec2[] m_positions;

        public static DDVertex FromLine(Vec2 begin, Vec2 end, Color color)
        {
            DDVertex ddv = new DDVertex();
            ddv.m_positions = new Vec2[2];
            ddv.m_positions[0] = begin;
            ddv.m_positions[1] = end;
            ddv.m_color = color;
            return ddv;
        }

        public static DDVertex FromPoint(Vec2 p, Color color)
        {
            DDVertex ddv = new DDVertex();
            ddv.m_positions = new Vec2[2];
            ddv.m_positions[0] = p;
            ddv.m_color = color;
            return ddv;
        }
    }

    public class DebugDraw
    {
        private DebugDraw()
        {
        }
        private static DebugDraw m_instance = new DebugDraw();
        public static DebugDraw Instance { get { return m_instance; } }

        /// <summary>
        /// 材质
        /// </summary>
        private  Material lineMaterial;

        private List<DDVertex> m_vertexListBatch = new List<DDVertex>();
        private List<DDVertex> m_lineListBatch = new List<DDVertex>();

        private void CreateLineMaterial()
        {
            //如果材质球不存在
            if (!lineMaterial)
            {
                //用代码的方式实例一个材质球
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                //设置参数
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //设置参数
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                //设置参数
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        public void Clear()
        {
            m_vertexListBatch.Clear();
            m_lineListBatch.Clear();
        }

        public static void _DrawPoint(Vector2 pos, float radius, int segment, Color color)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);
            for (int i = 0; i < segment; i++)
            {
                float angle1 = i / (float)segment * 360f;
                float angle2 = ((i + 1) == segment ? 0 : i + 1) / (float)segment * 360f;
                var pos1 = new Vector2(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad)) * radius + pos;
                var pos2 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad)) * radius + pos;
                GL.Vertex(pos);
                GL.Vertex(pos1);
                GL.Vertex(pos2);
            }
            GL.End();
        }

        private static void _DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.End();
        }

        public void DrawBatch()
        {
            //保存之前的变换矩阵
            GL.PushMatrix();
            //设置材质
            CreateLineMaterial();
            lineMaterial.SetPass(0);
            //设置位置变换矩阵
            //GL.LoadIdentity();
            //GL.LoadOrtho();
            //draw point
            foreach (var ddVertex in m_vertexListBatch)
            {
                _DrawPoint(new Vector2(ddVertex.m_positions[0].x, ddVertex.m_positions[0].y), 0.04f, 36,
                    ddVertex.m_color);
            }
            //draw line
            foreach (var ddVertex in m_lineListBatch)
            {
                var p1 = ddVertex.m_positions[0];
                var p2 = ddVertex.m_positions[1];
                _DrawLine(new Vector2(p1.x, p1.y),new Vector2(p2.x,p2.y), ddVertex.m_color);
            }
            GL.PopMatrix();
        }

        public void DrawLine(Vec2 p1, Vec2 p2, Color color)
        {
            m_lineListBatch.Add(DDVertex.FromLine(p1, p2, color));
        }

        public void DrawBox(Vec2 center, Vec2 size, float rotation, Color color)
        {
            Mat22 mR = new Mat22(rotation);
            Vec2 halfSize = size * 0.5f;
            Vec2 p1 = new Vec2(halfSize.x, halfSize.y);
            Vec2 p2 = new Vec2(-halfSize.x, halfSize.y);
            Vec2 p3 = new Vec2(-halfSize.x, -halfSize.y);
            Vec2 p4 = new Vec2(halfSize.x, -halfSize.y);
            p1 = mR * p1;
            p2 = mR * p2;
            p3 = mR * p3;
            p4 = mR * p4;
            p1 += center;
            p2 += center;
            p3 += center;
            p4 += center;
            DrawLine(p1, p2, color);
            DrawLine(p2, p3, color);
            DrawLine(p3, p4, color);
            DrawLine(p4, p1, color);
        }

        public void DrawPoint(Vec2 point, Color color)
        {
            m_vertexListBatch.Add(DDVertex.FromPoint(point, color));
        }

        public void DrawSingleArrowLine(Vec2 start, Vec2 end,float arrowLen, Color color)
        {
            DrawLine(start, end, color);
            var middle = (start + end) * 0.5f;
            var n = (end - start).Normalize();
            var dir1 = new Mat22(30 / 180f * 3.14159f) * n;
            var dir2 = new Mat22(-30 / 180f * 3.14159f) * n;
            var p1 = end - dir1 * arrowLen;
            var p2 = end - dir2 * arrowLen;
            DrawLine(p1, end, color);
            DrawLine(p2, end, color);
        }
    }
}
