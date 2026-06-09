using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace WuWuFramework.Camera
{
    public interface ICameraMgr
    {
        UnityCamera AddMainCamera();
        UnityCamera AddCamera(string cameraName);
        UnityCamera GetMainCamera();
        UnityCamera GetCamera(string cameraName);
        bool RemoveCamera(string cameraName);
        void AddUICamera(UnityCamera camera);
    }
}
