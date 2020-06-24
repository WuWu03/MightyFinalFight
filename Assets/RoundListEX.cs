using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum RoundScrollType
{
    LeftTopToBottom,
    LeftBottomToTop,
    CenterTopToBottom,
    CenterBottomToTop,
    RightTopToBottom,
    RightBottomToTop,
    UpperLeftToRight,
    UpperRightToLeft,
    MiddleLeftToRight,
    MiddleRightToLeft,
    LowerLeftToRight,
    LowerRightToLeft,
}

public class RoundItem
{
    public void Init(int index, RectTransform itemObjectRtf, bool isVertical)
    {
        m_index = index;
        m_itemObjectRtf = itemObjectRtf;

    }

    public void RemoveItemObject()
    {
        m_itemObjectRtf = null;
        m_checkmark = null;
        m_canvasGroup = null;
        if (m_itemBtn != null)
        {
            m_itemBtn.onClick.RemoveAllListeners();
            m_itemBtn = null;
        }
    }

    public void SetItemObject(RectTransform itemObjectRtf, bool isBtn, bool isAlpha, System.Action<RoundItem, int, bool> onClick)
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
            m_itemBtn.onClick.RemoveAllListeners();
            m_itemBtn.onClick.AddListener(delegate()
            {
                onClick(this, m_index, false);
            });
        }

        if (isAlpha)
        {
            m_canvasGroup = m_itemObjectRtf.GetComponent<CanvasGroup>();
        }
    }

    public void SetAlpha(float alpha) 
    {
        m_canvasGroup.alpha = alpha;
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

    public bool IsSelected 
    {
        get { return m_isSelected; }
        set 
        { 
            m_isSelected = value;
            if (m_checkmark != null) { m_checkmark.enabled = m_isSelected; }
        }
    }

    public float LocalPos
    {
        get { return m_localPos; }
        set { m_localPos = value; }
    }

    public int Index 
    {
        get { return m_index; }
    }

    int m_index;

    float m_localPos;

    RectTransform m_itemObjectRtf;
    Button m_itemBtn;
    MaskableGraphic m_checkmark;
    CanvasGroup m_canvasGroup;

    bool m_isSelected;
}

public class RoundListEX : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    // test
    //void Awake()
    //{
    //    Init(12, 0,
    //    delegate(RoundItem item, int index, bool isSel)
    //    {
    //        Debug.LogFormat("RoundListEX Click {0} {1}", index, isSel);
    //    },
    //    delegate(RoundItem item, int index, bool isSel)
    //    {
    //        Debug.LogFormat("RoundListEX Click {0} {1}", index, isSel);

    //    });
    //}

    public void Init(int itemCount, int selectIndex, System.Action<RoundItem, int, bool> onUpdateItemByIndex, System.Action<RoundItem, int, bool> onClick)
    {
        if (TmpItemObjectRtf == null)
        {
            Debug.LogError("RoundListEX Init Failed! TmpItem is NULL!");
            return;
        }
        m_ScrollRect = gameObject.GetComponent<ScrollRect>();
        if (m_ScrollRect == null) 
        {
            Debug.LogError("RoundListEX Init Failed! ScrollRect component not found!");
            return;
        }

        m_isInit = true;

        ResetShowItem();
        m_ItemList.Clear();
        m_ItemTotalCount = 0;
        m_CurrSelectItemBtn = 0;

        InitPool();
        m_IsTmpItemBtn = TmpItemObjectRtf.GetComponent<Button>() != null;
        m_IsTmpItemAlpha = TmpItemObjectRtf.GetComponent<CanvasGroup>() != null;
            
        m_IsVertical = scrollType == RoundScrollType.LeftBottomToTop || scrollType == RoundScrollType.LeftTopToBottom ||
                       scrollType == RoundScrollType.CenterBottomToTop || scrollType == RoundScrollType.CenterTopToBottom ||
                       scrollType == RoundScrollType.RightBottomToTop || scrollType == RoundScrollType.RightTopToBottom;
        m_NumSign = (scrollType == RoundScrollType.LeftTopToBottom || scrollType == RoundScrollType.CenterTopToBottom || scrollType == RoundScrollType.RightTopToBottom ||
                     scrollType == RoundScrollType.LowerRightToLeft || scrollType == RoundScrollType.MiddleRightToLeft || scrollType == RoundScrollType.UpperRightToLeft) ? (-1) : (1);
        
        m_ScrollRect.horizontal = !m_IsVertical;
        m_ScrollRect.vertical = m_IsVertical;

        m_ContainerTrans = m_ScrollRect.content;
        m_ViewPortRectTransform = m_ScrollRect.viewport;
        m_ViewPortSize = (((ShowItemCount - 1) / 2) * 2 + 1) * ItemSpacing;

        AdjustPivot(m_ViewPortRectTransform);
        AdjustPivot(m_ContainerTrans);
        AdjustAnchor(m_ContainerTrans);
        AdjustAnchor(TmpItemObjectRtf);

        if (m_IsVertical)
        {
            m_ContainerFrame = m_ViewPortRectTransform.rect.height / 2;
        }
        else
        {
            m_ContainerFrame = m_ViewPortRectTransform.rect.width / 2;
        }
        
        m_OnUpdateItemByIndex = onUpdateItemByIndex;
        m_OnClickItem = onClick;

        SetItemCount(itemCount, true);
        SelectItem(selectIndex, true, true, true);
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
            m_CurrSelectItemBtn = 0;

            m_ContainerTrans.anchoredPosition = Vector2.zero;
            m_LastPos = Vector2.zero;
        }

        if (itemCount == m_ItemTotalCount)
        {
            return;
        }

        while (m_ItemTotalCount < itemCount)
        {
            RoundItem item = new RoundItem();
            item.Init(m_ItemTotalCount, null, m_IsVertical);
            item.LocalPos = m_ItemTotalCount * ItemSpacing + m_ContainerFrame;
            m_ItemList.Add(item);

            m_ItemTotalCount++;
        }

        while (m_ItemTotalCount > itemCount) 
        {
            m_ItemTotalCount--;
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

        if (m_ItemTotalCount == 0)
        {
            m_ContainerTrans.anchoredPosition = Vector2.zero;
            m_LastPos = Vector2.zero;
        }
        else if (m_IsVertical)
        {
            int currCenterIndex = (int)(Mathf.Abs(m_ContainerTrans.anchoredPosition.y - ItemSpacing * 0.5f) / ItemSpacing);
            if (currCenterIndex > m_ItemTotalCount)
            {
                m_ContainerTrans.anchoredPosition = new Vector2(0, -m_NumSign * itemTotalSize);
            }
        }
        else
        {
            int currCenterIndex = (int)(Mathf.Abs(m_ContainerTrans.anchoredPosition.x - ItemSpacing * 0.5f) / ItemSpacing);
            if (currCenterIndex > m_ItemTotalCount)
            {
                m_ContainerTrans.anchoredPosition = new Vector2(-m_NumSign * itemTotalSize, 0);
            }
        }

        if (m_ItemTotalCount == 0) 
        {
            return; 
        }

        if (ItemType == ListItemType.ToggleGroup)
        {
            if (m_CurrSelectItemBtn > m_ItemTotalCount)
            {
                m_CurrSelectItemBtn = 0;
            }
            m_ItemList[m_CurrSelectItemBtn].IsSelected = true;
        }

        UpdateAllShownItemsPos();
    }

    public void ResetShowItem()
    {
        int showListCount = m_showItemList.Count;
        for (int i = 0; i < showListCount; ++i)
        {
            RectTransform itemRtf = m_showItemList[i].ItemObjectRtf;
            if (itemRtf != null)
            {
                PushItemObjRtf(itemRtf);
                m_showItemList[i].RemoveItemObject();
            }
        }
        m_showItemList.Clear();
    }

    public void SetItemDirty(int index)
    {
        RoundItem item = m_ItemList[index];
        if (item.ItemObjectRtf != null)
        {
            RunUpdateItemCallback(item, index);
        }
    }

    public void MovePanelToItemIndex(int itemIndex, bool isImmediate)
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

        if (isImmediate)
        {
            float targetPos = -m_NumSign * (m_ItemList[itemIndex].LocalPos - m_ContainerFrame);
            m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, targetPos)) : (new Vector2(targetPos, 0));
        }
        
        m_toTargetPos = -m_NumSign * (m_ItemList[itemIndex].LocalPos - m_ContainerFrame);
        m_isToTarget = true;
    }

    public void SelectItem(int itemIndex)
    {
        if (itemIndex < 0 || m_ItemTotalCount == 0 || itemIndex >= m_ItemTotalCount) { return; }

        RoundItem item = m_ItemList[itemIndex];
        SelectItem(itemIndex, !item.IsSelected, false, false);
    }

    public void SelectItem(int itemIndex, bool isOn)
    {
        SelectItem(itemIndex, isOn, false, false);
    }

    public void SelectItem(int itemIndex, bool isOn, bool isForce)
    {
        SelectItem(itemIndex, isOn, isForce, false);
    }

    public void SelectItem(int itemIndex, bool isOn, bool isForce, bool isImmediate)
    {
        if (itemIndex < 0 || m_ItemTotalCount == 0 || itemIndex >= m_ItemTotalCount) { return; }
        if (ItemType == ListItemType.Toggle && isForce) 
        {
            RoundItem item = m_ItemList[itemIndex];
            item.IsSelected = isOn;
            RunClickCallback(item, itemIndex, isOn);
            return;
        }
        OnClick(m_ItemList[itemIndex], itemIndex, isForce, isImmediate);
    }

    void OnClick(RoundItem item, int index, bool isForce)
    { 
        OnClick(item, index, isForce, false);
    }

    void OnClick(RoundItem item, int index, bool isForce, bool isImmediate)
    {
        if (ItemType == ListItemType.ToggleGroup)
        {
            if (m_CurrSelectItemBtn == index && isForce)
            {
                RunClickCallback(m_ItemList[index], index, true);
                return;
            }
            MovePanelToItemIndex(index, isImmediate);
        }
        else if (ItemType == ListItemType.Toggle)
        {
            bool isSelected = !item.IsSelected;
            RunClickCallback(item, index, isSelected);
            item.IsSelected = isSelected;
        }
        else if (ItemType == ListItemType.Button)
        {
            RunClickCallback(item, index, true);
        }
    }

    void Update()
    {
        if (!m_isInit) { return; }
        if (!m_isToTarget && (Mathf.Abs(m_LastPos.x - m_ContainerTrans.anchoredPosition.x) < 0.0001f && Mathf.Abs(m_LastPos.y - m_ContainerTrans.anchoredPosition.y) < 0.0001f)) { return; }

        UpdateAllShownItemsPos();
    }

    void UpdateAllShownItemsPos()
    {
        float currListPos = -m_NumSign * ((m_IsVertical) ? (m_ContainerTrans.anchoredPosition.y) : (m_ContainerTrans.anchoredPosition.x));
        float halfItemSpacing = ItemSpacing * 0.5f;
        int currCenterIndex = (int)((currListPos + halfItemSpacing) / ItemSpacing);
        if (currCenterIndex < 0)
        {
            currCenterIndex = 0;
        }
        else if (currCenterIndex >= m_ItemTotalCount) 
        {
            currCenterIndex = m_ItemTotalCount - 1;
        }

        if (!m_IsDraging)
        {
            bool isChange = false;
            if (m_isToTarget)
            {
                float targetPos = m_toTargetPos;
                float posOffset = Mathf.Abs(-currListPos - targetPos);
                if (posOffset > 10)
                {
                    targetPos = Mathf.Lerp(-currListPos, targetPos, Time.deltaTime * MoveSpeed / posOffset);
                    m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, targetPos)) : (new Vector2(targetPos, 0));
                    isChange = true;
                }
                else if (posOffset > 0)
                {
                    m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, targetPos)) : (new Vector2(targetPos, 0));
                    isChange = true;
                }
                currListPos = -targetPos;
            }
            else
            {
                float targetPos = -m_NumSign * (m_ItemList[currCenterIndex].LocalPos - m_ContainerFrame);
                float posOffset = Mathf.Abs(-currListPos - targetPos);
                if (posOffset > 10)
                {
                    targetPos = Mathf.Lerp(-currListPos, targetPos, Time.deltaTime * MoveSpeed / posOffset);
                    m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, targetPos)) : (new Vector2(targetPos, 0));
                    isChange = true;
                }
                else if (posOffset > 0)
                {
                    m_ContainerTrans.anchoredPosition = (m_IsVertical) ? (new Vector2(0, targetPos)) : (new Vector2(targetPos, 0));
                    isChange = true;
                }
                currListPos = -targetPos;
            }

            if (ItemType == ListItemType.ToggleGroup && !isChange)
            {
                if (m_CurrSelectItemBtn == currCenterIndex && m_isToTarget)
                {
                    RunClickCallback(m_ItemList[m_CurrSelectItemBtn], m_CurrSelectItemBtn, true);
                }
                else if (m_CurrSelectItemBtn != currCenterIndex)
                {
                    m_ItemList[m_CurrSelectItemBtn].IsSelected = false;
                    RunClickCallback(m_ItemList[m_CurrSelectItemBtn], m_CurrSelectItemBtn, false);

                    m_ItemList[currCenterIndex].IsSelected = true;
                    m_CurrSelectItemBtn = currCenterIndex;
                    RunClickCallback(m_ItemList[currCenterIndex], currCenterIndex, true);
                }
                if (m_isToTarget)
                {
                    m_isToTarget = false;
                }

                m_LastPos = m_ContainerTrans.anchoredPosition;
            }
        }

        int showListCount = m_showItemList.Count;
        for (int i = showListCount - 1; i >= 0; --i) 
        {
            float itemlocalPos = m_showItemList[i].LocalPos;
            if (itemlocalPos < currListPos + m_ContainerFrame - m_ViewPortSize * 0.5f - halfItemSpacing)
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
            else if (itemlocalPos > currListPos + m_ContainerFrame + m_ViewPortSize * 0.5f + halfItemSpacing)
            {
                PushItemObjRtf(m_showItemList[i].ItemObjectRtf);
                m_showItemList[i].RemoveItemObject();
                m_showItemList.RemoveAt(i);
            }
        }

        if (m_ItemTotalCount == 0) { return; }

        RoundItem item;
        RectTransform itemRtf = null;
        for (int i = currCenterIndex; i < m_ItemTotalCount; ++i) 
        {
            item = m_ItemList[i];
            if (item.LocalPos > currListPos + m_ContainerFrame + m_ViewPortSize * 0.5f + halfItemSpacing)
            {
                break;
            }
            if (item.ItemObjectRtf == null)
            {
                itemRtf = PopItemObjRtf();
                itemRtf.name = i.ToString();
                item.SetItemObject(itemRtf, m_IsTmpItemBtn, m_IsTmpItemAlpha, OnClick);
                m_showItemList.Add(item);
                RunUpdateItemCallback(item, i);
            }
            if (m_IsTmpItemAlpha) 
            {   
                float alpha = 1 - (item.LocalPos - (currListPos + m_ContainerFrame + m_ViewPortSize * 0.5f - halfItemSpacing)) / ItemSpacing;
                item.SetAlpha(Mathf.Clamp(alpha, 0, 1));
            }

            item.ItemObjectRtf.SetAsFirstSibling();

            float itemOffset = (currListPos + m_ContainerFrame > item.LocalPos) ? (currListPos + m_ContainerFrame - item.LocalPos) : (item.LocalPos - (currListPos + m_ContainerFrame));
            float curverate = (m_ContainerFrame - itemOffset) / m_ContainerFrame;

            float scale = ScaleCurve.Evaluate(curverate);
            item.ItemObjectRtf.localScale = new Vector3(scale, scale, scale);

            float itemTargetPos = (UseWall) ? (Mathf.Min((item.LocalPos), currListPos + m_ContainerFrame + m_ViewPortSize * 0.5f - halfItemSpacing)) : (item.LocalPos);
            item.ItemObjectRtf.anchoredPosition = (m_IsVertical) ? (new Vector2(0, m_NumSign * itemTargetPos)) : (new Vector2(m_NumSign * itemTargetPos, 0));
        }
        
        for (int i = currCenterIndex - 1; i >= 0; --i)
        {
            item = m_ItemList[i];
            if (item.LocalPos < currListPos + m_ContainerFrame - m_ViewPortSize * 0.5f - halfItemSpacing)
            {
                break;
            }
            if (item.ItemObjectRtf == null)
            {
                itemRtf = PopItemObjRtf();
                itemRtf.name = i.ToString();
                item.SetItemObject(itemRtf, m_IsTmpItemBtn, m_IsTmpItemAlpha, OnClick);
                m_showItemList.Add(item);
                RunUpdateItemCallback(item, i);
            }

            if (m_IsTmpItemAlpha)
            {
                float alpha = 1 - (currListPos + m_ContainerFrame - m_ViewPortSize * 0.5f + halfItemSpacing - item.LocalPos) / ItemSpacing;
                item.SetAlpha(Mathf.Clamp(alpha, 0, 1));
            }

            item.ItemObjectRtf.SetAsFirstSibling();

            float itemOffset = (currListPos + m_ContainerFrame > item.LocalPos) ? (currListPos + m_ContainerFrame - item.LocalPos) : (item.LocalPos - (currListPos + m_ContainerFrame));
            float curverate = (m_ContainerFrame - itemOffset) / m_ContainerFrame;

            float scale = ScaleCurve.Evaluate(curverate);
            item.ItemObjectRtf.localScale = new Vector3(scale, scale, scale);

            float itemTargetPos = (UseWall) ? (Mathf.Max((item.LocalPos), currListPos + m_ContainerFrame - m_ViewPortSize * 0.5f + halfItemSpacing)) : (item.LocalPos);
            item.ItemObjectRtf.anchoredPosition = (m_IsVertical) ? (new Vector2(0, m_NumSign * itemTargetPos)) : (new Vector2(m_NumSign * itemTargetPos, 0));
        }
    }

    void RunUpdateItemCallback(RoundItem item, int index) 
    { 
        if (m_OnUpdateItemByIndex != null) 
        {
            m_OnUpdateItemByIndex(item, index, item.IsSelected);
        }
    }

    void RunClickCallback(RoundItem item, int index, bool isSel)
    {
        if (m_OnClickItem != null) 
        {
            m_OnClickItem(item, index, isSel);
        }
    }

    float GetLineSizeWithIndex(int index)
    {
        return 0;
    }

    float GetItemListTotalSize()
    {
        if (m_IsVertical)
        {
             return (m_ItemTotalCount - 1) * ItemSpacing + m_ViewPortRectTransform.rect.height;
        }
        else
        {
            return (m_ItemTotalCount - 1) * ItemSpacing + m_ViewPortRectTransform.rect.width;
        }
    }

    void AdjustPivot(RectTransform rtf)
    {
        Vector2 pivot = rtf.pivot;
        if (scrollType == RoundScrollType.LeftBottomToTop || scrollType == RoundScrollType.CenterBottomToTop || scrollType == RoundScrollType.RightBottomToTop)
        {
            pivot.y = 0;
        }
        else if (scrollType == RoundScrollType.LeftTopToBottom || scrollType == RoundScrollType.CenterTopToBottom || scrollType == RoundScrollType.RightTopToBottom)
        {
            pivot.y = 1;
        }
        else if (scrollType == RoundScrollType.LowerLeftToRight || scrollType == RoundScrollType.MiddleLeftToRight || scrollType == RoundScrollType.UpperLeftToRight)
        {
            pivot.x = 0;
        }
        else if (scrollType == RoundScrollType.LowerRightToLeft || scrollType == RoundScrollType.MiddleRightToLeft || scrollType == RoundScrollType.UpperRightToLeft)
        {
            pivot.x = 1;
        }
        rtf.pivot = pivot;
    }

    void AdjustAnchor(RectTransform rtf)
    {
        Vector2 anchorMin = rtf.anchorMin;
        Vector2 anchorMax = rtf.anchorMax;
        if (scrollType == RoundScrollType.LeftBottomToTop || scrollType == RoundScrollType.CenterBottomToTop || scrollType == RoundScrollType.RightBottomToTop)
        {
            anchorMin.y = 0;
            anchorMax.y = 0;
        }
        else if (scrollType == RoundScrollType.LeftTopToBottom || scrollType == RoundScrollType.CenterTopToBottom || scrollType == RoundScrollType.RightTopToBottom)
        {
            anchorMin.y = 1;
            anchorMax.y = 1;
        }
        else if (scrollType == RoundScrollType.LowerLeftToRight || scrollType == RoundScrollType.MiddleLeftToRight || scrollType == RoundScrollType.UpperLeftToRight)
        {
            anchorMin.x = 0;
            anchorMax.x = 0;
        }
        else if (scrollType == RoundScrollType.LowerRightToLeft || scrollType == RoundScrollType.MiddleRightToLeft || scrollType == RoundScrollType.UpperRightToLeft)
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
            TmpItemObjectRtf.anchoredPosition = new Vector2(-2000, -2000);
            m_itemObjRtfPool.Add(TmpItemObjectRtf);
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
            return GameObject.Instantiate<GameObject>(TmpItemObjectRtf.gameObject, TmpItemObjectRtf.parent).GetComponent<RectTransform>();
        }
    }

    void PushItemObjRtf(RectTransform item)
    {
        item.anchoredPosition = new Vector2(-2000, -2000);
        m_itemObjRtfPool.Add(item);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        m_IsDraging = true;
        m_isToTarget = false;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        m_IsDraging = false;
    }

    Vector2 m_LastPos;
    bool m_isInit = false;      // 是否初始化
    bool m_IsDraging = false;
    
    ScrollRect m_ScrollRect;

    RectTransform m_ContainerTrans;
    RectTransform m_ViewPortRectTransform = null;

    System.Action<RoundItem, int, bool> m_OnUpdateItemByIndex = null;
    System.Action<RoundItem, int, bool> m_OnClickItem = null;
    
    List<RectTransform> m_itemObjRtfPool = new List<RectTransform>();
    List<RoundItem> m_ItemList = new List<RoundItem>();
    List<RoundItem> m_showItemList = new List<RoundItem>();

    float m_ContainerFrame;   // container两边留出一部分用于把第一个Item置于中间
    int m_ItemTotalCount = 0;   // Item数量
    float m_ViewPortSize;       // 显示区大小
    bool m_IsVertical;          // 是否垂直
    int m_NumSign;              // 位置正负号
    bool m_IsTmpItemBtn = false;// 模板Item是否可选
    bool m_IsTmpItemAlpha = false;// 模板Item是否可以设置Alpha
    int m_CurrSelectItemBtn = 0;
    bool m_isToTarget = false;
    float m_toTargetPos;

    public int ShowItemCount = 0;                       // 显示Item数量
    public int ItemSpacing = 100;                       // Item间距
    public int MoveSpeed = 10;
    public bool UseWall = true; 

    public AnimationCurve ScaleCurve;
    public AnimationCurve PositionCurve;
    public RectTransform TmpItemObjectRtf = null;       // Item模板
    public RoundScrollType scrollType = RoundScrollType.LowerLeftToRight;  //滑动类型
    public ListItemType ItemType = ListItemType.Normal;
}