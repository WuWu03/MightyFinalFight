using GameFrameWork.Event;

namespace GameFrameWork.Scene
{
    public class LoadSceneFailureEventArgs : GameFrameWorkEventArg
    {
        public string sceneName { get; set; }

        public string errorMessage { get; set; }

        public object arg {  get; set; }

        public static LoadSceneFailureEventArgs Create(string sceneName, string errorMessage, object arg)
        {
            LoadSceneFailureEventArgs failureEventArgs = ReferencePool.Acquire<LoadSceneFailureEventArgs>();
            failureEventArgs.sceneName = sceneName;
            failureEventArgs.errorMessage = errorMessage;
            failureEventArgs.arg = arg;
            return failureEventArgs;
        }

        public override void Clear()
        {
            sceneName = string.Empty;
            errorMessage = string.Empty;
            arg = null;
        }
    }
}