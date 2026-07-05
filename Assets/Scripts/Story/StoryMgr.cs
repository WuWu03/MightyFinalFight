using System;
using System.Collections.Generic;
using WuWuFramework;
using WuWuFramework.Event;

public class StoryMgr : Singleton<StoryMgr>
{
    private bool m_IsPause = false;
    private bool m_IsPlaying = false;
    private Dictionary<int, Type> m_StoryBuilders = null;
    private Dictionary<int, List<BaseClip>> m_Stories = null;
    private event WuWuFrameworkAction m_OnPlayCompleteEvent = null;

    public event WuWuFrameworkAction onPlayCompleteEvent
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

    public StoryMgr()
    {
        m_StoryBuilders = new();
        m_Stories = new();
        MonoBehaviourMgr.instance.updateEvent += OnUpdate;
    }

    public void Play(int storyId)
    {
        if (m_Stories.Count > 0)
        {
            return;
        }

        m_Stories.Clear();

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
        if (!m_Stories.TryGetValue(track, out List<BaseClip> stories))
        {
            stories = new();
            m_Stories[track] = stories;
        }

        stories.Add(story);
    }

    public override void Shutdown()
    {
        m_StoryBuilders.Clear();
        m_Stories.Clear();
        MonoBehaviourMgr.instance.updateEvent -= OnUpdate;
    }

    private void OnUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
    {
        if (!m_IsPlaying)
        {
            return;
        }

        bool isComplete = true;

        foreach (KeyValuePair<int, List<BaseClip>> kvp in m_Stories)
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
            foreach (KeyValuePair<int, List<BaseClip>> kvp in m_Stories)
            {
                for (int i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    BaseClip clip = kvp.Value[i];
                    clip.Release();
                    kvp.Value.Remove(clip);
                }
            }

            m_IsPlaying = false;
            m_Stories.Clear();
            m_OnPlayCompleteEvent?.Invoke();
            m_OnPlayCompleteEvent = null;
        }
    }
}