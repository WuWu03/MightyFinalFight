using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameFrameWork.UI;

public class ScrollRectPage : MonoBehaviour
{
    public int PageCount = 1;
    public GameObject[] PageIndex;//页数的标签
    [Range(0, 0.3f)] public float AdditiveSensitivity;//翻页灵敏度修正

    private void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
        m_ScrollPos = new float[PageCount];

        for (int i = 0; i < PageCount; i++)
        {
            m_ScrollPos[i] = (float)i / (PageCount - 1);
        }

        UIEventListener.Get(gameObject).onBeginDrag.AddListener(OnBeginDrag);
        UIEventListener.Get(gameObject).onEndDrag.AddListener(OnEndDrag);
    }

    private void Update()
    {
        if (m_CanMove)
        {
            m_ScrollRect.horizontalNormalizedPosition = Mathf.Lerp(m_ScrollRect.horizontalNormalizedPosition, m_ScrollPos[m_TargetIndex], 0.1f);
            if (Mathf.Abs(m_ScrollRect.horizontalNormalizedPosition - m_ScrollPos[m_TargetIndex]) <= 0.0001f)
            {
                m_CanMove = false;

                if (PageIndex == null)
                {
                    return;
                }

                for (int i = 0; i < PageIndex.Length; i++)
                {
                    if (PageIndex[i].GetComponent<Toggle>() != null)
                    {
                        PageIndex[i].GetComponent<Toggle>().isOn = i == m_TargetIndex;
                    }
                    else
                    {
                        PageIndex[i].SetActive(i == m_TargetIndex);
                    }
                }
            }
        }
    }

    private void OnBeginDrag(GameObject go, PointerEventData eventData, object arg)
    {
        m_OriginalPos = m_ScrollRect.horizontalNormalizedPosition;
    }

    private void OnEndDrag(GameObject go, PointerEventData eventData, object arg)
    {
        bool isRight = m_OriginalPos < m_ScrollRect.horizontalNormalizedPosition;
        float posX = m_ScrollRect.horizontalNormalizedPosition + (isRight ? AdditiveSensitivity : -AdditiveSensitivity);
        int index = 0;
        float offset = Mathf.Abs(m_ScrollPos[index] - posX);

        for (int i = 0; i < m_ScrollPos.Length; i++)
        {
            float offsetTemp = Mathf.Abs(m_ScrollPos[i] - posX);
            if (offsetTemp < offset)
            {
                index = i;
                offset = offsetTemp;
            }
        }

        m_TargetIndex = index;
        m_CanMove = true;
    }

    private ScrollRect m_ScrollRect = null;
    private float[] m_ScrollPos;
    private bool m_CanMove = false;
    private int m_TargetIndex = 0;
    private float m_OriginalPos;
}
