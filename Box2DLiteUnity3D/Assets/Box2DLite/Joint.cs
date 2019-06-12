using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace bluebean.Box2DLite
{
    /// <summary>
    /// 转动关节约束
    /// </summary>
    public class Joint
    {
        private Mat22 m_m;
        public Vec2 m_localAnchor1, m_localAnchor2;
        private Vec2 m_r1, m_r2;
        private Vec2 m_bias;
        private Vec2 m_p;
        public Body m_body1;
        public Body m_body2;
        public float m_biasFactor;
        public float m_softness;

        public void Set(Body body1, Body body2, Vec2 anchor)
        {
            m_body1 = body1;
            m_body2 = body2;

            Mat22 Rot1 = new Mat22(body1.m_rotation);
            Mat22 Rot2 = new Mat22(body2.m_rotation);
            Mat22 Rot1T = Rot1.Transpose();
            Mat22 Rot2T = Rot2.Transpose();

            m_localAnchor1 = Rot1T * (anchor - body1.m_position);
            m_localAnchor2 = Rot2T * (anchor - body2.m_position);

            m_p.Set(0.0f, 0.0f);

            m_softness = 0.0f;
            m_biasFactor = 0.2f;
        }

        public void PreStep(float deltaTime)
        {
            // Pre-compute anchors, mass matrix, and bias.
            Mat22 Rot1 = new Mat22(m_body1.m_rotation);
            Mat22 Rot2 = new Mat22(m_body2.m_rotation);

            m_r1 = Rot1 * m_localAnchor1;
            m_r2 = Rot2 * m_localAnchor2;

            Mat22 K1;
            K1.col1.x = m_body1.m_invMass + m_body2.m_invMass; K1.col2.x = 0.0f;
            K1.col1.y = 0.0f; K1.col2.y = m_body1.m_invMass + m_body2.m_invMass;

            Mat22 K2;
            K2.col1.x = m_body1.m_invI * m_r1.y * m_r1.y; K2.col2.x = -m_body1.m_invI * m_r1.x * m_r1.y;
            K2.col1.y = -m_body1.m_invI * m_r1.x * m_r1.y; K2.col2.y = m_body1.m_invI * m_r1.x * m_r1.x;

            Mat22 K3;
            K3.col1.x = m_body2.m_invI * m_r2.y * m_r2.y; K3.col2.x = -m_body2.m_invI * m_r2.x * m_r2.y;
            K3.col1.y = -m_body2.m_invI * m_r2.x * m_r2.y; K3.col2.y = m_body2.m_invI * m_r2.x * m_r2.x;

            Mat22 K = K1 + K2 + K3;
            K.col1.x += m_softness;
            K.col2.y += m_softness;

            m_m = K.Invert();

            Vec2 p1 = m_body1.m_position + m_r1;
            Vec2 p2 = m_body2.m_position + m_r2;
            Vec2 dp = p2 - p1;

            if (World.positionCorrection)
            {
                m_bias = -m_biasFactor * (1/deltaTime) * dp;
            }
            else
            {
                m_bias.Set(0.0f, 0.0f);
            }

            if (World.warmStarting)
            {
                // Apply accumulated impulse.
                m_body1.m_velocity -= m_body1.m_invMass * m_p;
                m_body1.m_angularVelocity -= m_body1.m_invI * Vec2.Cross(m_r1, m_p);

                m_body2.m_velocity += m_body2.m_invMass * m_p;
                m_body2.m_angularVelocity += m_body2.m_invI * Vec2.Cross(m_r2, m_p);
            }
            else
            {
                m_p.Set(0.0f, 0.0f);
            }
        }

        public void ApplyImpulse()
        {
            Vec2 dv = m_body2.m_velocity + Vec2.Cross(m_body2.m_angularVelocity, m_r2) - m_body1.m_velocity - Vec2.Cross(m_body1.m_angularVelocity, m_r1);

            Vec2 impulse;

            impulse = m_m * (m_bias - dv - m_softness * m_p);

            m_body1.m_velocity -= m_body1.m_invMass * impulse;
            m_body1.m_angularVelocity -= m_body1.m_invI * Vec2.Cross(m_r1, impulse);

            m_body2.m_velocity += m_body2.m_invMass * impulse;
            m_body2.m_angularVelocity += m_body2.m_invI * Vec2.Cross(m_r2, impulse);

            m_p += impulse;
        }
    }
}
