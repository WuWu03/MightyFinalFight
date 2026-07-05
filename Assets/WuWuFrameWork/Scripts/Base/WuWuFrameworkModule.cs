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
        /// 关闭模块
        /// </summary>
        public abstract void Shutdown();
    }
}