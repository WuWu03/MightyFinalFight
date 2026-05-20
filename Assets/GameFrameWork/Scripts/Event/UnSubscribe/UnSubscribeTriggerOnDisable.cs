namespace GameFrameWork.Event
{
    public class UnSubscribeTriggerOnDisable : UnSubscribeTrigger
    {
        private void OnDisable()
        {
            UnSubscribeAll();
        }
    }
}