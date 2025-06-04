using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;
using SRF;

namespace GameFrameWork.Timer
{
    public class Timer
    {
        public float duration { get; private set; }

        public bool isLooped { get; set; }

        public bool isCompleted { get; private set; }

        public bool usesRealTime { get; private set; }

        public bool isPaused
        {
            get
            {
                return this.m_TimeElapsedBeforePause.HasValue;
            }
        }

        public bool isCancelled
        {
            get
            {
                return this.m_TimeElapsedBeforeCancel.HasValue;
            }
        }

        public bool isDone
        {
            get
            {
                return this.isCompleted || this.isCancelled || this.isOwnerDestroyed;
            }
        }

        private bool isOwnerDestroyed
        {
            get
            {
                return this.m_HasAutoDestroyOwner && this.m_AutoDestroyOwner == null;
            }
        }


        public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            if (Timer.m_TimerManager == null)
            {
                TimerManager managerInScene = Object.FindAnyObjectByType<TimerManager>();

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
            if (this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforeCancel = this.GetTimeElapsed();
            this.m_TimeElapsedBeforePause = null;
        }

        public void Pause()
        {
            if (this.isPaused || this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = this.GetTimeElapsed();
        }

        public void Resume()
        {
            if (!this.isPaused || this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = null;
        }

        public float GetTimeElapsed()
        {
            if (this.isCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.duration;
            }

            return this.m_TimeElapsedBeforeCancel ??
                   this.m_TimeElapsedBeforePause ??
                   this.GetWorldTime() - this.m_StartTime;
        }

        public float GetTimeRemaining()
        {
            return this.duration - this.GetTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.duration;
        }

        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.duration;
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

        private Timer(float duration, Action onComplete, Action<float> onUpdate, bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
        {
            this.duration = duration;
            this.m_OnComplete = onComplete;
            this.m_OnUpdate = onUpdate;

            this.isLooped = isLooped;
            this.usesRealTime = usesRealTime;

            this.m_AutoDestroyOwner = autoDestroyOwner;
            this.m_HasAutoDestroyOwner = autoDestroyOwner != null;

            this.m_StartTime = this.GetWorldTime();
            this.m_LastUpdateTime = this.m_StartTime;
        }

        private float GetWorldTime()
        {
            return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return this.m_StartTime + this.duration;
        }

        private float GetTimeDelta()
        {
            return this.GetWorldTime() - this.m_LastUpdateTime;
        }

        private void Update()
        {
            if (this.isDone)
            {
                return;
            }

            if (this.isPaused)
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

                if (this.isLooped)
                {
                    this.m_StartTime = this.GetWorldTime();
                }
                else
                {
                    this.isCompleted = true;
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

                this.m_ListTimers.RemoveAll(t => t.isDone);
            }

            private List<Timer> m_ListTimers = new List<Timer>();
            private List<Timer> m_ListTimersToAdd = new List<Timer>();
        }
    }
}