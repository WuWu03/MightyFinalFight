using UnityEditor;
namespace SkillNew
{
    public abstract class SkillEventGUI : SkillBaseGUI
    {
        private SkillEditorConfigData.SkillEvent m_CurrEvent;
        public SkillEventGUI(EditorWindow window) : base(window)
        {
        }
        
        public SkillEditorConfigData.SkillEvent currEvent
        {
            get
            {
                return m_CurrEvent;
            }
        }

        public override void Draw()
        {
            if(m_CurrEvent != null)
            {
                OnGUI();
            }
        }

        public void UpdateSkillEvent(SkillEditorConfigData.SkillEvent skillEvent)
        {
            m_CurrEvent = skillEvent;
            OnUpdateSkillEvent();
        }

        public void ResetEvent()
        {
            if(m_CurrEvent != null)
            {
                OnResetEvent();
                m_CurrEvent = null;
            }
        }

        protected virtual void OnResetEvent()
        {
        }

        protected abstract void OnUpdateSkillEvent();
    }
}