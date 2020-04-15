using UnityEngine;
using FrameWork.UI;
using FrameWork.Input;
using FrameWork.Pool;
using FrameWork.Fsm;
using FrameWork.Resources;
using FrameWork.Sound;
using FrameWork.Camera;
using FrameWork.Event;

namespace FrameWork
{
    public abstract class FrameWorkEntry : MonoBehaviour
    {
        public RuntimeEnvironment Environment;
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            RuntimeEnvironment.Instance = Environment;
            UIMgr.Init();
            InputMgr.Init();
            ResMgr.Init();
            GameObjectPool.Init();
            AudioClipPool.Init();
            SpritePool.Init();
            SceneObjectPool.Init();
            FsmMgr.Init();
            CameraMgr.Init();
            SoundMgr.Init();
            EventManager.Init();

            OnInit();
        }

        private void Start()
        {
            OnStartGame();
        }

        protected virtual void OnApplicationQuit()
        {
            UIMgr.Ins.ShutDown();
            InputMgr.Ins.ShutDown();
            ResMgr.Ins.ShutDown();
            GameObjectPool.Ins.ShutDown();
            AudioClipPool.Ins.ShutDown();
            SpritePool.Ins.ShutDown();
            SceneObjectPool.Ins.ShutDown();
            FsmMgr.Ins.ShutDown();
            CameraMgr.Ins.ShutDown();
            SoundMgr.Ins.ShutDown();
            EventManager.Ins.ShutDown();
        }
        protected abstract void OnInit();
        protected abstract void OnStartGame();
    }
}