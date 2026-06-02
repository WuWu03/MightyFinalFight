using UnityEngine;
using UnityEditor;

namespace WuWuFramework.Editor
{
	public class EditorGUISplitView
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

        public Vector2 scrollPosition
		{
			get;
			private set;
		}

        private Rect m_AvailableRect;
        private Direction m_SplitDirection;
        private float m_SplitNormalizedPosition;
        private bool m_Resize;
        private float m_MinHorizontal = 0.2f;

        public EditorGUISplitView(Direction splitDirection)
		{
			m_SplitNormalizedPosition = 0.2f;
			m_SplitDirection = splitDirection;
		}

		public void BeginSplitView()
		{
			Rect tempRect;

			if (m_SplitDirection == Direction.Horizontal)
				tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			else
				tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

			if (tempRect.width > 0.0f)
			{
				m_AvailableRect = tempRect;
			}

			if (m_SplitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView, GUILayout.Width(m_AvailableRect.width * m_SplitNormalizedPosition));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(m_AvailableRect.height * m_SplitNormalizedPosition));
		}

		public void Split()
		{
			GUILayout.EndScrollView();
			ResizeSplitFirstView();

			if (m_SplitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView, GUILayout.Width(m_AvailableRect.width * (1 - m_SplitNormalizedPosition)));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(m_AvailableRect.height * (1 - m_SplitNormalizedPosition)));
		}

		public void EndSplitView()
		{
			if (m_SplitDirection == Direction.Horizontal)
				EditorGUILayout.EndHorizontal();
			else
				EditorGUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void ResizeSplitFirstView()
		{
			Rect resizeHandleRect;
			Rect drawRect;

			if (m_SplitDirection == Direction.Horizontal)
			{
				resizeHandleRect = new Rect(m_AvailableRect.width * m_SplitNormalizedPosition, m_AvailableRect.y, 5f, m_AvailableRect.height);
				drawRect = new Rect(resizeHandleRect.x - 0.5f, resizeHandleRect.y, 1f, m_AvailableRect.height);
			}
			else
			{
				resizeHandleRect = new Rect(m_AvailableRect.x, m_AvailableRect.height * m_SplitNormalizedPosition, m_AvailableRect.width, 2f);
				drawRect = new Rect(resizeHandleRect.x, resizeHandleRect.y + 0.5f, m_AvailableRect.height, 1f);
			}

			GUILayout.BeginArea(drawRect, GUI.skin.textArea);
			GUILayout.EndArea();

			if (m_SplitDirection == Direction.Horizontal)
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
			else
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

			if (UnityEngine.Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(UnityEngine.Event.current.mousePosition))
			{
				m_Resize = true;
			}
			if (m_Resize)
			{
				if (m_SplitDirection == Direction.Horizontal)
				{
					float temp = UnityEngine.Event.current.mousePosition.x / m_AvailableRect.width;
					m_SplitNormalizedPosition = Mathf.Clamp(temp, m_MinHorizontal, 1.0f);
				}
				else
					m_SplitNormalizedPosition = UnityEngine.Event.current.mousePosition.y / m_AvailableRect.height;
			}
			if (UnityEngine.Event.current.type == EventType.MouseUp)
				m_Resize = false;
		}
    }
}
