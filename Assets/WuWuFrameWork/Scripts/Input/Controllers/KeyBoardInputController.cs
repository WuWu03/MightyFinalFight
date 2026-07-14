using System;
using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Input
{
    public enum KeyboardInputKey : byte
    {
        /// <summary>
        /// 左摇杆，触发值类型 Vector2
        /// </summary>
        LeftAxis,
        /// <summary>
        /// 右摇杆，触发值类型 Vector2
        /// </summary>
        RightAxis,
        /// <summary>
        /// 十字键，触发值类型 Vector2
        /// </summary>
        DPad,
        /// <summary>
        /// A，无触发值类型
        /// </summary>
        A,
        /// <summary>
        /// B，无触发值类型
        /// </summary>
        B,
        /// <summary>
        /// X，无触发值类型
        /// </summary>
        X,
        /// <summary>
        /// Y，无触发值类型
        /// </summary>
        Y,
        /// <summary>
        /// Start，无触发值类型
        /// </summary>
        Start,
        /// <summary>
        /// Select，无触发值类型
        /// </summary>
        Select,
        /// <summary>
        /// LB，无触发值类型
        /// </summary>
        LB,
        /// <summary>
        /// RB，无触发值类型
        /// </summary>
        RB,
        /// <summary>
        /// LT，触发值类型 float
        /// </summary>
        LT,
        /// <summary>
        /// RT，触发值类型 float
        /// </summary>
        RT,
    }

    public class KeyboardInputController : BaseInputController
    {
        public override InputScheme inputScheme => InputScheme.Keyboard;

        public void AddInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction inputCall)
        {
            GetInputEvent(inputKey.ToString()).Add(inputEventCallType, inputCall);
        }

        public void AddInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<Vector2> inputCall)
        {
            GetInputEvent(inputKey.ToString()).Add(inputEventCallType, inputCall);
        }

        public void AddInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<float> inputCall)
        {
            GetInputEvent(inputKey.ToString()).Add(inputEventCallType, inputCall);
        }

        public void RemoveInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction inputCall)
        {
            GetInputEvent(inputKey.ToString()).Remove(inputEventCallType, inputCall);
        }

        public void RemoveInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<Vector2> inputCall)
        {
            GetInputEvent(inputKey.ToString()).Remove(inputEventCallType, inputCall);
        }

        public void RemoveInputEvent(KeyboardInputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<float> inputCall)
        {
            GetInputEvent(inputKey.ToString()).Remove(inputEventCallType, inputCall);
        }

        public bool RemoveInputEvent(KeyboardInputKey inputKey)
        {
            return RemoveInputEvent(inputKey.ToString());
        }

        public void Rebinding(KeyboardInputKey inputKey)
        {
            ReBinding(inputKey.ToString());
        }
    }
}