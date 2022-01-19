using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ListScrollType
{
    TopToBottom,
    BottomToTop,
    LeftToRight,
    RightToLeft,
}

public enum ListItemType
{
    Normal = 0,
    Button = 1,
    Toggle = 2,
    ToggleGroup = 3
}

public class ListItem
{
    public void Init(int index, RectTransform itemObjectRtf, float itemSize, bool isVertical)
    {
        m_index = index;
        m_itemObjectRtf = itemObjectRtf;
        m_itemSize = itemSize;
        m_isVertical = isVertical;
        m_isActive = true;
    }

    public void SetExpand(bool isExpanded, float expandSize)
    {
        if (isExpanded && m_itemSizeEX == 0)
        {
            m_itemSizeEX = expandSize;
        }
        else if (!isExpanded && m_itemSizeEX == 0)
        {
            m_itemSizeEX = 0;
            if (m_itemObjectRtf != null) 
            {
                m_itemObjectRtf.sizeDelta = (m_isVertical) ? (new Vector2(m_itemObjectRtf.sizeDelta.x, m_itemSize)) : (new Vector2(m_itemSize, m_itemObjectRtf.sizeDelta.y));
            }
        }
    }

    public void RemoveItemObject()
    {
        m_itemObjectRtf = null;
        m_checkmark = null;
        if (m_itemBtn != null)
        {
            m_itemBtn.onClick.RemoveAllListeners();
            m_itemBtn = null;
        }
    }

    public void SetItemObject(RectTransform itemObjectRtf, bool isBtn, System.Action<ListItem, int, bool> onClick)
    {
        m_itemObjectRtf = itemObjectRtf;
        if (isBtn) 
        {
            Transform trf = m_itemObjectRtf.Find("Checkmark");
            if (trf != null) 
            {
                m_checkmark = trf.GetComponent<MaskableGraphic>();
                m_checkmark.enabled = m_isSelected;
            }

            m_itemBtn = m_itemObjectRtf.GetComponent<Button>();
            if (m_itemBtn != null)
            {
                m_itemBtn.onClick.RemoveAllListeners();
                m_itemBtn.onClick.AddListener(delegate ()
                {
                    onClick(this, m_index, false);
                });
            }
        }
    }

    public float GetItemSize()
    {
        return m_itemSize + m_itemSizeEX;
    }

    public GameObject ItemObject
    {
        get 
        {
            if (m_itemObjectRtf != null)
            {
                return m_itemObjectRtf.gameObject; 
            }
            return null;
        }
    }

    public RectTransform ItemObjectRtf
    {
        get { return m_itemObjectRtf; }
    }

    public Vector2 GetLocalPostion()
    {
        return m_itemObjectRtf.anchoredPosition;
    }

    public float LocalPos
    {
        get { return m_localPos; }
        set { m_localPos = value; }
    }

    public float LineOffset 
    {
        get { return m_lineOffset; }
        set { m_lineOffset = value; }
    }

    public bool IsSelected 
    {
        get { return m_isSelected; }
        set 
        { 
            m_isSelected = value;
            if (m_checkmark != null) { m_checkmark.enabled = m_isSelected; }
        }
    }

    public int GetIndex()
    {
        return m_index;
    }

    public bool IsActive
    {
        set { m_isActive = value; }
        get { return m_isActive; }
    }

    int m_index;
    bool m_isVertical;
    bool m_isActive;

    RectTransform m_itemObjectRtf;
    Button m_itemBtn;
    MaskableGraphic m_checkmark;

    float m_itemSize;
    float m_itemSizeEX;

    float m_localPos;
    float m_lineOffset;

    bool m_isSelected;
}

public class ScrollListEX : MonoBehaviour
{
    // test
    //void Awake()
    //{
    //    Init(15, 0, delegate(ListItem item, int index, bool isSel)
    //    {
    //        if (item.ItemObject)
    //            item.ItemObject.transform.Find("Label").GetComponent<Text>().text = index.ToString();
    //    }, null);
    //}

    public void Init(int itemCount, int selectIndex, System.Action<ListItem, int, bool> onUpdateItemByIndex, System.Action<ListItem, int, bool> onClick)
    {
        if (tmpItemObjectRtf == null)
        {
            Debug.LogError("ScrollViewEX Init Failed! TmpItem is NULL!");
            return;
        }
        if (itemPerLineCount < 1) {
            Debug.LogError("ScrollViewEX Init Failed! PerLineCount is not less than 0 !");
            return;
        }
        m_scrollRect = gameObject.GetComponent<ScrollRect>();
        if (m_scrollRect == null) 
        {
            Debug.LogError("ScrollViewEX Init Failed! ScrollRect component not found!");
            return;
        }

        m_isInit = true;

        ResetShowItem();
        m_itemList.Clear();
        m_itemTotalCount = 0;
        m_curSelItemBtn = 0;

        InitPool();
        m_isTmpItemBtn = tmpItemObjectRtf.GetComponent<Button>() != null;
            
        m_isVertical = (scrollType == ListScrollType.TopToBottom || scrollType == ListScrollType.BottomToTop);
        m_numSign = (scrollType == ListScrollType.TopToBottom || scrollType == ListScrollType.RightToLeft) ? (-1) : (1);
        m_scrollRect.horizontal = !m_isVertical;
        m_scrollRect.vertical = m_isVertical;

        m_containerTrans = m_scrollRect.content;
        m_viewPortRectTransform = m_scrollRect.viewport;
        m_viewPortSize = (m_isVertical) ? (m_viewPortRectTransform.rect.height) : (m_viewPortRectTransform.rect.width);

        AdjustPivot(m_viewPortRectTransform);
        AdjustPivot(m_containerTrans);
        AdjustAnchor(m_containerTrans);
        AdjustPivot(tmpItemObjectRtf);
        AdjustAnchor(tmpItemObjectRtf);

        m_tmpItemSize = (m_isVertical) ? (tmpItemObjectRtf.rect.height) : (tmpItemObjectRtf.rect.width);
        m_tmpItemLineOffset = (m_isVertical) ? (tmpItemObjectRtf.rect.width) : (tmpItemObjectRtf.rect.height);
        
        m_onUpdateItemByIndex = onUpdateItemByIndex;
        m_onClickItem = onClick;

        SetItemCount(itemCount, true);
        SelectItem(selectIndex, true, true);
    }

    public void SetItemCount(int itemCount)
    {
        SetItemCount(itemCount, false);
    }
    public void SetItemCount(int itemCount, bool resetPos)
    {
        ResetShowItem();
        
        if (resetPos)
        {
            m_itemList.Clear();
            m_itemTotalCount = 0;
            m_curSelItemBtn = 0;

            m_containerTrans.anchoredPosition = Vector2.zero;
            m_lastPos = Vector2.zero;
        }

        float nextLinePos = (m_itemTotalCount > 0) ? (m_itemList[m_itemTotalCount - 1].LocalPos + m_itemList[m_itemTotalCount - 1].GetItemSize()) : (0);
        int nLine = (itemCount - 1) / itemPerLineCount;
        for (int i = (m_itemTotalCount - 1) / itemPerLineCount; i <= nLine; ++i)
        {
            float lineOffset = -(itemPerLineCount / 2.0f -  0.5f) * m_tmpItemLineOffset;
            int sj = m_itemTotalCount % itemPerLineCount;
            for (int j = sj; j < itemPerLineCount && m_itemTotalCount < itemCount; ++j)
            {
                ListItem item = new ListItem();
                item.Init(m_itemTotalCount, null, m_tmpItemSize, m_isVertical);
                item.LocalPos = nextLinePos;
                item.LineOffset = lineOffset;
                m_itemList.Add(item);

                lineOffset += m_tmpItemLineOffset;
                ++m_itemTotalCount;
            }

            nextLinePos += m_tmpItemSize;
        }

        while (m_itemTotalCount > itemCount)
        {
            --m_itemTotalCount;
            RectTransform itemRtf = m_itemList[m_itemTotalCount].ItemObjectRtf;
            if (itemRtf != null) 
            {
                PushItemObjRtf(itemRtf);
                m_itemList[m_itemTotalCount].RemoveItemObject();
            }
            m_itemList.RemoveAt(m_itemTotalCount);
        }

        float itemTotalSize = GetItemListTotalSize();
        Vector2 sizeDelta = m_containerTrans.sizeDelta;
        if (m_isVertical)
        {
            sizeDelta.y = itemTotalSize;
        }
        else
        {
            sizeDelta.x = itemTotalSize;
        }
        m_containerTrans.sizeDelta = sizeDelta;

        if (itemTotalSize < m_viewPortSize) 
        {
            m_containerTrans.anchoredPosition = Vector2.zero;
            m_lastPos = Vector2.zero;
        }
        else if (m_isVertical)
        {
            float curListPos = Mathf.Abs(m_containerTrans.anchoredPosition.y);
            if (curListPos + m_viewPortSize > itemTotalSize) 
            {
                m_containerTrans.anchoredPosition = new Vector2(0, -m_numSign * (itemTotalSize - m_viewPortSize));
            }
        }
        else
        {
            float curListPos = Mathf.Abs(m_containerTrans.anchoredPosition.x);
            if (curListPos + m_viewPortSize > itemTotalSize)
            {
                m_containerTrans.anchoredPosition = new Vector2(-m_numSign * (itemTotalSize - m_viewPortSize), 0);
            }
        }

        if (m_itemTotalCount == 0) { return; }

        if (itemType == ListItemType.ToggleGroup)
        {
            if (m_curSelItemBtn >= m_itemTotalCount)
            {
                m_curSelItemBtn = 0;
            }
            m_itemList[m_curSelItemBtn].IsSelected = true;
        }

        UpdateAllShownItemsPos();
    }

    public void ResetShowItem()
    {
        int showItemCount = m_showItemList.Count;
        for (int i = 0; i < showItemCount; ++i)
        {
            RectTransform itemRtf = m_showItemList[i].ItemObjectRtf;
            if (itemRtf != null)
            {
                PushItemObjRtf(itemRtf);
                m_showItemList[i].RemoveItemObject();
            }
        }
        m_showItemList.Clear();
        m_forceRefresh = true;
    }

    public void SetItemDirty(int index)
    {
        if (index < 0 || m_itemTotalCount == 0 || index >= m_itemTotalCount)
        {
            return;
        }
        ListItem item = m_itemList[index];
        if (item.ItemObjectRtf != null && item.IsActive)
        {
            RunUpdateItemCallback(item, index);
        }
    }

    public void InsertItem(int itemIndex)
    {
        int nPos = m_itemTotalCount % itemPerLineCount;

        float lineOffset = -(itemPerLineCount / 2.0f - 0.5f) * m_tmpItemLineOffset + m_tmpItemLineOffset * nPos;
        ListItem item = new ListItem();
        item.Init(m_itemTotalCount, null, m_tmpItemSize, m_isVertical);
        item.LocalPos = (m_itemTotalCount > 0) ? (m_itemList[m_itemTotalCount - 1].LocalPos + m_itemList[m_itemTotalCount - 1].GetItemSize()) : (0);
        item.LineOffset = lineOffset;
        m_itemList.Add(item);

        ++m_itemTotalCount;

        int showItemCount = m_showItemList.Count;
        for (int i = showItemCount-1; i >= 0; --i)
        {
            ListItem lisItem = m_showItemList[i];
            if (lisItem.GetIndex() >= itemIndex)
            {
                RectTransform itemRtf = lisItem.ItemObjectRtf;
                if (itemRtf != null)
                {
                    PushItemObjRtf(itemRtf);
                    lisItem.RemoveItemObject();
                    m_showItemList.RemoveAt(i);
                }

            }
        }
        m_forceRefresh = true;
    }

    public void RemoveItem(int itemIndex)
    {
        --m_itemTotalCount;
        int showItemCount = m_showItemList.Count;
        for (int i = showItemCount - 1; i >= 0; --i)
        {
            ListItem lisItem = m_showItemList[i];
            if (lisItem.GetIndex() >= itemIndex)
            {
                RectTransform itemRtf = lisItem.ItemObjectRtf;
                if (itemRtf != null)
                {
                    PushItemObjRtf(itemRtf);
                    lisItem.RemoveItemObject();
                    m_showItemList.RemoveAt(i);
                }
            }
        }
        m_forceRefresh = true;
    }

    public void MovePanelToItemIndex(int itemIndex, float offset)
    {
        m_scrollRect.StopMovement();
        if (itemIndex < 0 || m_itemTotalCount == 0)
        {
            m_containerTrans.anchoredPosition = Vector2.zero;
            return;
        }
        if (itemIndex >= m_itemTotalCount)
        {
            itemIndex = m_itemTotalCount - 1;
        }
        if (offset < 0)
        {
            offset = 0;
        }
        else if (offset > m_viewPortSize)
        {
            offset = m_viewPortSize;
        }

        ListItem endItem = m_itemList[m_itemTotalCount - 1];
        float itemTotalSize = endItem.LocalPos + endItem.GetItemSize();
        float moveToPos = m_itemList[itemIndex].LocalPos + offset;
        if (moveToPos + m_viewPortSize > itemTotalSize)
        {
            moveToPos = itemTotalSize - m_viewPortSize;
        }
        m_containerTrans.anchoredPosition = (m_isVertical) ? (new Vector2(0, -m_numSign * moveToPos)) : (new Vector2(-m_numSign * moveToPos, 0));
    }

    public void SelectItem(int itemIndex, bool isOn, bool isForce)
    {
        if (itemIndex < 0 || m_itemTotalCount == 0 || itemIndex >= m_itemTotalCount)
        {
            return;
        }
        if (isOn == false && itemType != ListItemType.Toggle)
        {
            return;
        }

        OnClick(m_itemList[itemIndex], itemIndex, isForce);
    }

    void OnClick(ListItem item, int index, bool isForce)
    {
        if (itemType == ListItemType.ToggleGroup)
        {
            if (m_curSelItemBtn == index)
            {
                if (isForce)
                {
                    RunClickCallback(m_itemList[index], index, true);
                }
                return;
            }

            int lastIndex = m_curSelItemBtn;
            m_isSwitch = true;

            item.IsSelected = true;
            m_curSelItemBtn = index;
            RunClickCallback(item, index, true);
            if (!m_isSwitch)
            {
                m_curSelItemBtn = lastIndex;
                item.IsSelected = false;
                m_itemList[lastIndex].IsSelected = true;
                return;
            }

            m_itemList[lastIndex].IsSelected = false;
            RunClickCallback(m_itemList[lastIndex], lastIndex, false);
        }
        else if (itemType == ListItemType.Toggle)
        {
            bool isSelected = !item.IsSelected;
            RunClickCallback(item, index, isSelected);
            item.IsSelected = isSelected;
        }
        else if (itemType == ListItemType.Button)
        {
            RunClickCallback(item, index, true);
        }
    }

    public void CancelClick()
    {
        m_isSwitch = false;
    }

    public void SetItemActive(int itemIndex, bool isActive)
    {
        int count = m_itemList.Count;
        if (itemIndex >= count) { return; }

        ListItem item = m_itemList[itemIndex];
        if (item.IsActive == isActive) { return; }

        item.IsActive = isActive;

        float nextItemPos = 0;
        if (isActive)
        {
            int lastShowIndex = itemIndex - 1;
            ListItem lastShowItem = null;
            while (lastShowIndex >= 0)
            {
                lastShowItem = m_itemList[lastShowIndex];
                --lastShowIndex;
                if (!lastShowItem.IsActive) { continue; }
                nextItemPos = lastShowItem.LocalPos + lastShowItem.GetItemSize();
                break;
            }
        }
        else
        {
            nextItemPos = item.LocalPos;
        }
        int index = (isActive) ? (itemIndex) : (itemIndex + 1);
        ListItem nextitem = null;
        while (index < count)
        {
            nextitem = m_itemList[index];
            if (!nextitem.IsActive) { continue; }
            nextitem.LocalPos = nextItemPos;
            nextItemPos += nextitem.GetItemSize();
            ++index;
        }

        float itemTotalSize = GetItemListTotalSize();
        Vector2 sizeDelta = m_containerTrans.sizeDelta;
        if (m_isVertical)
        {
            sizeDelta.y = itemTotalSize;
        }
        else
        {
            sizeDelta.x = itemTotalSize;
        }
        m_containerTrans.sizeDelta = sizeDelta;
        m_forceRefresh = true;
    }

    void Update()
    {
        if (!m_isInit) { return; }
        if (IsFixedList) { return; }
        if (m_forceRefresh || m_containerTrans.anchoredPosition != m_lastPos)
        {
            m_forceRefresh = false;
            m_lastPos = m_containerTrans.anchoredPosition;
            UpdateAllShownItemsPos();
        }
    }

    void UpdateAllShownItemsPos()
    {
        float curListPos = -m_numSign * ((m_isVertical) ? (m_containerTrans.anchoredPosition.y) : (m_containerTrans.anchoredPosition.x));
        int showItemCount = m_showItemList.Count;
        for (int i = showItemCount-1; i >= 0; --i) 
        {
            float itemlocalPos = m_showItemList[i].LocalPos;
            float itemSize = m_showItemList[i].GetItemSize();
            if (!m_showItemList[i].IsActive)
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
            else if (itemlocalPos < curListPos - itemSize - upSizeEX)
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
            else if (itemlocalPos > curListPos + m_viewPortSize + downSizeEX) 
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
        }

        if (m_itemTotalCount == 0) { return; }

        int target = 0;
        int start = 0;
        int end = (m_itemTotalCount - 1) / itemPerLineCount;
        int center = (end - start) / 2 + start;
        while (end >= start)
        {
            int startIndex = start * itemPerLineCount;
            float startlocalPos = m_itemList[startIndex].LocalPos;
            float startItemSize = m_itemList[startIndex].GetItemSize();
            if (startlocalPos >= curListPos - startItemSize && startlocalPos <= curListPos + m_viewPortSize)
            {
                target = start;
                break;
            }

            int endIndex = end * itemPerLineCount;
            float endlocalPos = m_itemList[endIndex].LocalPos;
            float endItemSize = m_itemList[endIndex].GetItemSize();
            if (endlocalPos + endItemSize >= curListPos && endlocalPos <= curListPos + m_viewPortSize)
            {
                target = end;
                break;
            }

            int centerIndex = center * itemPerLineCount;
            float centerlocalPos = m_itemList[centerIndex].LocalPos;
            float centerItemSize = m_itemList[centerIndex].GetItemSize();
            if (centerlocalPos + centerItemSize >= curListPos && centerlocalPos <= curListPos + m_viewPortSize)
            {
                target = center;
                break;
            }
            else if (centerlocalPos + centerItemSize < curListPos)
            {
                start = center + 1;
                --end;
                center = (end - start) / 2 + start;
            }
            else
            {
                ++start;
                end = center - 1;
                center = (end - start) / 2 + start;
            }
        }

        RectTransform itemRtf = null;
        ListItem item = null;
        for (int i = target-1; i >= 0; --i)
        {
            int j = i * itemPerLineCount;
            int jEnd = j + itemPerLineCount;
            while(j < jEnd) 
            {
                item = m_itemList[j];
                if (!item.IsActive) { ++j; continue; }
                float localPos = item.LocalPos;
                float itemSize = item.GetItemSize();
                if (localPos + itemSize < curListPos - upSizeEX || localPos > curListPos + m_viewPortSize + downSizeEX)
                {
                    break;
                }

                if (item.ItemObjectRtf == null)
                {
                    itemRtf = PopItemObjRtf();
                    itemRtf.name = j.ToString();
                    itemRtf.anchoredPosition = (m_isVertical) ? (new Vector2(item.LineOffset, m_numSign * item.LocalPos)) : (new Vector2(m_numSign * item.LocalPos, item.LineOffset));
                    item.SetItemObject(itemRtf, m_isTmpItemBtn, OnClick);
                    m_showItemList.Add(item);
                    RunUpdateItemCallback(item, j);
                }
                ++j;
            }
        }

        int nLine = (m_itemTotalCount - 1) / itemPerLineCount;
        for (int i = target; i <= nLine; ++i)
        {
            int j = i * itemPerLineCount;
            int jEnd = Mathf.Min(j + itemPerLineCount, m_itemTotalCount);
            while (j < jEnd) 
            {
                item = m_itemList[j];
                if (!item.IsActive) { ++j; continue; }
                float localPos = item.LocalPos;
                float itemSize = item.GetItemSize();
                if (localPos + itemSize < curListPos - upSizeEX || localPos > curListPos + m_viewPortSize + downSizeEX)
                {
                    break;
                }

                if (item.ItemObjectRtf == null)
                {
                    itemRtf = PopItemObjRtf();
                    itemRtf.name = j.ToString();
                    itemRtf.anchoredPosition = (m_isVertical) ? (new Vector2(item.LineOffset, m_numSign * item.LocalPos)) : (new Vector2(m_numSign * item.LocalPos, item.LineOffset));
                    item.SetItemObject(itemRtf, m_isTmpItemBtn, OnClick);
                    m_showItemList.Add(item);
                    RunUpdateItemCallback(item, j);
                }
                ++j;
            }
        }
    }

    void RunUpdateItemCallback(ListItem item, int index) 
    { 
        if (m_onUpdateItemByIndex != null) 
        {
            m_onUpdateItemByIndex(item, index, item.IsSelected);
        }
    }

    void RunClickCallback(ListItem item, int index, bool isSel)
    {
        if (m_onClickItem != null) 
        {
            m_onClickItem(item, index, isSel);
        }
    }

    float GetLineSizeWithIndex(int index)
    {
        int lineStart = index / itemPerLineCount * itemPerLineCount;
        int lineNum = (lineStart + itemPerLineCount <= m_itemTotalCount) ? (itemPerLineCount) : (m_itemTotalCount - lineStart);

        float itemSize = m_itemList[lineStart].GetItemSize();
        ++lineStart;
        while (lineStart < lineNum)
        {
            float nextItemSize = m_itemList[lineStart].GetItemSize();
            if (nextItemSize > itemSize)
            {
                itemSize = nextItemSize;
            }
        }
        return itemSize;
    }

    float GetItemListTotalSize()
    {
        if (m_itemTotalCount == 0) { return 0; }

        int lineStart = (m_itemTotalCount - 1) / itemPerLineCount * itemPerLineCount;
        int lineEnd = (lineStart + itemPerLineCount <= m_itemTotalCount) ? (lineStart + itemPerLineCount) : (m_itemTotalCount);

        ListItem item = m_itemList[lineStart];
        float itemSize = item.GetItemSize();
        float itemLocalPos = item.LocalPos;
        ++lineStart;
        while (lineStart < lineEnd)
        {
            float nextItemSize = m_itemList[lineStart].GetItemSize();
            if (nextItemSize > itemSize)
            {
                itemSize = nextItemSize;
            }
            ++lineStart;
        }
        return itemSize + itemLocalPos;
    }

    void AdjustPivot(RectTransform rtf)
    {
        Vector2 pivot = rtf.pivot;
        if (scrollType == ListScrollType.BottomToTop)
        {
            pivot.y = 0;
        }
        else if (scrollType == ListScrollType.TopToBottom)
        {
            pivot.y = 1;
        }
        else if (scrollType == ListScrollType.LeftToRight)
        {
            pivot.x = 0;
        }
        else if (scrollType == ListScrollType.RightToLeft)
        {
            pivot.x = 1;
        }
        rtf.pivot = pivot;
    }

    void AdjustAnchor(RectTransform rtf)
    {
        Vector2 anchorMin = rtf.anchorMin;
        Vector2 anchorMax = rtf.anchorMax;
        if (scrollType == ListScrollType.BottomToTop)
        {
            anchorMin.y = 0;
            anchorMax.y = 0;
        }
        else if (scrollType == ListScrollType.TopToBottom)
        {
            anchorMin.y = 1;
            anchorMax.y = 1;
        }
        else if (scrollType == ListScrollType.LeftToRight)
        {
            anchorMin.x = 0;
            anchorMax.x = 0;
        }
        else if (scrollType == ListScrollType.RightToLeft)
        {
            anchorMin.x = 1;
            anchorMax.x = 1;
        }
        rtf.anchorMin = anchorMin;
        rtf.anchorMax = anchorMax;
    }

    void InitPool()
    {
        if (m_itemObjRtfPool.Count == 0)
        {
            tmpItemObjectRtf.anchoredPosition = new Vector2(-2000, -2000);
        }
    }

    RectTransform PopItemObjRtf()
    {
        int index = m_itemObjRtfPool.Count - 1;
        if (index > -1)
        {
            RectTransform obj = m_itemObjRtfPool[index];
            m_itemObjRtfPool.RemoveAt(index);
            return obj;
        }
        else
        {
            RectTransform rtf = GameObject.Instantiate<GameObject>(tmpItemObjectRtf.gameObject, tmpItemObjectRtf.parent).GetComponent<RectTransform>();
            rtf.SetAsFirstSibling();
            return rtf;
        }
    }

    void PushItemObjRtf(RectTransform item)
    {
        if (item == null) { return; }

        item.anchoredPosition = new Vector2(-2000, -2000);
        m_itemObjRtfPool.Add(item);
    }

    Vector2 m_lastPos;
    bool m_isInit = false;      // 是否初始化
    bool m_forceRefresh = false; // 强制刷新
    
    ScrollRect m_scrollRect;

    RectTransform m_containerTrans;
    RectTransform m_viewPortRectTransform = null;

    System.Action<ListItem, int, bool> m_onUpdateItemByIndex = null;
    System.Action<ListItem, int, bool> m_onClickItem = null;
    
    List<RectTransform> m_itemObjRtfPool = new List<RectTransform>();
    List<ListItem> m_itemList = new List<ListItem>();
    List<ListItem> m_showItemList = new List<ListItem>();

    int m_itemTotalCount = 0;   //Item数量
    float m_viewPortSize;       // 显示区大小
    bool m_isVertical;          // 是否垂直
    int m_numSign;              // 位置正负号
    float m_tmpItemSize;        // 模板Item大小（垂直取高；水平取宽）
    float m_tmpItemLineOffset;  // 模板Item行偏移
    bool m_isTmpItemBtn = false;// 模板Item是否可选
    int m_curSelItemBtn = 0;
    bool m_isSwitch = false;

    public RectTransform tmpItemObjectRtf = null;                   //Item模板

    public bool IsFixedList = false;
    public float upSizeEX = 0.0f;
    public float downSizeEX = 0.0f;
    public int itemPerLineCount = 1;                                //每行Item数量
    public ListScrollType scrollType = ListScrollType.TopToBottom;  //滑动类型
    public ListItemType itemType = ListItemType.Normal;
}