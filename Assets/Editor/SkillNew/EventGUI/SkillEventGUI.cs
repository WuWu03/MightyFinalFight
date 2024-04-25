using UnityEditor;
namespace SkillNew
{
    public abstract class SkillEventGUI : SkillGUI
    {
        public SkillEventGUI(EditorWindow window) : base(window)
        {

        }

        public void UpdateSkillEvent(SkillEditorConfigData.SkillEvent skillEvent)
        {
            m_CurrEvent = skillEvent;
            OnUpdateSkillEvent();
        }

        protected abstract void OnUpdateSkillEvent();

        protected SkillEditorConfigData.SkillEvent m_CurrEvent = null;
    }
}