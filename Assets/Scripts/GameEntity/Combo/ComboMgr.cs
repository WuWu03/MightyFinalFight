using System.Collections.Generic;
using UnityEngine;
using WuWuFramework;
using WuWuFramework.Event;
using WuWuFramework.Input;

public enum ComboKey : byte
{
    Up,
    Down,
    Left,
    Right,
    A,
    B,
    X,
    Y,
    LB,
    RB,
    LT,
    RT,
    None,
}

public class ComboMgr : Singleton<ComboMgr>
{
    private event WuWuFrameworkFunc<int, bool> m_GetPreConditionEvent;
    private readonly List<ComboKey> m_ComboKeys;
    private readonly List<ComboKeyEventArgs> m_ComboKeyEvents;
    private readonly Dictionary<ComboKey, List<WuWuFrameworkAction>> m_DicAfterTriggerEvents;
    private float m_KeyDownTimer = -1f;
    private ComboKey m_CurrComboKey = ComboKey.None;
    private bool m_IsRunning = false;
    private const float KEY_DOWN_TIME_OFFSET = 0.05f;
    private Vector2 m_CurrLeftAxis = Vector2.zero;

    public event WuWuFrameworkFunc<int, bool> getPreConditionEvent
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

    public Vector2 currLeftAxis
    {
        get
        {
            return m_CurrLeftAxis;
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

    public ComboMgr()
    {
        m_ComboKeys = new();
        m_ComboKeyEvents = new();
        m_DicAfterTriggerEvents = new();
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, OnLeftAxisInput);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, OnAInput);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.B, InputEventCallType.Performed, OnBInput);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.X, InputEventCallType.Performed, OnXInput);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.Y, InputEventCallType.Performed, OnYInput);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.LeftAxis, InputEventCallType.Performed, OnLeftAxisInput);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.A, InputEventCallType.Performed, OnAInput);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.B, InputEventCallType.Performed, OnBInput);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.X, InputEventCallType.Performed, OnXInput);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.Y, InputEventCallType.Performed, OnYInput);
        MonoBehaviourMgr.instance.updateEvent += Update;
    }

    public void AddComboKeyEvent(ComboKey[] keys, int eventId, WuWuFrameworkAction<int, bool> keyEvent)
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
        m_GetPreConditionEvent = null;
        m_ComboKeyEvents.Clear();
    }

    public void AddAfterTriggerEvent(ComboKey keyType, WuWuFrameworkAction afterTriggerEvent)
    {
        if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<WuWuFrameworkAction> eventList))
        {
            if (!eventList.Contains(afterTriggerEvent))
            {
                eventList.Add(afterTriggerEvent);
            }

            return;
        }

        eventList = new() { afterTriggerEvent };
        m_DicAfterTriggerEvents.Add(keyType, eventList);
    }

    public void RemoveAfterTriggerEvent(ComboKey keyType, WuWuFrameworkAction afterTriggerEvent)
    {
        if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<WuWuFrameworkAction> eventList))
        {
            eventList.Remove(afterTriggerEvent);
        }
    }

    public void RemoveAllAfterTriggerEvent()
    {
        m_DicAfterTriggerEvents.Clear();
    }

    public override void Shutdown()
    {
        m_ComboKeys.Clear();
        m_ComboKeyEvents.Clear();
        m_DicAfterTriggerEvents.Clear();
        m_IsRunning = false;
        m_GetPreConditionEvent = null;
        GameEntry.inputMgr.keyBoardInputController.RemoveAllInputEvents();
        GameEntry.inputMgr.xboxInputController.RemoveAllInputEvents();
        MonoBehaviourMgr.instance.updateEvent -= Update;
    }

    private void OnLeftAxisInput(Vector2 axis)
    {
        m_CurrLeftAxis = axis;

        if (CheckComboAxis(axis))
        {
            m_KeyDownTimer = Time.time;
        }
    }

    private void OnAInput()
    {
        if (CheckComboKey(ComboKey.A))
        {
            m_KeyDownTimer = Time.time;
        }
    }

    private void OnBInput()
    {
        if (CheckComboKey(ComboKey.B))
        {
            m_KeyDownTimer = Time.time;
        }
    }

    private void OnXInput()
    {
        if (CheckComboKey(ComboKey.A))
        {
            m_KeyDownTimer = Time.time;
        }
    }

    private void OnYInput()
    {
        if (CheckComboKey(ComboKey.B))
        {
            m_KeyDownTimer = Time.time;
        }
    }

    private void CheckCombo()
    {
        if (!m_IsRunning || m_ComboKeyEvents == null || m_ComboKeyEvents.Count < 1)
        {
            return;
        }

        bool isTrigger = TriggerKeyEvent();

        if (m_KeyDownTimer > 0 && Time.time - m_KeyDownTimer >= KEY_DOWN_TIME_OFFSET)
        {
            if (!isTrigger && m_CurrComboKey != ComboKey.None)
            {
                if (m_DicAfterTriggerEvents.TryGetValue(m_CurrComboKey, out List<WuWuFrameworkAction> triggerEvents))
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

    private bool CheckComboAxis(Vector2 axis)
    {
        if (axis.y > 0)
        {
            m_CurrComboKey = ComboKey.Up;
            m_ComboKeys.Add(m_CurrComboKey);
        }
        else if (axis.y < 0)
        {
            m_CurrComboKey = ComboKey.Down;
            m_ComboKeys.Add(m_CurrComboKey);
        }

        if (axis.x != 0)
        {
            m_CurrComboKey = ComboKey.Right;
            m_ComboKeys.Add(m_CurrComboKey);
        }

        bool isX = axis.x > 0 || axis.x < 0;
        bool isY = axis.y > 0 || axis.y < 0;
        return isX || isY;
    }

    private bool CheckComboKey(ComboKey comboKey)
    {
        m_CurrComboKey = comboKey;

        if (m_ComboKeys.Count < 1 || m_ComboKeys[^1] != comboKey)
        {
            m_ComboKeys.Add(comboKey);
            return true;
        }

        return false;
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

    private bool IsMatch(ComboKey[] origin, List<ComboKey> input, int originIndex = 0, int inputIndex = 0)
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

    private void ResetComboKeys()
    {
        m_ComboKeys.Clear();
        m_KeyDownTimer = -1f;
        m_CurrComboKey = ComboKey.None;
    }

    private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
    {
        CheckCombo();
    }
}