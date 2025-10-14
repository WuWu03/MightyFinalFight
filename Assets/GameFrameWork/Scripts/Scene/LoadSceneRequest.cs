using GameFrameWork.Event;
using UnityEngine.SceneManagement;

namespace GameFrameWork.Scene
{
    public class LoadSceneRequest : GameFrameWorkEventArg
    {
        public string sceneName { get; private set; }

        public object arg { get; private set; }

        public LoadSceneMode mode { get; private set; }

        public bool isAutoAllowScene { get; private set; }

        public static LoadSceneRequest Create(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, object arg)
        {
            LoadSceneRequest request = ReferencePool.Acquire<LoadSceneRequest>();
            request.sceneName = sceneName;
            request.arg = arg;
            request.mode = mode;
            request.isAutoAllowScene = isAutoAllowScene;
            return request;
        }

        public override void Clear()
        {
            sceneName = string.Empty;
            arg = null;
            isAutoAllowScene = false;
        }
    }
}
