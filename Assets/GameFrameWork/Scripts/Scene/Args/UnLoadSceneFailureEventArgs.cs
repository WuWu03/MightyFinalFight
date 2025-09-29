namespace GameFrameWork.Scene
{
    public class UnLoadSceneFailureEventArgs : BaseEventArgs
    {
        public string sceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public string errorMessage
        {
            get
            {
                return m_ErrorMessage;
            }
        }

        public object arg
        {
            get
            {
                return m_Arg;
            }
        }

        public static UnLoadSceneFailureEventArgs Create(string sceneName, string errorMessage, object arg)
        {
            UnLoadSceneFailureEventArgs failureEventArgs = ReferencePool.Acquire<UnLoadSceneFailureEventArgs>();
            failureEventArgs.m_SceneName = sceneName;
            failureEventArgs.m_ErrorMessage = errorMessage;
            failureEventArgs.m_Arg = arg;
            return failureEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_ErrorMessage = string.Empty;
            m_Arg = null;
        }

        private string m_SceneName = string.Empty;
        private string m_ErrorMessage = string.Empty;
        private object m_Arg = null;
    }
}