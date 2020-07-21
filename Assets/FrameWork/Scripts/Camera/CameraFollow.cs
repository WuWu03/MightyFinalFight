using UnityEngine;

namespace FrameWork.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        class MapBorder
        {
            public float left;
            public float right;
        }

        public float Speed = 2.0f;
        public float Delta = 1.5f;

        public UnityEngine.Camera MainCamera = null;

        void Start()
        {
            m_InitSpeed = Speed;
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

        public Rect GetVision()//获取当前摄像机的视野范围 左 下 右 上
        {
            m_VisionRect.width = Screen.width * m_CurrAspectRate;
            m_VisionRect.height = Screen.height * m_CurrAspectRate;
            m_VisionRect.xMin = transform.position.x - Screen.width * m_CurrAspectRate / 100 / 2;
            m_VisionRect.xMax = transform.position.y - Screen.height * m_CurrAspectRate / 100 / 2;
            m_VisionRect.xMax = transform.position.x + Screen.width * m_CurrAspectRate / 100 / 2;
            m_VisionRect.yMax = transform.position.y + Screen.height * m_CurrAspectRate / 100 / 2;
            return m_VisionRect;
        }

        public void InitFollow(int width, int height, float orthographicSize = 0)
        {
            if (m_Target == null)
            {
                FrameWork.Log.Debugger.LogError("Don't have target to follow!");
                return;
            }

            if (orthographicSize <= 0)
            {
                if (MainCamera)
                {
                    orthographicSize = MainCamera.orthographicSize;
                }
            }

            m_CurrAspectRate = orthographicSize / (Screen.height / 2f / 100f);
            m_XBorder.left = (float)(-width + Screen.width * m_CurrAspectRate) / 100 / 2;
            m_XBorder.right = (float)(width - Screen.width * m_CurrAspectRate) / 100 / 2;
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
            if (!m_IsStart || m_Target == null) return;
            Vector3 pos = m_Target.position;

            if (pos.x < transform.position.x) return;

            float distance = Vector3.Distance(transform.position, pos);
            if (distance < 0.02f) return;

            Speed = (distance * distance / Delta) + m_InitSpeed;
            transform.position = Vector3.Lerp(transform.position, GetClampPos(pos), Time.deltaTime * Speed);
        }

        private Vector2 GetClampPos(Vector2 targetPos)
        {
            m_CameraClamp.x = Mathf.Clamp(targetPos.x, m_XBorder.left > targetPos.x ? m_XBorder.left : targetPos.x, m_XBorder.right < targetPos.x ? m_XBorder.right : targetPos.x);
            m_CameraClamp.y = Mathf.Clamp(targetPos.y, m_YBorder.left > targetPos.y ? m_YBorder.left : targetPos.y, m_YBorder.right < targetPos.y ? m_YBorder.right : targetPos.y);

            return m_CameraClamp;
        }

        private Rect m_VisionRect = Rect.zero;
        private float m_CurrAspectRate = 0f;
        private float m_InitSpeed;
        private bool m_IsStart = false;
        private Vector2 m_CameraClamp = Vector2.zero;
        private Transform m_Target = null;
        private MapBorder m_XBorder = new MapBorder();
        private MapBorder m_YBorder = new MapBorder();
    }
}