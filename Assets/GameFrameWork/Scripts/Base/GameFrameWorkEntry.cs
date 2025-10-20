using GameFrameWork.Utils;
using System.IO;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class GameFrameWorkEntry : MonoBehaviour
    {
        public static bool isStartUp
        {
            get
            {
                return s_Config is not null;
            }
        }

        public static GameFrameWorkConfig config
        {
            get
            {
                return s_Config;
            }
        }

        private static GameFrameWorkConfig s_Config;
        
        private void Awake()
        {
            MonoBehaviourMgr.Init(gameObject);
            s_Config = Resources.Load<GameFrameWorkConfig>(Path.GetFileNameWithoutExtension(PathUtil.gameFrameWorkConfigDataName));
            DontDestroyOnLoad(gameObject);
            OnInit(gameObject);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void Update()
        {
            GameFrameWorkMgr.Update(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }
        
        private void LateUpdate()
        {
            GameFrameWorkMgr.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }
        
        private void FixedUpdate()
        {
            GameFrameWorkMgr.FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime, Time.fixedTime, Time.fixedUnscaledTime);
        }

        private void OnApplicationQuit()
        {
            OnExit();
            GameFrameWorkMgr.Shutdown();
            s_Config = null;
        }

        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();
    }
}