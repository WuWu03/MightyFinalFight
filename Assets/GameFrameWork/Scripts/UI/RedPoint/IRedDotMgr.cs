using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public enum RedPointType
    {
        None,
        Eternal,//一直存在
        Once,//点击一次就消失
    }

    public enum RedPointState
    {
        None,
        Show,
        Hide,
    }
    
    public interface IRedDotMgr
    {
        public void Add(string key, string subKey, string parentKey, RedPointType type);
        public void Remove(string key, string subKey);
        public void Init(string key, string subKey, GameFrameWorkAction<RedPointState, int> showEvent, Button btn = null);
        public void SetState(string key, string subKey, RedPointState state, int data = 0);
    }
}