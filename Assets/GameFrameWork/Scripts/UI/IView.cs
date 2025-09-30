using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IView
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public UIBaseSettings settings { get; }
        public string assetPath { get; }
        public bool isOpen { get; }
        public float delayTime { get; }
        public void Open(object arg);
        public void Update();
        public void Close();
        public void Show();
        public void Hide();
        public void Destroy();
    }
}