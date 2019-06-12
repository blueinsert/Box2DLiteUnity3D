using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class ArbiterKey
    {
        Body body1;
        Body body2;

        public ArbiterKey(Body b1, Body b2)
        {
            body1 = b1;
            body2 = b2;
        }

        /// <summary>
        /// 相等性
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            var otherKey = obj as ArbiterKey;
            if ((this.body1 == otherKey.body1 && this.body2 == otherKey.body2)
                ||(this.body1 == otherKey.body2 && this.body2 == otherKey.body1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return body1.GetHashCode() * body2.GetHashCode();
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
            for (int i = 0; i < MAX_POINTS; i++)
            {
                m_contacts[i] = new Contact();
            }
            m_numContacts = Collide.CollideTest(m_contacts, m_body1, m_body2);
            m_friction = Mathf.Sqrt(m_body1.m_friction * m_body2.m_friction);
        }

        public void Update(Contact[] newContacts, int numNewContacts)
        {
            Contact[] mergedContacts = new Contact[2];

            for (int i = 0; i < numNewContacts; ++i)
            {
                Contact cNew = newContacts[i];
                int k = -1;
                for (int j = 0; j < m_numContacts; ++j)
                {
                    Contact cOld = m_contacts[j];
                    if (cNew.m_feature.value == cOld.m_feature.value)
                    {
                        k = j;
                        break;
                    }
                }

                if (k > -1)
                {
                    mergedContacts[i] = cNew;
                    Contact c = mergedContacts[i];
                    Contact cOld = m_contacts[k];
                    if (World.warmStarting)
                    {
                        c.m_pn = cOld.m_pn;
                        c.m_pt = cOld.m_pt;
                        c.m_pnb = cOld.m_pnb;
                    }
                    else
                    {
                        c.m_pn = 0.0f;
                        c.m_pt = 0.0f;
                        c.m_pnb = 0.0f;
                    }
                }
                else
                {
                    mergedContacts[i] = newContacts[i];
                }
            }

            for (int i = 0; i < numNewContacts; ++i)
                m_contacts[i] = mergedContacts[i];
            m_numContacts = numNewContacts;
        }

        public void PreStep(float deltaTime)
        {
            float k_allowedPenetration = 0.01f;
            float k_biasFactor = World.positionCorrection ? 0.2f : 0.0f;

            for (int i = 0; i < m_numContacts; ++i)
            {
                Contact c = m_contacts[i];

                Vec2 r1 = c.m_position - m_body1.m_position;
                Vec2 r2 = c.m_position - m_body2.m_position;
                 
                // Precompute normal mass, tangent mass, and bias.
                float rn1 = Vec2.Dot(r1, c.m_normal);
                float rn2 = Vec2.Dot(r2, c.m_normal);
                float kNormal = m_body1.m_invMass + m_body2.m_invMass;
                kNormal += m_body1.m_invI * (Vec2.Dot(r1, r1) - rn1 * rn1) + m_body2.m_invI * (Vec2.Dot(r2, r2) - rn2 * rn2);
                c.m_massNormal = 1.0f / kNormal;

                Vec2 tangent = Vec2.Cross(c.m_normal, 1.0f);
                float rt1 = Vec2.Dot(r1, tangent);
                float rt2 = Vec2.Dot(r2, tangent);
                float kTangent = m_body1.m_invMass + m_body2.m_invMass;
                kTangent += m_body1.m_invI * (Vec2.Dot(r1, r1) - rt1 * rt1) + m_body2.m_invI * (Vec2.Dot(r2, r2) - rt2 * rt2);
                c.m_massTangent = 1.0f / kTangent;

                c.m_bias = -k_biasFactor * (1/deltaTime) * MathUtils.Min(0.0f, c.m_separation + k_allowedPenetration);

                if (World.accumulateImpulses)
                {
                    // Apply normal + friction impulse
                    Vec2 P =  c.m_normal* c.m_pn + tangent * c.m_pt;

                    m_body1.m_velocity -= m_body1.m_invMass * P;
                    m_body1.m_angularVelocity -= m_body1.m_invI * Vec2.Cross(r1, P);

                    m_body2.m_velocity += m_body2.m_invMass * P;
                    m_body2.m_angularVelocity += m_body2.m_invI * Vec2.Cross(r2, P);
                }
            }
        }

        public void ApplyImpulse()
        {
            Body b1 = m_body1;
            Body b2 = m_body2;

            for (int i = 0; i < m_numContacts; ++i)
            {
                Contact c = m_contacts[i];
                c.m_r1 = c.m_position - b1.m_position;
                c.m_r2 = c.m_position - b2.m_position;

                // Relative velocity at contact
                Vec2 dv = b2.m_velocity + Vec2.Cross(b2.m_angularVelocity, c.m_r2) - b1.m_velocity -
                          Vec2.Cross(b1.m_angularVelocity, c.m_r1);

                // Compute normal impulse
                float vn = Vec2.Dot(dv, c.m_normal);

                float dPn = c.m_massNormal * (-vn + c.m_bias);

                if (World.accumulateImpulses)
                {
                    // Clamp the accumulated impulse
                    float Pn0 = c.m_pn;
                    c.m_pn = MathUtils.Max(Pn0 + dPn, 0.0f);
                    //Debug.Log(dPn);
                    dPn = c.m_pn - Pn0;
                }
                else
                {
                    dPn = MathUtils.Max(dPn, 0.0f);
                }

                // Apply contact impulse
                Vec2 Pn = dPn * c.m_normal;

                b1.m_velocity -= b1.m_invMass * Pn;
                b1.m_angularVelocity -= b1.m_invI * Vec2.Cross(c.m_r1, Pn);

                b2.m_velocity += b2.m_invMass * Pn;
                b2.m_angularVelocity += b2.m_invI * Vec2.Cross(c.m_r2, Pn);
                //Debug.Log(string.Format("key:{0} dPn:{1} vn:{2} bias:{3} vy:{4}", c.m_feature.value, dPn, vn, c.m_bias, b2.m_velocity.y));
                // Relative velocity at contact
                dv = b2.m_velocity + Vec2.Cross(b2.m_angularVelocity, c.m_r2) - b1.m_velocity -
                     Vec2.Cross(b1.m_angularVelocity, c.m_r1);

                Vec2 tangent = Vec2.Cross(c.m_normal, 1.0f);
                float vt = Vec2.Dot(dv, tangent);
                float dPt = c.m_massTangent * (-vt);

                if (World.accumulateImpulses)
                {
                    // Compute friction impulse
                    float maxPt = m_friction * c.m_pn;

                    // Clamp friction
                    float oldTangentImpulse = c.m_pt;
                    c.m_pt = MathUtils.Clamp(oldTangentImpulse + dPt, -maxPt, maxPt);
                    dPt = c.m_pt - oldTangentImpulse;
                }
                else
                {
                    float maxPt = m_friction * dPn;
                    dPt = MathUtils.Clamp(dPt, -maxPt, maxPt);
                }

                // Apply contact impulse
                Vec2 Pt = dPt * tangent;

                b1.m_velocity -= b1.m_invMass * Pt;
                b1.m_angularVelocity -= b1.m_invI * Vec2.Cross(c.m_r1, Pt);

                b2.m_velocity += b2.m_invMass * Pt;
                b2.m_angularVelocity += b2.m_invI * Vec2.Cross(c.m_r2, Pt);
            }
        }

    }
}
