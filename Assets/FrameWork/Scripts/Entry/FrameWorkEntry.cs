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
            ResPool.Init();
            ObjectPool.Init();
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

        protected abstract void OnInit();
        protected abstract void OnStartGame();
    }
}