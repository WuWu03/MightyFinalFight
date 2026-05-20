using GameFrameWork.Event;
using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IPresenter
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public UIBaseSettings settings { get; }
        public string assetPath { get; }
        public bool isOpen { get; }
        public float delayTime { get; }
        public void SetMgr(IUIMgr uiMgr, IGameObjectPoolMgr gameObjectPoolMgr);
        public void Open(object arg);
        public void Update();
        public void Close();
        public void Show();
        public void Hide();
        public void Destroy();
    }
}