using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class InputMgr : BaseMgr<InputMgr>
    {
        public GameFrameWorkFloatAction getDirectionEvent;
        public GameFrameWorkBooleanAction afterTriggerEvent;
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
            m_QueueKeyDown = new Queue<string>();

            InputHelper.Init();
        }

        public void AddKey(KeyType keyType, string keyName)
        {
            AddKey(keyType, keyName, KeyType.None, false);
        }

        public void AddKey(KeyType keyType, string keyName, KeyType replaceKeyType, bool isShift)
        {
            InputHelper.AddKey(keyType, keyName, replaceKeyType, isShift);
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
            afterTriggerEvent = null;
            getPreconditonEvent = null;
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

            if (m_AxisDownIndex >= 0)
            {
                InputHelper.SetAxisDown(m_AxisDownType, m_AxisDownIndex, true);
                m_AxisDownIndex = -1;
                m_AxisDownType = AxisType.None;
            }

            if (!m_IsRunning || m_ListComboKeyEvent == null || m_ListComboKeyEvent.Count < 1)
            {
                return;
            }

            if (m_KeyDownTimer > 0 && Time.time - m_KeyDownTimer >= KeyDownTime)
            {
                ResetComboKeys();
            }

            if (CheckComboAxis(AxisType.LeftAxis)) m_KeyDownTimer = Time.time;
            //if (CheckComboAxis(AxisType.CrossAxis)) m_KeyDownTime = Time.time;
            if (CheckComboKey(KeyType.A)) m_KeyDownTimer = Time.time;
            if (CheckComboKey(KeyType.B)) m_KeyDownTimer = Time.time;
            if (CheckComboKey(KeyType.X)) m_KeyDownTimer = Time.time;
            if (CheckComboKey(KeyType.Y)) m_KeyDownTimer = Time.time;
            if (CheckComboKey(KeyType.LB)) m_KeyDownTimer = Time.time;
            if (CheckComboKey(KeyType.RB)) m_KeyDownTimer = Time.time;
            //if (CheckComboKey(KeyType.LT)) m_KeyDownTime = Time.time;
            //if (CheckComboKey(KeyType.RT)) m_KeyDownTime = Time.time;

            if (!TriggerKeyEvent())
            {
                afterTriggerEvent?.Invoke();
            }
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

        public bool GetKeyDown(KeyType keyType, bool isOneKey = false)
        {
            KeyArgs key = InputHelper.GetKey(keyType);

            if (key == null)
            {
                return false;
            }

            return GetKeyDown(key.keyName, isOneKey);
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

        private bool CheckComboKey(KeyType keyType)
        {
            KeyArgs key = InputHelper.GetKey(keyType);
            bool keyDown = false;

            if (key == null)
            {
                return keyDown;
            }

            if (key.isShift)
            {
                if (GetKeyDown(key.keyName))
                {
                    KeyType replaceKeyType = key.replaceKeyType != KeyType.None ? key.replaceKeyType : keyType;
                    if (m_ListComboKey.Count < 1 || m_ListComboKey[m_ListComboKey.Count - 1] != replaceKeyType)
                    {
                        m_ListComboKey.Add(replaceKeyType);
                        keyDown = true;
                    }
                }
            }
            else
            {
                keyDown = GetKeyDown(key.keyName, true);
                if (keyDown) m_ListComboKey.Add(keyType);
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
                if (originIndex > 0) originIndex = 0;
                return IsMatch(origin, input, originIndex, inputIndex);
            }
            else
            {
                inputIndex++;
                originIndex++;
                return IsMatch(origin, input, originIndex, inputIndex);
            }
        }

        private bool GetKeyDown(string keyName, bool isOneKey = false)
        {
            bool isKeyDown = UnityEngine.Input.GetButton(keyName);

            if (!m_DicIsKeyDown.TryGetValue(keyName, out bool prev))
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
            m_KeyDownTimer = -1f;
            m_CurrDir = 0;
        }

        protected override void OnShutDown()
        {
            InputHelper.Dispose();
            m_DicIsKeyDown.Clear();
            m_ListComboKey.Clear();
            m_ListComboKeyEvent.Clear();
            m_QueueKeyDown.Clear();
            m_IsRunning = false;
        }

        private float m_CurrDir = 0;
        private float m_KeyDownTimer = -1f;
        private const float KeyDownTime = 0.04f;
        private bool m_IsRunning = false;

        private int m_AxisDownIndex = -1;//0 horizontal 1 vertical
        private AxisType m_AxisDownType = AxisType.None;
        private Dictionary<string, bool> m_DicIsKeyDown = new Dictionary<string, bool>();
        private List<KeyType> m_ListComboKey = null;
        private Queue<string> m_QueueKeyDown = null;
        private List<ComboKeyEventArgs> m_ListComboKeyEvent = null;
    }
}
