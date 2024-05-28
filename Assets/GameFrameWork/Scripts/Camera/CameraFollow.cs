using UnityEngine;

namespace GameFrameWork.Camera
{
    public enum FollowMode
    {
        Just,
        Lerp,
        Linear,
    }

    public class CameraFollow : MonoBehaviour
    {
        public float speed
        {
            get
            {
                return m_InitSpeed;
            }
            set
            {
                m_InitSpeed = value;
            }
        }

        public float delta 
        {
            get
            {
                return m_Delta;
            }
            set
            {
                m_Delta = value;
            }
        }

        public FollowMode followMode
        {
            get
            {
                return m_FollowMode;
            }
            set
            {
                m_FollowMode = value;
            }
        }

        public float orthographicSize
        {
            get
            {
                return m_OrthographicSize;
            }
            set
            {
                m_OrthographicSize = value;
            }
        }

        public bool allowHorizontalAxisFollow
        {
            get;
            set;
        }


        public bool allowVerticalAxisFollow
        {
            get;
            set;
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

        /// <summary>
        /// 获取当前摄像机的视野范围 左 右 下 上
        /// </summary>
        public Rect GetVision()
        {
            float aspectRate = (float)Screen.width / Screen.height;
            float orthographicSize = m_OrthographicSize;

            m_VisionRect.width = aspectRate * orthographicSize * 2;
            m_VisionRect.height = orthographicSize * 2;
            m_VisionRect.xMin = transform.position.x - aspectRate * orthographicSize;
            m_VisionRect.xMax = transform.position.x + aspectRate * orthographicSize;
            m_VisionRect.yMin = transform.position.y - orthographicSize;
            m_VisionRect.yMax = transform.position.y + orthographicSize;
            return m_VisionRect;
        }

        public void UpdateOrthographicSize(float orthographicSize)
        {
            m_OrthographicSize = orthographicSize;
            SetFollowSize(m_MapWidth, m_MapHeight);
        }

        public void SetFollowSize(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            m_MapWidth = width;
            m_MapHeight = height;

            float aspectRate = (float)Screen.width / Screen.height;
            float orthographicSize = m_OrthographicSize;

            m_Border.xMin = -width / 200f + orthographicSize * aspectRate;
            m_Border.xMax = width / 200f - orthographicSize * aspectRate;
            m_Border.yMin = -height / 200f + orthographicSize;
            m_Border.yMax = height / 200f - orthographicSize * aspectRate;

            if (m_Target != null)
            {
                transform.position = GetClampPos(m_Target.position);
                m_IsStart = m_Target != null;
            }
        }

        public void StartFollow()
        {
            m_IsStart = m_Target != null;
        }

        public void EndFollow()
        {
            m_IsStart = false;
        }

        private void LateUpdate()
        {
            if (!m_IsStart || m_Target == null || m_Target.position.x < transform.position.x)
            {
                return;
            }

            Vector3 pos = m_Target.position;
            float distance = Vector3.Distance(transform.position, pos);

            if (distance <= (m_FollowMode == FollowMode.Lerp ? 0.02f : 0f))
            {
                return;
            }

            float speed = distance * distance / m_Delta + m_InitSpeed;

            switch (m_FollowMode)
            {
                case FollowMode.Just:
                    transform.position = GetClampPos(pos);
                    break;
                case FollowMode.Lerp:                  
                    transform.position = Vector3.Lerp(transform.position, GetClampPos(pos), Time.deltaTime * speed);
                    break;
                case FollowMode.Linear:
                    transform.position += (GetClampPos(pos) - transform.position).normalized * speed * Time.deltaTime;
                    break;
            }
        }

        private Vector3 GetClampPos(Vector3 targetPos)
        {
            if (allowHorizontalAxisFollow)
            {
                m_CameraClamp.x = Mathf.Clamp(targetPos.x, m_Border.xMin > targetPos.x ? m_Border.xMin : targetPos.x, m_Border.xMax < targetPos.x ? m_Border.xMax : targetPos.x);
            }
            else
            {
                m_CameraClamp.x = 0;
            }

            if (allowVerticalAxisFollow)
            {
                m_CameraClamp.y = Mathf.Clamp(targetPos.y, m_Border.yMin > targetPos.y ? m_Border.yMin : targetPos.y, m_Border.yMax < targetPos.y ? m_Border.yMax : targetPos.y);
            }
            else
            {
                m_CameraClamp.y = 0;
            }

            m_CameraClamp.z = 0;
            return m_CameraClamp;
        }

        private Rect m_VisionRect = Rect.zero;
        private Vector3 m_CameraClamp = Vector3.zero;
        private FollowMode m_FollowMode = FollowMode.Just;
        private float m_InitSpeed = 0.5f;
        private float m_Delta = 1f;
        private bool m_IsStart = false;
        private float m_OrthographicSize = 0f;
        private Transform m_Target = null;
        private int m_MapWidth = 0;
        private int m_MapHeight = 0;
        private Rect m_Border = Rect.zero;
    }
}