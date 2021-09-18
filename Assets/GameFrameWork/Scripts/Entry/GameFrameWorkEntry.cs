using UnityEngine;
using GameFrameWork.UI;
using GameFrameWork.Input;
using GameFrameWork.Pool;
using GameFrameWork.Fsm;
using GameFrameWork.Resources;
using GameFrameWork.Sound;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.GameEntity;
using GameFrameWork.Scene;

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
            ResMgr.Init(m_Manager);
            GameObjectPool.Init(m_Manager);
            AudioClipPool.Init(m_Manager);
            SpritePool.Init(m_Manager);
            PoolMgr.Init(m_Manager);
            EntityMgr.Init(m_Manager);
            FsmMgr.Init(m_Manager);
            CameraMgr.Init(m_Manager);
            SoundMgr.Init(m_Manager);
            EventMgr.Init(m_Manager);
            SceneMgr.Init(m_Manager);
            OnInit(m_Manager);
        }

        private void Start()
        {
            OnStartGame();
        }

        private void OnApplicationQuit()
        {
            UIMgr.Ins.ShutDown();
            RedPointMgr.Ins.ShutDown();
            InputMgr.Ins.ShutDown();
            ResMgr.Ins.ShutDown();
            GameObjectPool.Ins.ShutDown();
            AudioClipPool.Ins.ShutDown();
            SpritePool.Ins.ShutDown();
            EntityMgr.Ins.ShutDown();
            FsmMgr.Ins.ShutDown();
            CameraMgr.Ins.ShutDown();
            SoundMgr.Ins.ShutDown();
            EventMgr.Ins.ShutDown();
            SceneMgr.Ins.ShutDown();
            ReferencePool.ClearAll();
            Destroy(m_Manager);
            OnExit();
        }
        protected abstract void OnInit(GameObject manager);
        protected abstract void OnStartGame();
        protected abstract void OnExit();

        private GameObject m_Manager = null;
    }
}