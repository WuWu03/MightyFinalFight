
using System;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework.Input
{
    public class VoidInputEvent : BaseInputEvent
    {
        private event WuWuFrameworkAction m_InputStartedEvent;
        private event WuWuFrameworkAction m_InputPerformedEvent;
        private event WuWuFrameworkAction m_InputCanceledEvent;

        public override Type inputValueType => null;

        public override void Add(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction tempAction)
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

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction]"));
        }

        public override void Remove(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction tempAction)
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

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction]"));
        }

        public override void Call(InputEventCallType inputEventCallType)
        {
            switch (inputEventCallType)
            {
                case InputEventCallType.Started:
                    m_InputStartedEvent?.Invoke();
                    break;
                case InputEventCallType.Performed:
                    m_InputPerformedEvent?.Invoke();
                    break;
                case InputEventCallType.Canceled:
                    m_InputCanceledEvent?.Invoke();
                    break;
            }
        }
    }
}