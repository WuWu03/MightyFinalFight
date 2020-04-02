using FrameWork.GameEntity;
using UnityEngine;

namespace Runtime
{
    public abstract class BaseCtrl : MonoBehaviour
    {
        public BaseObject Owner
        {
            get
            {
                return m_Owner;
            }
        }
        protected virtual void Awake()
        {
            m_Owner = GetComponent<BaseObject>();
        }

        protected virtual void Update()
        {

        }

        protected BaseObject m_Owner = null;
    }
}