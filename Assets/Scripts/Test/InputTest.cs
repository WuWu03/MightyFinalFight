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
        m_InputMgr.SetCurrScheme("Game");
        // m_InputMgr.AddInputEvent<Vector2>("LeftAxis",OnLeftAxis);
        // m_InputMgr.AddInputEvent<float>("LT",OnLT);
        // m_InputMgr.AddInputEvent("LB",OnLB);
        // action = new InputAction("fuck");
        // InputBinding binding = new InputBinding("Keyboard/W");
        // action.AddBinding(binding);
        // action.Enable();
        // action.performed += OnInput;
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

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         Debug.Log("开始重新绑定");
    //         action.Disable(); 
    //         action.PerformInteractiveRebinding(0).OnComplete(operation =>
    //         {
    //             action.Enable();
    //             operation.Dispose();
    //             Debug.Log("重新绑定");
    //         }).OnCancel(operation => {
    //             action.Enable();
    //             operation.Dispose();
    //         }).Start();
    //     }
    // }
    // private void OnInput(InputAction.CallbackContext obj)
    // {
    //     string str = action.GetBindingDisplayString(0);
    //     Debug.Log(str);
    // }
}
