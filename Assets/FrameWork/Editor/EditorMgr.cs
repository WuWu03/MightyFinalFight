using GameFrameWork.BehaviourTree;
using GameFrameWork.UI;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
	public static class EditorMgr
	{
		[MenuItem("GameFrameWork/UI/创建UI场景")]
		public static void NewUIScene()
		{
			UIUtility.NewUIScene();
		}

		[MenuItem("GameFrameWork/UI/EmojiText/CreateEmojiText")]
		public static void CreateEmojiText()
		{
			GameObject go = new GameObject("EmojiText", typeof(GameFrameWork.UI.EmojiText));
			go.transform.SetParent(Selection.activeTransform, false);
			go.GetComponent<EmojiText>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/UGUIEmoji.mat");
		}

		[MenuItem("GameFrameWork/UI/EmojiText/BuildEmoji")]
		public static void BuildEmojiText()
		{
			EmojiBuilder.BuildEmoji();
		}

		[MenuItem("GameFrameWork/SpriteSpliter")]
		public static void OpenSpriteSpliter()
		{
			Rect wr = new Rect(0, 0, 600, 300);
			EditorWindow window = EditorWindow.GetWindowWithRect(typeof(SpriteSplitWindow), wr);
			window.Show();
		}

		[MenuItem("GameFrameWork/AssetBundleEditor")]
		public static void AssetBundleEditor()
		{
			Rect wr = new Rect(0, 0, 700, 800);
			EditorWindow window = EditorWindow.GetWindowWithRect(typeof(AssetBundleWindow), wr);
			window.Show();
		}

		[MenuItem("GameFrameWork/BehaviourTreeEditor")]
		public static void BehaviourTreeEditor()
        {
			EditorWindow window = EditorWindow.GetWindow<BehaviourTreeWindow>();
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/Tab")]
		public static void TabDemoWinow()
		{
			Rect wr = new Rect(0, 0, 600, 600);
			TabDemo window = EditorWindow.GetWindowWithRect<TabDemo>(wr, true, "Unity Tab表签");
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/Styles&Icons")]
		public static void BuiltInDemo()
		{
			BuiltInDemo window = EditorWindow.GetWindow<BuiltInDemo>();
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/SplitView")]
		public static void Init()
		{
			EditorWindow window = EditorWindow.GetWindow<SplitViewDemo>();
			window.Show();
		}
	}
}