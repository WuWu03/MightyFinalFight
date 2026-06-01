using WuWuFramework.Utils;
using System.IO;
using UnityEngine;

namespace WuWuFramework
{
    public abstract class WuWuFrameworkEntry : MonoBehaviour
    {
        public static bool isStartUp
        {
            get
            {
                return s_Config is not null;
            }
        }

        public static WuWuFrameworkConfig config
        {
            get
            {
                return s_Config;
            }
        }

        private static WuWuFrameworkConfig s_Config;
        
        private void Awake()
        {
            MonoBehaviourMgr.Init(gameObject);
            s_Config = UnityEngine.Resources.Load<WuWuFrameworkConfig>(Path.GetFileNameWithoutExtension(PathUtil.WuWuFrameworkConfigDataName));
            DontDestroyOnLoad(gameObject);
            OnInit(gameObject);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void Update()
        {
            WuWuFrameworkMgr.Update(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }
        
        private void LateUpdate()
        {
            WuWuFrameworkMgr.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }
        
        private void FixedUpdate()
        {
            WuWuFrameworkMgr.FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime, Time.fixedTime, Time.fixedUnscaledTime);
        }

        private void OnApplicationQuit()
        {
            OnExit();
            WuWuFrameworkMgr.Shutdown();
            s_Config = null;
        }

        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();
    }
}