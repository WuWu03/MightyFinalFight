using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameFrameWork.Camera
{
    public class CameraMgr : BaseMgr<CameraMgr>
    {
        public GameObject cameraRoot
        {
            get
            {
                return m_CameraRoot;
            }
        }

        protected override void OnAwake()
        {
            m_ListCamera = new List<UnityEngine.Camera>();
            m_CameraRoot = new GameObject("CameraRoot");
            m_CameraFollow = m_CameraRoot.AddComponent<CameraFollow>();
   
            AllowAxisFollow(true, true);
            DontDestroyOnLoad(m_CameraRoot);
        }

        public UnityEngine.Camera AddOrthographicCamera(string name, int depth, string tag, float orthographicSize, params string[] maskName)
        {
            UnityEngine.Camera camera = new GameObject(name).AddComponent<UnityEngine.Camera>();
            camera.transform.SetParent(m_CameraRoot.transform, false);
            camera.transform.localPosition = Vector3.forward * -50;
            camera.tag = tag;
            camera.orthographic = true;
            camera.orthographicSize = orthographicSize;
            camera.nearClipPlane = -1000;
            camera.farClipPlane = 1000;
            camera.depth = depth;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = LayerMask.GetMask(maskName);

            if (tag.Equals("MainCamera"))
            {
                m_MainCamera = camera;
                m_CameraFollow.orthographicSize = orthographicSize;
            }

            m_ListCamera.Add(camera);
            return camera;
        }

        public UnityEngine.Camera GetCamera(string cameraName)
        {
            for (int i = 0; i < m_ListCamera.Count; i++)
            {
                if (m_ListCamera[i].name.Equals(cameraName))
                {
                    return m_ListCamera[i];
                }
            }

            return null;
        }

        public bool RemoveCamera(string cameraName)
        {
            for (int i = m_ListCamera.Count - 1; i >= 0; i--)
            {
                if (m_ListCamera[i].name.Equals(cameraName))
                {
                    GameObject.Destroy(m_ListCamera[i]);
                    m_ListCamera.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void SetOrthographicSize(string cameraName, float orthographicSize)
        {
            if (string.IsNullOrEmpty(cameraName))
            {
                Log.LogError("相机名称为空");
            }

            UnityEngine.Camera camera = GetCamera(cameraName);
            camera.orthographicSize = orthographicSize;

            if (camera.CompareTag("MainCamera"))
            {
                m_CameraFollow.UpdateOrthographicSize(orthographicSize);
            }
        }

        public void SetOrthographicSize(float orthographicSize)
        {
            for (int i = 0; i < m_ListCamera.Count; i++)
            {
                m_ListCamera[i].orthographicSize = orthographicSize;
            }

            m_CameraFollow.UpdateOrthographicSize(orthographicSize);
        }

        public void SetFollowTarget(Transform target)
        {
            m_CameraFollow.SetTarget(target);
        }

        public void SetFollowSize(int width, int height)
        {
            m_CameraFollow.SetFollowSize(width, height);
        }

        public void SetFollowMode(FollowMode followMode)
        {
            m_CameraFollow.followMode = followMode;
        }

        public void StartFollow(bool isForceStart = false)
        {
            if (isForceStart)
            {
                m_IsForceEnd = false;
            }

            if (!m_IsForceEnd)
            {
                m_CameraFollow.StartFollow();
            }
        }

        public void EndFollow(bool isForceEnd = false)
        {
            if (isForceEnd)
            {
                m_IsForceEnd = true;
            }

            m_CameraFollow.EndFollow();
        }

        public void AllowAxisFollow(bool allowHorizontalAxisFollow, bool allowVerticalAxisFollow)
        {
            m_CameraFollow.allowHorizontalAxisFollow = allowHorizontalAxisFollow;
            m_CameraFollow.allowVerticalAxisFollow = allowVerticalAxisFollow;
        }

        public bool IsOutVision(Vector2 targetPos)
        {
            Rect visionRect = m_CameraFollow.GetVision();
            bool xOut = targetPos.x - 0.1 <= visionRect.xMin || targetPos.x + 0.1 >= visionRect.xMax;
            bool yOut = targetPos.y - 0.1 <= visionRect.yMin || targetPos.y + 0.1 >= visionRect.yMax;
            return xOut || yOut;
        }

        public Rect GetVision()
        {
            return m_CameraFollow.GetVision();
        }

        public Vector3 WorldPosToScreenPos(Vector3 worldPos)
        {
            if (m_MainCamera)
            {
                Log.LogError("主相机不存在，请初始化主相机");
                return Vector3.zero;
            }

            return m_MainCamera.WorldToScreenPoint(worldPos);
        }

        public void Shake(float duration = 0.3f, float strength = 1f, int vibrato = 10, float randomness = 90f, bool snapping = false, bool fadeOut = true)
        {
            m_CameraFollow.EndFollow();
            m_CameraRoot.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(OnShakeComplete);
        }

        private void OnShakeComplete()
        {
            if (!m_IsForceEnd)
            {
                m_CameraFollow.StartFollow();
            }
        }

        protected override void OnShutDown()
        {
            m_ListCamera.Clear();
            m_ListCamera = null;
        }

        private List<UnityEngine.Camera> m_ListCamera = null;
        private CameraFollow m_CameraFollow = null;
        private UnityEngine.Camera m_MainCamera = null;
        private GameObject m_CameraRoot = null;
        private bool m_IsForceEnd = false;
    }
}