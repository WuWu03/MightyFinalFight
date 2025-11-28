using GameFrameWork.Event;
using UnityEngine;

namespace GameFrameWork.Input
{
    public interface IInputMgr
    {
        public event GameFrameWorkFunc<float> getDirectionEvent;
        public event GameFrameWorkFunc<int, bool> getPreConditionEvent;
        public event GameFrameWorkAction inputDeviceChangeEvent;
        public InputDeviceType inputDeviceType { get; }
        public bool isRunning { get; set; }
        public void SetKey(KeyType keyType, string keyName);
        public void SetKey(KeyType keyType, KeyCode keyCode);
        public void SetKey(KeyType keyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo);
        public void SetKey(KeyType keyType, KeyType replaceKeyType, bool isTurbo, bool isCheckCombo);
        public void SetKey(KeyType keyType, KeyType replaceKeyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo);
        public void SetKey(KeyType keyType, string keyName, KeyType replaceKeyType, KeyCode keyCode, bool isTurbo, bool isCheckCombo);
        public void SetAxis(AxisType axisType, string horizontal, string vertical);
        public void SetAxis(AxisType axisType, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative);
        public void SetAxis(AxisType axisType, string horizontal, string vertical, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative);
        public void AddComboKeyEvent(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent);
        public void RemoveComboKeyEvent(int eventID);
        public void RemoveAllComboKeyEvent();
        public void AddAfterTriggerEvent(KeyType keyType, GameFrameWorkAction afterTriggerEvent);
        public void RemoveAfterTriggerEvent(KeyType keyType, GameFrameWorkAction afterTriggerEvent);
        public void RemoveAllAfterTriggerEvent();
        public Vector2 GetAxis(AxisType axisType, bool isTurbo = false, bool checkKeyBoard = true);
        public bool GetKeyDown(KeyType keyType, bool checkKeyBoard = true);
    }
}