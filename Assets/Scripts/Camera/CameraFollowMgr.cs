using DG.Tweening;
using UnityEngine;
using WuWuFramework;

public class CameraFollowMgr : Singleton<CameraFollowMgr>
{
    private CameraFollow m_CameraFollow = null;
    public CameraFollow cameraFollow
    {
        get
        {
            return m_CameraFollow;
        }
    }

    public void Init()
    {
        Camera mainCamera = GameEntry.cameraMgr.AddMainCamera(LayerName.Map);
        m_CameraFollow = mainCamera.GetOrAddComponent<CameraFollow>();
        m_CameraFollow.followMode = CameraFollow.FollowMode.Just;
        m_CameraFollow.orthographicSize = 1.0f;
        m_CameraFollow.allowHorizontalAxisFollow = true;
        m_CameraFollow.allowVerticalAxisFollow = true;
        SetCameraInfo(mainCamera, LayerName.Map);
        Camera roleCamera = GameEntry.cameraMgr.AddCamera(CameraName.RoleCamera, LayerName.Unit);
        roleCamera.gameObject.transform.SetParent(mainCamera.transform, false);
        SetCameraInfo(roleCamera, LayerName.Unit);
    }

    public void Shake(float duration = 0.3f, float strength = 1f, int vibrato = 10, float randomness = 90f,
        bool snapping = false, bool fadeOut = true)
    {
        m_CameraFollow.EndFollow();
        m_CameraFollow.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(OnShakeComplete);
    }

    public override void Shutdown()
    {

    }

    private void OnShakeComplete()
    {
        m_CameraFollow.StartFollow();
    }

    private void SetCameraInfo(Camera camera, string layerName)
    {
        int layer = LayerMask.GetMask(layerName);
        camera.cullingMask = layer;
        camera.orthographic = true;
        camera.orthographicSize = 1.0f;
        camera.nearClipPlane = -1000;
        camera.farClipPlane = 1000;
        camera.backgroundColor = Color.black;
    }
}