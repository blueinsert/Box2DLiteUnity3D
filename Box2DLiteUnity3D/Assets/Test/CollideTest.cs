using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    public class CollideTest : MonoBehaviour
    {
        DebugDraw debugDraw = DebugDraw.Instance;
        Body body1 = new Body();
        Body body2 = new Body();
        Contact[] contacts = new Contact[2];
        int contactCount = 0;

        void Update()
        {
            debugDraw.Clear();
            ProcessInput();
            contactCount = Collide.CollideTest(contacts, body1, body2);

            debugDraw.DrawBox(body1.m_position, body1.m_size, body1.m_rotation, Color.blue);
            debugDraw.DrawBox(body2.m_position, body2.m_size, body2.m_rotation, Color.blue);
            for (int i = 0; i < contactCount; i++)
            {
                var contact = contacts[i];
                debugDraw.DrawPoint(contact.position, Color.red);
            }
           
        }

        void ProcessInput()
        {
            /*
            if (Input.GetKey(KeyCode.A))
            {
                body1.m_position.x -= 0.01f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                body1.m_position.x += 0.01f;
            }
            if (Input.GetKey(KeyCode.W))
            {
                body1.m_position.y += 0.01f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                body1.m_position.y -= 0.01f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                body1.m_rotation += 0.01f;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                body1.m_rotation -= 0.01f;
            }
            */
            if (Input.GetKey(KeyCode.Keypad4))
            {
                body2.m_position.x -= 0.01f;
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                body2.m_position.x += 0.01f;
            }
            if (Input.GetKey(KeyCode.Keypad8))
            {
                body2.m_position.y += 0.01f;
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                body2.m_position.y -= 0.01f;
            }
            if (Input.GetKey(KeyCode.Keypad9))
            {
                body2.m_rotation += 0.01f;
            }
            if (Input.GetKey(KeyCode.Keypad7))
            {
                body2.m_rotation -= 0.01f;
            }
        }

        void OnDrawGizmos()
        {
            debugDraw.DrawAllGizmos();
        }

        void OnPostRender()
        {
            debugDraw.DrawAllGL();
        }

    }
}
