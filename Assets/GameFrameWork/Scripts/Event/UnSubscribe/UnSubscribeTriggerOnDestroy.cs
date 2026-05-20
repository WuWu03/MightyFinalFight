namespace GameFrameWork.Event
{
    public class UnSubscribeTriggerOnDestroy : UnSubscribeTrigger
    {
        private void OnDestroy()
        {
            UnSubscribeAll();
        }
    }
}