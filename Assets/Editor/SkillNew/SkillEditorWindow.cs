using GameFrameWork.Utilities;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillEditorWindow : EditorWindow
    {
        private void OnEnable()
        {
            SkillEditorHelper.InitConfig(this);
            m_CurrPage = 0;
            m_SkillGUIs = new SkillGUI[3];
            m_SkillGUIs[0] = new SkillBaseGUI(this);
            m_SkillGUIs[1] = new SkillPrevConditionGUI(this);

            m_SkillGUIs[0].UpdateData();
            m_SkillGUIs[1].UpdateData();
            UpdateData();
        }

        public void OnDisable()
        {
            SkillEditorHelper.SaveConfig();
        }

        private void OnGUI()
        {
            SkillEditorHelper.InitGUIStyle();
            MainGUI();
        }

        private void MainGUI()
        {
            EditorGUILayout.BeginVertical();
            m_AddType = EditorGUILayout.TextField("技能类别", m_AddType);
            m_AddName = EditorGUILayout.TextField("技能名字", m_AddName);

            if (GUILayout.Button("增加"))
            {
                SkillEditorHelper.AddData(PathUtil.FormatPath(m_AddType, m_AddName));
                m_SkillGUIs[m_CurrPage].UpdateData();
                UpdateData();
            }

            if (GUILayout.Button("删除当前项"))
            {
                SkillEditorHelper.RemoveData();
                m_SkillGUIs[m_CurrPage].UpdateData();
                UpdateData();
            }

            EditorGUILayout.EndVertical();

            if (!SkillEditorHelper.HasData())
            {
                return;
            }

            EditorGUILayout.Space(10f);

            int selectIndex = EditorGUILayout.Popup("选择处理项", SkillEditorHelper.currSelectIndex, SkillEditorHelper.showNames);

            if (selectIndex != SkillEditorHelper.currSelectIndex)
            {
                SkillEditorHelper.currSelectIndex = selectIndex;
                m_SkillGUIs[m_CurrPage].UpdateData();
                UpdateData();
            }

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginHorizontal();
                bool isModify = m_CurrType != SkillEditorHelper.currShowType;
                m_CurrType = EditorGUILayout.TextField("类别", m_CurrType, SkillEditorHelper.GetTextFieldStyle(isModify));

                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    SkillEditorHelper.currConfigData.skillName = PathUtil.FormatPath(m_CurrType, m_CurrName);
                    SkillEditorHelper.SetShowNames();
                    m_CurrType = SkillEditorHelper.currShowType;
                    this.ShowNotification("更改成功");
                }
                EditorGUILayout.EndHorizontal();
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginHorizontal();
                bool isModify = m_CurrName != SkillEditorHelper.currShowName;
                m_CurrName = EditorGUILayout.TextField("名称", m_CurrName, SkillEditorHelper.GetTextFieldStyle(isModify));
               
                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    SkillEditorHelper.currConfigData.skillName = PathUtil.FormatPath(m_CurrType, m_CurrName);
                    SkillEditorHelper.SetShowNames();
                    m_CurrName = SkillEditorHelper.currShowName;
                    this.ShowNotification("更改成功");
                }

                EditorGUILayout.EndHorizontal();
            });

            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginHorizontal();
                bool isModify = m_CurrId != SkillEditorHelper.currConfigData.Id;
                m_CurrId = EditorGUILayout.IntField("Id", m_CurrId, SkillEditorHelper.GetTextFieldStyle(isModify));

                if (GUILayout.Button("更改", GUILayout.Width(100)))
                {
                    SkillEditorHelper.currConfigData.Id = m_CurrId;
                    this.ShowNotification("更改成功");
                }

                EditorGUILayout.EndHorizontal();
            });

            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < m_TabNames.Length; i++)
            {
                if (GUILayout.Button(m_TabNames[i], i == m_CurrPage ? SkillEditorHelper.selectButtonOnStyle : SkillEditorHelper.selectButtonStyle))
                {
                    m_CurrPage = i;
                    m_SkillGUIs[m_CurrPage].UpdateData();
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();

            m_SkillGUIs[m_CurrPage].Draw();
        }

        private void UpdateData()
        {
            if (!SkillEditorHelper.HasData())
            {
                return;
            }

            m_CurrName = SkillEditorHelper.currShowName;
            m_CurrType = SkillEditorHelper.currShowType;
            m_CurrId = SkillEditorHelper.currConfigData.Id;
        }

        private string GetTypeOrNameStr(string str)
        {
            return string.IsNullOrEmpty(str) ? null : str;
        }

        private int m_CurrPage = 0;
        private string m_AddType = string.Empty;
        private string m_AddName = string.Empty;
        private string m_CurrType = string.Empty;
        private string m_CurrName = string.Empty;
        private int m_CurrId = -1;
        private SkillGUI[] m_SkillGUIs = null;

        private string[] m_TabNames = new string[] { "SkillBaseConfig", "SkillPrevConditionConfig" };
    }
}