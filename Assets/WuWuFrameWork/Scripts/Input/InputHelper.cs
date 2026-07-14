using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework.Input
{
    public enum InputScheme : byte
    {
        None,
        Keyboard,
        Xbox
    }

    public enum InputEventCallType : byte
    {
        Started,
        Performed,
        Canceled
    }

    public static class InputHelper
    {
        private static readonly Dictionary<InputScheme, Dictionary<string, WuWuFrameworkFunc<BaseInputEvent>>> s_InputEventFactories = new()
        {
            [InputScheme.Xbox] = new()
            {
                [XboxInputKey.LeftAxis.ToString()] = GetInputEvent<Vector2InputEvent>,
                [XboxInputKey.RightAxis.ToString()] = GetInputEvent<Vector2InputEvent>,
                [XboxInputKey.DPad.ToString()] = GetInputEvent<Vector2InputEvent>,
                [XboxInputKey.A.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.B.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.X.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.Y.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.Start.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.Select.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.LB.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.RB.ToString()] = GetInputEvent<VoidInputEvent>,
                [XboxInputKey.LT.ToString()] = GetInputEvent<FloatInputEvent>,
                [XboxInputKey.RT.ToString()] = GetInputEvent<FloatInputEvent>,
            },

            [InputScheme.Keyboard] = new()
            {
                [KeyboardInputKey.LeftAxis.ToString()] = GetInputEvent<Vector2InputEvent>,
                [KeyboardInputKey.RightAxis.ToString()] = GetInputEvent<Vector2InputEvent>,
                [KeyboardInputKey.DPad.ToString()] = GetInputEvent<Vector2InputEvent>,
                [KeyboardInputKey.A.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.B.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.X.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.Y.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.Start.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.Select.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.LB.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.RB.ToString()] = GetInputEvent<VoidInputEvent>,
                [KeyboardInputKey.LT.ToString()] = GetInputEvent<FloatInputEvent>,
                [KeyboardInputKey.RT.ToString()] = GetInputEvent<FloatInputEvent>,
            }
        };

        private static readonly Dictionary<InputScheme, WuWuFrameworkFunc<InputActionAsset, BaseInputController>> s_InputContollerFactories = new()
        {
            [InputScheme.Xbox] = GetInputController<XboxInputController>,
            [InputScheme.Keyboard] = GetInputController<KeyboardInputController>,
        };

        public static BaseInputEvent GetInputEvent(InputScheme inputScheme, string keyName)
        {
            if (s_InputEventFactories.TryGetValue(inputScheme, out var factory))
            {
                if (factory.TryGetValue(keyName, out var builder))
                {
                    return builder.Invoke();
                }

                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 平台 [", keyName.ToString(), "] 不存在对应的输入事件"));
            }

            throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 不存在对应平台"));
        }

        public static BaseInputController GetInputController(InputScheme inputScheme, InputActionAsset inputActionAsset)
        {
            if (s_InputContollerFactories.TryGetValue(inputScheme, out var builder))
            {
                return builder.Invoke(inputActionAsset);
            }

            throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 不存在对应平台"));
        }

        public static bool IsKeyBoardInput()
        {
            if (Keyboard.current == null)
            {
                return false;
            }

            bool isKeyBoardInput = Keyboard.current.anyKey.isPressed;
            bool isMouseLeftButtonInput = Mouse.current.leftButton.isPressed;
            bool isMouseRightButtonInput = Mouse.current.rightButton.isPressed;
            bool isMouseMiddleButtonInput = Mouse.current.middleButton.isPressed;
            bool isMouseScrollInput = Mouse.current.scroll.ReadValue() != Vector2.zero;
            bool isMouseDeltaInput = Mouse.current.delta.ReadValue() != Vector2.zero;
            return isKeyBoardInput || isMouseLeftButtonInput || isMouseRightButtonInput || isMouseMiddleButtonInput || isMouseScrollInput || isMouseDeltaInput;
        }

        public static bool IsXboxInput()
        {
            if (Gamepad.current == null)
            {
                return false;
            }

            bool isActuated = Gamepad.current.IsActuated();
            bool isXbox = Gamepad.current.description.interfaceName == "XInput" || Gamepad.current.description.interfaceName == "XInputControllerWindows";
            return isActuated && isXbox;
        }

        public static bool IsPSInput()
        {
            if (Gamepad.current == null)
            {
                return false;
            }

            bool isActuated = Gamepad.current.IsActuated();
            bool isPS = Gamepad.current.description.interfaceName == "DualShock" || Gamepad.current.description.interfaceName == "DualSense";
            return isActuated && isPS;
        }

        public static bool IsSwitchInput()
        {
            if (Gamepad.current == null)
            {
                return false;
            }

            bool isActuated = Gamepad.current.IsActuated();
            bool isSwitch = Gamepad.current.description.interfaceName == "Nintendo Switch";
            return isActuated && isSwitch;
        }

        private static T GetInputEvent<T>() where T : BaseInputEvent, new()
        {
            return new T();
        }

        private static T GetInputController<T>(InputActionAsset inputActionAsset) where T : BaseInputController, new()
        {
            T result = new();
            result.SetInputActionAsset(inputActionAsset);
            return result;
        }
    }
}