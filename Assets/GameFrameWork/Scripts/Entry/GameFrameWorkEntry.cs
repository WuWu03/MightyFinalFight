using GameFrameWork.Assets;
using GameFrameWork.Audio;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using GameFrameWork.Download;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.Net;
using GameFrameWork.Pool;
using GameFrameWork.Scene;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using GameFrameWork.Version;
using GameFrameWork.WebRequest;
using System.IO;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class GameFrameWorkEntry : MonoBehaviour
    {
        public static GameFrameWorkConfig config
        {
            get
            {
                return s_Config;
            }
        }

        private void Awake()
        {
            s_Config = Resources.Load<GameFrameWorkConfig>(Path.GetFileNameWithoutExtension(PathUtil.gameFrameWorkConfigDataName));
            s_Manager = new GameObject("GameManager");
            
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(s_Manager);
    
            UIMgr.Init(s_Manager);
            RedPointMgr.Init(s_Manager);
            InputMgr.Init(s_Manager);
            AssetsMgr.Init(s_Manager);
            BehaviourTreeMgr.Init(s_Manager);
            GameObjectPoolMgr.Init(s_Manager);
            EntityMgr.Init(s_Manager);
            FsmMgr.Init(s_Manager);
            CameraMgr.Init(s_Manager);
            AudioMgr.Init(s_Manager);
            EventMgr.Init(s_Manager);
            SceneMgr.Init(s_Manager);
            AssetsPool.Init(s_Manager);
            LocalizationMgr.Init(s_Manager);
            TimerMgr.Init(s_Manager);
            NetMgr.Init(s_Manager);
            WebRequestMgr.Init(s_Manager);
            DownloadMgr.Init(s_Manager);
            VersionMgr.Init(s_Manager);
            OnInit(s_Manager);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void OnApplicationQuit()
        {
            OnExit();
            EntityMgr.instance.ShutDown();
            BehaviourTreeMgr.instance.ShutDown();
            FsmMgr.instance.ShutDown();
            UIMgr.instance.ShutDown();
            RedPointMgr.instance.ShutDown();
            InputMgr.instance.ShutDown();
            CameraMgr.instance.ShutDown();
            AudioMgr.instance.ShutDown();
            SceneMgr.instance.ShutDown();
            TimerMgr.instance.ShutDown();
            GameObjectPoolMgr.instance.ShutDown();
            AssetsPool.instance.ShutDown();
            AssetsMgr.instance.ShutDown();
            LocalizationMgr.instance.ShutDown();
            EventMgr.instance.ShutDown();
            NetMgr.instance.ShutDown();
            WebRequestMgr.instance.ShutDown();
            DownloadMgr.instance.ShutDown();
            VersionMgr.instance.ShutDown();
            ReferencePool.ShutDown();
            Destroy(s_Manager);
            s_Manager = null;
            s_Config = null;
        }

        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();

        private static GameFrameWorkConfig s_Config = null;
        private static GameObject s_Manager = null;
    }
}