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

        protected override void OnAwake()
        {
            m_ListEvent = new List<ComboKeyEventArgs>();
            m_ListKeyType = new List<KeyType>();
        }

        protected override void OnUpdate()
        {
            if (m_ListEvent != null && m_ListEvent.Count > 0)
            {
                if (m_KeyDownTime > 0 && Time.time - m_KeyDownTime >= KEY_DOWN_TIME)
                {
                    ResetKeys();
                }

                if (CheckKeyDown(KeyType.Up)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.Down)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.Left)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.Right)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.A)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.B)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.X)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.Y)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.LB)) m_KeyDownTime = Time.time;
                if (CheckKeyDown(KeyType.RB)) m_KeyDownTime = Time.time;

                if (!TriggerKeyEvent())
                {
                    AfterTrigger?.Invoke();
                }
            }
        }

        private static Dictionary<string, bool> m_DicIsKeyDown = new Dictionary<string, bool>();
        private static Vector2 m_Axis = Vector2.zero;

        public static Vector2 GetAxis(bool isOneKey = false)
        {
            float x = UnityEngine.Input.GetAxis("Horizontal");
            float y = UnityEngine.Input.GetAxis("Vertical");
            float speed = 1f;
            bool prevHorizontal = false;
            bool prevVertical = false;

            m_Axis.x = 0f;
            m_Axis.y = 0f;

            if (!m_DicIsKeyDown.TryGetValue("Horizontal", out prevHorizontal)) m_DicIsKeyDown.Add("Horizontal", false);
            if (!m_DicIsKeyDown.TryGetValue("Vertical", out prevVertical)) m_DicIsKeyDown.Add("Vertical", false);
  
            if (x != 0 || y != 0)
            {
                if (isOneKey)
                {
                    m_DicIsKeyDown[x != 0 ? "Horizontal" : "Vertical"] = true;
                    if (!prevHorizontal && x != 0) m_Axis.x = speed * (x > 0 ? 1 : -1);
                    if (!prevVertical && y != 0) m_Axis.y = speed * (y > 0 ? 1 : -1);
                }
                else
                {
                    m_Axis.x = x != 0 ? speed * (x > 0 ? 1 : -1) : 0;
                    m_Axis.y = y != 0 ? speed * (y > 0 ? 1 : -1) : 0;
                }

                return m_Axis;
            }
            else
            {
                if (x == 0) m_DicIsKeyDown["Horizontal"] = false;
                if (y == 0) m_DicIsKeyDown["Vertical"] = false;
            }

            return m_Axis;
        }

        
        public static bool GetKeyDown(KeyType keyType, bool isOneKey = false)
        {
            string keyName = Enum.GetName(typeof(KeyType), keyType);
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
                    m_DicIsKeyDown[keyName] = true;
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

        public void AddKeyEvent(KeyType[] keys,int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            m_ListEvent.Add(ComboKeyEventArgs.Create(keys, eventId, keyEvent));
        }

        public void RemoveKeyEvent(int eventID)
        {
            for (int i = m_ListEvent.Count - 1; i >= 0; i--)
            {
                if (m_ListEvent[i].EventId.Equals(eventID))
                {
                    ReferencePool.Release(m_ListEvent[i]);
                    m_ListEvent.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllKeyEvent()
        {
            GetDirection = null;
            AfterTrigger = null;
            GetPreconditon = null;
            m_ListEvent.Clear();
        }

        private bool CheckKeyDown(KeyType key)
        {
            bool keyDown = false;

            if (key == KeyType.Up || key == KeyType.Down || key == KeyType.Left || key == KeyType.Right)
            {
                Vector2 axis = GetAxis(true);
                if (m_CurrDir == 0) m_CurrDir = GetDirection != null ? GetDirection() : 1;

                if (axis.y > 0) m_ListKeyType.Add(KeyType.Up);
                if (axis.y < 0) m_ListKeyType.Add(KeyType.Down);
                if (axis.x > 0) m_ListKeyType.Add(m_CurrDir > 0 ? KeyType.Right : KeyType.Left);
                if (axis.x < 0)
                {
                    if (m_ListKeyType.Count > 0 && m_ListKeyType[m_ListKeyType.Count - 1] == KeyType.Right)
                    {
                        m_ListKeyType[m_ListKeyType.Count - 1] = KeyType.Left;
                        m_ListKeyType.Add(KeyType.Right);
                    }
                    else
                    {
                        if (m_CurrDir > 0) m_ListKeyType.Add(KeyType.Left);
                        if (m_CurrDir < 0) m_ListKeyType.Add(KeyType.Right);
                    }
                }

                bool isX = axis.x > 0 || axis.x < 0;
                bool isY = axis.y > 0 || axis.y < 0;
                keyDown = isX || isY;
            }
            else if (key == KeyType.X || key == KeyType.Y)
            {
                if (GetKeyDown(key))
                {
                    KeyType trans = key == KeyType.X ? KeyType.A : KeyType.B;
                    if (m_ListKeyType.Count < 1 || m_ListKeyType[m_ListKeyType.Count - 1] != trans)
                    {
                        m_ListKeyType.Add(trans);
                        keyDown = true;
                    }
                }
            }
            else
            {
                keyDown = GetKeyDown(key, true);
                if (keyDown) m_ListKeyType.Add(key);
            }

            return keyDown;
        }

        private bool TriggerKeyEvent()
        {
            if (m_ListKeyType.Count < 1)
            {
                return false;
            }

            for (int i = 0; i < m_ListEvent.Count; i++)
            {
                if (m_ListEvent[i].Keys.Length < 1 || m_ListKeyType.Count < m_ListEvent[i].Keys.Length) continue;

                bool isMatch = false;

                for (int j = 0; j < m_ListEvent[i].Keys.Length; j++)
                {
                    if (IsMatch(m_ListEvent[i].Keys, m_ListKeyType))
                    {
                        isMatch = true;
                        break;
                    }
                }
                
                if (!isMatch || GetPreconditon == null || !GetPreconditon(m_ListEvent[i].EventId))
                {
                    continue;
                }

                ResetKeys();
                m_ListEvent[i].KeyEvent?.Invoke(m_ListEvent[i].EventId, true);
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

        private void ResetKeys()
        {
            m_ListKeyType.Clear();
            m_KeyDownTime = -1f;
            m_CurrDir = 0;
        }

        protected override void OnShutDown()
        {
            m_ListKeyType.Clear();
            m_ListEvent.Clear();
        }

        private float m_CurrDir = 0;
        private float m_KeyDownTime = -1f;
        private const float KEY_DOWN_TIME = 0.04f;

        private List<KeyType> m_ListKeyType = null;
        private List<ComboKeyEventArgs> m_ListEvent = null;
    }
}
