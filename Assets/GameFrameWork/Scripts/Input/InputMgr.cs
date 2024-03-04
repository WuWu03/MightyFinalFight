using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class InputMgr : BaseMgr<InputMgr>
    {
        public GameFrameWorkFloatAction getDirectionEvent;
        public GameFrameWorkBooleanAction<int> getPreconditonEvent;

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

        protected override void OnAwake()
        {
            m_ListComboKeyEvent = new List<ComboKeyEventArgs>();
            m_ListComboKey = new List<KeyType>();
            m_TriggerKey = new List<KeyType>();
            m_QueueKeyDown = new Queue<string>();
            m_DicAfterTriggerEvents = new Dictionary<KeyType, List<GameFrameWorkAction>>();

            InputHelper.Init();
        }

        public void AddKey(KeyType keyType, string keyName, KeyType replaceKeyType = KeyType.None)
        {
            InputHelper.AddKey(keyType, keyName, replaceKeyType, false, false);
        }

        public void AddTurboKey(KeyType keyType, string keyName, KeyType replaceKeyType = KeyType.None)
        {
            InputHelper.AddKey(keyType, keyName, replaceKeyType, false, false);
        }

        public void AddComboKey(KeyType keyType, string keyName, KeyType replaceKeyType = KeyType.None)
        {
            InputHelper.AddKey(keyType, keyName, replaceKeyType, false, true);
        }

        public void AddTurboComboKey(KeyType keyType, string keyName, KeyType replaceKeyType = KeyType.None)
        {
            InputHelper.AddKey(keyType, keyName, replaceKeyType, true, true);
        }

        public void AddAxis(AxisType axisType, string horizontal, string vertical)
        {
            InputHelper.AddAxis(axisType, horizontal, vertical);
        }

        public void AddComboKeyEvent(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            m_ListComboKeyEvent.Add(ComboKeyEventArgs.Create(keys, eventId, keyEvent));
        }

        public void RemoveComboKeyEvent(int eventID)
        {
            for (int i = m_ListComboKeyEvent.Count - 1; i >= 0; i--)
            {
                if (m_ListComboKeyEvent[i].eventId.Equals(eventID))
                {
                    ReferencePool.Release(m_ListComboKeyEvent[i]);
                    m_ListComboKeyEvent.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllComboKeyEvent()
        {
            getDirectionEvent = null;
            getPreconditonEvent = null;
            m_ListComboKeyEvent.Clear();
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

        public Vector2 GetAxis(AxisType axisType, bool isOneKey = false)
        {
            AxisArgs axisArgs = InputHelper.GetAxis(axisType);
            Vector2 axis = Vector2.zero;

            if (axisArgs == null)
            {
                return axis;
            }

            float x = UnityEngine.Input.GetAxis(axisArgs.horizontal);
            float y = UnityEngine.Input.GetAxis(axisArgs.vertical);
            float speed = 1f;

            axis.x = 0f;
            axis.y = 0f;

            if (x != 0 || y != 0)
            {
                if (isOneKey)
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

                return axis;
            }
            else
            {
                if (x == 0) InputHelper.SetAxisDown(axisType, 0, false);
                if (y == 0) InputHelper.SetAxisDown(axisType, 1, false);
            }

            return axis;
        }

        //public float GetWheelsAxis(AxisType axisType)
        //{
        //    AxisArgs axisArgs = null;
        //    float axis = 0;

        //    if (!m_DicAxis.TryGetValue(axisType, out axisArgs))
        //    {
        //        return axis;
        //    }

        //    float x = UnityEngine.Input.scro(axisArgs.Horizontal);
        //}

        public bool GetKeyDown(KeyType keyType, bool isTurbo = false)
        {
            KeyArgs key = InputHelper.GetKey(keyType);

            if (key == null)
            {
                return false;
            }

            return GetKeyDown(key.keyName, isTurbo);
        }

        protected override void OnUpdate()
        {
            if (m_QueueKeyDown.Count > 0)
            {
                lock (m_QueueKeyDown)
                {
                    while (m_QueueKeyDown.Count > 0)
                        m_DicIsKeyDown[m_QueueKeyDown.Dequeue()] = true;
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

        private void CheckCombo()
        {
            if (!m_IsRunning || m_ListComboKeyEvent == null || m_ListComboKeyEvent.Count < 1)
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

                    if (m_DicAfterTriggerEvents.TryGetValue(keyType, out List<GameFrameWorkAction> eventList))
                    {
                        for (int i = 0; i < eventList.Count; i++)
                        {
                            eventList[i]?.Invoke();
                        }
                    }
                }

                ResetComboKeys();
            }
        }

        private bool CheckComboAxis(AxisType axisType)
        {
            bool keyDown = false;

            Vector2 axis = GetAxis(axisType, true);
            if (m_CurrDir == 0) m_CurrDir = getDirectionEvent != null ? getDirectionEvent() : 1;

            if (axis.y > 0) m_ListComboKey.Add(KeyType.Up);
            if (axis.y < 0) m_ListComboKey.Add(KeyType.Down);
            if (axis.x > 0) m_ListComboKey.Add(m_CurrDir > 0 ? KeyType.Right : KeyType.Left);
            if (axis.x < 0)
            {
                if (m_ListComboKey.Count > 0 && m_ListComboKey[m_ListComboKey.Count - 1] == KeyType.Right)
                {
                    m_ListComboKey[m_ListComboKey.Count - 1] = KeyType.Left;
                    m_ListComboKey.Add(KeyType.Right);
                }
                else
                {
                    if (m_CurrDir > 0) m_ListComboKey.Add(KeyType.Left);
                    if (m_CurrDir < 0) m_ListComboKey.Add(KeyType.Right);
                }
            }

            bool isX = axis.x > 0 || axis.x < 0;
            bool isY = axis.y > 0 || axis.y < 0;
            keyDown = isX || isY;

            return keyDown;
        }

        private bool CheckComboKey(KeyArgs key)
        {
            bool isKeyDown = false;

            if (key == null)
            {
                return isKeyDown;
            }

            if (GetKeyDown(key.keyName, key.isTurbo))
            {
                KeyType replaceKeyType = key.replaceKeyType != KeyType.None ? key.replaceKeyType : key.keyType;

                if (m_ListComboKey.Count < 1 || m_ListComboKey[m_ListComboKey.Count - 1] != replaceKeyType)
                {
                    m_ListComboKey.Add(replaceKeyType);
                    isKeyDown = true;
                }
            }

            return isKeyDown;
        }

        private bool TriggerKeyEvent()
        {
            if (m_ListComboKey.Count < 2)
            {
                return false;
            }

            for (int i = 0; i < m_ListComboKeyEvent.Count; i++)
            {
                if (m_ListComboKeyEvent[i].keys.Length < 1 || m_ListComboKey.Count < m_ListComboKeyEvent[i].keys.Length) continue;

                bool isMatch = false;

                for (int j = 0; j < m_ListComboKeyEvent[i].keys.Length; j++)
                {
                    if (IsMatch(m_ListComboKeyEvent[i].keys, m_ListComboKey))
                    {
                        isMatch = true;
                        break;
                    }
                }
                
                if (!isMatch || getPreconditonEvent == null || !getPreconditonEvent(m_ListComboKeyEvent[i].eventId))
                {
                    continue;
                }

                ResetComboKeys();
                m_ListComboKeyEvent[i].keyEvent?.Invoke(m_ListComboKeyEvent[i].eventId, true);
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
            else
            {
                inputIndex++;
                originIndex++;
                return IsMatch(origin, input, originIndex, inputIndex);
            }
        }

        private bool GetKeyDown(string keyName, bool isTurbo = false)
        {
            bool isKeyDown = UnityEngine.Input.GetButton(keyName);

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
                            m_QueueKeyDown.Enqueue(keyName);
                    }

                    return !prev;
                }

                return isKeyDown;
            }
            else
            {
                m_DicIsKeyDown[keyName] = false;
            }

            return false;
        }

        private void ResetComboKeys()
        {
            m_ListComboKey.Clear();
            m_KeyDownTimer = -1f;
            m_CurrKeyDown = -1;
            m_CurrDir = 0;
        }

        protected override void OnShutDown()
        {
            InputHelper.Dispose();
            m_DicIsKeyDown.Clear();
            m_ListComboKey.Clear();
            m_TriggerKey.Clear();
            m_ListComboKeyEvent.Clear();
            m_QueueKeyDown.Clear();
            m_DicAfterTriggerEvents.Clear();
            m_IsRunning = false;
        }

        private float m_CurrDir = 0;
        private float m_KeyDownTimer = -1f;
        private const float KeyDownTime = 0.05f;
        private bool m_IsRunning = false;
        private int m_CurrKeyDown = -1;
        private int m_AxisDownIndex = -1;//0 horizontal 1 vertical
        private AxisType m_AxisDownType = AxisType.None;
        private Dictionary<string, bool> m_DicIsKeyDown = new Dictionary<string, bool>();
        private List<KeyType> m_ListComboKey = null;
        private List<KeyType> m_TriggerKey = null;
        private Queue<string> m_QueueKeyDown = null;
        private List<ComboKeyEventArgs> m_ListComboKeyEvent = null;
        private Dictionary<KeyType, List<GameFrameWorkAction>> m_DicAfterTriggerEvents = null;
    }
}
