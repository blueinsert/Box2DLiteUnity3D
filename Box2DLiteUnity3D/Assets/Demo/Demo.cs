using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class Demo : MonoBehaviour
    {
        private float m_deltaTime = 1 / 60.0f;
        private static readonly Vec2 Gravity = new Vec2(0, -10f);
        private const int MaxIter = 20;

        private Body[] m_bodies = new Body[200];//Body缓存池
        private Joint[] m_joints = new Joint[100];//Joint缓存池

        World m_physicsWorld = new World(Gravity, MaxIter);

        delegate void DemoInitDelegate(Body[] bs, Joint[] js, World world);

        private DemoInitDelegate[] m_demoInitFuncs = new DemoInitDelegate[] { Demo1, Demo2, Demo3, Demo4, Demo5, Demo6, Demo7, Demo8, Demo9 };

        private static int m_numBodies = 0;
        private static int m_numJoints = 0;

        readonly string[] DemoStrings = {
            "Demo 1: A Single Box",
            "Demo 2: Simple Pendulum",
            "Demo 3: Varying Friction Coefficients",
            "Demo 4: Randomized Stacking",
            "Demo 5: Pyramid Stacking",
            "Demo 6: A Teeter",
            "Demo 7: A Suspension Bridge",
            "Demo 8: Dominos",
            "Demo 9: Multi-pendulum"

        };

        private int m_demoIndex = 0;

        DebugDraw m_debugDraw = DebugDraw.Instance;
        private bool m_drawContactPoint = true;
        private bool m_drawContactNormal = true;

        void Awake()
        {
            for (int i = 0; i < 200; i++)
            {
                var body = new Body();
                m_bodies[i] = body;
            }
            for (int i = 0; i < 100; i++)
            {
                m_joints[i] = new Joint();
            }
        }

        void Start()
        {
            InitDemo(0);
        }

        private static void LaunchBomb(Body[] bs, Joint[] js, World world)
        {
            Body bomb = bs[m_numBodies];
            bomb.m_isBomb = true;
            bomb.Set(new Vec2(1.0f, 1.0f), 50.0f);
            bomb.m_friction = 0.2f;
            world.Add(bomb); m_numBodies++;
            System.Random random = new System.Random(Time.frameCount);
            bomb.m_position.Set((float)(random.NextDouble() - 0.5) * 30, 15.0f);
            bomb.m_rotation = (float)(random.NextDouble() - 0.5) * 3;
            bomb.m_velocity = -1.5f * bomb.m_position;
            bomb.m_angularVelocity = (float)(random.NextDouble() - 0.5) * 40;
        }

        //single box
        private static void Demo1(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100f, 20f), float.MaxValue);
            b.m_position.Set(0, -0.5f * b.m_size.y);
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(1f, 1f), 200f);
            b.m_position.Set(0f, 4f);
            b.m_rotation = 50 / 180f * 3.14159f;
            world.Add(b); m_numBodies++;
        }

        private static void Demo2(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b1 = bs[i];
            b1.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b1.m_friction = 0.2f;
            b1.m_position.Set(0.0f, -0.5f * b1.m_size.y);
            b1.m_rotation = 0.0f;
            world.Add(b1);
            i++;
            Body b2 = bs[i];
            b2.Set(new Vec2(1.0f, 1.0f), 100.0f);
            b2.m_friction = 0.2f;
            b2.m_position.Set(9.0f, 11.0f);
            b2.m_rotation = 0.0f;
            world.Add(b2);
            m_numBodies += 2;

            int j = 0;
            Joint joint = js[j];
            joint.Set(b1, b2, new Vec2(5.0f, 11.0f));
            world.Add(joint);
            m_numJoints += 1;
        }

        // Varying friction coefficients
        private static void Demo3(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(-2.0f, 11.0f);
            b.m_rotation = -0.25f;
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(0.25f, 1.0f), float.MaxValue);
            b.m_position.Set(5.25f, 9.5f);
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(2.0f, 7.0f);
            b.m_rotation = 0.25f;
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(0.25f, 1.0f), float.MaxValue);
            b.m_position.Set(-5.25f, 5.5f);
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(-2.0f, 3.0f);
            b.m_rotation = -0.25f;
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];

            float[] friction = { 0.75f, 0.5f, 0.35f, 0.1f, 0.0f };
            for (int j = 0; j < 5; ++j)
            {
                b.Set(new Vec2(0.5f, 0.5f), 25.0f);
                b.m_friction = friction[j];
                b.m_position.Set(-7.5f + 2.0f * j, 14.0f);
                world.Add(b); m_numBodies++;
                i++;
                b = bs[i];
            }
        }

        private static void Demo4(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_friction = 0.2f;
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            b.m_rotation = 0.0f;
            world.Add(b); m_numBodies++;
            i++; b = bs[i];
            for (int j = 0; j < 10; ++j)
            {
                b.Set(new Vec2(1.0f, 1.0f), 1.0f);
                b.m_friction = 0.2f;
                System.Random random = new System.Random(Time.frameCount);
                float x = (float)((random.NextDouble() - 0.5f) / 5f);
                b.m_position.Set(x, 0.51f + 1.05f * j);
                world.Add(b); m_numBodies++;
                i++; b = bs[i];
            }
        }

        // A pyramid
        static void Demo5(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_friction = 0.2f;
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            b.m_rotation = 0.0f;
            world.Add(b); m_numBodies++;
            i++;
            b = bs[i];

            Vec2 x = new Vec2(-6.0f, 0.75f);
            Vec2 y;

            for (int j = 0; j < 12; ++j)
            {
                y = x;
                for (int k = j; k < 12; ++k)
                {
                    b.Set(new Vec2(1.0f, 1.0f), 10.0f);
                    b.m_friction = 0.2f;
                    b.m_position = y;
                    world.Add(b); m_numBodies++;
                    i++;
                    b = bs[i];

                    y += new Vec2(1.125f, 0.0f);
                }

                //x += Vec2(0.5625f, 1.125f);
                x += new Vec2(0.5625f, 2.0f);
            }
        }

        // A teeter
        static void Demo6(Body[] bs, Joint[] js, World world)
        {
            Body b1 = bs[0];
            b1.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b1.m_position.Set(0.0f, -0.5f * b1.m_size.y);
            world.Add(b1);

            Body b2 = bs[1];
            b2.Set(new Vec2(12.0f, 0.25f), 100.0f);
            b2.m_position.Set(0.0f, 1.0f);
            world.Add(b2);

            Body b3 = bs[2];
            b3.Set(new Vec2(0.5f, 0.5f), 25.0f);
            b3.m_position.Set(-5.0f, 2.0f);
            world.Add(b3);

            Body b4 = bs[3];
            b4.Set(new Vec2(0.5f, 0.5f), 25.0f);
            b4.m_position.Set(-5.5f, 2.0f);
            world.Add(b4);

            Body b5 = bs[4];
            b5.Set(new Vec2(1.0f, 1.0f), 100.0f);
            b5.m_position.Set(5.5f, 15.0f);
            world.Add(b5);

            m_numBodies += 5;

            Joint joint = js[0];
            joint.Set(b1, b2, new Vec2(0.0f, 1.0f));
            world.Add(joint);

            m_numJoints += 1;
        }

        // A suspension bridge
        static void Demo7(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_friction = 0.2f;
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            b.m_rotation = 0.0f;
            world.Add(b);
            ++i; ++m_numBodies;
            b = bs[i];

            const int numPlanks = 15;
            float mass = 50.0f;

            for (int index = 0; index < numPlanks; ++index)
            {
                b.Set(new Vec2(1.0f, 0.25f), mass);
                b.m_friction = 0.2f;
                b.m_position.Set(-8.5f + 1.25f * index, 5.0f);
                world.Add(b);
                ++i; ++m_numBodies;
                b = bs[i];
            }

            // Tuning
            float frequencyHz = 2.0f;
            float dampingRatio = 0.7f;

            // frequency in radians
            float omega = 2.0f * 3.14159f * frequencyHz;

            // damping coefficient
            float d = 2.0f * mass * dampingRatio * omega;

            // spring stifness
            float k = mass * omega * omega;

            float timeStep = 1 / 60f;
            // magic formulas
            float softness = 1.0f / (d + timeStep * k);
            float biasFactor = timeStep * k / (d + timeStep * k);

            int j = 0;
            Joint joint;
            for (j = 0; j < numPlanks; ++j)
            {
                joint = js[j];
                joint.Set(bs[j], bs[j + 1], new Vec2(-9.125f + 1.25f * j, 5.0f));
                joint.m_softness = softness;
                joint.m_biasFactor = biasFactor;
                world.Add(joint);
                ++m_numJoints;
            }
            joint = js[j];
            joint.Set(bs[numPlanks], bs[0], new Vec2(-9.125f + 1.25f * numPlanks, 5.0f));
            joint.m_softness = softness;
            joint.m_biasFactor = biasFactor;
            world.Add(joint);
            ++j; ++m_numJoints;
        }

        // Dominos
        static void Demo8(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            Body b1 = b;
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            world.Add(b);
            ++i; ++m_numBodies;
            b = bs[i];
            b.Set(new Vec2(12.0f, 0.5f), float.MaxValue);
            b.m_position.Set(-1.5f, 10.0f);
            world.Add(b);
            ++i; ++m_numBodies;
            b = bs[i];
            for (int j = 0; j < 10; ++j)
            {
                b.Set(new Vec2(0.2f, 2.0f), 10.0f);
                b.m_position.Set(-6.0f + 1.0f * j, 11.125f);
                b.m_friction = 0.1f;
                world.Add(b);
                ++i; ++m_numBodies;
                b = bs[i];
            }

            b.Set(new Vec2(14.0f, 0.5f), float.MaxValue);
            b.m_position.Set(1.0f, 6.0f);
            b.m_rotation = 0.3f;
            world.Add(b);
            ++i; ++m_numBodies;
            b = bs[i];
            Body b2 = b;
            b.Set(new Vec2(0.5f, 3.0f), float.MaxValue);
            b.m_position.Set(-7.0f, 4.0f);
            world.Add(b);
            ++i; ++m_numBodies;
            b = bs[i];
            Body b3 = b;
            b.Set(new Vec2(12.0f, 0.25f), 20.0f);
            b.m_position.Set(-0.9f, 1.0f);
            world.Add(b);
            ++i; ++m_numBodies;
            int jIndex = 0;
            Joint joint = js[jIndex];
            joint.Set(b1, b3, new Vec2(-2.0f, 1.0f));
            world.Add(joint);
            ++jIndex; ++m_numJoints;
            b = bs[i];
            Body b4 = b;
            b.Set(new Vec2(0.5f, 0.5f), 10.0f);
            b.m_position.Set(-10.0f, 15.0f);
            world.Add(b);
            ++i; ++m_numBodies;
            joint = js[jIndex];
            joint.Set(b2, b4, new Vec2(-7.0f, 15.0f));
            world.Add(joint);
            ++jIndex; ++m_numJoints;

            b = bs[i];
            Body b5 = b;
            b.Set(new Vec2(2.0f, 2.0f), 20.0f);
            b.m_position.Set(6.0f, 2.5f);
            b.m_friction = 0.1f;
            world.Add(b);
            ++i; ++m_numBodies;

            joint = js[jIndex];
            joint.Set(b1, b5, new Vec2(6.0f, 2.6f));
            world.Add(joint);
            ++jIndex; ++m_numJoints;

            b = bs[i];
            Body b6 = b;
            b.Set(new Vec2(2.0f, 0.2f), 10.0f);
            b.m_position.Set(6.0f, 3.6f);
            world.Add(b);
            ++i; ++m_numBodies;

            joint = js[jIndex];
            joint.Set(b5, b6, new Vec2(7.0f, 3.5f));
            world.Add(joint);
            ++jIndex; ++m_numJoints;
        }

        // A multi-pendulum
        static void Demo9(Body[] bs, Joint[] js, World world)
        {
            int bi = 0;
            Body b = bs[bi];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_friction = 0.2f;
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            b.m_rotation = 0.0f;
            world.Add(b);

            Body b1 = b;
            ++bi;
            ++m_numBodies;

            float mass = 10.0f;

            // Tuning
            float frequencyHz = 4.0f;
            float dampingRatio = 0.7f;

            // frequency in radians
            float omega = 2.0f * 3.14159f * frequencyHz;

            // damping coefficient
            float d = 2.0f * mass * dampingRatio * omega;

            // spring stiffness
            float k = mass * omega * omega;

            float timeStep = 1 / 60f;
            // magic formulas
            float softness = 1.0f / (d + timeStep * k);
            float biasFactor = timeStep * k / (d + timeStep * k);

            const float y = 12.0f;

            b = bs[bi];
            int ji = 0;
            Joint joint;
            for (int i = 0; i < 15; ++i)
            {
                Vec2 x = new Vec2(0.5f + i, y);
                b.Set(new Vec2(0.75f, 0.25f), mass);
                b.m_friction = 0.2f;
                b.m_position = x;
                b.m_rotation = 0.0f;
                world.Add(b);
                joint = js[ji];
                joint.Set(b1, b, new Vec2(i, y));
                joint.m_softness = softness;
                joint.m_biasFactor = biasFactor;
                world.Add(joint);

                b1 = b;
                ++bi;
                ++m_numBodies;
                ++ji;
                ++m_numJoints;
                b = bs[bi];
                joint = js[ji];
            }
        }

        void InitDemo(int index)
        {
            m_physicsWorld.Clear();
            m_numBodies = 0;
            m_numJoints = 0;
            m_demoInitFuncs[index](m_bodies, m_joints, m_physicsWorld);
            m_demoIndex = index;
        }

        void ProcessKeyboardEvent()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InitDemo(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InitDemo(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InitDemo(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                InitDemo(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                InitDemo(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                InitDemo(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                InitDemo(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                InitDemo(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                InitDemo(8);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchBomb(m_bodies, m_joints, m_physicsWorld);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                World.accumulateImpulses = !World.accumulateImpulses;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                World.warmStarting = !World.warmStarting;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                World.positionCorrection = !World.positionCorrection;
            }
        }



        void Update()
        {
            ProcessKeyboardEvent();
            m_physicsWorld.Step(m_deltaTime);
            Draw();
        }

        void Draw()
        {
            m_debugDraw.Clear();
            foreach (var body in m_physicsWorld.m_bodies)
            {
                m_debugDraw.DrawBox(body.m_position, body.m_size, body.m_rotation, body.m_isBomb ? Color.red : Color.blue);
            }
            foreach (var arbiter in m_physicsWorld.m_arbiters)
            {
                foreach (var contact in arbiter.Value.m_contacts)
                {
                    if (m_drawContactPoint)
                    {
                        m_debugDraw.DrawPoint(contact.m_position, Color.red);
                    }
                    if (m_drawContactNormal)
                    {
                        float normalLength = 0.5f;
                        float arrowLength = 0.2f;
                        m_debugDraw.DrawSingleArrowLine(contact.m_position,
                            contact.m_position + contact.m_normal * normalLength, arrowLength, Color.yellow);
                    }
                }
            }
            foreach (var joint in m_physicsWorld.m_joints)
            {
                Body b1 = joint.m_body1;
                Body b2 = joint.m_body2;
                Vec2 anchor1 = b1.m_position + new Mat22(b1.m_rotation) * joint.m_localAnchor1;
                Vec2 anchor2 = b2.m_position + new Mat22(b2.m_rotation) * joint.m_localAnchor2;
                m_debugDraw.DrawLine(b1.m_position, anchor1, Color.gray);
                m_debugDraw.DrawLine(b2.m_position, anchor2, Color.gray);
            }
        }

        void OnGUI()
        {
            GUILayout.Label(DemoStrings[m_demoIndex]);
            GUILayout.Label("Keys: 1-9 Demos, Space to Launch the Bomb");
            GUILayout.Label(string.Format("(A)ccumulation {0}", World.accumulateImpulses ? "ON" : "OFF"));
            GUILayout.Label(string.Format("(P)osition Correction {0}", World.positionCorrection ? "ON" : "OFF"));
            GUILayout.Label(string.Format("(W)arm Starting {0}", World.warmStarting ? "ON" : "OFF"));
            GUILayout.Space(3);
            GUILayout.Label("Render Options:");
            GUILayout.BeginHorizontal(GUIStyle.none);
            m_drawContactPoint = GUILayout.Toggle(m_drawContactPoint, "ContactPoint");
            m_drawContactNormal = GUILayout.Toggle(m_drawContactNormal, "ContactNormal");
            GUILayout.EndHorizontal();

        }
    }
}
