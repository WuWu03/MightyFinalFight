namespace GameFrameWork.Scene
{
    public class LoadSceneSuccessEventArgs : BaseEventArgs
    {
        public string sceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public object arg
        {
            get
            {
                return m_Arg;
            }
        }

        public static LoadSceneSuccessEventArgs Create(string sceneName,object arg)
        {
            LoadSceneSuccessEventArgs successEventArgs = ReferencePool.Acquire<LoadSceneSuccessEventArgs>();
            successEventArgs.m_SceneName = sceneName;
            successEventArgs.m_Arg = arg;
            return successEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_Arg = null;
        }

        private object m_Arg = null;
        private string m_SceneName = string.Empty;
    }
}
