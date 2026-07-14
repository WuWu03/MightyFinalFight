//using System;
//using System.IO;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using WuWuFramework.Input;
//using WuWuFramework.Utils;
//using WuWuPlayerPrefs = WuWuFramework.Serialize.PlayerPrefs;

//public class InputTest : MonoBehaviour
//{
//    private InputAction action;
//    private InputMgr m_InputMgr = new();
//    public InputActionAsset asset;
//    private string SAVE_KEY = "PlayerInputScheme";

//    void Start()
//    {
//        string jsonStr = WuWuPlayerPrefs.GetString(SAVE_KEY);
//        m_InputMgr.inputActionAsset = !string.IsNullOrEmpty(jsonStr) ? InputActionAsset.FromJson(jsonStr) : asset;
//        m_InputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, OnLeftAxis);
//        m_InputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.LT, InputEventCallType.Performed, OnLT);
//        m_InputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, OnA);
//        m_InputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.B, InputEventCallType.Performed, OnB);
//        m_InputMgr.keyBoardInputController.rebindingCompleteEvent += OnRebinding;
//        m_InputMgr.inputDeviceChangeEvent += OnDeviceChagne;
//        m_InputMgr.SetCurrScheme(InputScheme.KeyBoard);
//        m_InputMgr.AddInputEvent<Vector2>("LeftAxis", OnLeftAxis);
//        m_InputMgr.AddInputEvent<float>("LT", OnLT);
//        m_InputMgr.AddInputEvent("LB", OnLB);
//        action = new InputAction("fuck");
//        InputBinding binding = new InputBinding("Keyboard/W");
//        action.AddBinding(binding);
//        action.Enable();
//        action.performed += OnInput;
//        Debug.Log(StringUtil.GetChineseNum((decimal)30332325.33056));
//    }

//    private void OnA()
//    {
//        Debug.Log("A");

//        if (m_InputMgr.keyBoardInputController.isRebinding)
//        {
//            m_InputMgr.keyBoardInputController.CancelRebinding();
//        }
//    }

//    private void OnB()
//    {
//        Debug.Log("B");

//        if (m_InputMgr.keyBoardInputController.isRebinding)
//        {
//            m_InputMgr.keyBoardInputController.CancelRebinding();
//        }
//    }

//    private void OnRebinding(InputAction t, int index)
//    {
//        InputBinding bindling = t.bindings[index];
//        Debug.Log(bindling.action + " : " + bindling.effectiveInteractions);
//    }

//    private void OnDeviceChagne(InputScheme t)
//    {
//        Debug.Log("当前输入设备 ：" + t.ToString());
//    }

//    private void OnLT(float t)
//    {
//        Debug.Log(t);
//    }

//    private void OnLeftAxis(Vector2 t)
//    {
//        Debug.Log(t);
//    }

//    void Update()
//    {
//        m_InputMgr.Update();

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            m_InputMgr.keyBoardInputController.Rebinding(KeyboardInputKey.A);
//        }
//        else if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            m_InputMgr.keyBoardInputController.actionMap.RemoveAllBindingOverrides();
//        }
//        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
//        {
//            m_InputMgr.Save();
//        }
//    }
//}
