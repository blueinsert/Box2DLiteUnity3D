using System.Collections;
using System.Collections.Generic;
using Vec2 = UnityEngine.Vector2;

namespace bluebean.Box2DLite
{
    public struct ClipVertex
    {
        Vec2 v;
        FeaturePair feature;
    }

    public static class Collide
    {
        public static void Flip(ref FeaturePair fp)
        {
            MathUtils.Swap<char>(ref fp.inEdge1, ref fp.inEdge2);
            MathUtils.Swap<char>(ref fp.outEdge1, ref fp.outEdge2);
        }
        
    }
}
