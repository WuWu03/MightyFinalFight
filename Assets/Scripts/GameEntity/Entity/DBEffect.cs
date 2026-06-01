using DragonBones;
using WuWuFramework.Utils;
using UnityEngine;

public class DBEffect : BaseEffect
{
    private UnityArmatureComponent m_ArmatureAnimator;
    public override void Play()
    {
        base.Play();

        if (isAssetLoadComplete)
        {
            m_ArmatureAnimator.animation.timeScale = speed;
            m_ArmatureAnimator.animation.Play();
        }
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        m_ArmatureAnimator = go.GetComponent<UnityArmatureComponent>();
        m_ArmatureAnimator.animation.Stop();
        m_ArmatureAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);

        if (isPlaying)
        {
            m_ArmatureAnimator.animation.timeScale = speed;
            m_ArmatureAnimator.animation.Play();
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_ArmatureAnimator.animation.Stop();
        m_ArmatureAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_ArmatureAnimator = null;
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound", StringUtil.Append(eventObject.name, ".ogg")));
    }
}
