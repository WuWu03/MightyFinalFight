using System.IO;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using WuWuFramework.UI;
using WuWuFramework.Utils;

namespace WuWuFramework.Editor
{
    [CustomEditor(typeof(ImageEx))]
    public class ImageExEditor : ImageEditor
    {
        private ImageEx m_ImageEx;
        private Sprite m_CurrSprite;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ImageEx = target as ImageEx;
            m_CurrSprite = m_ImageEx.sprite;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SerializedProperty spriteNameProperty = serializedObject.FindProperty("m_SpriteName");
            if (m_CurrSprite != m_ImageEx.sprite)
            {
                m_CurrSprite = m_ImageEx.sprite;
                string uiSpritesPath = EditorMgr.GetWuWuFrameworkConfig().uiSpritesPath;
                string assetPath = AssetDatabase.GetAssetPath(m_CurrSprite);
                string atlasName = Path.GetDirectoryName(assetPath).Replace("\\", "/").Replace(uiSpritesPath, "");
                string spriteName = Path.GetFileNameWithoutExtension(assetPath);
                spriteNameProperty.stringValue = PathUtil.FormatPath(atlasName, spriteName);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}