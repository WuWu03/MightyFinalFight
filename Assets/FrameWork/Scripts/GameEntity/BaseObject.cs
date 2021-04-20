using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public class BaseObject : MonoBehaviour
    {
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public int ID
        {
            get
            {
                return m_ID;
            }
        }

        public string Layer 
        {
            get 
            {
                return m_Layer;
            }
        }

        public virtual void Init(int id, string name)
        {
            m_ID = id;
            m_Name = name;
            gameObject.name = name;
        }

        public virtual void Release() { }

        public void SetName(string name)
        {
            m_Name = name;
            gameObject.name = name;
        }

        public void SetID(int id)
        {
            m_ID = id;
        }

        public void SetParent(Transform parent, bool worldPossitionStays = false)
        {
            transform.SetParent(parent, worldPossitionStays);
        }

        public void SetLayer(string layer, bool isChild = true)
        {
            m_Layer = layer;
            gameObject.SetLayer(layer, isChild);
        }

        protected virtual void Awake() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }
       
        protected int m_ID = 0;
        protected string m_Name = string.Empty;
        protected string m_Layer = "Unit";   
    }
}
