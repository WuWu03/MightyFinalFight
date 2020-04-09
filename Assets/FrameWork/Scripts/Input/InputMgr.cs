using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Input
{
    public enum KeyType
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        A = 5,
        B = 6,
        X = 7,
        Y = 8,
        LB = 9,
        RB = 10,
    }

    public class InputMgr : BaseMgr<InputMgr>
    {
        public delegate float DirFunc();
        public DirFunc GetDirFunc;
        class ComboKeyEvent
        {
            public KeyType[] Keys;
            public int EventID;
            public VoidParamT2<int, bool> KeyEvent;
        }

        private void Awake()
        {
            m_ListEvent = new List<ComboKeyEvent>();
            m_ListKeyType = new List<KeyType>();
        }

        private void Update()
        {
            if (m_ListEvent == null || m_ListEvent.Count < 1) return;

            if (m_KeyDownTime > 0 && Time.time - m_KeyDownTime >= KEY_DOWN_TIME) ResetKeys();
  
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

            TriggerKeyEvent();
        }

        public static Vector2 GetAxis()
        {
            float x = UnityEngine.Input.GetAxis("Horizontal");
            float y = UnityEngine.Input.GetAxis("Vertical");
            float speed = 1f;
            if (x > 0) x = speed;
            else if (x < 0) x = -speed;

            if (y > 0) y = speed;
            else if (y < 0) y = -speed;

            return new Vector2(x, y);
        }

        public static Vector2 TestAxis()
        {
            Vector2 axis = Vector2.zero;
            if(UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            {
                axis.x = -1;
            }

            if(UnityEngine.Input.GetKey(KeyCode.RightArrow))
            {
                axis.x = 1;
            }

            if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
            {
                axis.y = 1;
            }

            if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
            {
                axis.y = -1;
            }

            return axis;
        }

        public void AddKeyEvent(KeyType[] keys,int eventID, VoidParamT2<int, bool> KeyEvent)
        {
            m_ListEvent.Add(new ComboKeyEvent()
            {
                Keys = keys,
                EventID = eventID,
                KeyEvent = KeyEvent,
            });
        }

        public void RemoveKeyEvent(int eventID)
        {
            for (int i = m_ListEvent.Count - 1; i >= 0; i--)
            {
                if (m_ListEvent[i].EventID.Equals(eventID))
                {
                    m_ListEvent.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllKeyEvent()
        {
            GetDirFunc = null;
            m_ListEvent.Clear();
        }

        private bool CheckKeyDown(KeyType key)
        {
            bool keyDown = false;

            if (key == KeyType.Up || key == KeyType.Down || key == KeyType.Left || key == KeyType.Right)
            {
                float x = UnityEngine.Input.GetAxisRaw("Horizontal");
                float y = UnityEngine.Input.GetAxisRaw("Vertical");

                if (m_CurrDir == 0) m_CurrDir = GetDirFunc != null ? GetDirFunc() : 1;

                if (y > 0 && m_KeyUpAdd)
                {
                    m_ListKeyType.Add(KeyType.Up);
                    m_KeyUpAdd = false;
                }
                if (y < 0 && m_KeyDownAdd)
                {
                    m_ListKeyType.Add(KeyType.Down);
                    m_KeyDownAdd = false;
                }

                if (x > 0 && m_KeyRightAdd)
                {
                    if (m_CurrDir > 0) m_ListKeyType.Add(KeyType.Right);
                    if (m_CurrDir < 0) m_ListKeyType.Add(KeyType.Left);
                    m_KeyRightAdd = false;
                }

                if (x < 0 && m_KeyLeftAdd)
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

                    m_KeyLeftAdd = false;
                }

                keyDown = x != 0 || y != 0;
            }
            else if (key == KeyType.X || key == KeyType.Y)
            {
                bool xDown = UnityEngine.Input.GetButton("X");
                bool yDown = UnityEngine.Input.GetButton("Y");

                if(xDown && m_KeyXAdd)
                {
                    m_ListKeyType.Add(KeyType.A);
                    m_KeyXAdd = false;
                }

                if (yDown && m_KeyYAdd)
                {
                    m_ListKeyType.Add(KeyType.B);
                    m_KeyYAdd = false;
                }

                keyDown = xDown || yDown;
            }
            else if (UnityEngine.Input.GetButtonDown(Enum.GetName(typeof(KeyType),key)))
            {
                keyDown = true;
                m_ListKeyType.Add(key);
            }

            return keyDown;
        }

        private void TriggerKeyEvent()
        {
            if (m_ListKeyType.Count < 1) return;
            for (int i = 0; i < m_ListEvent.Count; i++)
            {
                if (m_ListEvent[i].Keys.Length < 1 || m_ListKeyType.Count < m_ListEvent[i].Keys.Length) continue;

                bool isMatch = true;

                for (int j = 0; j < m_ListEvent[i].Keys.Length; j++)
                {
                    if(m_ListEvent[i].Keys[j] != m_ListKeyType[j])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (!isMatch) continue;
                ResetKeys();
                m_ListEvent[i].KeyEvent?.Invoke(m_ListEvent[i].EventID, true);
            }
        }



        private void ResetKeys()
        {
            m_ListKeyType.Clear();
            m_KeyDownTime = -1f;
            m_CurrDir = 0;
            m_KeyUpAdd = true;
            m_KeyDownAdd = true;
            m_KeyLeftAdd = true;
            m_KeyRightAdd = true;
            m_KeyXAdd = true;
            m_KeyYAdd = true;
        }

        public override void ShutDown()
        {
            
        }

        private bool m_KeyUpAdd = true;
        private bool m_KeyDownAdd = true;
        private bool m_KeyLeftAdd = true;
        private bool m_KeyRightAdd = true;
        private bool m_KeyXAdd = true;
        private bool m_KeyYAdd = true;

        private float m_CurrDir = 0;
        private float m_KeyDownTime = -1f;

        private const float KEY_DOWN_TIME = 0.05f;

        private List<KeyType> m_ListKeyType = null;
        private List<ComboKeyEvent> m_ListEvent = null;
    }
}
