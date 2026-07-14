using System;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework.Input
{
    public class FloatInputEvent : BaseInputEvent
    {
        private event WuWuFrameworkAction<float> m_InputStartedEvent;
        private event WuWuFrameworkAction<float> m_InputPerformedEvent;
        private event WuWuFrameworkAction<float> m_InputCanceledEvent;

        public override Type inputValueType => typeof(float);

        public override void Add(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction<float> tempAction)
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

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction<float>]"));
        }

        public override void Remove(InputEventCallType inputEventCallType, object action)
        {
            if (action is WuWuFrameworkAction<float> tempAction)
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

            throw new WuWuFrameworkException(StringUtil.Append("[", this.GetType().Name, "] 事件类型错误，必须是 [WuWuFrameworkAction<float>]"));
        }

        public override void Call(InputEventCallType inputEventCallType, float inputValue)
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