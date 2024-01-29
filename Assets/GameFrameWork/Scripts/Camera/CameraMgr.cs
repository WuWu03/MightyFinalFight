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

    public struct CameraDepthDefine
    {
        public const int MapCamera = 0;
        public const int RoleCamera = 1;
    }

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

            m_ListCamera.Add(InitCamera("MapCamera", CameraDepthDefine.MapCamera, "MainCamera", MaskName.Map));
            m_ListCamera.Add(InitCamera("RoleCamera", CameraDepthDefine.RoleCamera, maskName: MaskName.Unit));

            m_CameraFollow.camera = m_ListCamera[0];

            AllowAxisFollow(true, true);
            DontDestroyOnLoad(m_CameraRoot);
        }

        public UnityEngine.Camera GetCamera(string name)
        {
            for (int i = 0; i < m_ListCamera.Count; i++)
            {
                if(m_ListCamera[i].name.Equals(name))
                {
                    return m_ListCamera[i];
                }
            }

            return null;
        }

        public void SetOrthographicSize(float orthographicSize)
        {
            for (int i = 0; i < m_ListCamera.Count; i++)
            {
                m_ListCamera[i].orthographicSize = orthographicSize;
            }

            m_CameraFollow.UpdateOrthographicSize();
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

        public void SetFollowMode(FollowMode mode)
        {
            m_CameraFollow.followMode = mode;
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
            return m_ListCamera[0].WorldToScreenPoint(worldPos);
        }

        public void AllowAxisFollow(bool xFollow,bool yFollow)
        {
            m_CameraFollow.allowXAxisFollow = xFollow;
            m_CameraFollow.allowYAxisFollow = yFollow;
        }

        private void OnShakeComplete()
        {
            if (!m_IsForceEnd)
            {
                m_CameraFollow.StartFollow();
            }
        }

        private UnityEngine.Camera InitCamera(string name, int depth, string tag = "Untagged", params string[] maskName)
        {
            UnityEngine.Camera camera = new GameObject(name).AddComponent<UnityEngine.Camera>();
            camera.transform.SetParent(m_CameraRoot.transform, false);
            camera.transform.localPosition = Vector3.forward * -50;

            camera.tag = tag;
            camera.orthographic = true;
            camera.orthographicSize = Screen.height / 200f;
            camera.nearClipPlane = -500;
            camera.farClipPlane = 500;
            camera.depth = depth;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = LayerMask.GetMask(maskName);

            return camera;
        }

        protected override void OnShutDown()
        {
            m_ListCamera.Clear();
        }

        private const float NormalWidth = 1920f;
        private const float NormalHeight = 1080;

        private List<UnityEngine.Camera> m_ListCamera = null;
        private CameraFollow m_CameraFollow = null;
        private GameObject m_CameraRoot = null;
        private bool m_IsForceEnd = false;
    }
}