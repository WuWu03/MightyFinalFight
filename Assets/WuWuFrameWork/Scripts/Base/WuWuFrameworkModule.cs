namespace WuWuFramework
{
    public abstract class WuWuFrameworkModule
    {
        /// <summary>
        /// 优先级
        /// </summary>
        private byte m_Priority;

        public byte priority
        {
            get
            {
                if (m_Priority != 0)
                {
                    return m_Priority;
                }

                m_Priority = WuWuFrameworkModuleFactory.GetPriority(GetType());
                return m_Priority;
            }
        }


        /// <summary>
        /// 模块Update
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        /// <param name="time"></param>
        /// <param name="unscaledTime"></param>
        public virtual void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {

        }

        /// <summary>
        /// 模块LateUpdate
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        /// <param name="time"></param>
        /// <param name="unscaledTime"></param>
        public virtual void LateUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {

        }

        /// <summary>
        /// 模块FixedUpdate
        /// </summary>
        /// <param name="fixedDeltaTime"></param>
        /// <param name="fixedUnscaledDeltaTime"></param>
        /// <param name="fixedTime"></param>
        /// <param name="fixedUnscaledTime"></param>
        public virtual void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, float fixedTime, float fixedUnscaledTime)
        {

        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        public abstract void Shutdown();
    }
}