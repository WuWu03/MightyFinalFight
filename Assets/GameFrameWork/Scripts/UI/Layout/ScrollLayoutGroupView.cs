using GameFrameWork.Utilities;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TweenType = GameFrameWork.Utilities.TweenUtil.TweenType;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ScrollLayoutGroupView")]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollLayoutGroupView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
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
        public float spacing
        {
            get
            {
                return m_LayoutGroup.spacing;
            }
        }

        /// <summary>
        /// The maximum speed the scroller can go.
        /// </summary>
        public float maxVelocity;

        /// <summary>
        /// 
        /// </summary>
        public GameFrameWorkIntAction getDataCountEvent;

        /// <summary>
        /// 
        /// </summary>
        public GameFrameWorkAction<ScrollLayoutGroupViewItem> itemUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public GameFrameWorkFloatAction<int> getItemSizeEvent;

        /// <summary>
        /// This delegate is called when the scroll rect scrolls
        /// </summary>
        public GameFrameWorkAction<Vector2, float> scrolledEvent;

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
        public float scrollPosition
        {
            get
            {
                return m_ScrollPosition;
            }
            set
            {
                if (!m_Loop)
                {
                    value = Mathf.Clamp(value, 0, scrollSize);
                }

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
                var scrollPosition = this.scrollPosition;
                return (scrollPosition <= 0 ? 0 : m_ScrollPosition / scrollSize);
            }
        }

        /// <summary>
        /// Whether the scroller should loop the resulting items.
        /// </summary>
        public bool loop
        {
            get
            {
                return m_Loop;
            }
            set
            {
                if (m_Loop != value)
                {
                    var originalScrollPosition = m_ScrollPosition;

                    m_Loop = value;
                    Resize(false);

                    if (m_Loop)
                    {
                        scrollPosition = m_LoopFirstScrollPosition + originalScrollPosition;
                    }
                    else
                    {
                        scrollPosition = originalScrollPosition - m_LoopFirstScrollPosition;
                    }

                    scrollbarVisibility = m_ScrollbarVisibility;
                }
            }
        }

        /// <summary>
        /// Sets how the visibility of the scrollbars should be handled
        /// </summary>
        public ScrollbarVisibilityType scrollbarVisibility
        {
            get
            {
                return m_ScrollbarVisibility;
            }
            set
            {
                m_ScrollbarVisibility = value;


                if (m_Scrollbar != null)
                {
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

                        if (m_ItemOffsetArray.Last() < scrollRectSize || m_Loop)
                        {
                            m_Scrollbar.gameObject.SetActive(m_ScrollbarVisibility == ScrollbarVisibilityType.Always);
                        }
                        else
                        {
                            m_Scrollbar.gameObject.SetActive(m_ScrollbarVisibility != ScrollbarVisibilityType.Never);
                        }

                        if (!m_Scrollbar.gameObject.activeSelf)
                        {
                            scrollRect.verticalScrollbar = null;
                            scrollRect.horizontalScrollbar = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is the velocity of the scroller.
        /// </summary>
        public Vector2 velocity
        {
            get
            {
                return m_ScrollRect.velocity;
            }
            set
            {
                m_ScrollRect.velocity = value;
            }
        }

        /// <summary>
        /// The linear velocity is the velocity on one axis.
        /// </summary>
        public float linearVelocity
        {
            get
            {
                return (m_ScrollRect.vertical ? m_ScrollRect.velocity.y : m_ScrollRect.velocity.x);
            }
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
        public bool isScrolling
        {
            get; private set;
        }

        /// <summary>
        /// Whether the scroller is tweening or not
        /// </summary>
        public bool isTweening
        {
            get; private set;
        }

        /// <summary>
        /// This is the first cell view index showing in the scroller's visible area
        /// </summary>
        public int startItemIndex
        {
            get
            {
                return m_ActiveItemsStartIndex;
            }
        }

        /// <summary>
        /// This is the last cell view index showing in the scroller's visible area
        /// </summary>
        public int endItemIndex
        {
            get
            {
                return m_ActiveItemsEndIndex;
            }
        }

        /// <summary>
        /// This is the first data index showing in the scroller's visible area
        /// </summary>
        public int startDataIndex
        {
            get
            {
                return m_ActiveItemsStartIndex % itemCount;
            }
        }

        /// <summary>
        /// This is the last data index showing in the scroller's visible area
        /// </summary>
        public int endDataIndex
        {
            get
            {
                return m_ActiveItemsEndIndex % itemCount;
            }
        }

        /// <summary>
        /// This is the number of cells in the scroller
        /// </summary>
        public int itemCount
        {
            get
            {
                return (getDataCountEvent != null ? getDataCountEvent.Invoke() : 0);
            }
        }

        /// <summary>
        /// The amount of space to look ahead before the scroller position.
        /// </summary>

        public float lookAheadBefore
        {
            get
            {
                return m_LookAheadBefore;
            }
            set
            {
                m_LookAheadBefore = Mathf.Abs(value);
            }
        }

        /// <summary>
        /// The amount of space to look ahead after the last visible cell.
        /// </summary>     
        public float lookAheadAfter
        {
            get
            {
                return m_LookAheadAfter;
            }
            set
            {
                m_LookAheadAfter = Mathf.Abs(value);
            }
        }
        /// <summary>
        /// This is a convenience link to the scroller's scroll rect
        /// </summary>
        public ScrollRect scrollRect
        {
            get
            {
                return m_ScrollRect;
            }
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
        /// The first padder before the visible cells
        /// </summary>
        public LayoutElement firstPadder
        {
            get
            {
                return m_FirstPadder;
            }
        }

        /// <summary>
        /// The last padder after the visible cells
        /// </summary>
        public LayoutElement lastPadder
        {
            get
            {
                return m_LastPadder;
            }
        }

        /// <summary>
        /// Access to the scroll rect container
        /// </summary>
        public RectTransform content
        {
            get
            {
                return m_Content;
            }
        }

        private void Awake()
        {
            if(m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }
        }

        public void Init<T>(GameObject prefab) where T : ScrollLayoutGroupViewItem, new()
        {
            if (m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }

            if (m_ScrollRect == null)
            {
                Log.LogError(name, "scroll rect is invalid!");
                return;
            }

            if (m_ScrollRect.content == null)
            {
                Log.LogError(name, "scroll rect don't have a content,add it first!");
                return;
            }

            if (m_ScrollRect.viewport == null)
            {
                Log.LogError(name, "scroll rect don't have a viewport,add it first!");
                return;
            }

            m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();
            m_Content = m_ScrollRect.content;

            if (m_Content.GetComponent<ContentSizeFitter>() != null)
            {
                m_Content.GetComponent<ContentSizeFitter>().enabled = false;
            }

            if (m_ScrollRect.vertical)
            {
                m_Content.anchorMin = new Vector2(0, 1);
                m_Content.anchorMax = Vector2.one;
                m_Content.pivot = new Vector2(0.5f, 1f);
            }
            else
            {
                m_Content.anchorMin = Vector2.zero;
                m_Content.anchorMax = new Vector2(0, 1f);
                m_Content.pivot = new Vector2(0, 0.5f);
            }

            m_Content.offsetMax = Vector2.zero;
            m_Content.offsetMin = Vector2.zero;
            m_Content.anchoredPosition = Vector3.zero;
            m_Content.localRotation = Quaternion.identity;
            m_Content.localScale = Vector3.one;

            m_ScrollRect.content = m_Content;
            m_Content.SetParent(m_ScrollRect.viewport.transform, false);

            if (m_ScrollRect.vertical)
            {
                m_Scrollbar = m_ScrollRect.verticalScrollbar;
            }
            else
            {
                m_Scrollbar = m_ScrollRect.horizontalScrollbar;
            }

            m_LayoutGroup = m_Content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            m_LayoutGroup.childAlignment = TextAnchor.UpperLeft;
            m_LayoutGroup.childForceExpandHeight = true;
            m_LayoutGroup.childForceExpandWidth = true;
            m_LayoutGroup.childControlWidth = true;
            m_LayoutGroup.childControlHeight = true;

            GameObject go = new GameObject("First Padder", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(m_Content, false);
            m_FirstPadder = go.GetComponent<LayoutElement>();

            go = new GameObject("Last Padder", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(m_Content, false);
            m_LastPadder = go.GetComponent<LayoutElement>();

            go = new GameObject("Recycled Cells", typeof(RectTransform));
            go.transform.SetParent(m_ScrollRect.transform, false);
            m_RecycledItemsContent = go.GetComponent<RectTransform>();
            m_RecycledItemsContent.gameObject.SetActive(false);

            m_LastScrollRectSize = scrollRectSize;
            m_LastLoop = m_Loop;
            m_LastScrollbarVisibility = m_ScrollbarVisibility;
            m_ItemClassType = typeof(T);
            m_Prefab = prefab;

            m_ItemSizeArray = new SmallList<float>();
            m_ItemOffsetArray = new SmallList<float>();
            m_ActiveItems = new SmallList<ScrollLayoutGroupViewItem>();
            m_RecycledItems = new SmallList<ScrollLayoutGroupViewItem>();

            prefab.transform.SetParent(m_ScrollRect.viewport, false);
            prefab.SetActive(false);

            m_IsInitialized = true;
        }

        /// <summary>
        /// This resets the internal size list and refreshes the items
        /// </summary>
        public void RefreshData(float scrollPositionFactor = 0)
        {
            m_IsRefreshData = false;

            RecycleAllItems();
            Resize(false);

            if (m_ScrollRect == null || m_ScrollRectTransform == null || m_Content == null)
            {
                m_ScrollPosition = 0f;
                return;
            }

            m_ScrollPosition = Mathf.Clamp(scrollPositionFactor * scrollSize, 0, scrollSize);

            if (m_ScrollRect.vertical)
            {
                m_ScrollRect.verticalNormalizedPosition = 1f - scrollPositionFactor;
            }
            else
            {
                m_ScrollRect.horizontalNormalizedPosition = scrollPositionFactor;
            }

            RefreshActive();
        }

        /// <summary>
        /// This calls the RefreshCellView method on each active cell.
        /// </summary>
        public void RefreshActiveCellViews()
        {
            for (var i = 0; i < m_ActiveItems.Count; i++)
            {
                itemUpdateEvent?.Invoke(m_ActiveItems[i]);
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
        /// Turn looping on or off.
        /// </summary>
        public void ToggleLoop()
        {
            loop = !m_Loop;
        }

        /// <summary>
        /// Toggle whether the loop jump calculation is used.
        /// </summary>
        public void IgnoreLoopJump(bool ignore)
        {
            m_IgnoreLoopJump = ignore;
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
    
            var newScrollPosition = 0f;

            if (m_Loop)
            {
                var numberOfCells = itemCount;
                var set1CellViewIndex = m_LoopFirstCellIndex - (numberOfCells - dataIndex);
                var set2CellViewIndex = m_LoopFirstCellIndex + dataIndex;
                var set3CellViewIndex = m_LoopFirstCellIndex + numberOfCells + dataIndex;

                var set1Position = GetScrollPositionByItemIndex(set1CellViewIndex, ItemPositionType.Before);
                var set2Position = GetScrollPositionByItemIndex(set2CellViewIndex, ItemPositionType.Before);
                var set3Position = GetScrollPositionByItemIndex(set3CellViewIndex, ItemPositionType.Before);

                var set1Diff = (Mathf.Abs(m_ScrollPosition - set1Position));
                var set2Diff = (Mathf.Abs(m_ScrollPosition - set2Position));
                var set3Diff = (Mathf.Abs(m_ScrollPosition - set3Position));

                var currentSet = 0;
                var currentCellViewIndex = 0;
                var nextCellViewIndex = 0;

                if (loopJumpDirection == LoopJumpDirectionEnum.Up || loopJumpDirection == LoopJumpDirectionEnum.Down)
                {
                    currentCellViewIndex = GetItemIndexAtPosition(m_ScrollPosition + 0.0001f);

                    if (currentCellViewIndex < numberOfCells)
                    {
                        currentSet = 1;
                        nextCellViewIndex = dataIndex;
                    }
                    else if (currentCellViewIndex >= numberOfCells && currentCellViewIndex < (numberOfCells * 2))
                    {
                        currentSet = 2;
                        nextCellViewIndex = dataIndex + numberOfCells;
                    }
                    else
                    {
                        currentSet = 3;
                        nextCellViewIndex = dataIndex + (numberOfCells * 2);
                    }
                }

                switch (loopJumpDirection)
                {
                    case LoopJumpDirectionEnum.Closest:

                        if (set1Diff < set2Diff)
                        {
                            if (set1Diff < set3Diff)
                            {
                                newScrollPosition = set1Position;
                            }
                            else
                            {
                                newScrollPosition = set3Position;
                            }
                        }
                        else
                        {
                            if (set2Diff < set3Diff)
                            {
                                newScrollPosition = set2Position;
                            }
                            else
                            {
                                newScrollPosition = set3Position;
                            }
                        }

                        break;

                    case LoopJumpDirectionEnum.Up:

                        if (nextCellViewIndex < currentCellViewIndex)
                        {
                            newScrollPosition = (currentSet == 1 ? set1Position : (currentSet == 2 ? set2Position : set3Position));
                        }
                        else
                        {
                            if (currentSet == 1 && (currentCellViewIndex == dataIndex))
                            {
                                newScrollPosition = set1Position - m_SingleLoopGroupSize;
                            }
                            else
                            {
                                newScrollPosition = (currentSet == 1 ? set3Position : (currentSet == 2 ? set1Position : set2Position));
                            }
                        }

                        break;

                    case LoopJumpDirectionEnum.Down:

                        if (nextCellViewIndex > currentCellViewIndex)
                        {
                            newScrollPosition = (currentSet == 1 ? set1Position : (currentSet == 2 ? set2Position : set3Position));
                        }
                        else
                        {
                            if (currentSet == 3 && (currentCellViewIndex == nextCellViewIndex))
                            {
                                newScrollPosition = set3Position + m_SingleLoopGroupSize;
                            }
                            else
                            {
                                newScrollPosition = (currentSet == 1 ? set2Position : (currentSet == 2 ? set3Position : set1Position));
                            }
                        }

                        break;

                }

                if (useSpacing)
                {
                    newScrollPosition -= spacing;
                }
            }
            else
            {
                newScrollPosition = GetScrollPositionByDataIndex(dataIndex, ItemPositionType.Before);
                newScrollPosition = Mathf.Clamp(newScrollPosition - (useSpacing ? spacing : 0), 0, scrollSize);
            }

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
        public void Snap(float snapWatchOffset, bool useSpacing = true, TweenType tweenType = TweenType.None, float tweenTime = 0f)
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
                    ScrollLayoutGroupViewItem cellView = null;

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
            else
            {
                if (itemIndex < m_ItemOffsetArray.Count)
                {
                    if (insertPosition == ItemPositionType.Before)
                    {
                        return m_ItemOffsetArray[itemIndex - 1] + spacing + (m_ScrollRect.vertical ? m_LayoutGroup.padding.top : m_LayoutGroup.padding.left);
                    }
                    else
                    {
                        return m_ItemOffsetArray[itemIndex] + (m_ScrollRect.vertical ? m_LayoutGroup.padding.top : m_LayoutGroup.padding.left);
                    }
                }
                else
                {
                    return m_ItemOffsetArray[m_ItemOffsetArray.Count - 2];
                }
            }
        }

        /// <summary>
        /// Gets the scroll position in pixels from the start of the scroller based on the dataIndex
        /// </summary>
        public float GetScrollPositionByDataIndex(int dataIndex, ItemPositionType insertPosition)
        {
            return GetScrollPositionByItemIndex(m_Loop ? getDataCountEvent.Invoke() + dataIndex : dataIndex, insertPosition);
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
        public ScrollLayoutGroupViewItem GetItemByDataIndex(int dataIndex)
        {
            for (var i = 0; i < m_ActiveItems.Count; i++)
            {
                if (m_ActiveItems[i].dataIndex == dataIndex)
                {
                    return m_ActiveItems[i];
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

            if (m_DragFingerCount > 1)
            {
                return;
            }

            m_LoopBeforeDrag = m_Loop;
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


			m_Loop = m_LoopBeforeDrag;
		}

        /// <summary>
        /// Create a cell view, or recycle one if it already exists
        /// </summary>
        private ScrollLayoutGroupViewItem GetItem()
        {
            var item = GetRecycledItem();

            if (item == null)
            {
                var go = Instantiate(m_Prefab);
                item = Activator.CreateInstance(m_ItemClassType) as ScrollLayoutGroupViewItem;
                item.Create(go);
                item.transform.SetParent(m_Content);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
            }

            return item;
        }

        /// <summary>
        /// This function will create an internal list of sizes and offsets to be used in all calculations.
        /// </summary>
        private void Resize(bool keepPosition)
        {
            var originalScrollPosition = m_ScrollPosition;
            m_ItemSizeArray.Clear();
            var offset = AddItemSizes();

            if (m_Loop)
            {
                var itemCount = m_ItemSizeArray.Count;

                if (offset < scrollRectSize)
                {
                    int additionalRounds = Mathf.CeilToInt((float)Mathf.CeilToInt(scrollRectSize / offset) / 2.0f) * 2;
                    DuplicateItemSizes(additionalRounds, itemCount);
                    m_LoopFirstCellIndex = itemCount * (1 + (additionalRounds / 2));
                }
                else
                {
                    m_LoopFirstCellIndex = itemCount;
                }

                m_LoopLastCellIndex = m_LoopFirstCellIndex + itemCount - 1;
                DuplicateItemSizes(2, itemCount);
            }

            CalculateItemOffsets();

            if (m_ScrollRect.vertical)
            {
                m_Content.sizeDelta = new Vector2(m_Content.sizeDelta.x, m_ItemOffsetArray.Last() + m_LayoutGroup.padding.top + m_LayoutGroup.padding.bottom);
            }
            else
            {
                m_Content.sizeDelta = new Vector2(m_ItemOffsetArray.Last() + m_LayoutGroup.padding.left + m_LayoutGroup.padding.right, m_Content.sizeDelta.y);
            }

            if (m_Loop)
            {
                m_LoopFirstScrollPosition = GetScrollPositionByItemIndex(m_LoopFirstCellIndex, ItemPositionType.Before) + (spacing * 0.5f);
                m_LoopLastScrollPosition = GetScrollPositionByItemIndex(m_LoopLastCellIndex, ItemPositionType.After) - scrollRectSize + (spacing * 0.5f);

                m_LoopFirstJumpTrigger = m_LoopFirstScrollPosition - scrollRectSize;
                m_LoopLastJumpTrigger = m_LoopLastScrollPosition + scrollRectSize;
            }

            ResetVisibleItems();

            if (keepPosition)
            {
                scrollPosition = originalScrollPosition;
            }
            else
            {
                if (m_Loop)
                {
                    scrollPosition = m_LoopFirstScrollPosition;
                }
                else
                {
                    scrollPosition = 0;
                }
            }

            scrollbarVisibility = m_ScrollbarVisibility;
        }

        /// <summary>
        /// Updates the spacing on the scroller
        /// </summary>
        private void UpdateSpacing(float spacing)
        {
            m_UpdateSpacing = false;
            m_LayoutGroup.spacing = spacing;
            RefreshData(normalizedScrollPosition);
        }

        /// <summary>
        /// Creates a list of cell view sizes for faster access
        /// </summary>
        private float AddItemSizes()
        {
            var offset = 0f;
            m_SingleLoopGroupSize = 0;

            for (var i = 0; i < itemCount; i++)
            {
                m_ItemSizeArray.Add(getItemSizeEvent.Invoke(i) + (i == 0 ? 0 : spacing));
                m_SingleLoopGroupSize += m_ItemSizeArray[m_ItemSizeArray.Count - 1];
                offset += m_ItemSizeArray[m_ItemSizeArray.Count - 1];
            }

            return offset;
        }

        /// <summary>
        /// Create a copy of the cell view sizes. This is only used in looping
        /// </summary>
        private void DuplicateItemSizes(int numberOfTimes, int cellCount)
        {
            for (var i = 0; i < numberOfTimes; i++)
            {
                for (var j = 0; j < cellCount; j++)
                {
                    m_ItemSizeArray.Add(m_ItemSizeArray[j] + (j == 0 ? spacing : 0));
                }
            }
        }

        /// <summary>
        /// Calculates the offset of each cell, accumulating the values from previous cells
        /// </summary>
        private void CalculateItemOffsets()
        {
            m_ItemOffsetArray.Clear();
            var offset = 0f;
            for (var i = 0; i < m_ItemSizeArray.Count; i++)
            {
                offset += m_ItemSizeArray[i];
                m_ItemOffsetArray.Add(offset);
            }
        }

        /// <summary>
        /// Get a recycled cell with a given identifier if available
        /// </summary>
        private ScrollLayoutGroupViewItem GetRecycledItem()
        {
            if (m_RecycledItems != null && m_RecycledItems.Count > 0) 
            {
                var cellView = m_RecycledItems.RemoveAt(0);
                return cellView;
            }

            return null;
        }

        /// <summary>
        /// This sets up the visible cells, adding and recycling as necessary
        /// </summary>
        private void ResetVisibleItems()
        {
            int startIndex;
            int endIndex;

            CalculateCurrentActiveItemRange(out startIndex, out endIndex);

            // go through each previous active cell and recycle it if it no longer falls in the range
            var i = 0;
            SmallList<int> remainingCellIndices = new SmallList<int>();

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
                    if (i < remainingCellIndices.First())
                    {
                        AddItem(i, ListPositionType.First);
                    }
                }

                for (i = startIndex; i <= endIndex; i++)
                {
                    if (i > remainingCellIndices.Last())
                    {
                        AddItem(i, ListPositionType.Last);
                    }
                }
            }

            m_ActiveItemsStartIndex = startIndex;
            m_ActiveItemsEndIndex = endIndex;

            SetPadders();
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
        private void RecycleItem(ScrollLayoutGroupViewItem item)
        {
            m_ActiveItems.Remove(item);
            m_RecycledItems.Add(item);

            item.SetActive(false);
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

            var dataIndex = itemIndex % itemCount;
            var item = GetItem();

            item.itemIndex = itemIndex;
            item.dataIndex = dataIndex;
            item.transform.localScale = Vector3.one;
            item.transform.SetParent(m_Content, false);
            item.SetActive(true);

            LayoutElement layoutElement = item.gameObject.GetOrAddComponent<LayoutElement>();

            if (m_ScrollRect.vertical)
            {
                layoutElement.minHeight = m_ItemSizeArray[itemIndex] - (itemIndex > 0 ? spacing : 0);
            }
            else
            {
                layoutElement.minWidth = m_ItemSizeArray[itemIndex] - (itemIndex > 0 ? spacing : 0);
            }

            if (listPosition == ListPositionType.First)
            {
                m_ActiveItems.AddStart(item);
            }
            else
            {
                m_ActiveItems.Add(item);
            }

            if (listPosition == ListPositionType.Last)
            {
                item.transform.SetSiblingIndex(m_Content.childCount - 2);
            }

            else if (listPosition == ListPositionType.First)
            {
                item.transform.SetSiblingIndex(1);
            }

            itemUpdateEvent?.Invoke(item);
        }

        /// <summary>
        /// This function adjusts the two padders that control the first cell view's
        /// </summary>
        private void SetPadders()
        {
            if (itemCount == 0)
            {
                return;
            }

            var firstSize = m_ItemOffsetArray[m_ActiveItemsStartIndex] - m_ItemSizeArray[m_ActiveItemsStartIndex];
            var lastSize = m_ItemOffsetArray.Last() - m_ItemOffsetArray[m_ActiveItemsEndIndex];

            if (m_ScrollRect.vertical)
            {
                m_FirstPadder.minHeight = firstSize;
                m_FirstPadder.gameObject.SetActive(m_FirstPadder.minHeight > 0);
                m_LastPadder.minHeight = lastSize;
                m_LastPadder.gameObject.SetActive(m_LastPadder.minHeight > 0);
            }
            else
            {
                m_FirstPadder.minWidth = firstSize;
                m_FirstPadder.gameObject.SetActive(m_FirstPadder.minWidth > 0);
                m_LastPadder.minWidth = lastSize;
                m_LastPadder.gameObject.SetActive(m_LastPadder.minWidth > 0);
            }
        }

        /// <summary>
        /// This function is called if the scroller is scrolled, updating the active list of cells
        /// </summary>
        private void RefreshActive()
        {
            int startIndex;
            int endIndex;
            var velocity = Vector2.zero;

            if (m_Loop && !m_IgnoreLoopJump)
            {
                if (m_ScrollPosition < m_LoopFirstJumpTrigger)
                {
                    velocity = m_ScrollRect.velocity;
                    scrollPosition = m_LoopLastScrollPosition - (m_LoopFirstJumpTrigger - m_ScrollPosition) + spacing;
                    m_ScrollRect.velocity = velocity;
                }
                else if (m_ScrollPosition > m_LoopLastJumpTrigger)
                {
                    velocity = m_ScrollRect.velocity;
                    scrollPosition = m_LoopFirstScrollPosition + (m_ScrollPosition - m_LoopLastJumpTrigger) - spacing;
                    m_ScrollRect.velocity = velocity;
                }
            }

            CalculateCurrentActiveItemRange(out startIndex, out endIndex);

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
            startIndex = 0;
            endIndex = 0;

            var startPosition = m_ScrollPosition - lookAheadBefore;
            var endPosition = m_ScrollPosition + (m_ScrollRect.vertical ? m_ScrollRectTransform.rect.height : m_ScrollRectTransform.rect.width) + lookAheadAfter;

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
            var pad = m_ScrollRect.vertical ? m_LayoutGroup.padding.top : m_LayoutGroup.padding.left;

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

            if (m_UpdateSpacing)
            {
                UpdateSpacing(spacing);
                m_IsRefreshData = false;
            }

            if (m_IsRefreshData)
            {
                RefreshData();
            }

            if ((m_Loop && m_LastScrollRectSize != scrollRectSize) || (m_Loop != m_LastLoop))
            {
                Resize(true);
                m_LastScrollRectSize = scrollRectSize;
                m_LastLoop = m_Loop;
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
        /// Reacts to changes in the inspector
        /// </summary>
        private void OnValidate()
        {
            if (m_IsInitialized && spacing != m_LayoutGroup.spacing)
            {
                m_UpdateSpacing = true;
            }
        }

        /// <summary>
        /// Fired at the end of the frame.
        /// </summary>
        private void LateUpdate()
        {
            if(!m_IsInitialized)
            {
                return; 
            }

			if (maxVelocity > 0)
			{
				if (m_ScrollRect.horizontal)
				{
					velocity = new Vector2(Mathf.Clamp(Mathf.Abs(velocity.x), 0, maxVelocity) * Mathf.Sign(velocity.x), velocity.y);
				}
				else
				{
					velocity = new Vector2(velocity.x, Mathf.Clamp(Mathf.Abs(velocity.y), 0, maxVelocity) * Mathf.Sign(velocity.y));
				}
			}
        }

        private void OnEnable()
        {
            if (m_ScrollRect == null)
            {
                Log.LogError(name, "scroll rect is invalid!");
                return;
            }

            m_ScrollRect.onValueChanged.AddListener(ScrollRectOnValueChanged);
        }

        private void OnDisable()
        {
            if (m_ScrollRect == null)
            {
                Log.LogError(name, "scroll rect is invalid!");
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
        IEnumerator TweenPosition(TweenType tweenType, float time, float start, float end, Action tweenComplete, bool forceCalculateRange)
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

        private ScrollRect m_ScrollRect;
        private Scrollbar m_Scrollbar;
        private RectTransform m_ScrollRectTransform;
        private RectTransform m_RecycledItemsContent;
        private RectTransform m_Content;
        private LayoutElement m_FirstPadder;
        private LayoutElement m_LastPadder;
        private GameObject m_Prefab;
      
        private HorizontalOrVerticalLayoutGroup m_LayoutGroup;
        private SmallList<ScrollLayoutGroupViewItem> m_ActiveItems = null;
        private SmallList<ScrollLayoutGroupViewItem> m_RecycledItems = null;
        private SmallList<float> m_ItemSizeArray = null;
        private SmallList<float> m_ItemOffsetArray = null;

        [SerializeField]
        private ScrollbarVisibilityType m_ScrollbarVisibility;

        [SerializeField]
        private bool m_Loop;
        private bool m_IsInitialized = false;
        private bool m_UpdateSpacing = false;
        private bool m_IsRefreshData;
        private bool m_LastLoop;
        private bool m_LoopBeforeDrag;
        private bool m_IgnoreLoopJump;

        public float m_ScrollPosition;
        private float m_LoopFirstScrollPosition;
        private float m_LoopLastScrollPosition;
        private float m_LoopFirstJumpTrigger;
        private float m_LoopLastJumpTrigger;
        private float m_LastScrollRectSize;
        private float m_SingleLoopGroupSize;
        private float m_TweenTimer;
        private float m_LookAheadBefore;
        private float m_LookAheadAfter;

        private int m_ActiveItemsStartIndex;
        private int m_ActiveItemsEndIndex;
        private int m_LoopFirstCellIndex;
        private int m_LoopLastCellIndex;
        private int m_DragFingerCount;

        private Type m_ItemClassType = null;
        private ScrollbarVisibilityType m_LastScrollbarVisibility;
    }
}
