using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Input
{
    public class InputMgr : BaseMgr<InputMgr>
    {
        private void Awake()
        {
            m_ListKey = new List<KeyCodeType>();
        }
        private void Update()
        {
            //UpdateKey();

            //if (m_AnyKeyDown)
            //{
            //    m_IsStart = true;
            //    m_ListKey.Add(m_CurrKey);
            //}

            //if (m_IsStart)
            //{
            //    m_CurrFrame++;
            //    if (m_CurrFrame >= KeyFrame)
            //    {
            //        List<KeyCodeType> keys = GetKey();

            //        if (keys.Count > 0)
            //        {
            //            string key = "";
            //            for (int i = 0; i < keys.Count; i++)
            //            {
            //                key += keys[i].ToString();
            //                key += ",";
            //            }

            //            Debug.Log("Keys:" + key);
            //        }

            //        ResetKey();
            //    }
            //}
        }

        public List<KeyCodeType> GetKey()
        {
            return m_ListKey;
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

        private void UpdateKey()
        {
            m_AnyKeyDown = false;
            float x = UnityEngine.Input.GetAxis("Horizontal");
            float y = UnityEngine.Input.GetAxis("Vertical");

            if (x != 0 && y == 0)
            {
                m_CurrKey = x > 0 ? KeyCodeType.Rigth : KeyCodeType.Left;
                m_AnyKeyDown = true;
            }

            if (y != 0 && x == 0)
            {
                m_CurrKey = y > 0 ? KeyCodeType.Up : KeyCodeType.Down;
                m_AnyKeyDown = true;
            }

            if (UnityEngine.Input.GetButtonDown("A"))
            {
                m_CurrKey = KeyCodeType.Attack;
                m_AnyKeyDown = true;
            }

            if (UnityEngine.Input.GetButtonDown("B"))
            {
                m_CurrKey = KeyCodeType.Jump;
                m_AnyKeyDown = true;
            }
        }

        private void ResetKey()
        {
            m_ListKey.Clear();
            m_CurrFrame = 0;
            m_IsStart = false;
            m_AnyKeyDown = false;
        }

        public override void ShutDown()
        {
            throw new System.NotImplementedException();
        }

        private List<KeyCodeType> m_ListKey = null;
        private KeyCodeType m_CurrKey;
        private bool m_AnyKeyDown = false;
        private bool m_IsStart = false;
        private int m_CurrFrame = 0;
        private const int SaveFrame = 1;
        private const int KeyFrame = 4;
    }
}
