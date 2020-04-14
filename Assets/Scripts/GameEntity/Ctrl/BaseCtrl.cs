using FrameWork.GameEntity;
using UnityEngine;

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

    public virtual void Release()
    {
        m_Owner = null;
    }

    protected BaseObject m_Owner = null;
}