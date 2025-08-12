using System;

namespace GameFrameWork.Assets
{
    public class LoadRequest : BaseEventArgs
    {
        public string assetPath { get; set; }
        public Type assetType { get; set; }
        public GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction { get; set; }
        public object arg { get; set; }

        public static LoadRequest Create(string assetPath, Type assetType, GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction, object arg)
        {
            LoadRequest loadRequest = ReferencePool.Acquire<LoadRequest>();
            loadRequest.assetPath = assetPath;
            loadRequest.assetType = assetType;
            loadRequest.loadedAction = loadedAction;
            loadRequest.arg = arg;
            return loadRequest;
        }

        public void Loaded(UnityEngine.Object go)
        {
            if (loadedAction != null)
            {
                loadedAction?.Invoke(assetPath, go, arg);
            }
        }

        public override void Clear()
        {
            base.Clear();
            assetPath = null;
            assetType = null;
            loadedAction = null;
            arg = null;
        }
    }
}