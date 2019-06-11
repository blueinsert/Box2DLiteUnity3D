using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.Box2DLite
{
    [RequireComponent(typeof(Camera))]
    public class DebugDrawView : MonoBehaviour
    {
        DebugDraw m_debugDraw = DebugDraw.Instance;

        private Vector3 m_lastMiddleMouseStartDragPosition;

        private void Update()
        {
            PrecessMouseEvent();
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

        void OnPostRender()
        {
            m_debugDraw.DrawBatch();
        }

    }
}
