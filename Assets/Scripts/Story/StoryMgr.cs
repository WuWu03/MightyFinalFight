using GameFrameWork;
using System;
using System.Collections.Generic;

public class StoryMgr : BaseMgr<StoryMgr>
{
    public event GameFrameWorkAction onPlayCompleteEvent
    {
        add
        {
            m_OnPlayCompleteEvent += value;
        }
        remove
        {
            m_OnPlayCompleteEvent -= value;
        }
    }

    protected override void OnAwake()
    {
        m_Storys = new();
        m_StoryBuilders = new();
    }

    protected override void OnUpdate()
    {
        if (!m_IsPlaying)
        {
            return;
        }

        bool isComplete = true;

        foreach (KeyValuePair<int, List<BaseClip>> kvp in m_Storys)
        {
            foreach (var clip in kvp.Value)
            {
                clip.Play();

                if (m_IsPause)
                {
                    clip.Pause();
                }
                else
                {
                    clip.Resume();
                }

                if (!clip.IsComplete())
                {
                    isComplete = false;
                    break;
                }
            }
        }

        if (isComplete)
        {
            foreach (KeyValuePair<int, List<BaseClip>> kvp in m_Storys)
            {
                for (int i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    BaseClip clip = kvp.Value[i];
                    clip.Release();
                    kvp.Value.Remove(clip);
                }
            }

            m_IsPlaying = false;
            m_Storys.Clear();
            m_OnPlayCompleteEvent?.Invoke();
            m_OnPlayCompleteEvent = null;
        }
    }

    public void Play(int storyId)
    {
        if (m_Storys.Count > 0)
        {
            return;
        }

        m_Storys.Clear();

        if (m_StoryBuilders.TryGetValue(storyId, out Type builderType))
        {
            IStoryBuilder storyBuilder = Activator.CreateInstance(builderType) as IStoryBuilder;
            storyBuilder?.BuildStory();
        }

        m_IsPlaying = true;
    }

    public void Pause()
    {
        m_IsPause = true;
    }

    public void Resume()
    {
        m_IsPause = false;
    }

    public void AddStoryBuilder<T>(int storyId) where T : IStoryBuilder, new()
    {
        m_StoryBuilders.Add(storyId, typeof(T));
    }

    public void AddClip(int track, BaseClip story)
    {
        if (!m_Storys.TryGetValue(track, out List<BaseClip> stories))
        {
            stories = new();
            m_Storys[track] = stories;
        }

        stories.Add(story);
    }

    private bool m_IsPause = false;
    private bool m_IsPlaying = false;
    private Dictionary<int, Type> m_StoryBuilders = null;
    private Dictionary<int, List<BaseClip>> m_Storys = null;
    private event GameFrameWorkAction m_OnPlayCompleteEvent = null;
}