using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    internal static class InputHelper
    {
        internal static void Init()
        {
            m_Axis = new AxisArgs[(int)AxisType.None];
            m_Keys = new KeyArgs[(int)KeyType.None];
            m_AxisDown = new bool[((int)AxisType.None - 1) * 2];
            m_KeyDown = new bool[(byte)KeyType.None];
        }

        internal static void AddAxis(AxisType axisType, string horizontal, string vertical)
        {
            int index = (int)axisType - 1;
            m_Axis[index] = AxisArgs.Create(horizontal, vertical);
        }

        internal static AxisArgs GetAxis(AxisType axisType)
        {
            int index = (int)axisType - 1;
            return m_Axis[index];
        }

        internal static void AddKey(KeyType keyType, string keyName, KeyType replaceKeyType, bool isShift)
        {
            int index = (int)keyType - 1;
            m_Keys[index] = KeyArgs.Create(keyName, replaceKeyType, isShift);
        }

        internal static KeyArgs GetKey(KeyType keyType)
        {
            int index = (int)keyType - 1;
            return m_Keys[index];
        }

        internal static void SetAxisDown(AxisType axisType, int axisIndex, bool value)
        {
            int index = ((int)axisType - 1) * 2 + axisIndex;
            m_AxisDown[index] = value;
        }

        internal static bool GetAxisDown(AxisType axisType, int axisIndex)
        {
            int index = ((int)axisType - 1) * 2 + axisIndex;
            return m_AxisDown[index];
        }

        internal static void SetKeyDown(KeyType keyType, bool value)
        {
            int index = (int)keyType - 1;
            m_KeyDown[index] = value;
        }

        internal static bool GetKeyDown(KeyType keyType)
        {
            int index = (int)keyType - 1;
            return m_KeyDown[index];
        }

        internal static void Dispose()
        {
            for (int i = 0; i < m_Axis.Length; i++)
            {
                ReferencePool.Release(m_Axis[i]);
            }

            for (int i = 0; i < m_Keys.Length; i++)
            {
                ReferencePool.Release(m_Keys[i]);
            }

            m_Axis = null;
            m_Keys = null;
            m_AxisDown = null;
            m_KeyDown = null;
        }

        private static bool[] m_AxisDown = null;
        private static bool[] m_KeyDown = null;
        private static AxisArgs[] m_Axis = null;
        private static KeyArgs[] m_Keys = null;
    }
}
