using GameFrameWork.Pool;
using GameFrameWork.Utils;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.UI
{
    public abstract class UIBasePresenter<V, S> : IPresenter where V : UIBaseView, new() where S : UIBaseSettings, new()
    {
        private GameObject m_GameObject;
        private Transform m_Transform;
        private BaseModel m_Model;
        private IUIMgr m_UIMgr;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private V m_View;
        private S m_Settings;
        private string m_AssetPath;
        private bool m_IsOpen;
        private bool m_IsShow;
        private float m_DelayTime;
        private object m_Arg;
        private bool m_IsLoading;

        public UIBasePresenter()
        {
            m_AssetPath = string.Empty;
            m_IsOpen = false;
            m_DelayTime = 0f;
            m_IsShow = false;
            m_Settings = new S();
            m_View = new V();
        }

        public GameObject gameObject
        {
            get
            {
                return m_GameObject;
            }
        }

        public Transform transform
        {
            get
            {
                return m_Transform;
            }
        }

        public UIBaseSettings settings
        {
            get
            {
                return m_Settings;
            }
        }

        public string assetPath
        {
            get
            {
                return m_AssetPath;
            }
        }

        public bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        public bool isShow
        {
            get
            {
                return m_IsShow;
            }
        }

        public float delayTime
        {
            get
            {
                return m_DelayTime;
            }
        }

        protected V view
        {
            get
            {
                return m_View;
            }
        }

        public void SetMgr(IUIMgr uiMgr, IGameObjectPoolMgr gameObjectPoolMgr)
        {
            m_UIMgr = uiMgr;
            m_GameObjectPoolMgr = gameObjectPoolMgr;
        }

        public void Open(object arg)
        {
            if (m_IsLoading)
            {
                return;
            }

            if (arg != null)
            {
                m_Arg = arg;
            }

            if (m_IsOpen)
            {
                Show();
                return;
            }

            m_IsLoading = true;
            m_GameObjectPoolMgr.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), m_Settings.prefabName), OnLoadComplete);
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close()
        {
            m_IsOpen = false;
            m_DelayTime = Time.unscaledTime;
            Hide();
            OnClose();
        }

        public void Show()
        {
            if (m_IsShow)
            {
                return;
            }

            m_IsShow = true;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(true);
            }

            OnShow(m_Arg);
        }

        public void Hide()
        {
            if (!m_IsShow)
            {
                return;
            }

            m_IsShow = false;

            if (gameObject is not null)
            {
                gameObject.SetActiveSelf(false);
            }

            OnHide();
        }

        public void Destroy()
        {
            OnDestroy();
            m_IsOpen = false;
            m_IsShow = false;
            m_DelayTime = 0f;
            m_AssetPath = string.Empty;
            m_View = null;
            m_Settings = null;
            m_Arg = null;
            m_UIMgr = null;

        }

        private void OnLoadComplete(string assetPath, UnityObject uiGameObject, object arg)
        {
            m_GameObject = uiGameObject as GameObject;
            m_AssetPath = assetPath;
            m_IsOpen = true;
            m_IsLoading = false;

            if (m_GameObject is not null)
            {
                m_Transform = m_GameObject.transform;
                m_View.InitView(m_GameObject.GetComponent<UIRefRoot>());
                m_GameObject.SetLayer(LayerName.UI);
                m_Transform.SetParent(m_UIMgr.GetLayer(settings.layer), false);
            }

            OnOpen(m_Arg);
            Show();
        }

        protected void CloseSelf()
        {
            m_UIMgr.Close(this);
        }

        protected T AddModel<T>() where T : BaseModel, new()
        {
            if (m_Model == null)
            {
                T model = new T();
                MVPModels.RegistModel(model);
                m_Model = model;
                return model;
            }

            return null;
        }

        protected T GetModel<T>() where T : BaseModel
        {
            if(m_Model != null && m_Model is T)
            {
                return m_Model as T;
            }

            return null;
        }

        protected void RemoveModel()
        {
            if (m_Model != null)
            {
                MVPModels.UnRegistModel(m_Model);
                m_Model = null;
            }
        }

        protected abstract void OnOpen(object arg);
        protected abstract void OnShow(object arg);
        protected abstract void OnHide();
        protected abstract void OnUpdate();
        protected abstract void OnClose();
        protected abstract void OnDestroy();
        protected virtual void OnJoyStickUp() { }
        protected virtual void OnJoyStickLeft() { }
        protected virtual void OnJoyStickDown() { }
        protected virtual void OnJoyStickRight() { }
        protected virtual void OnButtonA() { }
        protected virtual void OnButtonB() { }
        protected virtual void OnButtonX() { }
        protected virtual void OnButtonY() { }
    }
}