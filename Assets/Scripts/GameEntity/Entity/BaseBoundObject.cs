using WuWuFramework;
using UnityEngine;

public class BaseBoundObject : BaseSceneObject
{
    private Rect m_Bound = Rect.zero;
    private BoxCollider2D m_BoxCollider2D;
    
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

    protected override void OnInit()
    {
        base.OnInit();
        m_BoxCollider2D = gameObject.GetOrAddComponent<BoxCollider2D>();
        m_BoxCollider2D.isTrigger = true;
        m_BoxCollider2D.enabled = false;
    }

    public override void UpdatePos(Vector2 pos, float posZ)
    {
        base.UpdatePos(pos, posZ);
        UpdateBound();
    }

    protected override void OnRelease()
    {
        m_Bound = Rect.zero;
        m_BoxCollider2D = null;
        base.OnRelease();
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
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
        m_Bound.xMin = transform.localPosition.x + m_BoxCollider2D.offset.x * dir - m_BoxCollider2D.size.x / 2;
        m_Bound.xMax = transform.localPosition.x + m_BoxCollider2D.offset.x * dir + m_BoxCollider2D.size.x / 2;
        m_Bound.yMin = transform.localPosition.y + m_BoxCollider2D.offset.y - m_BoxCollider2D.size.y / 2;
        m_Bound.yMax = transform.localPosition.y + m_BoxCollider2D.offset.y + m_BoxCollider2D.size.y / 2;
        m_Bound.center = new (m_Bound.xMin + m_Bound.width / 2, m_Bound.yMin + m_Bound.height / 2);
    }

    private void OnDrawGizmos()
    {
        //UpdateBound();
        Vector2 leftTop = new (bound.min.x, bound.max.y);
        Vector2 rightTop = new (bound.max.x, bound.max.y);
        Vector2 leftBottom = new (bound.min.x, bound.min.y);
        Vector2 rightBottom = new (bound.max.x, bound.min.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(leftTop, rightTop);
        Gizmos.DrawLine(rightTop, rightBottom);
        Gizmos.DrawLine(rightBottom, leftBottom);
        Gizmos.DrawLine(leftBottom, leftTop);
        Gizmos.DrawCube(bound.center, Vector3.one * 0.01f);
    }
}
