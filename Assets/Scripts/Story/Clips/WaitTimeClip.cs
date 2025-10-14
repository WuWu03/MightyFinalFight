using GameFrameWork;
using UnityEngine;

public class WaitTimeClip : BaseClip
{
    public static WaitTimeClip Create(float waitTime)
    {
        WaitTimeClip waitTimeStory = ReferencePool.Acquire<WaitTimeClip>();
        waitTimeStory.m_WaitTime = waitTime;
        return waitTimeStory;
    }

    public override bool IsComplete()
    {
        float timeOffest = Time.time - m_PlayTimeStamp;
        return isPlaying && timeOffest >= m_WaitTime;
    }

    protected override void OnClear()
    {
        m_WaitTime = 0;
        m_PlayTimeStamp = 0;
    }

    protected override void OnPause()
    {
        m_PauseTimeStamp = Time.time;
    }

    protected override void OnPlay()
    {
        m_PlayTimeStamp = Time.time;
    }

    protected override void OnResume()
    {
        m_PlayTimeStamp += Time.time - m_PauseTimeStamp;
        m_PauseTimeStamp = 0;
    }

    private float m_WaitTime = 0;
    private float m_PlayTimeStamp = 0;
    private float m_PauseTimeStamp = 0;
}
