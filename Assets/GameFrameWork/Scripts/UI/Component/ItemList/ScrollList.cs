using System;
using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollList : MonoBehaviour
    {
        enum ListPositionType
        {
            First,
            Last
        }

        public event GameFrameWorkAction<BaseListItem> itemUpdateEvent;
        public event GameFrameWorkFunc<int, Vector2> itemSizeEvent;
        public float xSpacing;
        public float ySpacing;
        public bool isHorizontalReverse;
        public bool isVerticalReverse;
        public GameObject prefab;

        private float scrollSize
        {
            get
            {
                if (m_ScrollRect.vertical)
                {
                    return Mathf.Max(m_ScrollRect.content.rect.height - m_ScrollRectTransform.rect.height, 0);
                }

                return Mathf.Max(m_ScrollRect.content.rect.width - m_ScrollRectTransform.rect.width, 0);
            }
        }

        private ScrollRect m_ScrollRect;
        private RectTransform m_ScrollRectTransform;
        private RectTransform m_PrefabRect;
        private Type m_ItemClassType;
        private int m_Row;
        private int m_Column;
        private int m_ItemCount;
        private int m_CurrStartRowOrColumn;
        private int m_CurrEndRowOrColumn;
        private float m_ScrollPosition;
        private LinkedList<BaseListItem> m_ActiveItems;
        private Queue<BaseListItem> m_RecycledItems;
        private List<Vector2> m_ItemSizeArray;
        private List<float> m_ItemOffsetArray;
        private List<int> m_RemainingItemIndexes;
        private bool m_IsInit;
        private bool m_HasAddEvent;
        private bool m_HasInitScrollPos;
        
        private void OnEnable()
        {
            if (!m_IsInit)
            {
                return;
            }

            if (m_ScrollRect is null)
            {
                throw new GameFrameWorkException("[Scroll Rect] 组件不存在");
            }

            AddEvent();
        }

        private void OnDisable()
        {
            if (!m_IsInit)
            {
                return;
            }

            if (m_ScrollRect is null)
            {
                throw new GameFrameWorkException("[Scroll Rect] 组件不存在");
            }

            RemoveEvent();
        }

        public void Init<T>() where T : BaseListItem
        {
            if (m_IsInit)
            {
                throw new GameFrameWorkException("不能重复初始化");
            }

            m_ScrollRect = GetComponent<ScrollRect>();

            if (m_ScrollRect is null)
            {
                throw new GameFrameWorkException("[Scroll Rect] 组件不存在");
            }

            if (m_ScrollRect.content is null || m_ScrollRect.viewport is null)
            {
                throw new GameFrameWorkException("ScrollRect组件设置异常");
            }

            if (prefab is null)
            {
                throw new GameFrameWorkException("prefab为空");
            }

            prefab.SetActiveSelf(false);
            AddEvent();

            if (m_ScrollRect.vertical)
            {
                m_ScrollRect.content.anchorMin = new Vector2(0, isVerticalReverse ? 0 : 1);
                m_ScrollRect.content.anchorMax = new Vector2(1, isVerticalReverse ? 0 : 1);
                m_ScrollRect.content.pivot = new Vector2(0, isVerticalReverse ? 0 : 1);
            }
            else
            {
                m_ScrollRect.content.anchorMin = new Vector2(isHorizontalReverse ? 1 : 0, 0);
                m_ScrollRect.content.anchorMax = new Vector2(isHorizontalReverse ? 1 : 0, 1);
                m_ScrollRect.content.pivot = new Vector2(isHorizontalReverse ? 1 : 0, 0);
            }

            m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();
            m_ScrollRect.content.offsetMax = Vector2.zero;
            m_ScrollRect.content.offsetMin = Vector2.zero;
            m_ScrollRect.content.anchoredPosition = Vector2.zero;
            m_ScrollRect.content.localRotation = Quaternion.identity;
            m_ScrollRect.content.localScale = Vector3.one;
            m_PrefabRect = prefab.GetComponent<RectTransform>();
            m_ItemClassType = typeof(T);
            m_ActiveItems = new();
            m_RecycledItems = new();
            m_ItemSizeArray = new();
            m_ItemOffsetArray = new();
            m_RemainingItemIndexes = new();
            m_IsInit = true;
        }

        public void SetItemCount(int count, bool keepPosition = true)
        {
            if (!m_IsInit)
            {
                throw new GameFrameWorkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_ItemCount == count)
            {
                RefreshActiveItems(keepPosition);
                return;
            }

            m_ItemCount = count;

            if (m_ScrollRect.vertical)
            {
                float realScrollWidth = m_ScrollRectTransform.sizeDelta.x + xSpacing;
                float realItemWidth = m_PrefabRect.sizeDelta.x + xSpacing;
                int column = Mathf.FloorToInt(realScrollWidth / realItemWidth);
                m_Column = Math.Max(1, column); //最少有一列
                m_Row = m_ItemCount / m_Column + (m_ItemCount % m_Column > 0 ? 1 : 0);
            }
            else
            {
                float realScrollHeight = m_ScrollRectTransform.sizeDelta.y + ySpacing;
                float realItemHeight = m_PrefabRect.sizeDelta.y + ySpacing;
                int row = Mathf.FloorToInt(realScrollHeight / realItemHeight);
                m_Row = Math.Max(1, row); //最少有一行
                m_Column = m_ItemCount / m_Row + (m_ItemCount % m_Row > 0 ? 1 : 0);
            }

            RecycleAllItems();
            Resize();

            if (!m_HasInitScrollPos)
            {
                ResetScrollPosition();
                m_HasInitScrollPos = true;
            }
            else if (!keepPosition)
            {
                ResetScrollPosition();
            }
            else
            {
                ResetVisibleItems();
            }
        }

        public void SetScrollPosition(float scrollPosition, bool isForce)
        {
            if (!m_IsInit)
            {
                throw new GameFrameWorkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_ItemCount < 1)
            {
                return;
            }

            scrollPosition = Mathf.Clamp(scrollPosition, 0, scrollSize);

            if (!isForce && Mathf.Approximately(m_ScrollPosition, scrollPosition))
            {
                return;
            }

            m_ScrollPosition = scrollPosition;

            if (m_ScrollRect.vertical)
            {
                m_ScrollRect.verticalNormalizedPosition = 1f - m_ScrollPosition / scrollSize;
            }
            else
            {
                m_ScrollRect.horizontalNormalizedPosition = m_ScrollPosition / scrollSize;
            }

            ResetVisibleItems();
        }

        public void RefreshActiveItems(bool keepPosition = true)
        {
            if (!m_IsInit)
            {
                throw new GameFrameWorkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_ItemCount < 1 || m_ActiveItems.Count < 1)
            {
                return;
            }

            if (!keepPosition)
            {
                ResetScrollPosition();
                return;
            }

            foreach (var activeItem in m_ActiveItems)
            {
                itemUpdateEvent?.Invoke(activeItem);
            }
        }

        private void ResetScrollPosition()
        {
            if (m_ScrollRect.vertical)
            {
                float scrollPositionFactor = isVerticalReverse ? 0 : 1;
                SetScrollPosition((1 - scrollPositionFactor) * scrollSize, true);
            }
            else
            {
                float scrollPositionFactor = isHorizontalReverse ? 1 : 0;
                SetScrollPosition(scrollPositionFactor * scrollSize, true);
            }
        }

        private void Resize()
        {
            if (m_ItemCount < 1)
            {
                return;
            }

            m_ItemSizeArray.Clear();
            m_ItemOffsetArray.Clear();
            float spacing = m_ScrollRect.vertical ? ySpacing : xSpacing;
            float offset = 0f;
            int rowOrColumn = m_ScrollRect.vertical ? m_Row : m_Column;
            int perCount = m_ScrollRect.vertical ? m_Column : m_Row;

            for (int i = 0; i < m_ItemCount; i++)
            {
                m_ItemSizeArray.Add(itemSizeEvent?.Invoke(i) ?? m_PrefabRect.sizeDelta);
            }

            for (var i = 0; i < rowOrColumn; i++) // 只存储行或列的第一个元素的宽或高
            {
                int index = i * perCount;
                float itemSize = m_ScrollRect.vertical ? m_ItemSizeArray[index].y : m_ItemSizeArray[index].x;
                offset = offset + itemSize + (i == 0 ? 0 : spacing);
                m_ItemOffsetArray.Add(offset);
            }

            float offest = m_ItemOffsetArray[^1];
            float width = m_ScrollRect.horizontal ? offest : m_ScrollRect.content.sizeDelta.x;
            float height = m_ScrollRect.vertical ? offest : m_ScrollRect.content.sizeDelta.y;
            m_ScrollRect.content.sizeDelta = new Vector2(width, height);
        }

        private void ResetVisibleItems()
        {
            if (m_ItemCount < 1)
            {
                return;
            }

            CalculateCurrentActiveItemRange(out int startRowOrColumn, out int endRowOrColumn);

            if (startRowOrColumn == m_CurrStartRowOrColumn && endRowOrColumn == m_CurrEndRowOrColumn)
            {
                return;
            }

            int perCount = m_ScrollRect.vertical ? m_Column : m_Row;
            int startIndex = startRowOrColumn * perCount;
            int endIndex = Mathf.Min(m_ItemCount - 1, endRowOrColumn * perCount + perCount - 1);
            m_RemainingItemIndexes.Clear();
            LinkedListNode<BaseListItem> current = m_ActiveItems.First;

            while (current != null)
            {
                if (current.Value.index < startIndex || current.Value.index > endIndex)
                {
                    RecycleItem(current.Value);
                    current = m_ActiveItems.First;
                }
                else
                {
                    m_RemainingItemIndexes.Add(current.Value.index);
                    current = current.Next;
                }
            }

            if (m_RemainingItemIndexes.Count == 0)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    AddItem(i, ListPositionType.Last);
                }
            }
            else
            {
                for (int i = endIndex; i >= startIndex; i--)
                {
                    if (i < m_RemainingItemIndexes[0])
                    {
                        AddItem(i, ListPositionType.First);
                    }
                }

                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i > m_RemainingItemIndexes[^1])
                    {
                        AddItem(i, ListPositionType.Last);
                    }
                }
            }

            m_CurrStartRowOrColumn = startRowOrColumn;
            m_CurrEndRowOrColumn = endRowOrColumn;
        }

        private void CalculateCurrentActiveItemRange(out int startRowOrColumn, out int endRowOrColumn)
        {
            float scrollRectSize = m_ScrollRect.vertical ? m_ScrollRectTransform.rect.height : m_ScrollRectTransform.rect.width;
            float startPosition = m_ScrollPosition;
            float endPosition = m_ScrollPosition + scrollRectSize;
            startRowOrColumn = GetItemIndexAtPosition(startPosition, 0, m_ItemOffsetArray.Count - 1);
            endRowOrColumn = GetItemIndexAtPosition(endPosition, 0, m_ItemOffsetArray.Count - 1);

            bool verticalReverseCondition = m_ScrollRect.vertical && isVerticalReverse;
            bool horizontalReverseCondition = m_ScrollRect.horizontal && isHorizontalReverse;

            if (horizontalReverseCondition || verticalReverseCondition)
            {
                int tempStartRowOrColumn = (m_ScrollRect.vertical ? m_Row : m_Column) - endRowOrColumn - 1;
                int tempEndRowOrColumn = (m_ScrollRect.vertical ? m_Row : m_Column) - startRowOrColumn - 1;
                startRowOrColumn = tempStartRowOrColumn;
                endRowOrColumn = tempEndRowOrColumn;
            }
        }

        private int GetItemIndexAtPosition(float position, int startRowOrColumn, int endRowOrColumn)
        {
            if (startRowOrColumn >= endRowOrColumn)
            {
                return startRowOrColumn;
            }

            while (startRowOrColumn < endRowOrColumn)
            {
                int middleRowOrColumn = (startRowOrColumn + endRowOrColumn) / 2;

                if (m_ItemOffsetArray[middleRowOrColumn] >= position)
                {
                    endRowOrColumn = middleRowOrColumn;
                }
                else
                {
                    startRowOrColumn = middleRowOrColumn + 1;
                }
            }

            return startRowOrColumn;
        }

        private void AddItem(int index, ListPositionType listPosition)
        {
            if (m_ItemCount == 0)
            {
                return;
            }

            var item = GetItem();
            item.index = index;
            item.transform.localScale = Vector3.one;
            item.transform.SetParent(m_ScrollRect.content, false);
            item.SetActive(true);
            item.rectTransform.anchorMin = new Vector2(isHorizontalReverse ? 1 : 0, isVerticalReverse ? 0 : 1);
            item.rectTransform.anchorMax = new Vector2(isHorizontalReverse ? 1 : 0, isVerticalReverse ? 0 : 1);
            item.rectTransform.pivot = new Vector2(isHorizontalReverse ? 1 : 0, isVerticalReverse ? 0 : 1);
            int row = m_ScrollRect.vertical ? index / m_Column : index % m_Row;
            int column = m_ScrollRect.vertical ? index % m_Column : index / m_Row;
            Vector2 size = m_ItemSizeArray[index];
            float itemXSpacing = column > 0 ? this.xSpacing : 0;
            float itemYSpacing = row > 0 ? this.ySpacing : 0;
            float posX = (column * size.x + itemXSpacing * column) * (isHorizontalReverse ? -1 : 1);
            float posY = (row * size.y + itemYSpacing * row) * (isVerticalReverse ? 1 : -1);
            item.rectTransform.anchoredPosition = new Vector2(posX, posY);

            if (item.rectTransform.sizeDelta != size)
            {
                item.rectTransform.sizeDelta = size;
            }

            if (listPosition == ListPositionType.First)
            {
                m_ActiveItems.AddFirst(item);
            }
            else
            {
                m_ActiveItems.AddLast(item);
            }

            itemUpdateEvent?.Invoke(item);
        }

        private BaseListItem GetItem()
        {
            if (m_RecycledItems.Count > 0)
            {
                return m_RecycledItems.Dequeue();
            }

            var go = Instantiate(prefab);
            var item = Activator.CreateInstance(m_ItemClassType) as BaseListItem;
            item?.Create(go);
            return item;
        }

        private void RecycleAllItems()
        {
            foreach (var activeItem in m_ActiveItems)
            {
                activeItem.SetActive(false);
                activeItem.index = 0;
                m_RecycledItems.Enqueue(activeItem);
            }

            m_ActiveItems.Clear();
            m_CurrStartRowOrColumn = 0;
            m_CurrEndRowOrColumn = 0;
        }

        private void RecycleItem(BaseListItem activeItem)
        {
            m_ActiveItems.Remove(activeItem);
            m_RecycledItems.Enqueue(activeItem);
            activeItem.SetActive(false);
            activeItem.index = 0;
        }

        private void AddEvent()
        {
            if (!m_HasAddEvent)
            {
                m_ScrollRect.onValueChanged.AddListener(ScrollRectOnValueChanged);
                m_HasAddEvent = true;
            }
        }

        private void RemoveEvent()
        {
            if (m_HasAddEvent)
            {
                m_ScrollRect.onValueChanged.RemoveListener(ScrollRectOnValueChanged);
                m_HasAddEvent = false;
            }
        }

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
            ResetVisibleItems();
        }
    }
}