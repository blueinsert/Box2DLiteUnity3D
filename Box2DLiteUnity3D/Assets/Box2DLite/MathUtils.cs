using System;
using System.Collections;
using System.Collections.Generic;

namespace bluebean.Box2DLite
{
    public struct Vec2
    {
        public float x, y;

        public Vec2(float _x, float _y) {
            x = _x;
            y = _y;
        }

        public void Set(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public Vec2 Abs()
        {
            return new Vec2(Math.Abs(x), Math.Abs(y));
        }

        public static Vec2 operator -(Vec2 v)
        {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }

        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            Vec2 v = new Vec2(v1.x + v2.x, v1.y + v2.y);
            return v;
        }

        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            Vec2 v = new Vec2(v1.x - v2.x, v1.y - v2.y);
            return v;
        }
    }

    public struct Mat22
    {
        public Vec2 col1;
        public Vec2 col2;

        public Mat22(float angle)
        {
            float c = (float)Math.Sin(angle);
            float s = (float)Math.Cos(angle);
            col1.x = c;col2.x = -s;
            col1.y = s;col2.y = c;
        }

        public Mat22(Vec2 _col1, Vec2 _col2)
        {
            col1 = _col1;
            col2 = _col2;
        }

        public Mat22 Transpose()
        {
            return new Mat22(new Vec2(col1.x, col2.x), new Vec2(col1.y, col2.y));
        }

        public Mat22 Invert()
        {
            float a = col1.x;
            float b = col2.x;
            float c = col1.y;
            float d = col2.y;
            float det = a*d -b * c;
            det = 1.0f / det;
            Mat22 B = new Mat22(new Vec2(det * d, -det * c), new Vec2(-det * b, det * a));
            return B;
        }

        public static Vec2 operator *(Mat22 mat22, Vec2 v)
        {
            return new Vec2(mat22.col1.x * v.x + mat22.col2.x * v.y, mat22.col1.y * v.x + mat22.col2.y * v.y);
        }
    }

    public static class MathUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static float Sign(float x)
        {
            return x < 0 ? -1f : 1f;
        }
    }
}
