using UnityEngine;

namespace GameFrameWork.Input
{
    public enum KeyType
    {
        A = 1,
        B = 2,
        X = 3,
        Y = 4,
        Start = 5,
        Select = 6,
        LB = 7,
        RB = 8,
        None = 9,
        Up = 10,
        Down = 11,
        Left = 12,
        Right = 13,
    }

    public enum AxisType
    {
        LeftAxis = 1,//左摇杆
        RightAxis = 2,//右摇杆
        CrossAxis = 3,//十字键
        LTRTAxis = 4,//LT,RT
        None = 5,//
    }

    public static class InputHelper
    {
        public static void Init()
        {
            m_Axis = new AxisArgs[(int)AxisType.None - 1];
            m_Keys = new KeyArgs[(int)KeyType.None - 1];
            m_AxisDown = new bool[((int)AxisType.None - 1) * 2];
            m_KeyDown = new bool[(int)KeyType.None - 1];
            AddAxis(AxisType.LeftAxis, "Horizontal", "Vertical", KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None);
            AddAxis(AxisType.RightAxis, "SubHorizontal", "SubVertical", KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None);
            AddAxis(AxisType.CrossAxis, "CrossHorizontal", "CrossVertical", KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None);
            AddAxis(AxisType.LTRTAxis, "LTRT", "LTRT", KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None);
            AddKey(KeyType.A, "A", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.B, "B", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.X, "X", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.Y, "Y", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.LB, "LB", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.RB, "RB", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.Select, "Select", KeyType.None, KeyCode.None, false, false);
            AddKey(KeyType.Start, "Start", KeyType.None, KeyCode.None, false, false);
        }

        public static AxisArgs GetAxis(AxisType axisType)
        {
            int index = (int)axisType - 1;

            if (index < 0 || index > m_Axis.Length)
            {
                return null;
            }

            return m_Axis[index];
        }

        public static void SetAxis(AxisType axisType, string horizontal, string vertical)
        {
            AxisArgs axisArgs = GetAxis(axisType);

            if (axisArgs == null)
            {
                return;
            }

            axisArgs.horizontal = horizontal;
            axisArgs.vertical = vertical;
        }

        public static void SetAxis(AxisType axisType, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            AxisArgs axisArgs = GetAxis(axisType);

            if (axisArgs == null)
            {
                return;
            }

            axisArgs.keyCodeHorizontalPositive = keyCodeHorizontalPositive;
            axisArgs.keyCodeHorizontalNegative = keyCodeHorizontalNegative;
            axisArgs.keyCodeVerticalPositive = keyCodeVerticalPositive;
            axisArgs.keyCodeVerticalNegative = keyCodeVerticalNegative;
        }

        public static void SetAxis(AxisType axisType, string horizontal, string vertical, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            AxisArgs axisArgs = GetAxis(axisType);

            if (axisArgs == null)
            {
                return;
            }

            axisArgs.horizontal = horizontal;
            axisArgs.vertical = vertical;
            axisArgs.keyCodeHorizontalPositive = keyCodeHorizontalPositive;
            axisArgs.keyCodeHorizontalNegative = keyCodeHorizontalNegative;
            axisArgs.keyCodeVerticalPositive = keyCodeVerticalPositive;
            axisArgs.keyCodeVerticalNegative = keyCodeVerticalNegative;
        }

        public static KeyArgs GetKey(KeyType keyType)
        {
            int index = (int)keyType - 1;

            if (index < 0 || index > m_Keys.Length)
            {
                return null;
            }

            return m_Keys[index];
        }

        public static void SetKey(KeyType keyType, string keyName)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.keyName = keyName;
        }

        public static void SetKey(KeyType keyType, KeyCode keyCode)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.keyCode = keyCode;
        }

        public static void SetKey(KeyType keyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.keyCode = keyCode;
            keyArgs.isTurbo = isTurbo;
            keyArgs.isCheckCombo = isCheckCombo;
        }

        public static void SetKey(KeyType keyType, KeyType replaceKeyType, bool isTurbo, bool isCheckCombo)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.replaceKeyType = replaceKeyType;
            keyArgs.isTurbo = isTurbo;
            keyArgs.isCheckCombo = isCheckCombo;
        }

        public static void SetKey(KeyType keyType, KeyType replaceKeyType,KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.replaceKeyType = replaceKeyType;
            keyArgs.keyCode = keyCode;
            keyArgs.isTurbo = isTurbo;
            keyArgs.isCheckCombo = isCheckCombo;
        }

        public static void SetKey(KeyType keyType, string keyName, KeyType replaceKeyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            KeyArgs keyArgs = GetKey(keyType);

            if (keyArgs == null)
            {
                return;
            }

            keyArgs.keyName = keyName;
            keyArgs.replaceKeyType = replaceKeyType;
            keyArgs.keyCode = keyCode;
            keyArgs.isTurbo = isTurbo;
            keyArgs.isCheckCombo = isCheckCombo;
        }

        public static KeyArgs[] GetAllKeys()
        {
            return m_Keys;
        }

        public static AxisArgs[] GetAllAxis()
        {
            return m_Axis;
        }

        public static void SetAxisDown(AxisType axisType, int axisIndex, bool value)
        {
            int index = ((int)axisType - 1) * 2 + axisIndex;
            m_AxisDown[index] = value;
        }

        public static bool GetAxisDown(AxisType axisType, int axisIndex)
        {
            int index = ((int)axisType - 1) * 2 + axisIndex;
            return m_AxisDown[index];
        }

        public static void SetKeyDown(KeyType keyType, bool value)
        {
            int index = (int)keyType - 1;
            m_KeyDown[index] = value;
        }

        public static bool GetKeyDown(KeyType keyType)
        {
            int index = (int)keyType - 1;
            return m_KeyDown[index];
        }

        public static void Dispose()
        {
            for (int i = 0; i < m_Axis.Length; i++)
            {
                if (m_Axis[i] == null)
                {
                    continue;
                }

                m_Axis[i].Release();
            }

            for (int i = 0; i < m_Keys.Length; i++)
            {
                if (m_Keys[i] == null)
                {
                    continue;
                }

                m_Keys[i].Release();
            }

            m_Axis = null;
            m_Keys = null;
            m_AxisDown = null;
            m_KeyDown = null;
        }

        private static void AddAxis(AxisType axisType, string horizontal, string vertical, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            int index = (int)axisType - 1;
            m_Axis[index] = AxisArgs.Create(horizontal, vertical, keyCodeHorizontalPositive, keyCodeHorizontalNegative, keyCodeVerticalPositive, keyCodeVerticalNegative);
        }

        private static void AddKey(KeyType keyType, string keyName, KeyType replaceKeyType, UnityEngine.KeyCode keyCode, bool isTurbo, bool isCheckCombo)
        {
            int index = (int)keyType - 1;
            m_Keys[index] = KeyArgs.Create(keyName, keyType, replaceKeyType, keyCode, isTurbo, isCheckCombo);
        }

        private static bool[] m_AxisDown = null;
        private static bool[] m_KeyDown = null;
        private static AxisArgs[] m_Axis = null;
        private static KeyArgs[] m_Keys = null;
    }
}