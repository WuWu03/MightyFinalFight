namespace WuWuFramework.UI
{
    public abstract class UIBaseViewPresenter<V> : IUIViewPresenter where V : class, IUIView, new()
    {
        private V m_View;
        private BaseModel m_Model;
        private IUIMgr m_UIMgr;

        protected V view
        {
            get
            {
                return m_View;
            }
        }

        public void SetView(IUIView view)
        {
            m_View = view as V;
        }

        public void SetUIMgr(IUIMgr uiMgr)
        {
            m_UIMgr = uiMgr;
        }

        public void Open(object arg)
        {
            OnOpen(arg);
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Close()
        {
            OnClose();
        }

        public void Show(object arg)
        {
            OnShow(arg);
        }

        public void Hide()
        {
            OnHide();
        }

        public void Destroy()
        {
            OnDestroy();
            m_Model = null;
            m_View = null;
            m_UIMgr = null;
        }

        protected void CloseSelf(bool isForceDestroy = false)
        {
            m_UIMgr.Close<V>(isForceDestroy);
        }

        protected T AddModel<T>() where T : BaseModel, new()
        {
            if (m_Model == null)
            {
                T model = MVPModels.GetModel<T>();

                if (model == null)
                {
                    model = new T();
                    MVPModels.RegistModel(model);
                }

                m_Model = model;
            }

            return m_Model as T;
        }

        protected T GetModel<T>() where T : BaseModel
        {
            if (m_Model != null && m_Model is T)
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