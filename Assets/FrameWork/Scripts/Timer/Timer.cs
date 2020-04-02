using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace FrameWork.Timer
{
    public class Timer
    {
        public float Duration { get; private set; }

        public bool IsLooped { get; set; }

        public bool IsCompleted { get; private set; }

        public bool UsesRealTime { get; private set; }

        public bool IsPaused
        {
            get
            {
                return this.m_TimeElapsedBeforePause.HasValue;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return this.m_TimeElapsedBeforeCancel.HasValue;
            }
        }

        public bool IsDone
        {
            get
            {
                return this.IsCompleted || this.IsCancelled || this.IsOwnerDestroyed;
            }
        }

        private bool IsOwnerDestroyed
        {
            get
            {
                return this.m_HasAutoDestroyOwner && this.m_AutoDestroyOwner == null;
            }
        }


        public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            if (Timer.m_TimerManager == null)
            {
                TimerManager managerInScene = Object.FindObjectOfType<TimerManager>();
                if (managerInScene != null)
                {
                    Timer.m_TimerManager = managerInScene;
                }
                else
                {
                    GameObject managerObject = new GameObject("TimerManager");
                    Timer.m_TimerManager = managerObject.GetOrAddComponent<TimerManager>();
                }
            }

            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            Timer.m_TimerManager.RegisterTimer(timer);
            return timer;
        }

        public static void Cancel(Timer timer)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
        }

        public static void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.Pause();
            }
        }

        public static void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.Resume();
            }
        }

        public static void CancelAllRegisteredTimers()
        {
            if (Timer.m_TimerManager != null)
            {
                Timer.m_TimerManager.CancelAllTimers();
            }
        }

        public static void PauseAllRegisteredTimers()
        {
            if (Timer.m_TimerManager != null)
            {
                Timer.m_TimerManager.PauseAllTimers();
            }
        }

        public static void ResumeAllRegisteredTimers()
        {
            if (Timer.m_TimerManager != null)
            {
                Timer.m_TimerManager.ResumeAllTimers();
            }
        }

        public void Cancel()
        {
            if (this.IsDone)
            {
                return;
            }

            this.m_TimeElapsedBeforeCancel = this.GetTimeElapsed();
            this.m_TimeElapsedBeforePause = null;
        }

        public void Pause()
        {
            if (this.IsPaused || this.IsDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = this.GetTimeElapsed();
        }

        public void Resume()
        {
            if (!this.IsPaused || this.IsDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = null;
        }

        public float GetTimeElapsed()
        {
            if (this.IsCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.Duration;
            }

            return this.m_TimeElapsedBeforeCancel ??
                   this.m_TimeElapsedBeforePause ??
                   this.GetWorldTime() - this.m_StartTime;
        }

        public float GetTimeRemaining()
        {
            return this.Duration - this.GetTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.Duration;
        }

        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.Duration;
        }

        private static TimerManager m_TimerManager;

        private readonly Action m_OnComplete;
        private readonly Action<float> m_OnUpdate;
        private float m_StartTime;
        private float m_LastUpdateTime;
        private float? m_TimeElapsedBeforeCancel;
        private float? m_TimeElapsedBeforePause;

        private readonly MonoBehaviour m_AutoDestroyOwner;
        private readonly bool m_HasAutoDestroyOwner;

        private Timer(float duration, Action onComplete, Action<float> onUpdate,
            bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
        {
            this.Duration = duration;
            this.m_OnComplete = onComplete;
            this.m_OnUpdate = onUpdate;

            this.IsLooped = isLooped;
            this.UsesRealTime = usesRealTime;

            this.m_AutoDestroyOwner = autoDestroyOwner;
            this.m_HasAutoDestroyOwner = autoDestroyOwner != null;

            this.m_StartTime = this.GetWorldTime();
            this.m_LastUpdateTime = this.m_StartTime;
        }

        private float GetWorldTime()
        {
            return this.UsesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return this.m_StartTime + this.Duration;
        }

        private float GetTimeDelta()
        {
            return this.GetWorldTime() - this.m_LastUpdateTime;
        }

        private void Update()
        {
            if (this.IsDone)
            {
                return;
            }

            if (this.IsPaused)
            {
                this.m_StartTime += this.GetTimeDelta();
                this.m_LastUpdateTime = this.GetWorldTime();
                return;
            }

            this.m_LastUpdateTime = this.GetWorldTime();

            if (this.m_OnUpdate != null)
            {
                this.m_OnUpdate(this.GetTimeElapsed());
            }

            if (this.GetWorldTime() >= this.GetFireTime())
            {

                if (this.m_OnComplete != null)
                {
                    this.m_OnComplete();
                }

                if (this.IsLooped)
                {
                    this.m_StartTime = this.GetWorldTime();
                }
                else
                {
                    this.IsCompleted = true;
                }
            }
        }

        private class TimerManager : MonoBehaviour
        {
            public void RegisterTimer(Timer timer)
            {
                this.m_ListTimersToAdd.Add(timer);
            }

            public void CancelAllTimers()
            {
                foreach (Timer timer in this.m_ListTimers)
                {
                    timer.Cancel();
                }

                this.m_ListTimers.Clear();
                this.m_ListTimersToAdd.Clear();
            }

            public void PauseAllTimers()
            {
                foreach (Timer timer in this.m_ListTimers)
                {
                    timer.Pause();
                }
            }

            public void ResumeAllTimers()
            {
                foreach (Timer timer in this.m_ListTimers)
                {
                    timer.Resume();
                }
            }

            [UsedImplicitly]
            private void Update()
            {
                this.UpdateAllTimers();
            }

            private void UpdateAllTimers()
            {
                if (this.m_ListTimersToAdd.Count > 0)
                {
                    this.m_ListTimers.AddRange(this.m_ListTimersToAdd);
                    this.m_ListTimersToAdd.Clear();
                }

                foreach (Timer timer in this.m_ListTimers)
                {
                    timer.Update();
                }

                this.m_ListTimers.RemoveAll(t => t.IsDone);
            }

            private List<Timer> m_ListTimers = new List<Timer>();
            private List<Timer> m_ListTimersToAdd = new List<Timer>();
        }
    }
}