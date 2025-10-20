using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class InputMgr : GameFrameWorkModule , IInputMgr
    {
        private event GameFrameWorkFloatAction m_GetDirectionEvent;
        private event GameFrameWorkBooleanAction<int> m_GetPreConditionEvent;
        private event GameFrameWorkAction m_InputDeviceChangeEvent;
        private readonly Dictionary<string, bool> m_DicIsKeyDown;
        private readonly List<KeyType> m_ComboKeys;
        private readonly Queue<string> m_QueueKeyDown;
        private readonly List<ComboKeyEventArgs> m_ComboKeyEvents;
        private readonly Dictionary<KeyType, List<GameFrameWorkAction>> m_DicAfterTriggerEvents;
        
        private float m_CurrDir;
        private float m_KeyDownTimer = -1f;
        private bool m_IsRunning;
        private bool m_IsJoystickInput;
        private int m_CurrKeyDown = -1;
        private int m_AxisDownIndex = -1;//0 horizontal 1 vertical
        private AxisType m_AxisDownType = AxisType.None;
        private const float KeyDownTime = 0.05f;
        
        public InputMgr()
        {
            m_DicIsKeyDown = new();
            m_ComboKeys = new();
            m_QueueKeyDown = new();
            m_ComboKeyEvents = new();
            m_DicAfterTriggerEvents = new();
            InputHelper.Init();
        }
        
        public event GameFrameWorkFloatAction getDirectionEvent
        {
            add
            {
                m_GetDirectionEvent += value;
            }
            remove
            {
                m_GetDirectionEvent -= value;
            }
        }

        public event GameFrameWorkBooleanAction<int> getPreConditonEvent
        {
            add
            {
                m_GetPreConditionEvent += value;
            }
            remove
            {
                m_GetPreConditionEvent -= value;
            }
        }

        public event GameFrameWorkAction inputDeviceChangeEvent
        {
            add
            {
                m_InputDeviceChangeEvent += value;
            }
            remove
            {
                m_InputDeviceChangeEvent -= value;
            }
        }

        public bool isJoystickInput
        {
            get
            {
                return m_IsJoystickInput;
            }
        }

        public bool isRunning
        {
            get
            {
                return m_IsRunning;
            }
            set
            {
                m_IsRunning = value;
            }
        }

        public override void Shutdown()
        {
            InputHelper.Dispose();
            m_DicIsKeyDown.Clear();
            m_ComboKeys.Clear();
            m_QueueKeyDown.Clear();
            m_ComboKeyEvents.Clear();
            m_DicAfterTriggerEvents.Clear();
            m_IsRunning = false;
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            string[] joystickNames = UnityEngine.Input.GetJoystickNames();
            bool isConnected = joystickNames.Length > 0 && !string.IsNullOrEmpty(joystickNames[0]);

            if (!isConnected || (m_IsJoystickInput && !AnyJoystickInput() && UnityEngine.Input.anyKeyDown))
            {
                m_IsJoystickInput = false;
                m_InputDeviceChangeEvent?.Invoke();
            }
            else if (!m_IsJoystickInput && AnyJoystickInput())
            {
                m_IsJoystickInput = true;
                m_InputDeviceChangeEvent?.Invoke();
            }

            if (m_QueueKeyDown.Count > 0)
            {
                lock (m_QueueKeyDown)
                {
                    while (m_QueueKeyDown.Count > 0)
                    {
                        m_DicIsKeyDown[m_QueueKeyDown.Dequeue()] = true;
                    }
                }
            }

            if (m_AxisDownIndex >= 0)
            {
                InputHelper.SetAxisDown(m_AxisDownType, m_AxisDownIndex, true);
                m_AxisDownIndex = -1;
                m_AxisDownType = AxisType.None;
            }

            CheckCombo();
        }
        
        public void SetKey(KeyType keyType, string keyName)
        {
            InputHelper.SetKey(keyType, keyName);
        }

        public void SetKey(KeyType keyType, KeyCode keyCode)
        {
            InputHelper.SetKey(keyType, keyCode);
        }

        public void SetKey(KeyType keyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            InputHelper.SetKey(keyType, keyCode, isTurbo, isCheckCombo);
        }

        public void SetKey(KeyType keyType, KeyType replaceKeyType, bool isTurbo, bool isCheckCombo)
        {
            InputHelper.SetKey(keyType, replaceKeyType, isTurbo, isCheckCombo);
        }

        public void SetKey(KeyType keyType, KeyType replaceKeyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            InputHelper.SetKey(keyType, replaceKeyType, keyCode, isTurbo, isCheckCombo);
        }

        public void SetKey(KeyType keyType, string keyName, KeyType replaceKeyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            InputHelper.SetKey(keyType, keyName, replaceKeyType, keyCode, isTurbo, isCheckCombo);
        }

        public void SetAxis(AxisType axisType, string horizontal, string vertical)
        {
            InputHelper.SetAxis(axisType, horizontal, vertical);
        }

        public void SetAxis(AxisType axisType, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            InputHelper.SetAxis(axisType, keyCodeHorizontalPositive, keyCodeHorizontalNegative, keyCodeVerticalPositive, keyCodeVerticalNegative);
        }

        public void SetAxis(AxisType axisType, string horizontal, string vertical, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            InputHelper.SetAxis(axisType, horizontal, vertical, keyCodeHorizontalPositive, keyCodeHorizontalNegative, keyCodeVerticalPositive, keyCodeVerticalNegative);
        }

        public void AddComboKeyEvent(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            m_ComboKeyEvents.Add(ComboKeyEventArgs.Create(keys, eventId, keyEvent));
        }

        public void RemoveComboKeyEvent(int eventID)
        {
            for (int i = m_ComboKeyEvents.Count - 1; i >= 0; i--)
            {
                if (m_ComboKeyEvents[i].eventId.Equals(eventID))
                {
                    m_ComboKeyEvents[i].Release();
                    m_ComboKeyEvents.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllComboKeyEvent()
        {
            m_GetDirectionEvent = null;
            m_GetPreConditionEvent = null;
            m_ComboKeyEvents.Clear();
        }

        public void AddAfterTriggerEvent(KeyType keyType, GameFrameWorkAction afterTriggerEvent)
        {
            if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<GameFrameWorkAction> eventList))
            {
                if (!eventList.Contains(afterTriggerEvent))
                {
                    eventList.Add(afterTriggerEvent);
                }

                return;
            }

            eventList = new List<GameFrameWorkAction>() { afterTriggerEvent };
            m_DicAfterTriggerEvents.Add(keyType, eventList);
        }

        public void RemoveAfterTriggerEvent(KeyType keyType, GameFrameWorkAction afterTriggerEvent)
        {
            if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<GameFrameWorkAction> eventList))
            {
                eventList.Remove(afterTriggerEvent);
            }
        }

        public void RemoveAllAfterTriggerEvent()
        {
            m_DicAfterTriggerEvents.Clear();
        }

        public Vector2 GetAxis(AxisType axisType, bool isTurbo = false, bool checkKeyBoard = true)
        {
            Vector2 tempAxis = GetAxis(InputHelper.GetAxis(axisType), checkKeyBoard);
            Vector2 axis = Vector2.zero;
            float x = tempAxis.x;
            float y = tempAxis.y;
            float speed = 1f;

            if (x != 0 || y != 0)
            {
                if (!isTurbo)
                {
                    bool prevHorizontal = InputHelper.GetAxisDown(axisType, 0);
                    bool prevVertical = InputHelper.GetAxisDown(axisType, 1);
                    m_AxisDownIndex = x != 0 ? 0 : 1;
                    m_AxisDownType = axisType;

                    if (!prevHorizontal && x != 0) axis.x = speed * (x > 0 ? 1 : -1);
                    if (!prevVertical && y != 0) axis.y = speed * (y > 0 ? 1 : -1);
                }
                else
                {
                    axis.x = x != 0 ? speed * (x > 0 ? 1 : -1) : 0;
                    axis.y = y != 0 ? speed * (y > 0 ? 1 : -1) : 0;
                }
            }
            else
            {
                if (x == 0) InputHelper.SetAxisDown(axisType, 0, false);
                if (y == 0) InputHelper.SetAxisDown(axisType, 1, false);
            }

            return axis;
        }

        public bool GetKeyDown(KeyType keyType, bool checkKeyBoard = true)
        {
            KeyArgs key = InputHelper.GetKey(keyType);

            if (key == null)
            {
                return false;
            }

            return GetKeyDown(key, checkKeyBoard);
        }

        private void CheckCombo()
        {
            if (!m_IsRunning || m_ComboKeyEvents == null || m_ComboKeyEvents.Count < 1)
            {
                return;
            }

            if (CheckComboAxis(AxisType.LeftAxis))
            {
                m_KeyDownTimer = Time.time;
            }

            KeyArgs[] allKeys = InputHelper.GetAllKeys();

            for (int i = 0; i < allKeys.Length; i++)
            {
                if (allKeys[i] != null && allKeys[i].isCheckCombo && CheckComboKey(allKeys[i]))
                {
                    m_CurrKeyDown = i;
                    m_KeyDownTimer = Time.time;
                    break;
                }
            }

            bool isTrigger = TriggerKeyEvent();

            if (m_KeyDownTimer > 0 && Time.time - m_KeyDownTimer >= KeyDownTime)
            {
                if (!isTrigger && m_CurrKeyDown >= 0)
                {
                    KeyType keyType = allKeys[m_CurrKeyDown].replaceKeyType != KeyType.None ? allKeys[m_CurrKeyDown].replaceKeyType : allKeys[m_CurrKeyDown].keyType;

                    if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<GameFrameWorkAction> triggerEvents))
                    {
                        foreach (var triggerEvent in triggerEvents)
                        {
                            triggerEvent?.Invoke();
                        }
                    }
                }

                ResetComboKeys();
            }
        }

        private bool CheckComboAxis(AxisType axisType)
        {
            Vector2 axis = GetAxis(axisType);

            if (m_CurrDir == 0)
            {
                m_CurrDir = m_GetDirectionEvent?.Invoke() ?? 1;
            }

            if (axis.y > 0)
            {
                m_ComboKeys.Add(KeyType.Up);
            }
            else if (axis.y < 0)
            {
                m_ComboKeys.Add(KeyType.Down);
            }

            if (axis.x > 0)
            {
                m_ComboKeys.Add(m_CurrDir > 0 ? KeyType.Right : KeyType.Left);
            }
            else if (axis.x < 0)
            {
                if (m_ComboKeys.Count > 0 && m_ComboKeys[^1] == KeyType.Right)
                {
                    m_ComboKeys[^1] = KeyType.Left;
                    m_ComboKeys.Add(KeyType.Right);
                }
                else
                {
                    if (m_CurrDir > 0) m_ComboKeys.Add(KeyType.Left);
                    if (m_CurrDir < 0) m_ComboKeys.Add(KeyType.Right);
                }
            }

            bool isX = axis.x > 0 || axis.x < 0;
            bool isY = axis.y > 0 || axis.y < 0;
            return isX || isY;
        }

        private bool CheckComboKey(KeyArgs key)
        {
            if (key == null)
            {
                return false;
            }
            
            bool isKeyDown = false;
            
            if (GetComboKeyDown(key.keyName, key.keyCode, key.isTurbo))
            {
                KeyType replaceKeyType = key.replaceKeyType != KeyType.None ? key.replaceKeyType : key.keyType;

                if (m_ComboKeys.Count < 1 || m_ComboKeys[^1] != replaceKeyType)
                {
                    m_ComboKeys.Add(replaceKeyType);
                    isKeyDown = true;
                }
            }

            return isKeyDown;
        }

        private bool TriggerKeyEvent()
        {
            if (m_ComboKeys.Count < 2)
            {
                return false;
            }

            foreach (var comboKeyEvent in m_ComboKeyEvents)
            {
                if (comboKeyEvent.keys.Length < 1 || m_ComboKeys.Count < comboKeyEvent.keys.Length)
                {
                    continue;
                }

                bool isMatch = IsMatch(comboKeyEvent.keys, m_ComboKeys);
                
                if (!isMatch || m_GetPreConditionEvent == null || !m_GetPreConditionEvent(comboKeyEvent.eventId))
                {
                    continue;
                }

                ResetComboKeys();
                comboKeyEvent.keyEvent?.Invoke(comboKeyEvent.eventId, true);
                return true;
            }

            return false;
        }

        private bool IsMatch(KeyType[] origin, List<KeyType> input, int originIndex = 0, int inputIndex = 0)
        {
            if (input.Count < origin.Length) return false;
            if (originIndex >= origin.Length) return true;
            if (inputIndex >= input.Count) return false;

            if (input[inputIndex] != origin[originIndex])
            {
                inputIndex++;

                if (originIndex > 0)
                {
                    originIndex = 0;
                }

                return IsMatch(origin, input, originIndex, inputIndex);
            }
            
            inputIndex++;
            originIndex++;
            return IsMatch(origin, input, originIndex, inputIndex);
        }

        private bool GetComboKeyDown(string keyName, KeyCode keyCode, bool isTurbo)
        {
            bool isKeyDown = UnityEngine.Input.GetButton(keyName);

            if (!isKeyDown)
            {
                isKeyDown = UnityEngine.Input.GetKey(keyCode);
            }

            if (!m_DicIsKeyDown.TryGetValue(keyName, out bool prev))
            {
                m_DicIsKeyDown.Add(keyName, false);
            }

            if (isKeyDown)
            {
                if (!isTurbo)
                {
                    if (!m_QueueKeyDown.Contains(keyName))
                    {
                        lock (m_QueueKeyDown)
                        {
                            m_QueueKeyDown.Enqueue(keyName);
                        }
                    }

                    return !prev;
                }

                return true;
            }
            
            m_DicIsKeyDown[keyName] = false;
            return false;
        }

        private bool AnyJoystickInput()
        {
            AxisArgs[] allAxes = InputHelper.GetAllAxis();

            if (allAxes is { Length: > 0 })
            {
                foreach (var axis in allAxes)
                {
                    if (GetAxis(axis, false) != Vector2.zero)
                    {
                        return true;
                    }
                }
            }

            KeyArgs[] allKeys = InputHelper.GetAllKeys();

            if (allKeys is { Length: > 0 })
            {
                foreach (var key in allKeys)
                {
                    if (GetKeyDown(key, false))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetKeyDown(KeyArgs key, bool checkKeyBoard = true)
        {
            bool isKeyDown = UnityEngine.Input.GetButtonDown(key.keyName);

            if (!isKeyDown && checkKeyBoard)
            {
                isKeyDown = UnityEngine.Input.GetKeyDown(key.keyCode);
            }

            return isKeyDown;
        }

        private Vector2 GetAxis(AxisArgs axis, bool checkKeyBoard)
        {
            Vector2 axisValue = Vector2.zero;

            if (axis == null)
            {
                return axisValue;
            }

            float x = UnityEngine.Input.GetAxis(axis.horizontal);
            float y = UnityEngine.Input.GetAxis(axis.vertical);
            bool xPositive = checkKeyBoard && UnityEngine.Input.GetKey(axis.keyCodeHorizontalPositive);
            bool xNegative = checkKeyBoard && UnityEngine.Input.GetKey(axis.keyCodeHorizontalNegative);
            bool yPositive = checkKeyBoard && UnityEngine.Input.GetKey(axis.keyCodeVerticalPositive);
            bool yNegative = checkKeyBoard && UnityEngine.Input.GetKey(axis.keyCodeVerticalNegative);

            if (x == 0)
            {
                if (xPositive)
                {
                    x = 1f;
                }
                else if (xNegative)
                {
                    x = -1f;
                }
            }

            if (y == 0)
            {
                if (yPositive)
                {
                    y = 1f;
                }
                else if (yNegative)
                {
                    y = -1f;
                }
            }

            axisValue.x = x;
            axisValue.y = y;
            return axisValue;
        }

        private void ResetComboKeys()
        {
            m_ComboKeys.Clear();
            m_KeyDownTimer = -1f;
            m_CurrKeyDown = -1;
            m_CurrDir = 0;
        }
    }
}