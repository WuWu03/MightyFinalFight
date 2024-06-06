using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.Utilities;
using UnityEngine;

public class DBEffect : BaseEffect
{
    public override void Play()
    {
        base.Play();

        if (m_IsAssetLoadComplete)
        {
            m_ArmatureAnimator.animation.timeScale = m_Speed;
            m_ArmatureAnimator.animation.Play();
        }
    }

    protected override void OnLoadAssetComplete(GameObject go, object[] param)
    {
        base.OnLoadAssetComplete(go, param);
        m_ArmatureAnimator = go.GetComponent<UnityArmatureComponent>();
        m_ArmatureAnimator.animation.Stop();
        m_ArmatureAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);

        if (m_IsPlaying)
        {
            m_ArmatureAnimator.animation.timeScale = m_Speed;
            m_ArmatureAnimator.animation.Play();
        }
    }

    public override void Release()
    {
        m_ArmatureAnimator.animation.Stop();
        m_ArmatureAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_ArmatureAnimator = null;
        base.Release();
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, PathUtil.FormatPath("Sound", eventObject.name) + ".ogg");
    }

    private DragonBones.UnityArmatureComponent m_ArmatureAnimator = null;
}
