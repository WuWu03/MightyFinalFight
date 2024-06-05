using GameFrameWork.Assets;
using GameFrameWork.Audio;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.Pool;
using GameFrameWork.Scene;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork
{
    [RequireComponent(typeof(AppConfig))]
    public abstract class GameFrameWorkEntry : MonoBehaviour
    {
        public static bool IsApplicationRunning()
        {
            return s_Manager != null;
        }

        private void Awake()
        {
            s_Manager = new GameObject("GameManager");

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(s_Manager);
    
            UIMgr.Init(s_Manager);
            RedPointMgr.Init(s_Manager);
            InputMgr.Init(s_Manager);
            AssetsMgr.Init(s_Manager);
            BehaviourTreeMgr.Init(PathUtil.behaviourTreeConfigDataPath);
            GameObjectPool.Init(s_Manager);
            EntityMgr.Init(s_Manager);
            FsmMgr.Init(s_Manager);
            CameraMgr.Init(s_Manager);
            AudioMgr.Init(s_Manager);
            EventMgr.Init(s_Manager);
            SceneMgr.Init(s_Manager);
            AssetsPool.Init(s_Manager);
            LocalizationMgr.Init(s_Manager);
            OnInit(s_Manager);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void OnApplicationQuit()
        {
            OnExit();
            EventMgr.instance.DispatchNow(this, GameEventArgs.Create(GameFrameWorkCommonEvent.ApplicationQuitEvent));

            EntityMgr.instance.ShutDown();
            UIMgr.instance.ShutDown();
            RedPointMgr.instance.ShutDown();
            InputMgr.instance.ShutDown();
            CameraMgr.instance.ShutDown();
            AudioMgr.instance.ShutDown();
            SceneMgr.instance.ShutDown();
            FsmMgr.instance.ShutDown();
            GameObjectPool.instance.ShutDown();
            AssetsPool.instance.ShutDown();
            AssetsMgr.instance.ShutDown();
            ReferencePool.ShutDown();
            LocalizationMgr.instance.ShutDown();
            EventMgr.instance.ShutDown();
            Destroy(s_Manager);
            s_Manager = null;
        }

        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();

        private static GameObject s_Manager = null;
    }
}