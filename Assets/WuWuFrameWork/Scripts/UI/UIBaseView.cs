using WuWuFramework.Pool;
using WuWuFramework.Utils;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.UI
{
    public abstract class UIBaseView<V, P, S> : IUIView where V : class, IUIView, new() where P : class, IUIViewPresenter, new() where S : class, IUIViewSettings, new()
    {
        private GameObject m_GameObject;
        private Transform m_Transform;
        private IUIMgr m_UIMgr;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private string m_AssetPath;
        private bool m_IsOpen;
        private bool m_IsLoaded;
        private bool m_IsShow;
        private float m_DelayTime;
        private object m_Arg;
        private bool m_IsLoading;
        private P m_Presenter;
        private S m_Settings;

        public UIBaseView()
        {
            m_Presenter = new P();
            m_Settings = new S();
            m_Presenter.SetView(this);
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

        public P presenter
        {
            get
            {
                return m_Presenter;
            }
        }

        public IUIViewSettings settings
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

            if (m_IsLoaded)
            {
                Show();
                m_IsOpen = true;
                return;
            }

            m_IsLoading = true;
            m_GameObjectPoolMgr.GetFromAsset(PathUtil.FormatPath(PathUtil.GetUIPrefabsPath(), m_Settings.prefabName), OnLoadComplete);
        }

        public void Update()
        {
            m_Presenter.Update();
        }

        public void Close()
        {
            m_IsOpen = false;
            m_DelayTime = Time.unscaledTime;
            Hide();
            m_Presenter.Close();
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

            m_Presenter.Show(m_Arg);
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

            m_Presenter.Hide();
        }

        public void Destroy()
        {
            m_Presenter.Destroy();
            m_IsOpen = false;
            m_IsShow = false;
            m_IsLoaded = false;
            m_DelayTime = 0f;
            m_AssetPath = string.Empty;
            m_Settings = null;
            m_Arg = null;
            m_UIMgr = null;
        }

        private void OnLoadComplete(string assetPath, UnityObject uiGameObject, object arg)
        {
            m_GameObject = uiGameObject as GameObject;
            m_AssetPath = assetPath;
            m_IsOpen = true;
            m_IsLoaded = true;
            m_IsLoading = false;

            if (m_GameObject is not null)
            {
                m_Transform = m_GameObject.transform;
                OnInitView(m_GameObject.GetComponent<UIRefRoot>());
                m_GameObject.SetLayer(LayerName.UI);
                m_Transform.SetParent(m_UIMgr.uiRoot.GetLayer(settings.layer), false);
            }

            m_Presenter.Open(arg);
            Show();
        }

        protected abstract void OnInitView(UIRefRoot root);
    }
}