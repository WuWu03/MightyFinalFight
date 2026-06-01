using System;
using System.Collections.Generic;
using WuWuFramework;
using WuWuFramework.Resources;
using WuWuFramework.Event;
using WuWuFramework.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputEventType : byte
{
    Started,
    Performed,
    Cancelled,
}

public class InputEvent : WuWuFrameworkEventArg
{
    private object m_Event;

    public Type inputValueType { get; private set; }

    public static InputEvent Create(Type valueType)
    {
        InputEvent inputEvent = ReferencePool.Acquire<InputEvent>();
        inputEvent.inputValueType = valueType;
        return inputEvent;
    }

    public void Add(WuWuFrameworkAction<InputEventType> inputEvent)
    {
        if (m_Event is WuWuFrameworkAction<InputEventType> events)
        {
            events += inputEvent;
        }
    }

    public void Add<InputValueType>(WuWuFrameworkAction<InputValueType, InputEventType> inputEvent) where InputValueType : struct
    {
        if (m_Event is WuWuFrameworkAction<InputValueType, InputEventType> events)
        {
            events += inputEvent;
        }
    }

    public void Call(InputEventType inputEventType)
    {
        if (m_Event is WuWuFrameworkAction<InputEventType> inputCallback)
        {
            inputCallback.Invoke(inputEventType);
        }
    }

    public void Call<InputValueType>(InputValueType value, InputEventType inputEventType) where InputValueType : struct
    {
        if (m_Event is WuWuFrameworkAction<InputValueType, InputEventType> inputCallback)
        {
            inputCallback.Invoke(value, inputEventType);
        }
    }


    public override void Clear()
    {
        m_Event = null;
        inputValueType = null;
    }
}

public class TestInputMgr
{
    public event WuWuFrameworkAction<InputDevice, InputDeviceChange> inputDeviceChangeEvent
    {
        add { m_InputDeviceChangeEvent += value; }
        remove { m_InputDeviceChangeEvent -= value; }
    }

    private event WuWuFrameworkAction<InputDevice, InputDeviceChange> m_InputDeviceChangeEvent;
    private InputActionAsset m_InputActionAsset;
    private InputActionMap m_CurrActionMap;
    private readonly Dictionary<string, InputEvent> m_InputEvens = new();
    private string m_SaveKey = string.Empty;

    public InputActionAsset inputActionAsset
    {
        get => m_InputActionAsset;
        set => m_InputActionAsset = value;
    }

    public void Init(IResourcesMgr resourceMgr, string configDataName, string saveKey)
    {
        m_SaveKey = saveKey;
        string jsonStr = PlayerPrefs.GetString(saveKey, string.Empty);

        if (string.IsNullOrEmpty(jsonStr))
        {
            string configDataPath = WuWuFrameworkEntry.config.configDataPath;
            string filePath = PathUtil.FormatPath(configDataPath, configDataName);
            byte[] buffer = resourceMgr.Load<TextAsset>(filePath).bytes;
            jsonStr = System.Text.Encoding.UTF8.GetString(buffer);
        }

        m_InputActionAsset = InputActionAsset.FromJson(jsonStr);
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        m_InputDeviceChangeEvent?.Invoke(inputDevice, inputDeviceChange);
    }

    public void SetCurrScheme(string schemeName)
    {
        if (m_InputActionAsset is null)
        {
            throw new WuWuFrameworkException("配置文件不存在");
        }

        m_CurrActionMap = m_InputActionAsset.FindActionMap(schemeName);
    }

    public void AddInputEvent(string actionName, WuWuFrameworkAction<InputEventType> inputCall)
    {
        //this.AddInputEvent(actionName, inputCall, null);
    }

    public void AddInputEvent<InputValueType>(string actionName, WuWuFrameworkAction<InputValueType, InputEventType> inputCall) where InputValueType : struct
    {
        if (!CanAddInputEvent(actionName))
        {
            return;
        }

        if (!m_InputEvens.TryGetValue(actionName, out InputEvent inputEvent))
        {
            // m_InputEvens.Add(actionName, InputEvent.Create(inputCall, typeof(InputEventType)));
        }
        else
        {
            //if (inputEvent.)
        }
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
            inputAction.started += OnInputStarted;
            inputAction.performed += OnInputPerformed;
            inputAction.canceled += OnInputCanceled;
        }

        return true;
    }

    private void OnInputStarted(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventType.Started);
    }

    private void OnInputPerformed(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventType.Performed);
    }

    private void OnInputCanceled(InputAction.CallbackContext obj)
    {
        InvokeInputEvent(obj.action, InputEventType.Cancelled);
    }

    private void InvokeInputEvent(InputAction action, InputEventType inputEventType)
    {
        if (action == null)
        {
            throw new WuWuFrameworkException("输入映射不存在");
        }

        if (!m_InputEvens.TryGetValue(action.name, out InputEvent inputEvent))
        {
            throw new WuWuFrameworkException("输入事件不存在");
        }

        if (inputEvent.inputValueType == null)
        {
            inputEvent.Call(inputEventType);
        }
        else if (inputEvent.inputValueType == typeof(Vector2))
        {
            inputEvent.Call(action.ReadValue<Vector2>(), inputEventType);
        }
        else if (inputEvent.inputValueType == typeof(Vector3))
        {
            inputEvent.Call(action.ReadValue<Vector3>(), inputEventType);
        }
        else if (inputEvent.inputValueType == typeof(float))
        {
            inputEvent.Call(action.ReadValue<float>(), inputEventType);
        }
    }

    // private AddInputEvent(string actionName, object inputCall, Type valueType)
    // {
    // }
}