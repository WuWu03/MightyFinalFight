using WuWuFramework.Resources;
using WuWuFramework.Event;
using UnityEngine.SceneManagement;

namespace WuWuFramework.Scene
{
    public interface ISceneMgr
    {
        public event WuWuFrameworkAction<LoadSceneSuccessEventArgs> loadSceneSuccessEvent;
        public event WuWuFrameworkAction<LoadSceneFailureEventArgs> loadSceneFailuerEvent;
        public event WuWuFrameworkAction<LoadSceneUpdateEventArgs> loadSceneUpdateEvent;
        public bool isLoading { get; }
        public string currSceneName { get; }
        public int loadedSceneCount { get; }
        
        public void SetResourceMgr(IResourcesMgr resourceMgr);
        public void LoadSceneAsync(string sceneName, object arg = null);
        public void LoadSceneAsync(string sceneName, bool isAutoAllowScene, object arg = null);
        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, object arg = null);
        public void LoadScene(string sceneName, object arg = null);
        public void LoadScene(string sceneName, LoadSceneMode mode, object arg = null);
        public void UnLoadScene(string sceneName, params object[] args);
        public bool IsSceneLoaded(string sceneName);
        public bool IsSceneLoading(string sceneName);
        public void AllowScene();
    }
}