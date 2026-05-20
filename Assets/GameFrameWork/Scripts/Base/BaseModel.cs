namespace GameFrameWork
{
    public abstract class BaseModel
    {
        public BaseModel()
        {
            OnInit();
        }

        protected abstract void OnInit();

        abstract public void Clear();
    }
}