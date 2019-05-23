using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public static class MathUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
