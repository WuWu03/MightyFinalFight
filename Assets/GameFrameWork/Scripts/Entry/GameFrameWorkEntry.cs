using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Scene;
using GameFrameWork.Audio;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace GameFrameWork
{
    [RequireComponent(typeof(AppConfig))]
    public abstract class GameFrameWorkEntry : MonoBehaviour
    {
        private void Awake()
        {
            m_Manager = new GameObject("GameManager");

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_Manager);
    
            UIMgr.Init(m_Manager);
            RedPointMgr.Init(m_Manager);
            InputMgr.Init(m_Manager);
            ResourcesMgr.Init(m_Manager);
            BehaviourTreeMgr.Init(PathUtil.behaviourTreeConfigDataPath);
            GameObjectPool.Init(m_Manager);
            EntityMgr.Init(m_Manager);
            FsmMgr.Init(m_Manager);
            CameraMgr.Init(m_Manager);
            AudioMgr.Init(m_Manager);
            EventMgr.Init(m_Manager);
            SceneMgr.Init(m_Manager);
            ResourcesPool.Init(m_Manager);
            OnInit(m_Manager);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void OnApplicationQuit()
        {
            OnExit();
            EntityMgr.instance.ShutDown();
            UIMgr.instance.ShutDown();
            RedPointMgr.instance.ShutDown();
            InputMgr.instance.ShutDown();   
            CameraMgr.instance.ShutDown();
            AudioMgr.instance.ShutDown();
            EventMgr.instance.ShutDown();
            SceneMgr.instance.ShutDown();
            FsmMgr.instance.ShutDown();
            GameObjectPool.instance.ShutDown();
            ResourcesPool.instance.ShutDown();
            ResourcesMgr.instance.ShutDown();
            ReferencePool.ReleaseAll();

            Destroy(m_Manager);
        }

        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();

        private GameObject m_Manager = null;
    }
}