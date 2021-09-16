using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class InputMgr : BaseMgr<InputMgr>
    {
        public GameFrameWorkFloatAction GetDirection;
        public GameFrameWorkBooleanAction AfterTrigger;
        public GameFrameWorkBooleanAction<int> GetPreconditon;
        public bool IsRunning
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
            m_DicKeys = new Dictionary<KeyType, KeyNameArgs>();
            m_DicAxis = new Dictionary<AxisType, AxisArgs>();
            m_QueueKeyDown = new Queue<string>();
        }

        public void AddKey(KeyType keyType, string keyName)
        {
            AddKey(keyType, keyName, KeyType.None, false);
        }

        public void AddKey(KeyType keyType, string keyName, KeyType replaceKeyType, bool isShift)
        {
            m_DicKeys.Add(keyType, KeyNameArgs.Create(keyName, replaceKeyType, isShift));
        }

        public void AddAxis(AxisType axisType, string horizontal, string vertical)
        {
            m_DicAxis.Add(axisType, AxisArgs.Create(horizontal, vertical));
        }

        public void AddComboKeyEvent(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            m_ListComboKeyEvent.Add(ComboKeyEventArgs.Create(keys, eventId, keyEvent));
        }

        public void RemoveComboKeyEvent(int eventID)
        {
            for (int i = m_ListComboKeyEvent.Count - 1; i >= 0; i--)
            {
                if (m_ListComboKeyEvent[i].EventId.Equals(eventID))
                {
                    ReferencePool.Release(m_ListComboKeyEvent[i]);
                    m_ListComboKeyEvent.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllComboKeyEvent()
        {
            GetDirection = null;
            AfterTrigger = null;
            GetPreconditon = null;
            m_ListComboKeyEvent.Clear();
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

            if (!m_IsRunning || m_ListComboKeyEvent == null || m_ListComboKeyEvent.Count < 1)
            {
                return;
            }

            if (m_KeyDownTime > 0 && Time.time - m_KeyDownTime >= KEY_DOWN_TIME)
            {
                ResetComboKeys();
            }

            if (CheckComboAxis(AxisType.LeftAxis)) m_KeyDownTime = Time.time;
            //if (CheckComboAxis(AxisType.CrossAxis)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.A)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.B)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.X)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.Y)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.LB)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.RB)) m_KeyDownTime = Time.time;
            //if (CheckComboKey(KeyType.LT)) m_KeyDownTime = Time.time;
            //if (CheckComboKey(KeyType.RT)) m_KeyDownTime = Time.time;

            if (!TriggerKeyEvent())
            {
                AfterTrigger?.Invoke();
            }
        }

        public Vector2 GetAxis(AxisType axisType, bool isOneKey = false)
        {
            AxisArgs axisArgs = null;
            Vector2 axis = Vector2.zero;

            if (!m_DicAxis.TryGetValue(axisType, out axisArgs))
            {
                return axis;
            }

            float x = UnityEngine.Input.GetAxis(axisArgs.Horizontal);
            float y = UnityEngine.Input.GetAxis(axisArgs.Vertical);
            float speed = 1f;
            bool prevHorizontal = false;
            bool prevVertical = false;

            axis.x = 0f;
            axis.y = 0f;

            if (!m_DicIsKeyDown.TryGetValue(axisArgs.Horizontal, out prevHorizontal)) m_DicIsKeyDown.Add(axisArgs.Horizontal, false);
            if (!m_DicIsKeyDown.TryGetValue(axisArgs.Vertical, out prevVertical)) m_DicIsKeyDown.Add(axisArgs.Vertical, false);

            if (x != 0 || y != 0)
            {
                if (isOneKey)
                {
                    string axisName = x != 0 ? axisArgs.Horizontal : axisArgs.Vertical;
                    if (!m_QueueKeyDown.Contains(axisName))
                    {
                        lock (m_QueueKeyDown)
                            m_QueueKeyDown.Enqueue(axisName);
                    }

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
                if (x == 0) m_DicIsKeyDown[axisArgs.Horizontal] = false;
                if (y == 0) m_DicIsKeyDown[axisArgs.Vertical] = false;
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

        public bool GetKeyDown(KeyType keyType, bool isOneKey = false)
        {
            KeyNameArgs keyNameArgs = null;

            if (!m_DicKeys.TryGetValue(keyType, out keyNameArgs))
            {
                return false;
            }

            return GetKeyDown(keyNameArgs.KeyName, isOneKey);
        }
 
        private bool CheckComboAxis(AxisType axisType)
        {
            bool keyDown = false;

            Vector2 axis = GetAxis(axisType, true);
            if (m_CurrDir == 0) m_CurrDir = GetDirection != null ? GetDirection() : 1;

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

        private bool CheckComboKey(KeyType key)
        {
            KeyNameArgs keyNameArgs = null;
            bool keyDown = false;

            if (!m_DicKeys.TryGetValue(key, out keyNameArgs))
            {
                return keyDown;
            }

            if (keyNameArgs.IsShift)
            {
                if (GetKeyDown(keyNameArgs.KeyName))
                {
                    KeyType trans = keyNameArgs.ReplaceKeyType != KeyType.None ? keyNameArgs.ReplaceKeyType : key;
                    if (m_ListComboKey.Count < 1 || m_ListComboKey[m_ListComboKey.Count - 1] != trans)
                    {
                        m_ListComboKey.Add(trans);
                        keyDown = true;
                    }
                }
            }
            else
            {
                keyDown = GetKeyDown(keyNameArgs.KeyName, true);
                if (keyDown) m_ListComboKey.Add(key);
            }

            return keyDown;
        }

        private bool TriggerKeyEvent()
        {
            if (m_ListComboKey.Count < 2)
            {
                return false;
            }

            for (int i = 0; i < m_ListComboKeyEvent.Count; i++)
            {
                if (m_ListComboKeyEvent[i].Keys.Length < 1 || m_ListComboKey.Count < m_ListComboKeyEvent[i].Keys.Length) continue;

                bool isMatch = false;

                for (int j = 0; j < m_ListComboKeyEvent[i].Keys.Length; j++)
                {
                    if (IsMatch(m_ListComboKeyEvent[i].Keys, m_ListComboKey))
                    {
                        isMatch = true;
                        break;
                    }
                }
                
                if (!isMatch || GetPreconditon == null || !GetPreconditon(m_ListComboKeyEvent[i].EventId))
                {
                    continue;
                }

                ResetComboKeys();
                m_ListComboKeyEvent[i].KeyEvent?.Invoke(m_ListComboKeyEvent[i].EventId, true);
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
                inputIndex = inputIndex + 1;
                if (originIndex > 0) originIndex = 0;
                return IsMatch(origin, input, originIndex, inputIndex);
            }
            else
            {
                inputIndex = inputIndex + 1;
                originIndex = originIndex + 1;
                return IsMatch(origin, input, originIndex, inputIndex);
            }
        }

        private bool GetKeyDown(string keyName, bool isOneKey = false)
        {
            bool isKeyDown = UnityEngine.Input.GetButton(keyName);
            bool prev = false;

            if (!m_DicIsKeyDown.TryGetValue(keyName, out prev))
            {
                m_DicIsKeyDown.Add(keyName, false);
            }

            if (isKeyDown)
            {
                if (isOneKey)
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
            m_KeyDownTime = -1f;
            m_CurrDir = 0;
        }

        protected override void OnShutDown()
        {
            m_DicIsKeyDown.Clear();
            m_DicAxis.Clear();
            m_DicKeys.Clear();
            m_ListComboKey.Clear();
            m_ListComboKeyEvent.Clear();
            m_QueueKeyDown.Clear();
            m_IsRunning = false;
        }

        private float m_CurrDir = 0;
        private float m_KeyDownTime = -1f;
        private const float KEY_DOWN_TIME = 0.04f;
        private bool m_IsRunning = false;

        private Dictionary<string, bool> m_DicIsKeyDown = new Dictionary<string, bool>();
        private Dictionary<AxisType, AxisArgs> m_DicAxis = null;
        private Dictionary<KeyType, KeyNameArgs> m_DicKeys = null;
        private List<KeyType> m_ListComboKey = null;
        private Queue<string> m_QueueKeyDown = null;
        private List<ComboKeyEventArgs> m_ListComboKeyEvent = null;
    }
}
