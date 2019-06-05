using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public struct ArbiterKey
    {
        Body body1;
        Body body2;

        public ArbiterKey(Body b1, Body b2)
        {
            body1 = b1;
            body2 = b2;
        }
    }

    /// <summary>
    /// 接触约束
    /// </summary>
    public class Arbiter
    {
        const int MAX_POINTS = 2;
        public Body m_body1;
        public Body m_body2;

        public Contact[] m_contacts = new Contact[MAX_POINTS];
        public int m_numContacts;

        public float m_friction;

        public Arbiter(Body b1, Body b2)
        {
            m_body1 = b1;
            m_body2 = b2;
            m_numContacts = Collide.CollideTest(m_contacts, m_body1, m_body2);
            m_friction = Mathf.Sqrt(m_body1.m_friction * m_body2.m_friction);
        }

        public void Update(Contact[] contacts,int numContacts)
        {

        }

        public void PreStep(float deltaTime)
        {

        }

        public void ApplyImpulse()
        {

        }
    }
}
