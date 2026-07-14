using WuWuFramework.Event;
using WuWuFramework.Resources;

namespace WuWuFramework.Input
{
    public interface IInputMgr
    {
        event WuWuFrameworkAction<InputScheme> inputDeviceChangeEvent;

        XboxInputController xboxInputController { get; }

        KeyboardInputController keyBoardInputController { get; }

        void SetResourcesMgr(IResourcesMgr resourceMgr);

        void AddInputController(InputScheme inputScheme);

        void SetCurrScheme(InputScheme inputScheme);
    }
}