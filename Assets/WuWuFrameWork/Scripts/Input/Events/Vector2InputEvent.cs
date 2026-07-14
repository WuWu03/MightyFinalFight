
using System;
using UnityEngine;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework.Input
{
    public class Vector2InputEvent : BaseInputEvent
    {
        private WuWuFrameworkAction<Vector2> m_InputStartedEvent;
        private WuWuFrameworkAction<Vector2> m_InputPerformedEvent;
        private WuWuFrameworkAction<Vector2> m_InputCanceledEvent;

        public override Type inputValueType => typeof(Vector2);

        public override void Add(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction<Vector2> tempAction)
            {
                switch (inputEventCallType)
                {
                    case InputEventCallType.Started:
                        m_InputStartedEvent += tempAction;
                        break;
                    case InputEventCallType.Performed:
                        m_InputPerformedEvent += tempAction;
                        break;
                    case InputEventCallType.Canceled:
                        m_InputCanceledEvent += tempAction;
                        break;
                }

                return;
            }

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction<Vector2>]"));
        }

        public override void Remove(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction<Vector2> tempAction)
            {
                switch (inputEventCallType)
                {
                    case InputEventCallType.Started:
                        m_InputStartedEvent -= tempAction;
                        break;
                    case InputEventCallType.Performed:
                        m_InputPerformedEvent -= tempAction;
                        break;
                    case InputEventCallType.Canceled:
                        m_InputCanceledEvent -= tempAction;
                        break;
                }
                return;
            }

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction<Vector2>]"));
        }

        public override void RemoveAll()
        {
            m_InputStartedEvent = null;
            m_InputPerformedEvent = null;
            m_InputCanceledEvent = null;
        }

        public override void Call(InputEventCallType inputEventCallType, Vector2 inputValue)
        {
            switch (inputEventCallType)
            {
                case InputEventCallType.Started:
                    m_InputStartedEvent?.Invoke(inputValue);
                    break;
                case InputEventCallType.Performed:
                    m_InputPerformedEvent?.Invoke(inputValue);
                    break;
                case InputEventCallType.Canceled:
                    m_InputCanceledEvent?.Invoke(inputValue);
                    break;
            }
        }
    }
}