using FrameWork.Pool;
using FrameWork.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UI
{
    public abstract class BasePanel
    {
        public abstract string PanelName { get; }
        public abstract float PanelUnLoadTime { get; }
        public abstract UIMgr.Type PanelType { get; }
        public abstract UIMgr.Layer PanelLayer { get; }
        public abstract UIMgr.CloseMode PanelCloseMode { get; }
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        public string ResPath { get; private set; }
        public BasePanelCtrl PanelCtrl { get; private set; }
        protected UIRefRoot UIRefRoot { get; private set; }

        public void Init(GameObject root, BasePanelCtrl panelCtrl)
        {
            gameObject = root;
            transform = root.transform;
            UIRefRoot = root.GetComponent<UIRefRoot>();
            ResPath = UITools.GetUIResPath(PanelName);
            PanelCtrl = panelCtrl;

            if (UIRefRoot == null)
            {
                Debug.LogError("UIRefRoot is null!");
                return;
            }

            transform.SetParent(UIMgr.Ins.GetUILayer(PanelLayer), false);
            GameObject.DontDestroyOnLoad(gameObject);
            OnInit();
        }
        
        protected abstract void OnInit();
    }
}