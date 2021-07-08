using UnityEngine;
using System.Collections;
using UnityEditor;

namespace GameFrameWork.Editor
{
	public class EditorGUISplitView
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

		Direction splitDirection;
		float splitNormalizedPosition;
		bool resize;
		public Vector2 scrollPosition;
		Rect availableRect;
		float minHorizontal = 0.2f;

		public EditorGUISplitView(Direction splitDirection)
		{
			splitNormalizedPosition = 0.3f;
			this.splitDirection = splitDirection;
	
		}

		public void BeginSplitView()
		{
			Rect tempRect;

			if (splitDirection == Direction.Horizontal)
				tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			else
				tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

			if (tempRect.width > 0.0f)
			{
				availableRect = tempRect;
			}

			if (splitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView, GUILayout.Width(availableRect.width * splitNormalizedPosition));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));
		}

		public void Split()
		{
			GUILayout.EndScrollView();
			ResizeSplitFirstView();

			if (splitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView, GUILayout.Width(availableRect.width * (1 - splitNormalizedPosition)));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * (1 - splitNormalizedPosition)));
		}

		public void EndSplitView()
		{
			if (splitDirection == Direction.Horizontal)
				EditorGUILayout.EndHorizontal();
			else
				EditorGUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void ResizeSplitFirstView()
		{

			Rect resizeHandleRect;
			Rect drawRect;
			if (splitDirection == Direction.Horizontal)
			{
				resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition, availableRect.y, 5f, availableRect.height);
				drawRect = new Rect(resizeHandleRect.x - 0.5f, resizeHandleRect.y, 1f, availableRect.height);
			}
			else
			{
				resizeHandleRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition, availableRect.width, 2f);
				drawRect = new Rect(resizeHandleRect.x, resizeHandleRect.y + 0.5f, availableRect.height, 1f);
			}


			GUILayout.BeginArea(drawRect, GUI.skin.textArea);
			GUILayout.EndArea();

			if (splitDirection == Direction.Horizontal)
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
			else
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

			if (UnityEngine.Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(UnityEngine.Event.current.mousePosition))
			{
				resize = true;
			}
			if (resize)
			{
				if (splitDirection == Direction.Horizontal)
				{
					float temp = UnityEngine.Event.current.mousePosition.x / availableRect.width;
					splitNormalizedPosition = Mathf.Clamp(temp, minHorizontal, 1.0f);
				}
				else
					splitNormalizedPosition = UnityEngine.Event.current.mousePosition.y / availableRect.height;
			}
			if (UnityEngine.Event.current.type == EventType.MouseUp)
				resize = false;
		}
	}
}
