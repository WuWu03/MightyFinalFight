using UnityEngine;

namespace GameFrameWork.Timer
{
    public class Timer : IReference
    {
        public float duration
        {
            get
            {
                return m_Duration;
            }
        }

        public bool isLooped
        {
            get
            {
                return m_IsLooped;
            }
        }

        public bool isCompleted
        {
            get
            {
                return m_IsCompleted;
            }
        }

        public bool usesRealTime
        {
            get
            {
                return m_UsesRealTime;
            }
        }

        public bool isPaused
        {
            get
            {
                return m_TimeElapsedBeforePause > 0;
            }
        }

        public bool isCancelled
        {
            get
            {
                return m_TimeElapsedBeforeCancel > 0;
            }
        }

        public bool isDone
        {
            get
            {
                return isCompleted || isCancelled;
            }
        }

        public static Timer Create(float duration, GameFrameWorkAction onComplete, GameFrameWorkAction<float> onUpdate, bool isLooped, bool usesRealTime)
        {
            Timer timer = ReferencePool.Acquire<Timer>();
            timer.m_Duration = duration;
            timer.m_OnComplete = onComplete;
            timer.m_OnUpdate = onUpdate;
            timer.m_IsLooped = isLooped;
            timer.m_UsesRealTime = usesRealTime;
            timer.m_StartTime = timer.GetWorldTime();
            timer.m_LastUpdateTime = timer.m_StartTime;
            return timer;
        }

        public void Update()
        {
            if (isDone)
            {
                return;
            }

            if (isPaused)
            {
                m_StartTime += GetTimeDelta();
                m_LastUpdateTime = GetWorldTime();
                return;
            }

            m_LastUpdateTime = GetWorldTime();

            m_OnUpdate?.Invoke(GetTimeElapsed());

            if (GetWorldTime() >= GetFireTime())
            {
                m_OnComplete?.Invoke();
                if (isLooped)
                {
                    m_StartTime = GetWorldTime();
                }
                else
                {
                    m_IsCompleted = true;
                }
            }
        }

        public void Cancel()
        {
            if (isDone)
            {
                return;
            }

            m_TimeElapsedBeforeCancel = GetTimeElapsed();
            m_TimeElapsedBeforePause = -1;
        }

        public void Pause()
        {
            if (isPaused || isDone)
            {
                return;
            }

            m_TimeElapsedBeforePause = GetTimeElapsed();
        }

        public void Resume()
        {
            if (!isPaused || isDone)
            {
                return;
            }

            m_TimeElapsedBeforePause = -1;
        }

        public float GetTimeElapsed()
        {
            if (isCompleted || GetWorldTime() >= GetFireTime())
            {
                return duration;
            }

            if (m_TimeElapsedBeforeCancel > 0)
            {
                return m_TimeElapsedBeforeCancel;
            }

            if (m_TimeElapsedBeforePause > 0)
            {
                return m_TimeElapsedBeforePause;
            }

            return GetWorldTime() - m_StartTime;
        }

        public float GetTimeRemaining()
        {
            return duration - GetTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return GetTimeElapsed() / duration;
        }

        public float GetRatioRemaining()
        {
            return GetTimeRemaining() / duration;
        }

        private float GetWorldTime()
        {
            return usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return m_StartTime + duration;
        }

        private float GetTimeDelta()
        {
            return GetWorldTime() - m_LastUpdateTime;
        }

        public void Clear()
        {
            m_OnComplete = null;
            m_OnUpdate = null;
            m_Duration = -1;
            m_StartTime = -1;
            m_LastUpdateTime = -1;
            m_TimeElapsedBeforeCancel = -1;
            m_TimeElapsedBeforePause = -1;
            m_IsLooped = false;
            m_IsCompleted = false;
            m_UsesRealTime = false;
        }

        private GameFrameWorkAction m_OnComplete = null;
        private GameFrameWorkAction<float> m_OnUpdate = null;

        private float m_Duration = -1;
        private float m_StartTime = -1;
        private float m_LastUpdateTime = -1;
        private float m_TimeElapsedBeforeCancel = -1;
        private float m_TimeElapsedBeforePause = -1;
        private bool m_IsLooped = false;
        private bool m_IsCompleted = false;
        private bool m_UsesRealTime = false;
    }
}