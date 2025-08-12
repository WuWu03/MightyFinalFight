using GameFrameWork;
using UnityEngine;

public class WaitTimeStory : BaseStory
{
    public static WaitTimeStory Create(float waitTime)
    {
        WaitTimeStory waitTimeStory = ReferencePool.Acquire<WaitTimeStory>();
        waitTimeStory.isWaitComplete = true;
        waitTimeStory.m_WaitTime = waitTime;
        return waitTimeStory;
    }

    public override bool IsStoryComplete()
    {
        float timeOffest = Time.time - m_PlayTimeStamp;
        return isPlaying && timeOffest >= m_WaitTime;
    }

    protected override void OnClear()
    {
        m_WaitTime = 0;
        m_PlayTimeStamp = 0;
    }

    protected override void OnPauseStory()
    {
        m_PauseTimeStamp = Time.time;
    }

    protected override void OnPlayStory()
    {
        m_PlayTimeStamp = Time.time;
    }

    protected override void OnResumeStory()
    {
        m_PlayTimeStamp += Time.time - m_PauseTimeStamp;
        m_PauseTimeStamp = 0;
    }

    private float m_WaitTime = 0;
    private float m_PlayTimeStamp = 0;
    private float m_PauseTimeStamp = 0;
}
