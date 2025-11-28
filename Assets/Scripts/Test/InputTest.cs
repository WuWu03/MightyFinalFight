using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering;

public class InputTest : MonoBehaviour
{
    private class InputEvent:Test.IPlayer1Actions
    {
        public void OnStick(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnButton(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnKeyBoard(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<float>());
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InputAction action;
    void Start()
    {
        action = new InputAction("fuck");
        InputBinding binding = new InputBinding("Keyboard/W");
        action.AddBinding(binding);
        action.Enable();
        action.performed += OnInput;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("开始重新绑定");
            action.Disable();
            action.PerformInteractiveRebinding(0).OnComplete(operation =>
            {
                action.Enable();
                operation.Dispose();
                Debug.Log("重新绑定");
            }).OnCancel(operation => {
                action.Enable();
                operation.Dispose();
            }).Start();
        }
    }
    private void OnInput(InputAction.CallbackContext obj)
    {
        string str = action.GetBindingDisplayString(0);
        Debug.Log(str);
    }
}
