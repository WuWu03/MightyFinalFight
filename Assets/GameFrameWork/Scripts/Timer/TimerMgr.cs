using System.Collections.Generic;
using GameFrameWork.Event;

namespace GameFrameWork.Timer
{
    public class TimerMgr : BaseMgr<TimerMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_TimerList = new List<Timer>();
            m_TimerQueue = new Queue<Timer>();
        }

        protected override void OnUpdate()
        {
            UpdateAllTimers();
        }

        protected override void OnShutDown()
        {
            CancelAllTimers();
            m_TimerList = null;
            m_TimerQueue = null;
        }

        public Timer Register(float duration, GameFrameWorkAction onComplete, GameFrameWorkAction<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            Timer timer = Timer.Create(duration, onComplete, onUpdate, isLooped, useRealTime);
            RegisterTimer(timer);
            return timer;
        }

        public void RegisterTimer(Timer timer)
        {
            m_TimerQueue.Enqueue(timer);
        }

        public void CancelAllTimers()
        {
            foreach (Timer timer in m_TimerList)
            {
                timer.Cancel();
                timer.Release();
            }

            while(m_TimerQueue.Count > 0)
            {
                m_TimerQueue.Dequeue().Release();
            }

            m_TimerList.Clear();
            m_TimerQueue.Clear();
        }

        public void PauseAllTimers()
        {
            foreach (Timer timer in m_TimerList)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (Timer timer in m_TimerList)
            {
                timer.Resume();
            }
        }

        private void UpdateAllTimers()
        {
            while(m_TimerQueue.Count > 0)
            {
                m_TimerList.Add(m_TimerQueue.Dequeue());
            }

            foreach (Timer timer in m_TimerList)
            {
                timer.Update();
            }

            for (int i = m_TimerList.Count - 1; i >= 0; i--)
            {
                Timer timer = m_TimerList[i];
                if (timer.isDone)
                {
                    timer.Release();
                    m_TimerList.Remove(timer);
                }
            }
        }

        private List<Timer> m_TimerList = null;
        private Queue<Timer> m_TimerQueue = null;
    }
}