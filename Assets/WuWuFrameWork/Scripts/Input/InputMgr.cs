using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework.Event;
using WuWuFramework.Resources;
using WuWuFramework.Utils;

namespace WuWuFramework.Input
{
    public class InputMgr : WuWuFrameworkModule, IInputMgr
    {
        private Dictionary<InputScheme, BaseInputController> m_InputContollers = new();
        private XboxInputController m_XboxInputController;
        private KeyboardInputController m_KeyBoardInputController;
        private event WuWuFrameworkAction<InputScheme> m_InputDeviceChangeEvent;
        private InputActionAsset m_InputActionAsset;
        private InputScheme m_CurrInputScheme = InputScheme.None;
        private const string InputConfigDataName = "InputConfigData.bytes";

        public event WuWuFrameworkAction<InputScheme> inputDeviceChangeEvent
        {
            add
            {
                value?.Invoke(m_CurrInputScheme);
                m_InputDeviceChangeEvent += value;
            }
            remove
            {
                m_InputDeviceChangeEvent -= value;
            }
        }

        public InputScheme currInputScheme
        {
            get
            {
                return m_CurrInputScheme;
            }
        }

        public XboxInputController xboxInputController
        {
            get
            {
                return m_XboxInputController;
            }
        }

        public KeyboardInputController keyBoardInputController
        {
            get
            {
                return m_KeyBoardInputController;
            }
        }

        public InputMgr()
        {
            MonoBehaviourMgr.instance.updateEvent += Update;
        }

        public void SetResourcesMgr(IResourcesMgr resourceMgr)
        {
            string configDataPath = WuWuFrameworkEntry.config.configDataPath;
            string filePath = PathUtil.FormatPath(configDataPath, InputConfigDataName);
            byte[] buffer = resourceMgr.Load<TextAsset>(filePath).bytes;
            string jsonStr = System.Text.Encoding.UTF8.GetString(ZlibHelper.DeCompressBytes(buffer));
            resourceMgr.Unload(filePath);
            m_InputActionAsset = InputActionAsset.FromJson(jsonStr);
        }

        public void AddInputController(InputScheme inputScheme)
        {
            if (m_InputContollers.ContainsKey(inputScheme))
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 控制器已存在"));
            }

            if (inputScheme == InputScheme.None)
            {
                throw new WuWuFrameworkException(StringUtil.Append("控制器方案错误 InputScheme.None"));
            }

            if (inputScheme == InputScheme.Keyboard)
            {
                m_KeyBoardInputController = InputHelper.GetInputController(InputScheme.Keyboard, m_InputActionAsset) as KeyboardInputController;
                m_InputContollers.Add(InputScheme.Keyboard, m_KeyBoardInputController);
            }
            else if (inputScheme == InputScheme.Xbox)
            {
                m_XboxInputController = InputHelper.GetInputController(InputScheme.Xbox, m_InputActionAsset) as XboxInputController;
                m_InputContollers.Add(InputScheme.Xbox, m_XboxInputController);
            }
        }

        public void Save()
        {
            m_KeyBoardInputController?.SaveBindings();
            m_XboxInputController?.SaveBindings();
        }

        public void SetCurrScheme(InputScheme inputScheme)
        {
            if (m_CurrInputScheme == inputScheme)
            {
                return;
            }

            if (m_InputActionAsset == null)
            {
                throw new WuWuFrameworkException("配置文件不存在");
            }

            if (m_CurrInputScheme != InputScheme.None)
            {
                if (m_InputContollers.TryGetValue(m_CurrInputScheme, out BaseInputController oldController))
                {
                    oldController.actionMap.Disable();
                }
                else
                {
                    throw new WuWuFrameworkException(StringUtil.Append("未添加 [", inputScheme.ToString(), "] 控制器"));
                }
            }

            m_CurrInputScheme = inputScheme;

            if (m_InputContollers.TryGetValue(m_CurrInputScheme, out BaseInputController currController))
            {
                currController.actionMap.Enable();
            }
            else
            {
                throw new WuWuFrameworkException(StringUtil.Append("未添加 [", inputScheme.ToString(), "] 控制器"));
            }
        }

        public override void Shutdown()
        {
            m_InputContollers.Clear();
            m_InputDeviceChangeEvent = null;
            MonoBehaviourMgr.instance.updateEvent -= Update;
        }

        private void Update(float t1, float t2, float t3, float t4)
        {
            bool isDeviceChanged = false;

            if (InputHelper.IsKeyBoardInput() && m_CurrInputScheme != InputScheme.Keyboard)
            {
                isDeviceChanged = true;
                SetCurrScheme(InputScheme.Keyboard);
            }
            else if (InputHelper.IsXboxInput() && m_CurrInputScheme != InputScheme.Xbox)
            {
                isDeviceChanged = true;
                SetCurrScheme(InputScheme.Xbox);
            }

            if (isDeviceChanged)
            {
                m_InputDeviceChangeEvent?.Invoke(m_CurrInputScheme);
            }
        }
    }
}