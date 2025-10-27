using System;
using System.Collections.Generic;
using GameFrameWork.Assets;
using GameFrameWork.Audio;
using GameFrameWork.BehaviourTree;
using GameFrameWork.ConfigData;
using GameFrameWork.Download;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.Net;
using GameFrameWork.Pool;
using GameFrameWork.Scene;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Version;
using GameFrameWork.WebRequest;

namespace GameFrameWork
{
    public abstract class GameFrameWorkModule
    {
        private byte m_Priority;
        private static byte s_TempPriority = byte.MaxValue;
        private static byte GetPriority()
        {
            return s_TempPriority--;
        }
        
        private static readonly Dictionary<Type,byte> s_ModulePriorities = new()
        {
            [typeof(ConfigDataMgr)] = GetPriority(),
            [typeof(NetMgr)] = GetPriority(),
            [typeof(EventMgr)] = GetPriority(),
            [typeof(WebRequestMgr)] = GetPriority(),
            [typeof(DownloadMgr)] = GetPriority(),
            [typeof(VersionMgr)] = GetPriority(),
            [typeof(IFsmMgr)] = GetPriority(),
            [typeof(EntityMgr)] = GetPriority(),
            [typeof(BehaviourTreeMgr)] = GetPriority(),
            [typeof(UIMgr)] = GetPriority(),
            [typeof(RedDotMgr)] = GetPriority(),
            [typeof(InputMgr)] = GetPriority(),
            [typeof(SoundMgr)] = GetPriority(),
            [typeof(SceneMgr)] = GetPriority(),
            [typeof(TimerMgr)] = GetPriority(),
            [typeof(GameObjectPoolMgr)] = GetPriority(),
            [typeof(ResourcePoolMgr)] = GetPriority(),
            [typeof(LocalizationMgr)] = GetPriority(),
            [typeof(ResourceMgr)] = GetPriority(),
        };
        
        /// <summary>
        /// 优先级
        /// </summary>
        public byte priority
        {
            get
            {
                if (m_Priority != 0)
                {
                    return m_Priority;
                }
                
                if (s_ModulePriorities.TryGetValue(this.GetType(), out byte tempPriority))
                {
                    m_Priority = tempPriority;
                }
                
                return m_Priority;
            }
        }

        public virtual void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            
        }

        public virtual void LateUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            
        }

        public virtual void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, float fixedTime, float fixedUnscaledTime)
        {

        }
        
        /// <summary>
        /// 关闭模块
        /// </summary>
        public abstract void Shutdown();
    }
}