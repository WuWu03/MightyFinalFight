using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoundObject : BaseSceneObject
{
    public BoxCollider2D boxCollider2D
    {
        get
        {
            return m_BoxCollider2D;
        }
    }

    public Rect bound
    {
        get
        {
            if (!gameObject.activeSelf)
            {
                return Rect.zero;
            }

            UpdateBound();
            return m_Bound;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        m_BoxCollider2D = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_BoxCollider2D.isTrigger = true;
        m_BoxCollider2D.enabled = false;
    }

    public override void UpdatePos(Vector2 pos, float posZ)
    {
        base.UpdatePos(pos, posZ);
        UpdateBound();
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        m_BoxCollider2D.enabled = true;
    }

    protected void SetCollider(Vector2 offset, Vector2 size)
    {
        m_BoxCollider2D.offset = offset;
        m_BoxCollider2D.size = size;
        UpdateBound();
    }

    protected void UpdateBound()
    {
        m_Bound.width = m_BoxCollider2D.size.x;
        m_Bound.height = m_BoxCollider2D.size.y;
        m_Bound.xMin = transform.localPosition.x + m_BoxCollider2D.offset.x * m_Dir - m_BoxCollider2D.size.x / 2;
        m_Bound.xMax = transform.localPosition.x + m_BoxCollider2D.offset.x * m_Dir + m_BoxCollider2D.size.x / 2;
        m_Bound.yMin = transform.localPosition.y + m_BoxCollider2D.offset.y - m_BoxCollider2D.size.y / 2;
        m_Bound.yMax = transform.localPosition.y + m_BoxCollider2D.offset.y + m_BoxCollider2D.size.y / 2;
        m_Bound.center = new Vector2(m_Bound.xMin + m_Bound.width / 2, m_Bound.yMin + m_Bound.height / 2);
    }

    private void OnDrawGizmos()
    {
        //UpdateBound();
        Vector2 leftTop = new Vector2(bound.min.x, bound.max.y);
        Vector2 rightTop = new Vector2(bound.max.x, bound.max.y);
        Vector2 leftBottom = new Vector2(bound.min.x, bound.min.y);
        Vector2 rightBottom = new Vector2(bound.max.x, bound.min.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(leftTop, rightTop);
        Gizmos.DrawLine(rightTop, rightBottom);
        Gizmos.DrawLine(rightBottom, leftBottom);
        Gizmos.DrawLine(leftBottom, leftTop);
        Gizmos.DrawCube(bound.center, Vector3.one * 0.01f);
    }

    protected Rect m_Bound = Rect.zero;
    protected BoxCollider2D m_BoxCollider2D = null;
}
