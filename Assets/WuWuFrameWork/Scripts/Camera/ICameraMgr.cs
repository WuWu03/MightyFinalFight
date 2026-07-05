using UnityCamera = UnityEngine.Camera;

namespace WuWuFramework.Camera
{
    public interface ICameraMgr
    {
        UnityCamera AddMainCamera(string layerName = null);
        UnityCamera AddCamera(string cameraName, string layerName = null);
        UnityCamera GetMainCamera();
        UnityCamera GetCamera(string cameraName);
        bool RemoveCamera(string cameraName);
        void AddUICamera(UnityCamera camera);
    }
}
