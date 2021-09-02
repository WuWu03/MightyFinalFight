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
        class MapBorder
        {
            public float left;
            public float right;
        }

        public float Speed
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

        public float Delta 
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

        public FollowMode FollowMode
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

        public UnityEngine.Camera Camera
        {
            get
            {
                return m_Camera;
            }
            set
            {
                m_Camera = value;
            }
        }


        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

        public Rect GetVision()//获取当前摄像机的视野范围 左 右 下 上
        {
            m_VisionRect.width = Screen.width * m_CurrAspectRate;
            m_VisionRect.height = Screen.height * m_CurrAspectRate;
            m_VisionRect.xMin = transform.position.x - Screen.width * m_CurrAspectRate / 100 / 2;
            m_VisionRect.xMax = transform.position.x + Screen.width * m_CurrAspectRate / 100 / 2;
            m_VisionRect.yMin = transform.position.y - Screen.height * m_CurrAspectRate / 100 / 2;
            m_VisionRect.yMax = transform.position.y + Screen.height * m_CurrAspectRate / 100 / 2;
            return m_VisionRect;
        }

        public void SetFollowSize(int width, int height, float orthographicSize = 0)
        {
            if (m_Target == null)
            {
                Log.GameFrameworkLog.LogError("Don't have target to follow!");
                return;
            }

            if (orthographicSize <= 0)
            {
                if (m_Camera)
                {
                    orthographicSize = m_Camera.orthographicSize;
                }
            }

            m_CurrAspectRate = orthographicSize / (Screen.height / 2f / 100f);
            m_XBorder.left = -(float)(width - Screen.width * m_CurrAspectRate) / 100 / 2;
            m_XBorder.right = (float)(width - Screen.width * m_CurrAspectRate) / 100 / 2;

            float a = (float)(height - Screen.height * m_CurrAspectRate) / 100 / 2;
            float b = -(float)(height - Screen.height * m_CurrAspectRate) / 100 / 2;

            m_YBorder.left = 0;// (float)(height - Screen.height * m_CurrAspectRate) / 100 / 2;
            m_YBorder.right = 0;//(float)(-height + Screen.height * m_CurrAspectRate) / 100 / 2;

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
            m_CameraClamp.x = Mathf.Clamp(targetPos.x, m_XBorder.left > targetPos.x ? m_XBorder.left : targetPos.x, m_XBorder.right < targetPos.x ? m_XBorder.right : targetPos.x);
            m_CameraClamp.y = Mathf.Clamp(targetPos.y, m_YBorder.left > targetPos.y ? m_YBorder.left : targetPos.y, m_YBorder.right < targetPos.y ? m_YBorder.right : targetPos.y);
            m_CameraClamp.z = 0;

            return m_CameraClamp;
        }

        private Rect m_VisionRect = Rect.zero;
        private Vector3 m_CameraClamp = Vector3.zero;
        private FollowMode m_FollowMode = FollowMode.Just;
        private float m_CurrAspectRate = 0f;
        private float m_InitSpeed = 0.5f;
        private float m_Delta = 1f;
        private bool m_IsStart = false;
        private UnityEngine.Camera m_Camera = null;
        private Transform m_Target = null;
        private MapBorder m_XBorder = new MapBorder();
        private MapBorder m_YBorder = new MapBorder();
    }
}