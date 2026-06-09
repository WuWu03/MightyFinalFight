using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityCamera = UnityEngine.Camera;

namespace WuWuFramework.Camera
{
    public class CameraMgr : WuWuFrameworkModule, ICameraMgr
    {
        private UnityCamera m_MainCamera = null;
        private UnityCamera m_UICamera = null;
        private const string MainCameraName = "MainCamera";
        private Dictionary<string, UnityCamera> m_Cameras = new();

        public UnityCamera AddMainCamera()
        {
            if (m_MainCamera != null)
            {
                return m_MainCamera;
            }

            m_MainCamera = new GameObject(MainCameraName).GetOrAddComponent<UnityCamera>();
            m_MainCamera.transform.position = Vector3.zero;
            m_MainCamera.tag = MainCameraName;
            UniversalAdditionalCameraData udcData = m_MainCamera.GetUniversalAdditionalCameraData();
            udcData.renderType = CameraRenderType.Base;
            udcData.volumeLayerMask = 0;
            Object.DontDestroyOnLoad(m_MainCamera.gameObject);
            return m_MainCamera;
        }

        public void AddUICamera(UnityCamera camera)
        {
            if (m_UICamera != null)
            {
                return;
            }

            m_UICamera = camera;

            if (m_MainCamera == null)
            {
                AddMainCamera();
            }

            m_UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            UniversalAdditionalCameraData uacData = m_MainCamera.GetUniversalAdditionalCameraData();
            uacData.cameraStack.Remove(m_UICamera);
            uacData.cameraStack.Add(m_UICamera);
        }

        public UnityCamera AddCamera(string cameraName)
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            UnityCamera camera = new GameObject(cameraName).GetOrAddComponent<UnityCamera>();
            camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            UniversalAdditionalCameraData uacData = m_MainCamera.GetUniversalAdditionalCameraData();
            uacData.cameraStack.Add(camera);

            if (m_UICamera != null)
            {
                uacData.cameraStack.Remove(m_UICamera);
                uacData.cameraStack.Add(m_UICamera);
            }

            m_Cameras.Add(cameraName, camera);
            return camera;
        }



        public UnityCamera GetMainCamera()
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            return m_MainCamera;
        }

        public UnityCamera GetCamera(string cameraName)
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            if (cameraName == MainCameraName)
            {
                return m_MainCamera;
            }

            if (!m_Cameras.TryGetValue(cameraName, out UnityCamera camera))
            {
                throw new WuWuFrameworkException($"未找到名为 {cameraName} 的摄像机");
            }

            return camera;
        }

        public bool RemoveCamera(string cameraName)
        {
            if (cameraName == MainCameraName)
            {
                throw new WuWuFrameworkException("主摄像机无法删除");
            }

            if (m_Cameras.TryGetValue(cameraName, out UnityCamera camera))
            {
                m_MainCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(camera);
                Object.Destroy(camera.gameObject);
                m_Cameras.Remove(cameraName);
                return true;
            }

            return false;
        }

        public override void Shutdown()
        {
            m_Cameras.Clear();
        }
    }
}