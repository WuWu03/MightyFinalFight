using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework;
using WuWuFramework.Event;
using WuWuFramework.Resources;
using WuWuFramework.Utils;

public enum InputKey : byte
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
    RT,//RT
}

public enum InputScheme : byte
{
    None,
    KeyBoard,
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
    private static Dictionary<InputKey, WuWuFrameworkFunc<BaseInputEvent>> s_InputEventFactories = new()
    {
        [InputKey.LeftAxis] = GetInputEvent<Vector2InputEvent>,
        [InputKey.A] = GetInputEvent<FloatInputEvent>,
        [InputKey.LT] = GetInputEvent<FloatInputEvent>,
    };

    private static Dictionary<string, InputKey> s_InputEventKeysMap = new()
    {
        [InputKey.LeftAxis.ToString()] = InputKey.LeftAxis,
        [InputKey.A.ToString()] = InputKey.A,
        [InputKey.LT.ToString()] = InputKey.LT,
    };

    private static T GetInputEvent<T>() where T : BaseInputEvent, new()
    {
        return new T();
    }

    public static BaseInputEvent GetInputEvent(InputKey inputKey)
    {
        if (s_InputEventFactories.TryGetValue(inputKey, out var result))
        {
            return result.Invoke();
        }

        throw new WuWuFrameworkException(StringUtil.Append("[", inputKey.ToString(), "] 不存在对应的输入事件"));
    }

    public static InputKey GetInputKeyByKeyName(string inputKeyName)
    {
        if (s_InputEventKeysMap.TryGetValue(inputKeyName, out var result))
        {
            return result;
        }

        throw new WuWuFrameworkException(StringUtil.Append("不存在按键 [", inputKeyName, "]"));
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
        if(Gamepad.current == null)
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
}


public abstract class BaseInputEvent
{
    public abstract Type inputValueType { get; }
    public abstract void Add(InputEventCallType inputEventCallType, object action);
    public abstract void Remove(InputEventCallType inputEventCallType, object action);
    public virtual void Call(InputEventCallType inputEventCallType) { }
    public virtual void Call(InputEventCallType inputEventCallType, Vector2 inputValue) { }
    public virtual void Call(InputEventCallType inputEventCallType, float inputValue) { }
}

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



public class TestInputMgr
{
    public event WuWuFrameworkAction<InputScheme> inputDeviceChangeEvent
    {
        add { m_InputDeviceChangeEvent += value; }
        remove { m_InputDeviceChangeEvent -= value; }
    }

    public InputScheme currInputScheme
    {
        get
        {
            return m_CurrInputScheme;
        }
    }

    private event WuWuFrameworkAction<InputScheme> m_InputDeviceChangeEvent;
    private InputActionAsset m_InputActionAsset;
    private InputActionMap m_CurrActionMap;
    private InputScheme m_CurrInputScheme = InputScheme.None;
    private readonly Dictionary<InputKey, BaseInputEvent> m_InputEvents = new();
    private string m_SaveKey = string.Empty;
    private bool m_IsInit = false;
    private IResourcesMgr m_ResourcesMgr;
    private const string InputConfigDataName = "InputConfigData";

    public InputActionAsset inputActionAsset
    {
        get => m_InputActionAsset;
        set => m_InputActionAsset = value;
    }

    public void SetMgr(IResourcesMgr resourceMgr)
    {
        m_ResourcesMgr = resourceMgr;
    }

    public void InitInput(string saveKey = null, InputScheme inputScheme = InputScheme.KeyBoard)
    {
        m_SaveKey = saveKey;
        string jsonStr = string.IsNullOrEmpty(m_SaveKey) ? null : PlayerPrefs.GetString(saveKey, string.Empty);

        if (string.IsNullOrEmpty(jsonStr))
        {
            string configDataPath = WuWuFrameworkEntry.config.configDataPath;
            string filePath = PathUtil.FormatPath(configDataPath, InputConfigDataName);
            byte[] buffer = m_ResourcesMgr.Load<TextAsset>(filePath).bytes;
            jsonStr = System.Text.Encoding.UTF8.GetString(ZlibHelper.DeCompressBytes(buffer));
            m_ResourcesMgr.Unload(filePath);
        }

        m_InputActionAsset = InputActionAsset.FromJson(jsonStr);

        if (!m_IsInit)
        {
            m_IsInit = true;
            SetCurrScheme(inputScheme);
        }
    }

    public void SetCurrScheme(InputScheme inputScheme)
    {
        if (m_CurrInputScheme == inputScheme)
        {
            return;
        }

        if (m_InputActionAsset is null)
        {
            throw new WuWuFrameworkException("配置文件不存在");
        }

        m_CurrInputScheme = inputScheme;
        m_CurrActionMap = m_InputActionAsset.FindActionMap(inputScheme.ToString());
    }

    public void AddInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction inputCall)
    {
        GetInputEvent(inputKey).Add(inputEventCallType, inputCall);
    }

    public void AddInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<Vector2> inputCall)
    {
        GetInputEvent(inputKey).Add(inputEventCallType, inputCall);
    }

    public void AddInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<float> inputCall)
    {
        GetInputEvent(inputKey).Add(inputEventCallType, inputCall);
    }

    public void RemoveInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction inputCall)
    {
        GetInputEvent(inputKey).Remove(inputEventCallType, inputCall);
    }

    public void RemoveInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<Vector2> inputCall)
    {
        GetInputEvent(inputKey).Remove(inputEventCallType, inputCall);
    }

    public void RemoveInputEvent(InputKey inputKey, InputEventCallType inputEventCallType, WuWuFrameworkAction<float> inputCall)
    {
        GetInputEvent(inputKey).Remove(inputEventCallType, inputCall);
    }

    public bool RemoveInputEvent(InputKey inputKey)
    {
        return m_InputEvents.Remove(inputKey);
    }

    public void ReBindInput(InputKey inputKey)
    {
        string actionName = inputKey.ToString();
        InputAction inputAction = m_CurrActionMap.FindAction(actionName, true);

        if (!inputAction.enabled)
        {
            return;
        }

        Debug.Log("开始重新绑定");
        inputAction.Disable();
        Debug.Log(inputAction.bindings.Count);
        for (int i = 0; i < inputAction.bindings.Count; i++)
        {
            Debug.Log(inputAction.bindings[i]);
        }

        if (inputAction.bindings[0].isComposite)
        {
            for (int i = 1; i < inputAction.bindings.Count; i++)
            {
                Debug.Log(inputAction.bindings[i].isComposite);
            }
        }
    }

    public void Update()
    {
        bool isDeviceChanged = false;

        if (InputHelper.IsKeyBoardInput() && m_CurrInputScheme != InputScheme.KeyBoard)
        {
            isDeviceChanged = true;
            SetCurrScheme(InputScheme.KeyBoard);
        }
        else if (InputHelper.IsXboxInput() && m_CurrInputScheme != InputScheme.Xbox)
        {
            isDeviceChanged = true;
            SetCurrScheme(InputScheme.Xbox);
        }

        if (isDeviceChanged)
        {
            foreach (KeyValuePair<InputKey, BaseInputEvent> kvp in m_InputEvents)
            {
                string actionName = kvp.Key.ToString();
                CanAddInputEvent(actionName);
            }

            m_InputDeviceChangeEvent?.Invoke(m_CurrInputScheme);
        }
    }

    private BaseInputEvent GetInputEvent(InputKey inputKey)
    {
        string actionName = inputKey.ToString();

        if (!CanAddInputEvent(actionName))
        {
            return null;
        }

        if (!m_InputEvents.TryGetValue(inputKey, out BaseInputEvent inputEvent))
        {
            inputEvent = InputHelper.GetInputEvent(inputKey);
            m_InputEvents.Add(inputKey, inputEvent);
        }

        return inputEvent;
    }

    private bool CanAddInputEvent(string actionName)
    {
        if (m_CurrActionMap == null)
        {
            throw new WuWuFrameworkException("输入方案不存在");
        }

        InputAction inputAction = m_CurrActionMap.FindAction(actionName, true);

        if (inputAction == null)
        {
            throw new WuWuFrameworkException("输入映射不存在");
        }

        if (!inputAction.enabled)
        {
            inputAction.Enable();
        }

        inputAction.started -= OnInputStarted;
        inputAction.performed -= OnInputPerformed;
        inputAction.canceled -= OnInputCanceled;
        inputAction.started += OnInputStarted;
        inputAction.performed += OnInputPerformed;
        inputAction.canceled += OnInputCanceled;
        return true;
    }

    private void OnInputStarted(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventCallType.Started);
    }

    private void OnInputPerformed(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventCallType.Performed);
    }

    private void OnInputCanceled(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventCallType.Canceled);
    }

    private void InvokeInputEvent(InputAction action, InputEventCallType inputEventCallType)
    {
        if (action == null)
        {
            throw new WuWuFrameworkException("输入映射不存在");
        }

        InputKey inputKey = InputHelper.GetInputKeyByKeyName(action.name);

        if (!m_InputEvents.TryGetValue(inputKey, out BaseInputEvent inputEvent))
        {
            throw new WuWuFrameworkException("输入事件不存在");
        }

        if (inputEvent.inputValueType == null)
        {
            inputEvent.Call(inputEventCallType);
        }
        else if (inputEvent.inputValueType == typeof(Vector2))
        {
            inputEvent.Call(inputEventCallType, action.ReadValue<Vector2>());
        }
        else if (inputEvent.inputValueType == typeof(float))
        {
            inputEvent.Call(inputEventCallType, action.ReadValue<float>());
        }
    }
}