using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.Universal;
using UnityCamera = UnityEngine.Camera;

namespace WuWuFramework.Camera
{
    public class CameraMgr : WuWuFrameworkModule, ICameraMgr
    {
        private UnityCamera m_MainCamera = null;
        private UnityCamera m_UICamera = null;
        private const string MainCameraName = "MainCamera";
        private Dictionary<string, UnityCamera> m_Cameras = new();

        /// <summary>
        /// URP下创建全局唯一主像机
        /// </summary>
        /// <returns></returns>
        public UnityCamera AddMainCamera(string layerName = null)
        {
            if (m_MainCamera != null)
            {
                return m_MainCamera;
            }

            m_MainCamera = new GameObject(MainCameraName).GetOrAddComponent<UnityCamera>();
            m_MainCamera.transform.position = Vector3.zero;
            m_MainCamera.tag = MainCameraName;

            //if (GraphicsSettings.currentRenderPipeline != null)
            //{
            //    UniversalAdditionalCameraData udcData = m_MainCamera.GetUniversalAdditionalCameraData();
            //    udcData.renderType = CameraRenderType.Base;
            //    udcData.volumeLayerMask = string.IsNullOrEmpty(layerName) ? 0 : LayerMask.GetMask(layerName);
            //}
            //else
            {
                m_MainCamera.depth = 0;
            }

            Object.DontDestroyOnLoad(m_MainCamera.gameObject);
            return m_MainCamera;
        }

        /// <summary>
        /// URP下创建全局唯一UI相机
        /// </summary>
        /// <param name="camera"></param>
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

            //if (GraphicsSettings.currentRenderPipeline != null)
            //{
            //    m_UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            //    UniversalAdditionalCameraData udcData = m_MainCamera.GetUniversalAdditionalCameraData();
            //    udcData.cameraStack.Remove(m_UICamera);
            //    udcData.cameraStack.Add(m_UICamera);
            //}
        }

        /// <summary>
        /// URP下创建混合相机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public UnityCamera AddCamera(string cameraName, string layerName = null)
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            UnityCamera camera = new GameObject(cameraName).GetOrAddComponent<UnityCamera>();

            //if (GraphicsSettings.currentRenderPipeline != null)
            //{
            //    camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            //    UniversalAdditionalCameraData udcData = m_MainCamera.GetUniversalAdditionalCameraData();
            //    udcData.volumeLayerMask = string.IsNullOrEmpty(layerName) ? 0 : LayerMask.GetMask(layerName);
            //    udcData.cameraStack.Add(camera);

            //    if (m_UICamera != null)
            //    {
            //        udcData.cameraStack.Remove(m_UICamera);
            //        udcData.cameraStack.Add(m_UICamera);
            //    }
            //}
            //else
            {
                camera.depth = m_Cameras.Count + 1;
            }

            m_Cameras.Add(cameraName, camera);
            return camera;
        }

        /// <summary>
        /// 获取主相机
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public UnityCamera GetMainCamera()
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            return m_MainCamera;
        }

        /// <summary>
        /// 获取混合相机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public UnityCamera GetCamera(string cameraName)
        {
            if (m_MainCamera == null)
            {
                throw new WuWuFrameworkException("未找到主摄像机，调用AddMainCamera方法创建主摄像机");
            }

            if (!m_Cameras.TryGetValue(cameraName, out UnityCamera camera))
            {
                throw new WuWuFrameworkException($"未找到名为 {cameraName} 的摄像机");
            }

            return camera;
        }

        /// <summary>
        /// 移除混合相机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public bool RemoveCamera(string cameraName)
        {
            if (cameraName == MainCameraName)
            {
                throw new WuWuFrameworkException("主摄像机无法删除");
            }

            if (m_Cameras.TryGetValue(cameraName, out UnityCamera camera))
            {
                //if (GraphicsSettings.currentRenderPipeline != null)
                //{
                //    m_MainCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(camera);
                //}

                Object.Destroy(camera.gameObject);
                m_Cameras.Remove(cameraName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 框架关闭时清理相机
        /// </summary>
        public override void Shutdown()
        {
            m_Cameras.Clear();
        }
    }
}