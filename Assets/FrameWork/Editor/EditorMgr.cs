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
			Rect wr = new Rect(0, 0, 600, 600);
			EditorWindow s = EditorWindow.GetWindowWithRect(typeof(SpriteSplitWindow), wr);
			s.Show();
		}

		[MenuItem("GameFrameWork/AssetBundleEditor")]
		public static void AssetBundleEditor()
		{
			Rect wr = new Rect(0, 0, 700, 800);
			EditorWindow s = EditorWindow.GetWindowWithRect(typeof(AssetBundleWindow), wr);
			s.Show();
		}

		[MenuItem("GameFrameWork/BehaviourTreeEditor")]
		public static void BehaviourTreeEditor()
        {
			EditorWindow s = EditorWindow.GetWindow<BehaviourTreeWindow>();
			s.Show();
		}

		[MenuItem("GameFrameWork/BehaviourTreeEditortttt")]
		public static void BehaviourTreeEditotttr()
		{
			EditorWindow s = EditorWindow.GetWindow<BehaviourTreeWindow>();
			s.Close();
		}

		public static void CreateBehaviorConfig(string name,string extend,string path)
		{
			EditorUtility.CreateConfigData<BehaviourTreeConfig, BehaviourTreeData>(name, extend, path);
		}
	}
}