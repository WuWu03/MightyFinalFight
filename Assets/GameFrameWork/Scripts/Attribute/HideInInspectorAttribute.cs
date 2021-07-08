using UnityEngine;

namespace GameFrameWork
{
    public class HideInInspectorExAttribute : PropertyAttribute
    {
        public bool Condition = false;
        public HideInInspectorExAttribute(bool condition)
        {
            Condition = condition;
        }
    }
}
