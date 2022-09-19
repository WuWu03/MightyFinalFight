using DragonBones;
using GameFrameWork.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBEffect : BaseEffect
{
    public override void Play()
    {
        base.Play();

        if (m_IsResComplete)
        {
            m_ArmatureAnimator.animation.timeScale = m_Speed;
            m_ArmatureAnimator.animation.Play();
        }
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
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
        base.Release();
        m_ArmatureAnimator.animation.Stop();
        m_ArmatureAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_ArmatureAnimator = null;
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/" + eventObject.name);
    }

    private DragonBones.UnityArmatureComponent m_ArmatureAnimator = null;
}
