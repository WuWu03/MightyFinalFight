namespace WuWuFramework.Event
{
    public class UnSubscribeTriggerOnDisable : UnSubscribeTrigger
    {
        private void OnDisable()
        {
            UnSubscribeAll();
        }
    }
}