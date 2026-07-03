using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InputAction action;
    private TestInputMgr m_InputMgr = new();
    public InputActionAsset asset;
    void Start()
    {
        m_InputMgr.inputActionAsset = asset;
        m_InputMgr.SetCurrScheme(InputScheme.KeyBoard);
        m_InputMgr.AddInputEvent(InputKey.LeftAxis, InputEventCallType.Performed, OnLeftAxis);
        m_InputMgr.AddInputEvent(InputKey.LT, InputEventCallType.Performed, OnLT);
        m_InputMgr.inputDeviceChangeEvent += OnDeviceChagne;
        //m_InputMgr.AddInputEvent<Vector2>("LeftAxis",OnLeftAxis);
        // m_InputMgr.AddInputEvent<float>("LT", OnLT);
        // m_InputMgr.AddInputEvent("LB",OnLB);
        // action = new InputAction("fuck");
        // InputBinding binding = new InputBinding("Keyboard/W");
        // action.AddBinding(binding);
        // action.Enable();
        // action.performed += OnInput;
    }

    private void OnDeviceChagne(InputScheme t)
    {
        Debug.Log("当前输入设备 ：" + t.ToString());
    }

    private void OnLB()
    {
        Debug.Log("LB");
    }

    private void OnLT(float t)
    {
        Debug.Log(t);
    }

    private void OnLeftAxis(Vector2 t)
    {
        Debug.Log(t);
    }

    void Update()
    {
        m_InputMgr.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // m_InputMgr.ReBindInput(InputKey.LeftAxis);
        }
    }
    // private void OnInput(InputAction.CallbackContext obj)
    // {
    //     string str = action.GetBindingDisplayString(0);
    //     Debug.Log(str);
    // }
}
