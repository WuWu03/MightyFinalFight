using GameFrameWork.Utilities;
using System;

namespace GameFrameWork.Resources
{
    public class LoadRequest : IReference
    {
        public string assetPath { get; set; }
        public Type assetType { get; set; }
        public GameFrameWorkAction<string, UnityEngine.Object, object[]> action { get; set; }
        public object[] args { get; set; }

        public static LoadRequest Create()
        {
            LoadRequest loadRequest = ReferencePool.Acquire<LoadRequest>();
            return loadRequest;
        }

        public void Call(UnityEngine.Object go)
        {
            if (action != null)
            {
                action?.Invoke(assetPath, go, args);
            }
        }

        public void Clear()
        {
            assetPath = null;
            assetType = null;
            action = null;
            args = null;
        }
    }
}