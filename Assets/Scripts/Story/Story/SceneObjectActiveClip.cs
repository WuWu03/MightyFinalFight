using GameFrameWork;
using GameFrameWork.GameEntity;

public class SceneObjectActiveClip : BaseClip
{
    public static SceneObjectActiveClip Create(string objectName, bool isActive)
    {
        SceneObjectActiveClip sceneObjectActiveClip = ReferencePool.Acquire<SceneObjectActiveClip>();
        sceneObjectActiveClip.m_ObjectName = objectName;
        sceneObjectActiveClip.m_IsActive = isActive;
        return sceneObjectActiveClip;
    }

    protected override void OnClear()
    {
        m_ObjectName = string.Empty;
        m_IsActive = false;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        BaseSceneObject sceneObject = EntityMgr.instance.FindEntity<BaseSceneObject>(m_ObjectName);

        if (sceneObject != null) 
        {
            sceneObject.SetActiveSelf(m_IsActive);
        }

        Complete();
    }

    protected override void OnResume()
    {

    }

    private string m_ObjectName = string.Empty;
    private bool m_IsActive = false;
}
