using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameFrameWork.Camera
{
    public struct MaskName
    {
        public const string UI = "UI";
        public const string Map = "Map";
        public const string Unit = "Unit";
    }

    public class CameraMgr : BaseMgr<CameraMgr>
    {
        public struct CameraDepthDefine
        {
            public const int MapCamera = 0;
            public const int RoleCamera = 1;
        }

        public GameObject CameraRoot
        {
            get
            {
                return m_CameraRoot;
            }
        }

        private void Awake()
        {
            m_ListCamera = new List<UnityEngine.Camera>();
            m_CameraRoot = new GameObject("CameraRoot");

            m_CameraFollow = m_CameraRoot.AddComponent<CameraFollow>();

            m_ListCamera.Add(InitCamera("MapCamera", CameraDepthDefine.MapCamera, "MainCamera", MaskName.Map));
            m_ListCamera.Add(InitCamera("RoleCamera", CameraDepthDefine.RoleCamera, maskName: MaskName.Unit));

            m_CameraFollow.Camera = UnityEngine.Camera.main;
            DontDestroyOnLoad(m_CameraRoot);
        }

        public void SetTarget(Transform target)
        {
            m_CameraFollow.SetTarget(target);
        }

        public void SetFollowSize(int width, int height)
        {
            m_CameraFollow.SetFollowSize(width, height);
        }

        public void StartFollow(bool isForceStart = false)
        {
            if (isForceStart) m_IsForceEnd = false;
            if (!m_IsForceEnd) m_CameraFollow.StartFollow();
        }

        public void EndFollow(bool isForceEnd = false)
        {
            if (isForceEnd) m_IsForceEnd = true;
            m_CameraFollow.EndFollow();
        }

        public void Shake(float time = 0.3f)
        {
            m_CameraFollow.EndFollow();
            m_CameraRoot.transform.DOShakePosition(time, 0.1f, 20, 100).OnComplete(OnShakeComplete);
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

        private void LateUpdate()
        {

        }

        private void OnShakeComplete()
        {
            if (!m_IsForceEnd)
                m_CameraFollow.StartFollow();
        }

        private UnityEngine.Camera InitCamera(string name, int depth, string tag = "Untagged", params string[] maskName)
        {
            UnityEngine.Camera camera = new GameObject(name).AddComponent<UnityEngine.Camera>();
            camera.transform.SetParent(m_CameraRoot.transform, false);
            camera.transform.localPosition = Vector3.forward * -50;

            camera.tag = tag;
            camera.orthographic = true;
            camera.orthographicSize = GetOrthgraphicSize();
            camera.nearClipPlane = -500;
            camera.farClipPlane = 500;
            camera.depth = depth;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = LayerMask.GetMask(maskName);

            return camera;
        }

        public float GetOrthgraphicSize()
        {
            float cameraRate = (float)Screen.width / Screen.height;
            float sizeRate = cameraRate / m_NormalRate;

            if (sizeRate < 1)
            {
                sizeRate = 1;
            }

            if (sizeRate > 1)
            {
                sizeRate = m_NormalRate / cameraRate;
            }

            return sizeRate * m_NormalSize / 100;
        }

        protected override void OnShutDown()
        {
            m_ListCamera.Clear();
        }

        private const float m_NormalRate = 1280f / 720f;
        private const float m_NormalSize = 200f / 2;

        private List<UnityEngine.Camera> m_ListCamera = null;
        private CameraFollow m_CameraFollow = null;
        private GameObject m_CameraRoot = null;
        private bool m_IsForceEnd = false;
    }
}