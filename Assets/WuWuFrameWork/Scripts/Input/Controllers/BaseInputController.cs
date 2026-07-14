
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework.Event;
using WuWuFramework.Utils;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using WuWuPlayerPrefs = WuWuFramework.Serialize.PlayerPrefs;

namespace WuWuFramework.Input
{
    public abstract class BaseInputController
    {
        private readonly Dictionary<string, BaseInputEvent> m_InputEvents = new();
        private RebindingOperation m_CurrRebindingOperation;
        private InputAction m_CurrRebindingInputAction;
        private int m_CurrRebindingIndex;
        private event WuWuFrameworkAction<InputAction, int> m_RebindingCompleteEvent;
        private event WuWuFrameworkAction m_RebindingCancelEvent;

        public InputActionMap actionMap { get; private set; }

        public bool isRebinding
        {
            get
            {
                return m_CurrRebindingOperation != null;
            }
        }

        public event WuWuFrameworkAction<InputAction, int> rebindingCompleteEvent
        {
            add
            {
                m_RebindingCompleteEvent += value;
            }
            remove
            {
                m_RebindingCompleteEvent -= value;
            }
        }

        public event WuWuFrameworkAction rebindingCancelEvent
        {
            add
            {
                m_RebindingCancelEvent += value;
            }
            remove
            {
                m_RebindingCancelEvent -= value;
            }
        }

        public abstract InputScheme inputScheme { get; }

        public void SetInputActionAsset(InputActionAsset inputActionAsset)
        {
            actionMap = inputActionAsset.FindActionMap(inputScheme.ToString());
            actionMap.Disable();
            int index = 0;

            while (index < actionMap.bindings.Count)
            {
                var binding = actionMap.bindings[index];

                if (binding == null || string.IsNullOrEmpty(binding.action))
                {
                    continue;
                }

                InputAction action = actionMap.FindAction(binding.action);

                if (action == null)
                {
                    continue;
                }

                if (action.bindings[0].isComposite)
                {
                    for (int i = 1; i < action.bindings.Count; i++)
                    {
                        string key = StringUtil.Append(this.GetType().Name, "_", (i + index).ToString());
                        string overridePath = WuWuPlayerPrefs.GetString(key);

                        if (!string.IsNullOrEmpty(overridePath))
                        {
                            action.ApplyBindingOverride(i, overridePath);
                        }
                    }

                    index += action.bindings.Count;
                }
                else
                {
                    string key = StringUtil.Append(this.GetType().Name, "_", index.ToString());
                    string overridePath = WuWuPlayerPrefs.GetString(key);

                    if (!string.IsNullOrEmpty(overridePath))
                    {
                        action.ApplyBindingOverride(0, overridePath);
                    }

                    index++;
                }
            }
        }

        public void CancelRebinding()
        {
            if (m_CurrRebindingOperation != null)
            {
                m_CurrRebindingOperation.Cancel();
                m_CurrRebindingOperation.Dispose();
                m_CurrRebindingOperation = null;
            }
        }

        public void SaveBindings()
        {
            for (int i = 0; i < actionMap.bindings.Count; i++)
            {
                var binding = actionMap.bindings[i];
                string key = StringUtil.Append(this.GetType().Name, "_", i.ToString());
                Debug.Log(key + " : " + binding.overridePath);
                WuWuPlayerPrefs.SetString(key, binding.overridePath);
            }
        }

        protected void ReBinding(string keyName)
        {
            m_CurrRebindingInputAction = actionMap.FindAction(keyName, true) ?? throw new WuWuFrameworkException("按键不存在");

            if (m_CurrRebindingInputAction.bindings[0].isComposite)
            {
                m_CurrRebindingIndex = 1;
            }
            else
            {
                m_CurrRebindingIndex = 0;
            }

            StartRebinding();
        }

        protected BaseInputEvent GetInputEvent(string keyName)
        {
            if (!CanAddInputEvent(keyName))
            {
                return null;
            }

            if (!m_InputEvents.TryGetValue(keyName, out BaseInputEvent inputEvent))
            {
                inputEvent = InputHelper.GetInputEvent(inputScheme, keyName);
                m_InputEvents.Add(keyName, inputEvent);
            }

            return inputEvent;
        }

        protected bool RemoveInputEvent(string keyName)
        {
            return m_InputEvents.Remove(keyName);
        }

        private bool CanAddInputEvent(string actionName)
        {
            if (actionMap == null)
            {
                throw new WuWuFrameworkException("输入方案不存在");
            }

            InputAction inputAction = actionMap.FindAction(actionName, true) ?? throw new WuWuFrameworkException("输入映射不存在");

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

            if (!m_InputEvents.TryGetValue(action.name, out BaseInputEvent inputEvent))
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

        private void StartRebinding()
        {
            if (m_CurrRebindingInputAction == null || m_CurrRebindingIndex < 0)
            {
                return;
            }

            m_CurrRebindingInputAction.Disable();
            m_CurrRebindingOperation = m_CurrRebindingInputAction.PerformInteractiveRebinding(m_CurrRebindingIndex);
            m_CurrRebindingOperation.OnComplete(OnRebindingComplete);
            m_CurrRebindingOperation.OnCancel(OnRebindingCancel);
            m_CurrRebindingOperation.Start();
        }

        private void OnRebindingComplete(RebindingOperation operation)
        {
            m_CurrRebindingInputAction.Enable();
            m_CurrRebindingOperation.Dispose();
            m_RebindingCompleteEvent?.Invoke(m_CurrRebindingInputAction, m_CurrRebindingIndex);
            m_CurrRebindingOperation = null;

            if (m_CurrRebindingIndex < m_CurrRebindingInputAction.bindings.Count - 1)
            {
                m_CurrRebindingIndex++;
                StartRebinding();
                return;
            }

            m_CurrRebindingInputAction = null;
        }

        private void OnRebindingCancel(RebindingOperation operation)
        {
            m_CurrRebindingInputAction.Enable();
            m_CurrRebindingOperation.Dispose();
            m_RebindingCancelEvent?.Invoke();
            m_CurrRebindingInputAction = null;
            m_CurrRebindingOperation = null;
        }
    }
}