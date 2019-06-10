using System.Collections;
using System.Collections.Generic;
using Vec2 = UnityEngine.Vector2;

namespace bluebean.Box2DLite
{
    public class Body
    {
        public Vec2 m_position;
        public float m_rotation;

        public Vec2 m_velocity;
        public float m_angularVelocity;

        public Vec2 m_force;
        public float m_torque;//力矩       

        public float m_friction;

        public float m_mass, m_invMass;
        public float m_I, m_invI;//转动惯量

        public Vec2 m_size;//几何形状为矩形

        public int Index { get; set; }

        public Body() {
            m_position = new Vec2(0, 0);
            m_rotation = 0;
            m_velocity = new Vec2(0, 0);
            m_angularVelocity = 0;
            m_force = new Vec2(0, 0);
            m_friction = 0.2f;

            m_size = new Vec2(1, 1);
            m_mass = float.MaxValue;
            m_invMass = 0;
            m_I = float.MaxValue;
            m_invI = 0;
        }

        public void Clear()
        {
            m_position = new Vec2(0, 0);
            m_rotation = 0;
            m_velocity = new Vec2(0, 0);
            m_angularVelocity = 0;
            m_force = new Vec2(0, 0);
            m_friction = 0.2f;

            m_size = new Vec2(1, 1);
            m_mass = float.MaxValue;
            m_invMass = 0;
            m_I = float.MaxValue;
            m_invI = 0;
        }

        public void Set(Vec2 size, float m)
        {
            m_size = size;
            m_mass = m;
            if(m_mass < float.MaxValue)
            {
                m_invMass = 1.0f / m_mass;
                //计算矩形的转动惯量
                m_I = m_mass * (m_size.x * m_size.x + m_size.y * m_size.y) / 12.0f;
                m_invI = 1.0f / m_I;
            }else
            {
                m_invMass = 0;
                m_I = float.MaxValue;
                m_invI = 0;
            }
        }

        public void AddForce(Vec2 f)
        {
            m_force += f;
        }
    }
}
