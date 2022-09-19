using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public RectTransform itemTemplete = null;                   //Item模板
    public bool isFixedList = false;
    public float upSizeEX = 0.0f;
    public float downSizeEX = 0.0f;
    public int itemPerLineCount = 1;                                //每行Item数量
    public ListScrollType scrollType = ListScrollType.TopToBottom;  //滑动类型
    public ListItemType itemType = ListItemType.Normal;


    public void Init(int itemCount, int selectIndex, System.Action<ListItem, int, bool> onUpdateItemByIndex, System.Action<ListItem, int, bool> onClick)
    {
        if (itemTemplete == null)
        {
            Debug.LogError("ScrollViewEX Init Failed! TmpItem is NULL!");
            return;
        }
        if (itemPerLineCount < 1) {
            Debug.LogError("ScrollViewEX Init Failed! PerLineCount is not less than 0 !");
            return;
        }
        m_ScrollRect = gameObject.GetComponent<ScrollRect>();

        if (m_ScrollRect == null) 
        {
            Debug.LogError("ScrollViewEX Init Failed! ScrollRect component not found!");
            return;
        }

        m_isInit = true;

        ResetShowItem();

        m_ItemList.Clear();
        m_ItemTotalCount = 0;
        m_CurSelItemBtn = 0;

        InitPool();

        m_IsTmpItemBtn = itemTemplete.GetComponent<Button>() != null;  
        m_IsVertical = (scrollType == ListScrollType.TopToBottom || scrollType == ListScrollType.BottomToTop);
        m_NumSign = (scrollType == ListScrollType.TopToBottom || scrollType == ListScrollType.RightToLeft) ? (-1) : (1);
        m_ScrollRect.horizontal = !m_IsVertical;
        m_ScrollRect.vertical = m_IsVertical;

        m_ContainerTrans = m_ScrollRect.content;
        m_ViewPortRectTransform = m_ScrollRect.viewport;
        m_ViewPortSize = (m_IsVertical) ? (m_ViewPortRectTransform.rect.height) : (m_ViewPortRectTransform.rect.width);

        AdjustPivot(m_ViewPortRectTransform);
        AdjustPivot(m_ContainerTrans);
        AdjustAnchor(m_ContainerTrans);
        AdjustPivot(itemTemplete);
        AdjustAnchor(itemTemplete);

        m_TmpItemSize = (m_IsVertical) ? (itemTemplete.rect.height) : (itemTemplete.rect.width);
        m_TmpItemLineOffset = (m_IsVertical) ? (itemTemplete.rect.width) : (itemTemplete.rect.height);
        
        m_OnUpdateItemByIndex = onUpdateItemByIndex;
        m_OnClickItem = onClick;

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
            m_ItemList.Clear();
            m_ItemTotalCount = 0;
            m_CurSelItemBtn = 0;

            m_ContainerTrans.anchoredPosition = Vector2.zero;
            m_lastPos = Vector2.zero;
        }

        float nextLinePos = (m_ItemTotalCount > 0) ? (m_ItemList[m_ItemTotalCount - 1].LocalPos + m_ItemList[m_ItemTotalCount - 1].GetItemSize()) : (0);
        int nLine = (itemCount - 1) / itemPerLineCount;

        for (int i = (m_ItemTotalCount - 1) / itemPerLineCount; i <= nLine; ++i)
        {
            float lineOffset = -(itemPerLineCount / 2.0f -  0.5f) * m_TmpItemLineOffset;
            int sj = m_ItemTotalCount % itemPerLineCount;

            for (int j = sj; j < itemPerLineCount && m_ItemTotalCount < itemCount; ++j)
            {
                ListItem item = new ListItem();
                item.Init(m_ItemTotalCount, null, m_TmpItemSize, m_IsVertical);
                item.LocalPos = nextLinePos;
                item.LineOffset = lineOffset;
                m_ItemList.Add(item);

                lineOffset += m_TmpItemLineOffset;
                ++m_ItemTotalCount;
            }

            nextLinePos += m_TmpItemSize;
        }

        while (m_ItemTotalCount > itemCount)
        {
            --m_ItemTotalCount;
            RectTransform itemRtf = m_ItemList[m_ItemTotalCount].ItemObjectRtf;

            if (itemRtf != null) 
            {
                PushItemObjRtf(itemRtf);
                m_ItemList[m_ItemTotalCount].RemoveItemObject();
            }

            m_ItemList.RemoveAt(m_ItemTotalCount);
        }

        float itemTotalSize = GetItemListTotalSize();
        Vector2 sizeDelta = m_ContainerTrans.sizeDelta;

        if (m_IsVertical)
        {
            sizeDelta.y = itemTotalSize;
        }
        else
        {
            sizeDelta.x = itemTotalSize;
        }
        m_ContainerTrans.sizeDelta = sizeDelta;

        if (itemTotalSize < m_ViewPortSize) 
        {
            m_ContainerTrans.anchoredPosition = Vector2.zero;
            m_lastPos = Vector2.zero;
        }
        else if (m_IsVertical)
        {
            float curListPos = Mathf.Abs(m_ContainerTrans.anchoredPosition.y);
            if (curListPos + m_ViewPortSize > itemTotalSize) 
            {
                m_ContainerTrans.anchoredPosition = new Vector2(0, -m_NumSign * (itemTotalSize - m_ViewPortSize));
            }
        }
        else
        {
            float curListPos = Mathf.Abs(m_ContainerTrans.anchoredPosition.x);
            if (curListPos + m_ViewPortSize > itemTotalSize)
            {
                m_ContainerTrans.anchoredPosition = new Vector2(-m_NumSign * (itemTotalSize - m_ViewPortSize), 0);
            }
        }

        if (m_ItemTotalCount == 0) 
        { 
            return;
        }

        if (itemType == ListItemType.ToggleGroup)
        {
            if (m_CurSelItemBtn >= m_ItemTotalCount)
            {
                m_CurSelItemBtn = 0;
            }

            m_ItemList[m_CurSelItemBtn].IsSelected = true;
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
        if (index < 0 || m_ItemTotalCount == 0 || index >= m_ItemTotalCount)
        {
            return;
        }

        ListItem item = m_ItemList[index];

        if (item.ItemObjectRtf != null && item.IsActive)
        {
            RunUpdateItemCallback(item, index);
        }
    }

    public void InsertItem(int itemIndex)
    {
        int nPos = m_ItemTotalCount % itemPerLineCount;

        float lineOffset = -(itemPerLineCount / 2.0f - 0.5f) * m_TmpItemLineOffset + m_TmpItemLineOffset * nPos;
        ListItem item = new ListItem();
        item.Init(m_ItemTotalCount, null, m_TmpItemSize, m_IsVertical);
        item.LocalPos = (m_ItemTotalCount > 0) ? (m_ItemList[m_ItemTotalCount - 1].LocalPos + m_ItemList[m_ItemTotalCount - 1].GetItemSize()) : (0);
        item.LineOffset = lineOffset;
        m_ItemList.Add(item);

        ++m_ItemTotalCount;

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
        --m_ItemTotalCount;
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
        m_ScrollRect.StopMovement();
        if (itemIndex < 0 || m_ItemTotalCount == 0)
        {
            m_ContainerTrans.anchoredPosition = Vector2.zero;
            return;
        }
        if (itemIndex >= m_ItemTotalCount)
        {
            itemIndex = m_ItemTotalCount - 1;
        }
        if (offset < 0)
        {
            offset = 0;
        }
        else if (offset > m_ViewPortSize)
        {
            offset = m_ViewPortSize;
        }

        ListItem endItem = m_ItemList[m_ItemTotalCount - 1];
        float itemTotalSize = endItem.LocalPos + endItem.GetItemSize();
        float moveToPos = m_ItemList[itemIndex].LocalPos + offset;

        if (moveToPos + m_ViewPortSize > itemTotalSize)
        {
            moveToPos = itemTotalSize - m_ViewPortSize;
        }

        m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, -m_NumSign * moveToPos)) : (new Vector2(-m_NumSign * moveToPos, 0));
    }

    public void SelectItem(int itemIndex, bool isOn, bool isForce)
    {
        if (itemIndex < 0 || m_ItemTotalCount == 0 || itemIndex >= m_ItemTotalCount)
        {
            return;
        }
        if (isOn == false && itemType != ListItemType.Toggle)
        {
            return;
        }

        OnClick(m_ItemList[itemIndex], itemIndex, isForce);
    }

    private void OnClick(ListItem item, int index, bool isForce)
    {
        if (itemType == ListItemType.ToggleGroup)
        {
            if (m_CurSelItemBtn == index)
            {
                if (isForce)
                {
                    RunClickCallback(m_ItemList[index], index, true);
                }
                return;
            }

            int lastIndex = m_CurSelItemBtn;
            m_isSwitch = true;

            item.IsSelected = true;
            m_CurSelItemBtn = index;
            RunClickCallback(item, index, true);
            if (!m_isSwitch)
            {
                m_CurSelItemBtn = lastIndex;
                item.IsSelected = false;
                m_ItemList[lastIndex].IsSelected = true;
                return;
            }

            m_ItemList[lastIndex].IsSelected = false;
            RunClickCallback(m_ItemList[lastIndex], lastIndex, false);
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
        int count = m_ItemList.Count;

        if (itemIndex >= count) 
        { 
            return; 
        }

        ListItem item = m_ItemList[itemIndex];

        if (item.IsActive == isActive) 
        { 
            return; 
        }

        item.IsActive = isActive;

        float nextItemPos = 0;

        if (isActive)
        {
            int lastShowIndex = itemIndex - 1;
            ListItem lastShowItem = null;

            while (lastShowIndex >= 0)
            {
                lastShowItem = m_ItemList[lastShowIndex];
                --lastShowIndex;

                if (!lastShowItem.IsActive) 
                { 
                    continue; 
                }
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
            nextitem = m_ItemList[index];

            if (!nextitem.IsActive) 
            { 
                continue; 
            }
            nextitem.LocalPos = nextItemPos;
            nextItemPos += nextitem.GetItemSize();
            ++index;
        }

        float itemTotalSize = GetItemListTotalSize();
        Vector2 sizeDelta = m_ContainerTrans.sizeDelta;

        if (m_IsVertical)
        {
            sizeDelta.y = itemTotalSize;
        }
        else
        {
            sizeDelta.x = itemTotalSize;
        }
        m_ContainerTrans.sizeDelta = sizeDelta;
        m_forceRefresh = true;
    }

    private void Update()
    {
        if (!m_isInit) 
        { 
            return; 
        }
        
        if (isFixedList) 
        { 
            return; 
        }

        if (m_forceRefresh || m_ContainerTrans.anchoredPosition != m_lastPos)
        {
            m_forceRefresh = false;
            m_lastPos = m_ContainerTrans.anchoredPosition;
            UpdateAllShownItemsPos();
        }
    }

    private void UpdateAllShownItemsPos()
    {
        float curListPos = -m_NumSign * ((m_IsVertical) ? (m_ContainerTrans.anchoredPosition.y) : (m_ContainerTrans.anchoredPosition.x));
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
            else if (itemlocalPos > curListPos + m_ViewPortSize + downSizeEX) 
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
        }

        if (m_ItemTotalCount == 0) 
        { 
            return; 
        }

        int target = 0;
        int start = 0;
        int end = (m_ItemTotalCount - 1) / itemPerLineCount;
        int center = (end - start) / 2 + start;

        while (end >= start)
        {
            int startIndex = start * itemPerLineCount;
            float startlocalPos = m_ItemList[startIndex].LocalPos;
            float startItemSize = m_ItemList[startIndex].GetItemSize();

            if (startlocalPos >= curListPos - startItemSize && startlocalPos <= curListPos + m_ViewPortSize)
            {
                target = start;
                break;
            }

            int endIndex = end * itemPerLineCount;
            float endlocalPos = m_ItemList[endIndex].LocalPos;
            float endItemSize = m_ItemList[endIndex].GetItemSize();

            if (endlocalPos + endItemSize >= curListPos && endlocalPos <= curListPos + m_ViewPortSize)
            {
                target = end;
                break;
            }

            int centerIndex = center * itemPerLineCount;
            float centerlocalPos = m_ItemList[centerIndex].LocalPos;
            float centerItemSize = m_ItemList[centerIndex].GetItemSize();

            if (centerlocalPos + centerItemSize >= curListPos && centerlocalPos <= curListPos + m_ViewPortSize)
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

        for (int i = target - 1; i >= 0; --i)
        {
            int j = i * itemPerLineCount;
            int jEnd = j + itemPerLineCount;

            while (j < jEnd)
            {
                item = m_ItemList[j];

                if (!item.IsActive)
                {
                    ++j;
                    continue;
                }

                float localPos = item.LocalPos;
                float itemSize = item.GetItemSize();

                if (localPos + itemSize < curListPos - upSizeEX || localPos > curListPos + m_ViewPortSize + downSizeEX)
                {
                    break;
                }

                if (item.ItemObjectRtf == null)
                {
                    itemRtf = PopItemObjRtf();
                    itemRtf.name = j.ToString();
                    itemRtf.anchoredPosition = (m_IsVertical) ? (new Vector2(item.LineOffset, m_NumSign * item.LocalPos)) : (new Vector2(m_NumSign * item.LocalPos, item.LineOffset));
                    item.SetItemObject(itemRtf, m_IsTmpItemBtn, OnClick);
                    m_showItemList.Add(item);
                    RunUpdateItemCallback(item, j);
                }
                ++j;
            }
        }

        int nLine = (m_ItemTotalCount - 1) / itemPerLineCount;

        for (int i = target; i <= nLine; ++i)
        {
            int j = i * itemPerLineCount;
            int jEnd = Mathf.Min(j + itemPerLineCount, m_ItemTotalCount);

            while (j < jEnd) 
            {
                item = m_ItemList[j];

                if (!item.IsActive) 
                { 
                    ++j; 
                    continue; 
                }

                float localPos = item.LocalPos;
                float itemSize = item.GetItemSize();

                if (localPos + itemSize < curListPos - upSizeEX || localPos > curListPos + m_ViewPortSize + downSizeEX)
                {
                    break;
                }

                if (item.ItemObjectRtf == null)
                {
                    itemRtf = PopItemObjRtf();
                    itemRtf.name = j.ToString();
                    itemRtf.anchoredPosition = (m_IsVertical) ? (new Vector2(item.LineOffset, m_NumSign * item.LocalPos)) : (new Vector2(m_NumSign * item.LocalPos, item.LineOffset));
                    item.SetItemObject(itemRtf, m_IsTmpItemBtn, OnClick);
                    m_showItemList.Add(item);
                    RunUpdateItemCallback(item, j);
                }

                ++j;
            }
        }
    }

    private void RunUpdateItemCallback(ListItem item, int index) 
    { 
        if (m_OnUpdateItemByIndex != null) 
        {
            m_OnUpdateItemByIndex(item, index, item.IsSelected);
        }
    }

    private void RunClickCallback(ListItem item, int index, bool isSel)
    {
        if (m_OnClickItem != null) 
        {
            m_OnClickItem(item, index, isSel);
        }
    }

    private float GetLineSizeWithIndex(int index)
    {
        int lineStart = index / itemPerLineCount * itemPerLineCount;
        int lineNum = (lineStart + itemPerLineCount <= m_ItemTotalCount) ? (itemPerLineCount) : (m_ItemTotalCount - lineStart);

        float itemSize = m_ItemList[lineStart].GetItemSize();
        ++lineStart;
        while (lineStart < lineNum)
        {
            float nextItemSize = m_ItemList[lineStart].GetItemSize();
            if (nextItemSize > itemSize)
            {
                itemSize = nextItemSize;
            }
        }
        return itemSize;
    }

    private float GetItemListTotalSize()
    {
        if (m_ItemTotalCount == 0) 
        { 
            return 0; 
        }

        int lineStart = (m_ItemTotalCount - 1) / itemPerLineCount * itemPerLineCount;
        int lineEnd = (lineStart + itemPerLineCount <= m_ItemTotalCount) ? (lineStart + itemPerLineCount) : (m_ItemTotalCount);

        ListItem item = m_ItemList[lineStart];
        float itemSize = item.GetItemSize();
        float itemLocalPos = item.LocalPos;
        ++lineStart;

        while (lineStart < lineEnd)
        {
            float nextItemSize = m_ItemList[lineStart].GetItemSize();

            if (nextItemSize > itemSize)
            {
                itemSize = nextItemSize;
            }
            ++lineStart;
        }

        return itemSize + itemLocalPos;
    }

    private void AdjustPivot(RectTransform rtf)
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

    private void AdjustAnchor(RectTransform rtf)
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

    private void InitPool()
    {
        if (m_itemObjRtfPool.Count == 0)
        {
            itemTemplete.anchoredPosition = new Vector2(-2000, -2000);
        }
    }

    private RectTransform PopItemObjRtf()
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
            RectTransform rtf = GameObject.Instantiate<GameObject>(itemTemplete.gameObject, itemTemplete.parent).GetComponent<RectTransform>();
            rtf.SetAsFirstSibling();
            return rtf;
        }
    }

    private void PushItemObjRtf(RectTransform item)
    {
        if (item == null) 
        { 
            return; 
        }

        item.anchoredPosition = new Vector2(-2000, -2000);
        m_itemObjRtfPool.Add(item);
    }

    private int m_ItemTotalCount = 0;   //Item数量
    private int m_NumSign;              // 位置正负号
    private int m_CurSelItemBtn = 0;

    private float m_ViewPortSize;       // 显示区大小
    private float m_TmpItemSize;        // 模板Item大小（垂直取高；水平取宽）
    private float m_TmpItemLineOffset;  // 模板Item行偏移

    private bool m_isInit = false;      // 是否初始化
    private bool m_forceRefresh = false; // 强制刷新
    private bool m_IsVertical;          // 是否垂直
    private bool m_IsTmpItemBtn = false;// 模板Item是否可选
    private bool m_isSwitch = false;

    private Vector2 m_lastPos;

    private ScrollRect m_ScrollRect;
    private RectTransform m_ContainerTrans;
    private RectTransform m_ViewPortRectTransform = null;
    private Action<ListItem, int, bool> m_OnUpdateItemByIndex = null;
    private Action<ListItem, int, bool> m_OnClickItem = null;
    private List<RectTransform> m_itemObjRtfPool = new List<RectTransform>();
    private List<ListItem> m_ItemList = new List<ListItem>();
    private List<ListItem> m_showItemList = new List<ListItem>();
}