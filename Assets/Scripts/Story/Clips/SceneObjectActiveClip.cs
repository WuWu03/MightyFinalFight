using WuWuFramework;

public class SceneObjectActiveClip : BaseClip
{
    private string m_ObjectName = string.Empty;
    private bool m_IsActive;
    
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
        BaseSceneObject sceneObject = GameEntry.entityMgr.FindEntity<BaseSceneObject>(m_ObjectName);

        if (sceneObject is not null) 
        {
            sceneObject.SetActiveSelf(m_IsActive);
        }

        Complete();
    }

    protected override void OnResume()
    {

    }
}
