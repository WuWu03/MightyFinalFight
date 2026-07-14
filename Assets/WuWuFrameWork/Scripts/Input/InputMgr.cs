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
        private Dictionary<InputScheme, BaseInputController> m_InputControllers = new();
        private XboxInputController m_XboxInputController;
        private KeyboardInputController m_KeyboardInputController;
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
                if (m_XboxInputController == null)
                {
                    throw new WuWuFrameworkException("XboxInputController不存在，调用AddInputController添加");
                }

                return m_XboxInputController;
            }
        }

        public KeyboardInputController keyBoardInputController
        {
            get
            {
                if (m_KeyboardInputController == null)
                {
                    throw new WuWuFrameworkException("KeyboardInputController不存在，调用AddInputController添加");
                }

                return m_KeyboardInputController;
            }
        }

        public InputMgr()
        {
            MonoBehaviourMgr.instance.updateEvent += Update;
        }

        public void SetResourcesMgr(IResourcesMgr resourceMgr)
        {
            try
            {
                string configDataPath = WuWuFrameworkEntry.config.configDataPath;
                string filePath = PathUtil.FormatPath(configDataPath, InputConfigDataName);
                byte[] buffer = resourceMgr.Load<TextAsset>(filePath).bytes;
                string jsonStr = System.Text.Encoding.UTF8.GetString(ZlibHelper.DeCompressBytes(buffer));
                resourceMgr.Unload(filePath);
                m_InputActionAsset = InputActionAsset.FromJson(jsonStr);
            }
            catch
            {

            }
        }

        public void AddInputController(InputScheme inputScheme)
        {
            if (m_InputControllers.ContainsKey(inputScheme))
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 控制器已存在"));
            }

            if (inputScheme == InputScheme.None)
            {
                throw new WuWuFrameworkException("控制器方案错误 InputScheme.None");
            }

            if (inputScheme == InputScheme.Keyboard)
            {
                m_KeyboardInputController = InputHelper.GetInputController(InputScheme.Keyboard, m_InputActionAsset) as KeyboardInputController
                     ?? throw new WuWuFrameworkException("[KeyboardInputController] 控制器创建失败");
                m_InputControllers.Add(InputScheme.Keyboard, m_KeyboardInputController);
            }
            else if (inputScheme == InputScheme.Xbox)
            {
                m_XboxInputController = InputHelper.GetInputController(InputScheme.Xbox, m_InputActionAsset) as XboxInputController
                    ?? throw new WuWuFrameworkException("[XboxInputController] 控制器创建失败");
                m_InputControllers.Add(InputScheme.Xbox, m_XboxInputController);
            }
        }

        public void Save()
        {
            m_KeyboardInputController?.SaveBindings();
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
                if (m_InputControllers.TryGetValue(m_CurrInputScheme, out BaseInputController oldController))
                {
                    oldController.Disable();
                }
                else
                {
                    throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 控制器不存在，调用AddInputController添加"));
                }
            }

            m_CurrInputScheme = inputScheme;

            if (m_InputControllers.TryGetValue(m_CurrInputScheme, out BaseInputController currController))
            {
                currController.Enable();
            }
            else
            {
                throw new WuWuFrameworkException(StringUtil.Append("[", inputScheme.ToString(), "] 控制器不存在，调用AddInputController添加"));
            }
        }

        public override void Shutdown()
        {
            foreach (KeyValuePair<InputScheme, BaseInputController> keyValuePair in m_InputControllers)
            {
                keyValuePair.Value.Dispose();
            }

            m_InputControllers.Clear();
            m_XboxInputController = null;
            m_KeyboardInputController = null;
            m_InputDeviceChangeEvent = null;
            m_InputActionAsset = null;
            MonoBehaviourMgr.instance.updateEvent -= Update;
        }

        private void Update(float t1, float t2, float t3, float t4)
        {
            if (m_CurrInputScheme == InputScheme.None)
            {
                return;
            }

            bool isDeviceChanged = false;

            if (m_CurrInputScheme != InputScheme.Keyboard && InputHelper.IsKeyBoardInput())
            {
                isDeviceChanged = true;
                SetCurrScheme(InputScheme.Keyboard);
            }
            else if (m_CurrInputScheme != InputScheme.Xbox && InputHelper.IsXboxInput())
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