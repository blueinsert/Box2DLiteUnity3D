using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class Demo : MonoBehaviour
    {
        private float m_deltaTime = 1 / 60.0f;
        private static readonly Vec2 Gravity = new Vec2(0,-10f);
        private const int MaxIter = 20;
        
        private Body[] m_bodies = new Body[200];//Body缓存池
        private Joint[] m_joints = new Joint[100];//Joint缓存池

        World m_physicsWorld = new World(Gravity, MaxIter);

        DebugDraw m_debugDraw = DebugDraw.Instance;

        delegate void DemoInitDelegate(Body[] bs, Joint[] js, World world);

        private DemoInitDelegate[] m_demoInitFuncs = new DemoInitDelegate[] {Demo1, Demo2, Demo3, Demo4, Demo5};

        private int m_numBodies = 0;

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

        private Vector3 m_lastMiddleMouseStartDragPosition;

        void Awake()
        {
            for (int i = 0; i < 200; i++)
            {
                var body = new Body();
                body.Index = i;
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

        private static void LaunchBomb(World world)
        {
            Body bomb = new Body();
            bomb.Set(new Vec2(1.0f, 1.0f), 50.0f);
            bomb.m_friction = 0.2f;
            world.Add(bomb);
            System.Random random = new System.Random(Time.frameCount);
            bomb.m_position.Set((float)(random.NextDouble() - 0.5) * 30, 15.0f);
            bomb.m_rotation = (float) (random.NextDouble() - 0.5) * 3;
            bomb.m_velocity = -1.5f * bomb.m_position;
            bomb.m_angularVelocity = (float)(random.NextDouble() - 0.5) * 40;
        }

        //single box
        private static void Demo1(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100f,20f), float.MaxValue);
            b.m_position.Set(0, -0.5f * b.m_size.y);
            world.Add(b);
            i++;

            b = bs[i];
            b.Set(new Vec2(1f, 1f), 200f);
            b.m_position.Set(0f,4f);
            world.Add(b);
        }

        private static void Demo2(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(1f, 1f), 200f);
            b.m_position.Set(0f, 4f);
            world.Add(b);
            i++;

            b = bs[i];
            b.Set(new Vec2(100f, 20f), float.MaxValue);
            b.m_position.Set(0, -0.5f * b.m_size.y);
            world.Add(b);
        }

        // Varying friction coefficients
        private static void Demo3(Body[] bs, Joint[] js, World world)
        {
            int i = 0;
            Body b = bs[i];
            b.Set(new Vec2(100.0f, 20.0f), float.MaxValue);
            b.m_position.Set(0.0f, -0.5f * b.m_size.y);
            world.Add(b);
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(-2.0f, 11.0f);
            b.m_rotation = -0.25f;
            world.Add(b);
            i++;
            b = bs[i];
            b.Set(new Vec2(0.25f, 1.0f), float.MaxValue);
            b.m_position.Set(5.25f, 9.5f);
            world.Add(b);
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(2.0f, 7.0f);
            b.m_rotation = 0.25f;
            world.Add(b);
            i++;
            b = bs[i];
            b.Set(new Vec2(0.25f, 1.0f), float.MaxValue);
            b.m_position.Set(-5.25f, 5.5f);
            world.Add(b);
            i++;
            b = bs[i];
            b.Set(new Vec2(13.0f, 0.25f), float.MaxValue);
            b.m_position.Set(-2.0f, 3.0f);
            b.m_rotation = -0.25f;
            world.Add(b);
            i++;
            b = bs[i];

            float[] friction = { 0.75f, 0.5f, 0.35f, 0.1f, 0.0f };
            for (int j = 0; j < 5; ++j)
            {
                b.Set(new Vec2(0.5f, 0.5f), 25.0f);
                b.m_friction = friction[j];
                b.m_position.Set(-7.5f + 2.0f * j, 14.0f);
                world.Add(b);
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
            world.Add(b);
            i++; b = bs[i];
            for (int j = 0; j < 10; ++j)
            {
                b.Set(new Vec2(1.0f, 1.0f), 1.0f);
                b.m_friction = 0.2f;
                System.Random random = new System.Random(Time.frameCount);
                float x = (float) ((random.NextDouble() - 0.5f) / 5f);
                b.m_position.Set(x, 0.51f + 1.05f * j);
                world.Add(b);
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
            world.Add(b);
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
                    world.Add(b);
                    i++;
                    b = bs[i];

                    y += new Vec2(1.125f, 0.0f);
                }

                //x += Vec2(0.5625f, 1.125f);
                x += new Vec2(0.5625f, 2.0f);
            }
        }

        void InitDemo(int index)
        {
            m_physicsWorld.Clear();
            m_demoInitFuncs[index](m_bodies, m_joints, m_physicsWorld);
            m_demoIndex = index;
        }

        void ProcessKeyboardEvent()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                InitDemo(0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                InitDemo(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                InitDemo(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                InitDemo(3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                InitDemo(4);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchBomb(m_physicsWorld);
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

        private void PrecessMouseEvent()
        {
            //鼠标滚轮进行缩放
            if (Input.mouseScrollDelta.y != 0f)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize * (1f - Input.mouseScrollDelta.y / 10f);
            }
            //按住鼠标中键进行拖动
            if (Input.GetMouseButtonDown(2))
            {
                m_lastMiddleMouseStartDragPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                if (m_lastMiddleMouseStartDragPosition != Input.mousePosition)
                {
                    //var offset = Input.mousePosition - lastMiddleMouseStartDragPosition;

                    var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)) - Camera.main.ScreenToWorldPoint(new Vector3(m_lastMiddleMouseStartDragPosition.x, m_lastMiddleMouseStartDragPosition.y, 0f));
                    Camera.main.transform.position -= worldPos;
                    m_lastMiddleMouseStartDragPosition = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButton(2))
            {
                m_lastMiddleMouseStartDragPosition = Vector2.zero;
            }
        }
        
        void Update()
        {
            ProcessKeyboardEvent();
            PrecessMouseEvent();
            m_physicsWorld.Step(m_deltaTime);
            Draw();
        }

        void Draw()
        {
            m_debugDraw.Clear();
            foreach (var body in m_physicsWorld.m_bodies)
            {
                m_debugDraw.DrawBox(body.m_position, body.m_size, body.m_rotation, Color.blue);
            }
            foreach (var arbiter in m_physicsWorld.m_arbiters)
            {
                foreach (var contact in arbiter.Value.m_contacts)
                {
                    m_debugDraw.DrawPoint(contact.m_position, Color.red);
                }
            }
        }

        void OnPostRender()
        {
            m_debugDraw.DrawAllGL();
        }

        void OnGUI()
        {
            GUILayout.Label(DemoStrings[m_demoIndex]);
            GUILayout.Label("Keys: 1-9 Demos, Space to Launch the Bomb");
            GUILayout.Label(string.Format("(A)ccumulation {0}", World.accumulateImpulses ? "ON" : "OFF"));
            GUILayout.Label(string.Format("(P)osition Correction {0}", World.positionCorrection ? "ON" : "OFF"));
            GUILayout.Label(string.Format("(W)arm Starting {0}", World.warmStarting ? "ON" : "OFF"));
        }
    }
}
