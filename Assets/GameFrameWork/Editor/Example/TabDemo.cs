using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TabDemo : EditorWindow
{
    Vector2 m_scrollPosition = new Vector2(0, 0);

    List<UnityEngine.Object> m_Icons;

    TextEditor m_te = new TextEditor();

    enum ETAB
    {
        State_Config,      //状态配置
        GUI_Style,         //系统GUI
        Unity_Icon,        //系统ICON
    }
    static string[] TAB = new string[] { "状态配置", "系统GUI", "系统ICON" };
    GUIContent[] GUIArr;

    ETAB m_Tab = ETAB.State_Config;
    static private Vector2 m_winSize = new Vector2(800, 750);
    GUIStyle m_tabBtnStyle;
    GUIStyle m_tabBtnOnStyle;
    GUIStyle m_textStyle;
    GUIStyle m_labStyle;
    GUIStyle m_tabTextSytel;
    GUIStyle m_tabStyle;
    GUIStyle m_iconNameStyle;
    GUIStyle m_saveBtnStyle;
    GUIStyle m_toggleStyle;
    GUIStyle m_titleStyle;

    //------------------------------------------------------
    private void OnGUI()
    {
        InitGUIStyle();
        InitIconTabs();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        //for (int i = 0; i < TAB.Length; ++i)
        for (int i = 0; i < GUIArr.Length; ++i)
        {
            GUILayout.Space(3);

            if (i == (int)m_Tab)
                m_tabStyle = m_tabBtnOnStyle;
            else
                m_tabStyle = m_tabBtnStyle;

            //if (GUILayout.Button(TAB[i], m_tabStyle, GUILayout.Width(90)))
            if (GUILayout.Button(GUIArr[i], m_tabStyle, GUILayout.Width(90)))
            {
                m_Tab = (ETAB)i;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        switch (m_Tab)
        {
            case ETAB.State_Config:
                StateConfig();
                break;
            case ETAB.GUI_Style:
                SystemGUI();
                break;
            case ETAB.Unity_Icon:
                SystemIcon();
                break;
        }
    }

    private void StateConfig()
    {
        GUILayout.BeginVertical("PopupCurveSwatchBackground");
        GUILayout.Space(5);
        GUILayout.Label("青玉案·元夕", m_textStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("【作者】辛弃疾 【朝代】宋", m_titleStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("东风夜放花千树", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("更吹落、星如雨", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("宝马雕车香满路", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("凤箫声动，玉壶光转，一夜鱼龙舞", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("蛾儿雪柳黄金缕", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("笑语盈盈暗香去", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("众里寻他千百度", m_labStyle, GUILayout.Width(590));
        GUILayout.Space(5);
        GUILayout.Label("蓦然回首，那人却在，灯火阑珊处", m_labStyle, GUILayout.Width(590));
        GUILayout.EndVertical();
    }
    private void SystemGUI()
    {
        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);

        foreach (var style in GUI.skin.customStyles)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
            if (GUILayout.Button(style.name, style, GUILayout.Width(280)))
            {
                EditorGUIUtility.systemCopyBuffer = style.name;
                Debug.LogError(style.name);
            }
            GUILayout.Space(20);
            EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
    private void SystemIcon()
    {
        InitIconList();

        GUILayout.BeginHorizontal("HelpBox");
        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
        if (m_Icons != null)
        {
            for (int i = 0; i < m_Icons.Count; i++)
            {
                if (i % 5 == 0)
                {
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    for (int j = 0; j < 5; j++)
                    {
                        if (i + j >= m_Icons.Count)
                            break;
                        var icon = m_Icons[i + j];
                        GUILayout.BeginVertical();
                        if (GUILayout.Button((Texture)icon, GUILayout.Width(100), GUILayout.Height(100)))
                        {
                            m_te.text = icon.name;
                            m_te.SelectAll();
                            m_te.Copy();
                            this.ShowNotification(new GUIContent("Unity 体统不表：" + icon.name + "已经复制到剪切板！"));
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Label((i + j).ToString(), GUILayout.Width(20));
                        GUILayout.Label(icon.name, m_iconNameStyle, GUILayout.Width(80));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);
                }
            }
        }
        GUILayout.EndScrollView();
    }
    private void InitIconList()
    {
        if (m_Icons == null || m_Icons.Count == 0)
        {
            m_Icons = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(Texture)));
            m_Icons.Sort((pA, pB) => System.String.Compare(pA.name, pB.name, System.StringComparison.OrdinalIgnoreCase));
        }
    }


    //------------------------------------------------------
    private void InitIconTabs()
    {
        GUIArr = new GUIContent[]{  EditorGUIUtility.IconContent("d_BuildSettings.Switch") ,
                                    EditorGUIUtility.IconContent("d_BuildSettings.PS4") ,
                                    EditorGUIUtility.IconContent("d_BuildSettings.XboxOne") };
    }
    //------------------------------------------------------
    private void InitGUIStyle()
    {
        if (m_tabTextSytel == null)
        {
            m_tabTextSytel = new GUIStyle("BoldLabel");
        }
        if (m_tabBtnStyle == null)
        {
            m_tabBtnStyle = new GUIStyle("flow node 0");
            m_tabBtnStyle.alignment = TextAnchor.MiddleCenter;
            m_tabBtnStyle.fontStyle = m_tabTextSytel.fontStyle;
            m_tabBtnStyle.fontSize = 18;
            //color = m_tabBtnStyle.normal.textColor;
        }
        if (m_tabBtnOnStyle == null)
        {
            m_tabBtnOnStyle = new GUIStyle("flow node 1 on");
            m_tabBtnOnStyle.alignment = TextAnchor.MiddleCenter;
            m_tabBtnOnStyle.fontStyle = m_tabTextSytel.fontStyle;
            m_tabBtnOnStyle.fontSize = 18;
        }

        if (m_textStyle == null)
        {
            m_textStyle = new GUIStyle("HeaderLabel");
            m_textStyle.fontSize = 20;
            m_textStyle.alignment = TextAnchor.MiddleCenter;
        }
        if (m_labStyle == null)
        {
            m_labStyle = new GUIStyle("CenteredLabel");
            m_labStyle.fontSize = 18;
        }
        if (m_titleStyle == null)
        {
            m_titleStyle = new GUIStyle("AM VuValue");
            m_titleStyle.alignment = TextAnchor.MiddleRight;
            m_titleStyle.fontSize = 15;
        }
        if (m_iconNameStyle == null)
        {
            m_iconNameStyle = new GUIStyle("WarningOverlay");
            m_iconNameStyle.fontSize = 12;
        }
        if (m_toggleStyle == null)
        {
            m_toggleStyle = new GUIStyle("OL ToggleWhite");
        }
        if (m_saveBtnStyle == null)
        {
            m_saveBtnStyle = new GUIStyle("flow node 1");
            m_saveBtnStyle.fontSize = 20;
            m_saveBtnStyle.fixedHeight = 40;
            m_saveBtnStyle.alignment = TextAnchor.MiddleCenter;
        }
    }
}