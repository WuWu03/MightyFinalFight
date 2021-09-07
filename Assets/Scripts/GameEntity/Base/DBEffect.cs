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
            m_UAC.animation.timeScale = m_Speed;
            m_UAC.animation.Play();
        }
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        m_UAC = go.GetComponent<UnityArmatureComponent>();
        m_UAC.animation.Stop();
        m_UAC.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);

        if (m_IsPlaying)
        {
            m_UAC.animation.timeScale = m_Speed;
            m_UAC.animation.Play();
        }
    }

    public override void Release()
    {
        base.Release();
        m_UAC.animation.Stop();
        m_UAC.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_UAC = null;
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/" + eventObject.name);
    }

    private DragonBones.UnityArmatureComponent m_UAC = null;
}
