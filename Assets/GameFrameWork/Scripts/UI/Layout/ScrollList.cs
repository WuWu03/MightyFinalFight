using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TweenType = GameFrameWork.Utils.TweenUtil.TweenType;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ScrollList")]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollList : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        enum ListPositionType
        {
            First,
            Last
        }

        /// <summary>
        /// Which side of a cell to reference.
        /// </summary>
        public enum ItemPositionType
        {
            Before,
            After
        }

        /// <summary>
        /// This will set how the scroll bar should be shown based on the data. 
        /// </summary>
        public enum ScrollbarVisibilityType
        {
            Auto,
            Always,
            Never
        }

        public enum LoopJumpDirectionEnum
        {
            Closest,
            Up,
            Down
        }

        /// <summary>
        /// The number of pixels between items, starting after the first cell view
        /// </summary>
        [SerializeField] private float m_Spacing = 0;

        public float spacing
        {
            get { return m_Spacing; }
        }

        /// <summary>
        /// The maximum speed the scroller can go.
        /// </summary>
        public float maxVelocity;

        /// <summary>
        /// 获取数据长度
        /// </summary>
        public GameFrameWorkFunc<int> getDataCountEvent;

        /// <summary>
        /// 
        /// </summary>
        public GameFrameWorkFunc<int, float> getItemSizeEvent;

        /// <summary>
        /// This delegate is called when the scroll rect scrolls
        /// </summary>
        public GameFrameWorkAction<Vector2, float> scrolledEvent;
        
        public GameFrameWorkAction beginDragEvent;
        public GameFrameWorkAction endDragEvent;

        /// <summary>
        /// This delegate is called when the scroller has snapped to a position
        /// </summary>
        public GameFrameWorkAction<GameObject, int, int> snappedEvent;

        /// <summary>
        /// This delegate is called when the scroller has started or stopped scrolling
        /// </summary>
        public GameFrameWorkAction<bool> scrollingChangedEvent;

        /// <summary>
        /// This delegate is called when the scroller has started or stopped tweening
        /// </summary>
        public GameFrameWorkAction<bool> tweeningChangedEvent;

        /// <summary>
        /// The absolute position in pixels from the start of the scroller
        /// </summary>
        private float m_ScrollPosition;

        public float scrollPosition
        {
            get { return m_ScrollPosition; }
            set
            {
                value = Mathf.Clamp(value, 0, scrollSize);
                if (m_ScrollPosition != value)
                {
                    m_ScrollPosition = value;

                    if (m_ScrollRect.vertical)
                    {
                        m_ScrollRect.verticalNormalizedPosition = 1f - (m_ScrollPosition / scrollSize);
                    }
                    else
                    {
                        m_ScrollRect.horizontalNormalizedPosition = (m_ScrollPosition / scrollSize);
                    }
                }
            }
        }

        /// <summary>
        /// The size of the active item content minus the visibile portion of the scroller
        /// </summary>
        public float scrollSize
        {
            get
            {
                if (m_ScrollRect.vertical)
                {
                    return Mathf.Max(m_Content.rect.height - m_ScrollRectTransform.rect.height, 0);
                }

                else
                {
                    return Mathf.Max(m_Content.rect.width - m_ScrollRectTransform.rect.width, 0);
                }
            }
        }

        /// <summary>
        /// The normalized position of the scroller between 0 and 1
        /// </summary>
        public float normalizedScrollPosition
        {
            get
            {
                float scrollPosition = this.scrollPosition;
                return (scrollPosition <= 0 ? 0 : m_ScrollPosition / scrollSize);
            }
        }

        /// <summary>
        /// Sets how the visibility of the scrollbars should be handled
        /// </summary>
        [SerializeField] private ScrollbarVisibilityType m_ScrollbarVisibility;

        public ScrollbarVisibilityType scrollbarVisibility
        {
            get { return m_ScrollbarVisibility; }
            set
            {
                m_ScrollbarVisibility = value;

                if (m_Scrollbar is null)
                {
                    return;
                }

                if (m_ItemOffsetArray != null && m_ItemOffsetArray.Count > 0)
                {
                    if (m_ScrollRect.vertical)
                    {
                        scrollRect.verticalScrollbar = m_Scrollbar;
                    }
                    else
                    {
                        scrollRect.horizontalScrollbar = m_Scrollbar;
                    }

                    if (m_ItemOffsetArray[^1] < scrollRectSize)
                    {
                        m_Scrollbar.gameObject.SetActiveSelf(m_ScrollbarVisibility == ScrollbarVisibilityType.Always);
                    }
                    else
                    {
                        m_Scrollbar.gameObject.SetActiveSelf(m_ScrollbarVisibility != ScrollbarVisibilityType.Never);
                    }

                    if (!m_Scrollbar.gameObject.activeSelf)
                    {
                        scrollRect.verticalScrollbar = null;
                        scrollRect.horizontalScrollbar = null;
                    }
                }
            }
        }

        /// <summary>
        /// This is the velocity of the scroller.
        /// </summary>
        public Vector2 velocity
        {
            get { return m_ScrollRect.velocity; }
            set { m_ScrollRect.velocity = value; }
        }

        /// <summary>
        /// The linear velocity is the velocity on one axis.
        /// </summary>
        public float linearVelocity
        {
            get { return (m_ScrollRect.vertical ? m_ScrollRect.velocity.y : m_ScrollRect.velocity.x); }
            set
            {
                if (m_ScrollRect.vertical)
                {
                    m_ScrollRect.velocity = new Vector2(0, value);
                }
                else
                {
                    m_ScrollRect.velocity = new Vector2(value, 0);
                }
            }
        }

        /// <summary>
        /// Whether the scroller is scrolling or not
        /// </summary>
        public bool isScrolling { get; private set; }

        /// <summary>
        /// Whether the scroller is tweening or not
        /// </summary>
        public bool isTweening { get; private set; }

        /// <summary>
        /// This is the first cell view index showing in the scroller's visible area
        /// </summary>
        private int m_ActiveItemsStartIndex;

        public int startItemIndex
        {
            get { return m_ActiveItemsStartIndex; }
        }

        /// <summary>
        /// This is the last cell view index showing in the scroller's visible area
        /// </summary>
        private int m_ActiveItemsEndIndex;

        public int endItemIndex
        {
            get { return m_ActiveItemsEndIndex; }
        }

        /// <summary>
        /// This is the first data index showing in the scroller's visible area
        /// </summary>
        public int startDataIndex
        {
            get { return m_ActiveItemsStartIndex % itemCount; }
        }

        /// <summary>
        /// This is the last data index showing in the scroller's visible area
        /// </summary>
        public int endDataIndex
        {
            get { return m_ActiveItemsEndIndex % itemCount; }
        }

        /// <summary>
        /// This is the number of cells in the scroller
        /// </summary>
        public int itemCount
        {
            get { return getDataCountEvent?.Invoke() ?? 0; }
        }

        /// <summary>
        /// The amount of space to look ahead before the scroller position.
        /// </summary>
        private float m_LookAheadBefore;

        public float lookAheadBefore
        {
            get { return m_LookAheadBefore; }
            set { m_LookAheadBefore = Mathf.Abs(value); }
        }

        /// <summary>
        /// The amount of space to look ahead after the last visible cell.
        /// </summary>
        private float m_LookAheadAfter;

        public float lookAheadAfter
        {
            get { return m_LookAheadAfter; }
            set { m_LookAheadAfter = Mathf.Abs(value); }
        }

        /// <summary>
        /// This is a convenience link to the scroller's scroll rect
        /// </summary>
        private ScrollRect m_ScrollRect;

        public ScrollRect scrollRect
        {
            get { return m_ScrollRect; }
        }

        /// <summary>
        /// The size of the visible portion of the scroller
        /// </summary>
        public float scrollRectSize
        {
            get
            {
                if (m_ScrollRect.vertical)
                {
                    return m_ScrollRectTransform.rect.height;
                }

                else
                {
                    return m_ScrollRectTransform.rect.width;
                }
            }
        }

        /// <summary>
        /// Access to the scroll rect container
        /// </summary>
        private RectTransform m_Content;

        public RectTransform content
        {
            get { return m_Content; }
        }

        /// <summary>
        /// 边框
        /// </summary>
        [SerializeField] private RectOffset m_Padding;

        public RectOffset padding
        {
            get { return m_Padding; }
        }

        [SerializeField] private TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;

        public TextAnchor childAlignment
        {
            get { return m_ChildAlignment; }
        }

        /// <summary>
        /// 列表预制体
        /// </summary>
        [SerializeField] private GameObject m_Prefab;

        public GameObject prefab
        {
            get { return m_Prefab; }
        }

        private Scrollbar m_Scrollbar;
        private RectTransform m_ScrollRectTransform;
        private RectTransform m_RecycledItemsContent;
        private List<ScrollListItem> m_ActiveItems = null;
        private List<ScrollListItem> m_RecycledItems = null;
        private List<float> m_ItemSizeArray = null;
        private List<float> m_ItemOffsetArray = null;
        private bool m_IsInitialized = false;
        private float m_TweenTimer;
        private int m_DragFingerCount;
        private Type m_ItemClassType = null;
        private ScrollbarVisibilityType m_LastScrollbarVisibility;

        private void Awake()
        {
            m_ScrollRect ??= GetComponent<ScrollRect>();
        }

        public void Init<T>() where T : ScrollListItem, new()
        {
            if (m_ScrollRect is null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }

            if (m_ScrollRect is null)
            {
                Log.LogError(name, "[Scroll Rect] 组件不存在");
                return;
            }

            if (m_ScrollRect.content is null)
            {
                Log.LogError(name, "[Scroll Rect] 组件没有content");
                return;
            }

            if (m_ScrollRect.viewport is null)
            {
                Log.LogError(name, "[Scroll Rect] 组件没有viewport");
                return;
            }

            m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();
            m_Content = m_ScrollRect.content;

            if (m_ScrollRect.vertical)
            {
                float verticalChildAlignment = GetVerticalChildAlignment();
                m_Content.anchorMin = new Vector2(0, verticalChildAlignment);
                m_Content.anchorMax = new Vector2(1, verticalChildAlignment);
                m_Content.pivot = new Vector2(0.5f, verticalChildAlignment);
            }
            else
            {
                float horizontalChildAlignment = GetHorizontalChildAlignment();
                m_Content.anchorMin = new Vector2(0, horizontalChildAlignment);
                m_Content.anchorMax = new Vector2(1, horizontalChildAlignment);
                m_Content.pivot = new Vector2(0.5f, horizontalChildAlignment);
            }

            m_Content.offsetMax = Vector2.zero;
            m_Content.offsetMin = Vector2.zero;
            m_Content.anchoredPosition = Vector2.zero;
            m_Content.localRotation = Quaternion.identity;
            m_Content.localScale = Vector3.one;

            if (m_ScrollRect.vertical)
            {
                m_Scrollbar = m_ScrollRect.verticalScrollbar;
            }
            else
            {
                m_Scrollbar = m_ScrollRect.horizontalScrollbar;
            }

            m_RecycledItemsContent = new GameObject("Recycled Cells", typeof(RectTransform)).GetComponent<RectTransform>();
            m_RecycledItemsContent.transform.SetParent(m_ScrollRect.transform, false);
            m_RecycledItemsContent.gameObject.SetActiveSelf(false);
            m_LastScrollbarVisibility = m_ScrollbarVisibility;
            m_ItemClassType = typeof(T);
            m_ItemSizeArray = new();
            m_ItemOffsetArray = new();
            m_ActiveItems = new();
            m_RecycledItems = new();
            m_Prefab.transform.SetParent(m_ScrollRect.viewport, false);
            m_Prefab.SetActiveSelf(false);
            m_IsInitialized = true;
        }

        private bool m_HasSetPosition = false;

        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="keepPosition">是否保持位置不变</param>
        public void RefreshItems(bool keepPosition = false)
        {
            RecycleAllItems();
            Resize();

            if (m_ScrollRect is null || m_ScrollRectTransform is null || m_Content is null)
            {
                m_ScrollPosition = 0f;
                return;
            }

            if (!keepPosition || !m_HasSetPosition)
            {
                float scrollPositionFactor;
                if (m_ScrollRect.vertical)
                {
                    scrollPositionFactor = GetVerticalChildAlignment();
                    m_ScrollRect.verticalNormalizedPosition = scrollPositionFactor;
                    m_ScrollPosition = Mathf.Clamp((1 - scrollPositionFactor) * scrollSize, 0, scrollSize);
                }
                else
                {
                    scrollPositionFactor = GetHorizontalChildAlignment();
                    m_ScrollRect.horizontalNormalizedPosition = scrollPositionFactor;
                    m_ScrollPosition = Mathf.Clamp(scrollPositionFactor * scrollSize, 0, scrollSize);
                }

                m_HasSetPosition = true;
            }

            RefreshActive();
        }

        private float GetVerticalChildAlignment()
        {
            if (m_ChildAlignment == TextAnchor.UpperLeft ||
                m_ChildAlignment == TextAnchor.UpperCenter ||
                m_ChildAlignment == TextAnchor.UpperRight)
            {
                return 1f;
            }

            if (m_ChildAlignment == TextAnchor.MiddleLeft ||
                m_ChildAlignment == TextAnchor.MiddleCenter ||
                m_ChildAlignment == TextAnchor.MiddleRight)
            {
                return 0.5f;
            }
            
            return 0f;
        }

        private float GetHorizontalChildAlignment()
        {
            if (m_ChildAlignment == TextAnchor.UpperLeft ||
                m_ChildAlignment == TextAnchor.MiddleLeft ||
                m_ChildAlignment == TextAnchor.LowerLeft)
            {
                return 0f;
            }

            if (m_ChildAlignment == TextAnchor.UpperCenter ||
                m_ChildAlignment == TextAnchor.MiddleCenter ||
                m_ChildAlignment == TextAnchor.LowerCenter)
            {
                return 0.5f;
            }

            if (m_ChildAlignment == TextAnchor.UpperRight ||
                m_ChildAlignment == TextAnchor.MiddleRight ||
                m_ChildAlignment == TextAnchor.LowerRight)
            {
                return 1f;
            }

            return 0f;
        }

        /// <summary>
        /// This calls the RefreshCellView method on each active cell.
        /// </summary>
        public void RefreshActiveCellViews()
        {
            foreach (var item in m_ActiveItems)
            {
                item.OnUpdate();
            }
        }

        /// <summary>
        /// Removes all cells, both active and recycled from the scroller.
        /// </summary>
        public void ClearAll()
        {
            ClearActive();
            ClearRecycled();
        }

        /// <summary>
        /// Removes all the active items.
        /// </summary>
        public void ClearActive()
        {
            for (var i = 0; i < m_ActiveItems.Count; i++)
            {
                DestroyImmediate(m_ActiveItems[i].gameObject);
            }

            m_ActiveItems.Clear();
        }

        /// <summary>
        /// Removes all the recycled items.
        /// </summary>
        public void ClearRecycled()
        {
            for (var i = 0; i < m_RecycledItems.Count; i++)
            {
                DestroyImmediate(m_RecycledItems[i].gameObject);
            }

            m_RecycledItems.Clear();
        }

        /// <summary>
        /// Sets the scroll position and refresh the active cells.
        /// </summary>
        public void SetScrollPositionImmediately(float scrollPosition)
        {
            this.scrollPosition = scrollPosition;
            RefreshActive();
        }

        /// <summary>
        /// Jump to a position in the scroller based on a dataIndex.
        /// </summary>
        public void JumpToDataIndex(int dataIndex,
            bool useSpacing = true,
            TweenType tweenType = TweenType.None,
            float tweenTime = 0f,
            Action jumpComplete = null,
            LoopJumpDirectionEnum loopJumpDirection = LoopJumpDirectionEnum.Closest,
            bool forceCalculateRange = false)
        {
            float newScrollPosition = GetScrollPositionByDataIndex(dataIndex, ItemPositionType.Before);
            newScrollPosition = Mathf.Clamp(newScrollPosition - (useSpacing ? spacing : 0), 0, scrollSize);

            if (newScrollPosition == m_ScrollPosition)
            {
                jumpComplete?.Invoke();
                return;
            }

            StartCoroutine(TweenPosition(tweenType, tweenTime, scrollPosition, newScrollPosition, jumpComplete, forceCalculateRange));
        }

        /// <summary>
        /// Snaps the scroller on command. 
        /// </summary>
        public void Snap(float snapWatchOffset, bool useSpacing = true, TweenType tweenType = TweenType.None,
            float tweenTime = 0f)
        {
            if (itemCount == 0)
            {
                return;
            }

            linearVelocity = 0;

            bool inertia = m_ScrollRect.inertia;
            float snapPosition = scrollSize * Mathf.Clamp01(snapWatchOffset);
            int sapItemIndex = GetItemIndexAtPosition(snapPosition);
            int snapDataIndex = sapItemIndex % itemCount;

            m_ScrollRect.inertia = false;

            JumpToDataIndex(snapDataIndex, useSpacing, tweenType, tweenTime, () =>
            {
                m_ScrollRect.inertia = inertia;

                if (snappedEvent != null)
                {
                    ScrollListItem cellView = null;

                    for (var i = 0; i < m_ActiveItems.Count; i++)
                    {
                        if (m_ActiveItems[i].dataIndex == snapDataIndex)
                        {
                            cellView = m_ActiveItems[i];
                            break;
                        }
                    }

                    snappedEvent.Invoke(cellView.gameObject, cellView.dataIndex, cellView.itemIndex);
                }
            });
        }

        /// <summary>
        /// Gets the scroll position in pixels from the start of the scroller based on the itemIndex.
        /// </summary>
        public float GetScrollPositionByItemIndex(int itemIndex, ItemPositionType insertPosition)
        {
            if (itemCount == 0)
            {
                return 0;
            }

            if (itemIndex < 0) itemIndex = 0;

            if (itemIndex == 0 && insertPosition == ItemPositionType.Before)
            {
                return 0;
            }

            if (itemIndex < m_ItemOffsetArray.Count)
            {
                if (insertPosition == ItemPositionType.Before)
                {
                    return m_ItemOffsetArray[itemIndex - 1] + spacing + (m_ScrollRect.vertical ? m_Padding.top : m_Padding.left);
                }

                return m_ItemOffsetArray[itemIndex] + (m_ScrollRect.vertical ? m_Padding.top : m_Padding.left);
            }

            return m_ItemOffsetArray[^2];
        }

        /// <summary>
        /// Gets the scroll position in pixels from the start of the scroller based on the dataIndex
        /// </summary>
        public float GetScrollPositionByDataIndex(int dataIndex, ItemPositionType insertPosition)
        {
            return GetScrollPositionByItemIndex(dataIndex, insertPosition);
        }

        /// <summary>
        /// Gets the index of a cell view at a given position
        /// </summary>
        public int GetItemIndexAtPosition(float position)
        {
            return GetItemIndexAtPosition(position, 0, m_ItemOffsetArray.Count - 1);
        }

        /// <summary>
        /// Get a cell view for a particular data index.
        /// </summary>
        public ScrollListItem GetItemByDataIndex(int dataIndex)
        {
            foreach (var activeItem in m_ActiveItems)
            {
                if (activeItem.dataIndex == dataIndex)
                {
                    return activeItem;
                }
            }

            return null;
        }

        /// <summary>
        /// This event is fired when the user begins dragging on the scroller.
        /// </summary>
        public void OnBeginDrag(PointerEventData data)
        {
            m_DragFingerCount++;
            beginDragEvent?.Invoke();
        }

        /// <summary>
        /// This event is fired when the user ends dragging on the scroller.
        /// </summary>
        public void OnEndDrag(PointerEventData data)
        {
            m_DragFingerCount--;

            if (m_DragFingerCount < 0)
            {
                m_DragFingerCount = 0;
            }
            
            endDragEvent?.Invoke();
        }

        /// <summary>
        /// Create a cell view, or recycle one if it already exists
        /// </summary>
        private ScrollListItem GetItem()
        {
            var item = GetRecycledItem();

            if (item == null)
            {
                var go = Instantiate(m_Prefab);
                item = Activator.CreateInstance(m_ItemClassType) as ScrollListItem;
                item.Create(go);
            }

            return item;
        }

        /// <summary>
        /// This function will create an internal list of sizes and offsets to be used in all calculations.
        /// </summary>
        private void Resize()
        {
            m_ItemSizeArray.Clear();
            AddItemSizes();
            CalculateItemOffsets();

            if (m_ScrollRect.vertical)
            {
                m_Content.sizeDelta = new Vector2(m_Content.sizeDelta.x, m_ItemOffsetArray[^1] + m_Padding.top + m_Padding.bottom);
            }
            else
            {
                m_Content.sizeDelta = new Vector2(m_ItemOffsetArray[^1] + m_Padding.left + m_Padding.right, m_Content.sizeDelta.y);
            }

            ResetVisibleItems();
            scrollbarVisibility = m_ScrollbarVisibility;
        }

        private void AddItemSizes()
        {
            for (var i = 0; i < itemCount; i++)
            {
                m_ItemSizeArray.Add(getItemSizeEvent.Invoke(i) + (i == 0 ? 0 : spacing));
            }
        }

        /// <summary>
        /// Calculates the offset of each cell, accumulating the values from previous cells
        /// </summary>
        private void CalculateItemOffsets()
        {
            m_ItemOffsetArray.Clear();
            float offset = 0f;

            for (var i = 0; i < m_ItemSizeArray.Count; i++)
            {
                offset += m_ItemSizeArray[i];
                m_ItemOffsetArray.Add(offset);
            }
        }

        /// <summary>
        /// Get a recycled cell with a given identifier if available
        /// </summary>
        private ScrollListItem GetRecycledItem()
        {
            if (m_RecycledItems != null && m_RecycledItems.Count > 0)
            {
                var cellView = m_RecycledItems[0];
                m_RecycledItems.RemoveAt(0);
                return cellView;
            }

            return null;
        }

        /// <summary>
        /// This sets up the visible cells, adding and recycling as necessary
        /// </summary>
        private void ResetVisibleItems()
        {
            CalculateCurrentActiveItemRange(out int startIndex, out int endIndex);
            int i = 0;
            List<int> remainingCellIndices = new();

            while (i < m_ActiveItems.Count)
            {
                if (m_ActiveItems[i].itemIndex < startIndex || m_ActiveItems[i].itemIndex > endIndex)
                {
                    RecycleItem(m_ActiveItems[i]);
                }
                else
                {
                    remainingCellIndices.Add(m_ActiveItems[i].itemIndex);
                    i++;
                }
            }

            if (remainingCellIndices.Count == 0)
            {
                for (i = startIndex; i <= endIndex; i++)
                {
                    AddItem(i, ListPositionType.Last);
                }
            }
            else
            {
                for (i = endIndex; i >= startIndex; i--)
                {
                    if (i < remainingCellIndices[0])
                    {
                        AddItem(i, ListPositionType.First);
                    }
                }

                for (i = startIndex; i <= endIndex; i++)
                {
                    if (i > remainingCellIndices[^1])
                    {
                        AddItem(i, ListPositionType.Last);
                    }
                }
            }

            m_ActiveItemsStartIndex = startIndex;
            m_ActiveItemsEndIndex = endIndex;
        }

        /// <summary>
        /// Recycles all the active cells
        /// </summary>
        private void RecycleAllItems()
        {
            while (m_ActiveItems.Count > 0)
            {
                RecycleItem(m_ActiveItems[0]);
            }

            m_ActiveItemsStartIndex = 0;
            m_ActiveItemsEndIndex = 0;
        }

        /// <summary>
        /// Recycles one cell view
        /// </summary>
        private void RecycleItem(ScrollListItem item)
        {
            m_ActiveItems.Remove(item);
            m_RecycledItems.Add(item);

            item.SetActiveSelf(false);
            item.itemIndex = 0;
            item.dataIndex = 0;
        }

        /// <summary>
        /// Creates a cell view, or recycles if it can
        /// </summary>
        private void AddItem(int itemIndex, ListPositionType listPosition)
        {
            if (itemCount == 0)
            {
                return;
            }

            int realItemIndex = itemIndex;//Mathf.Abs(itemCount - 1 - itemIndex);
            var dataIndex = realItemIndex % itemCount;
            var item = GetItem();

            item.itemIndex = realItemIndex;
            item.dataIndex = dataIndex;
            item.transform.localScale = Vector3.one;
            item.transform.SetParent(m_Content, false);
            item.SetActiveSelf(true);
            
            if (m_ScrollRect.vertical)
            {
                float size = m_ItemSizeArray[itemIndex];
                float height = m_ItemSizeArray[0] / 2 + size * dataIndex;
                float posY = m_Content.rect.height / 2 - height;
                item.rectTransform.anchoredPosition = new Vector2(0, posY);
            }
            
            if (listPosition == ListPositionType.First)
            {
                m_ActiveItems.Insert(0, item);
            }
            else
            {
                m_ActiveItems.Add(item);
            }
            
            item.OnUpdate();
        }

        /// <summary>
        /// This function is called if the scroller is scrolled, updating the active list of cells
        /// </summary>
        private void RefreshActive()
        {
            CalculateCurrentActiveItemRange(out int startIndex, out int endIndex);

            if (startIndex == m_ActiveItemsStartIndex && endIndex == m_ActiveItemsEndIndex)
            {
                return;
            }

            ResetVisibleItems();
        }

        /// <summary>
        /// Determines which cells can be seen
        /// </summary>
        private void CalculateCurrentActiveItemRange(out int startIndex, out int endIndex)
        {
            float startPosition = m_ScrollPosition - lookAheadBefore;
            float endPosition = m_ScrollPosition +
                                (m_ScrollRect.vertical
                                    ? m_ScrollRectTransform.rect.height
                                    : m_ScrollRectTransform.rect.width) + lookAheadAfter;
            startIndex = GetItemIndexAtPosition(startPosition);
            endIndex = GetItemIndexAtPosition(endPosition);
        }

        /// <summary>
        /// Gets the index of a cell at a given position based on a subset range.
        /// </summary>
        private int GetItemIndexAtPosition(float position, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return startIndex;
            }

            var middleIndex = (startIndex + endIndex) / 2;
            float pad = m_ScrollRect.vertical ? m_Padding.top : m_Padding.left;

            if ((m_ItemOffsetArray[middleIndex] + pad) >= (position + (pad == 0 ? 0 : 1.00001f)))
            {
                return GetItemIndexAtPosition(position, startIndex, middleIndex);
            }

            return GetItemIndexAtPosition(position, middleIndex + 1, endIndex);
        }

        private void Update()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            if (m_LastScrollbarVisibility != m_ScrollbarVisibility)
            {
                scrollbarVisibility = m_ScrollbarVisibility;
                m_LastScrollbarVisibility = m_ScrollbarVisibility;
            }

            if (linearVelocity != 0 && !isScrolling)
            {
                isScrolling = true;
                scrollingChangedEvent?.Invoke(true);
            }
            else if (linearVelocity == 0 && isScrolling)
            {
                isScrolling = false;
                scrollingChangedEvent?.Invoke(false);
            }
        }

        /// <summary>
        /// Fired at the end of the frame.
        /// </summary>
        private void LateUpdate()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            if (maxVelocity > 0)
            {
                if (m_ScrollRect.horizontal)
                {
                    velocity = new Vector2(Mathf.Clamp(Mathf.Abs(velocity.x), 0, maxVelocity) * Mathf.Sign(velocity.x),
                        velocity.y);
                }
                else
                {
                    velocity = new Vector2(velocity.x,
                        Mathf.Clamp(Mathf.Abs(velocity.y), 0, maxVelocity) * Mathf.Sign(velocity.y));
                }
            }
        }

        private void OnEnable()
        {
            if (m_ScrollRect is null)
            {
                Log.LogError(name, "[Scroll Rect] 组件不存在");
                return;
            }

            m_ScrollRect.onValueChanged.AddListener(ScrollRectOnValueChanged);
        }

        private void OnDisable()
        {
            if (m_ScrollRect is null)
            {
                Log.LogError(name, "[Scroll Rect] 组件不存在");
                return;
            }

            m_ScrollRect.onValueChanged.RemoveListener(ScrollRectOnValueChanged);
        }

        /// <summary>
        /// Handler for when the scroller changes value
        /// </summary>
        private void ScrollRectOnValueChanged(Vector2 position)
        {
            if (m_ScrollRect.vertical)
            {
                m_ScrollPosition = (1f - position.y) * scrollSize;
            }
            else
            {
                m_ScrollPosition = position.x * scrollSize;
            }

            m_ScrollPosition = Mathf.Clamp(m_ScrollPosition, 0, scrollSize);
            scrolledEvent?.Invoke(position, m_ScrollPosition);
            RefreshActive();
        }


        /// <summary>
        /// Moves the scroll position over time between two points given an easing function.
        /// </summary>
        IEnumerator TweenPosition(TweenType tweenType, float time, float start, float end, Action tweenComplete,
            bool forceCalculateRange)
        {
            if (!(tweenType == TweenType.None || time == 0))
            {
                m_ScrollRect.velocity = Vector2.zero;
                m_TweenTimer = 0;

                isTweening = true;
                tweeningChangedEvent?.Invoke(true);

                while (m_TweenTimer < time)
                {
                    scrollPosition = TweenUtil.Tween(tweenType, start, end, m_TweenTimer / time);
                    m_TweenTimer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            scrollPosition = end;

            if (forceCalculateRange)
            {
                RefreshActive();
            }

            tweenComplete?.Invoke();
            isTweening = false;
            tweeningChangedEvent?.Invoke(false);
        }
    }
}