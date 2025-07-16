using UnityEngine;

namespace GameFrameWork.UI
{
    public interface IPanel
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public BasePanelSettings settings { get; }
        public string assetPath { get; }
        public bool isOpen { get; }
        public bool isInit { get; }
        public float delayTime { get; }
        public void Init(GameObject uiGameObject, string assetPath, object[] param);
        public void Open();
        public void Update();
        public void Close();
        public void Show();
        public void Hide();
        public void Destroy();
    }
}