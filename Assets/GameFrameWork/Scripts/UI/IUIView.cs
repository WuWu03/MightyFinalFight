using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IUIView
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        public IUIViewSettings settings { get; }

        public string assetPath { get; }

        public bool isOpen { get; }

        public bool isShow { get; }

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
