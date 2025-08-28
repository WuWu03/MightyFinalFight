using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public int id
        {
            get
            {
                return m_Id;
            }
        }

        public string layer
        {
            get
            {
                return m_Layer;
            }
        }

        public GameObject asset
        {
            get
            {
                return m_Asset;
            }
        }

        public bool isAssetLoadComplete
        {
            get
            {
                return m_IsAssetLoadComplete;
            }
        }


        public void Init(int id, string name)
        {
            m_IsAssetLoadComplete = false;
            m_Id = id;
            this.name = name;
            OnInit();
        }

        private void Update()
        {
            if (!m_IsAssetLoadComplete)
            {
                return;
            }

            OnUpdate();
        }

        private void LateUpdate()
        {

            if (!m_IsAssetLoadComplete)
            {
                return;
            }

            OnLateUpdate();
        }

        private void FixedUpdate()
        {

            if (!m_IsAssetLoadComplete)
            {
                return;
            }

            OnFixedUpdate();
        }

        public void Release()
        {
            if (m_Id == -1)
            {
                return;
            }

            if (m_Asset != null)
            {
                GameObjectPoolMgr.instance.Put(m_AssetPath, m_Asset);
            }

            m_Id = -1;
            m_Layer = string.Empty;
            m_IsAssetLoadComplete = false;
            m_AssetPath = null;
            m_Asset = null;
            gameObject.SetActiveSelf(false);
            OnRelease();
            EntityMgr.instance.PutEntity(this);
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            gameObject.transform.SetParent(parent, worldPossitionStays);
            gameObject.transform.localPosition = Vector3.zero;
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                m_Layer = layer;
            }

            gameObject.SetLayer(m_Layer, isChild);
        }

        public void SetAsset(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (m_AssetPath == assetPath)
            {
                return;
            }

            m_AssetPath = assetPath;
            m_IsAssetLoadComplete = false;
            GameObjectPoolMgr.instance.GetFromAsset(assetPath, LoadAssetComplete);
        }

        protected void SetLayer(bool isChild = true)
        {
            SetLayer(m_Layer, isChild);
        }

        private void LoadAssetComplete(string assetPath, UnityEngine.Object obj, object arg)
        {
            if (obj == null)
            {
                Release();
                return;
            }

            m_Asset = obj as GameObject;
            m_Asset.transform.SetParent(transform, false);
            m_Asset.transform.localPosition = Vector3.zero;
            m_Asset.SetActiveSelf(true);
            SetLayer();
            OnLoadAssetComplete(m_Asset, arg);
            m_IsAssetLoadComplete = true;
        }

        protected virtual void OnInit() { }
        protected virtual void OnLoadAssetComplete(GameObject go, object arg) { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnRelease() { }

        private int m_Id = 0;
        private string m_Layer = string.Empty;
        private bool m_IsAssetLoadComplete = false;
        private string m_AssetPath = string.Empty;
        private GameObject m_Asset;
    }
}