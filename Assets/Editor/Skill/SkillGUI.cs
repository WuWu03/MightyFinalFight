using UnityEditor;
using UnityEngine;

public class SkillGUI
{
    public SkillGUI(EditorWindow window)
    {
        m_EditorWindow = window;
    }

    public void UpdateData()
    {
        OnUpdateData();
    }

    public void Draw()
    {
        OnGUI();
    }

    protected virtual void OnUpdateData() { }
    protected virtual void OnGUI() { }

    protected void ShowNotification(string content)
    {
        m_EditorWindow.ShowNotification(new GUIContent(content));
    }

    private EditorWindow m_EditorWindow = null;
}
