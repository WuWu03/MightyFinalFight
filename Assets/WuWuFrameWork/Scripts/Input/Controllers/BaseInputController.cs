using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework.Event;
using WuWuFramework.Utils;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using WuWuPlayerPrefs = WuWuFramework.Serialize.PlayerPrefs;

namespace WuWuFramework.Input
{
    public abstract class BaseInputController : IDisposable
    {
        private readonly Dictionary<string, BaseInputEvent> m_InputEvents = new();
        private RebindingOperation m_CurrRebindingOperation;
        private InputAction m_CurrRebindingInputAction;
        private int m_CurrRebindingIndex;
        private event WuWuFrameworkAction<InputAction, int> m_RebindingCompleteEvent;
        private event WuWuFrameworkAction m_RebindingCancelEvent;
        private InputActionMap m_ActionMap;

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
            m_ActionMap = inputActionAsset.FindActionMap(inputScheme.ToString());
            m_ActionMap.Disable();
            int index = 0;

            while (index < m_ActionMap.bindings.Count)
            {
                var binding = m_ActionMap.bindings[index];

                if (binding == null || string.IsNullOrEmpty(binding.action))
                {
                    continue;
                }

                InputAction action = m_ActionMap.FindAction(binding.action);

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
            for (int i = 0; i < m_ActionMap.bindings.Count; i++)
            {
                var binding = m_ActionMap.bindings[i];
                string key = StringUtil.Append(this.GetType().Name, "_", i.ToString());
                Debug.Log(key + " : " + binding.overridePath);
                WuWuPlayerPrefs.SetString(key, binding.overridePath);
            }
        }

        public void Enable()
        {
            m_ActionMap?.Enable();
        }

        public void Disable()
        {
            m_ActionMap.Disable();
        }

        public void RemoveAllInputEvents()
        {
            foreach (KeyValuePair<string, BaseInputEvent> keyValuePair in m_InputEvents)
            {
                keyValuePair.Value.RemoveAll();
            }

            m_InputEvents.Clear();
        }

        public void Dispose()
        {
            RemoveAllInputEvents();
            m_CurrRebindingOperation?.Dispose();
            m_CurrRebindingOperation = null;
            m_CurrRebindingInputAction = null;
            m_RebindingCompleteEvent = null;
            m_RebindingCancelEvent = null;
            m_ActionMap = null;
        }

        protected void ReBinding(string keyName)
        {
            m_CurrRebindingInputAction = m_ActionMap.FindAction(keyName, true) ?? throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] [", keyName, "] 输入映射不存在"));

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

        protected InputAction GetInputAction(string keyName)
        {
            if (m_ActionMap == null)
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 输入方案不存在"));
            }

            return m_ActionMap?.FindAction(keyName);
        }

        protected InputBinding GetInputBinding(string keyName, int bindingIndex)
        {
            if (m_ActionMap == null)
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 输入方案不存在"));
            }

            InputAction inputAction = m_ActionMap.FindAction(keyName) ?? throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] [", keyName, "] 输入映射不存在"));
            return inputAction.bindings[bindingIndex];
        }

        protected bool RemoveInputEvent(string keyName)
        {
            return m_InputEvents.Remove(keyName);
        }

        private bool CanAddInputEvent(string keyName)
        {
            if (m_ActionMap == null)
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 输入方案不存在"));
            }

            InputAction inputAction = m_ActionMap.FindAction(keyName, true) ?? throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] [", keyName, "] 输入映射不存在"));

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
            m_CurrRebindingInputAction?.Enable();
            m_CurrRebindingOperation?.Dispose();
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
            m_CurrRebindingInputAction?.Enable();
            m_CurrRebindingOperation?.Dispose();
            m_RebindingCancelEvent?.Invoke();
            m_CurrRebindingInputAction = null;
            m_CurrRebindingOperation = null;
        }
    }
}