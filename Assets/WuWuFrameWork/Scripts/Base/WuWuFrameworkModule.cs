using System;
using System.Collections.Generic;
using WuWuFramework.Audio;
using WuWuFramework.BehaviourTree;
using WuWuFramework.Camera;
using WuWuFramework.ConfigData;
using WuWuFramework.Download;
using WuWuFramework.Event;
using WuWuFramework.Fsm;
using WuWuFramework.GameEntity;
using WuWuFramework.Input;
using WuWuFramework.Localization;
using WuWuFramework.Net;
using WuWuFramework.Pool;
using WuWuFramework.Resources;
using WuWuFramework.Scene;
using WuWuFramework.Timer;
using WuWuFramework.UI;
using WuWuFramework.Version;
using WuWuFramework.WebRequest;

namespace WuWuFramework
{
    public abstract class WuWuFrameworkModule
    {
        private byte m_Priority;
        private static byte s_TempPriority = byte.MaxValue;
        private static byte GetPriority()
        {
            return s_TempPriority--;
        }

        private static readonly Dictionary<Type, byte> s_ModulePriorities = new()
        {
            [typeof(IConfigDataMgr)] = GetPriority(),
            [typeof(INetMgr)] = GetPriority(),
            [typeof(IEventMgr)] = GetPriority(),
            [typeof(IWebRequestMgr)] = GetPriority(),
            [typeof(IDownloadMgr)] = GetPriority(),
            [typeof(IVersionMgr)] = GetPriority(),
            [typeof(IFsmMgr)] = GetPriority(),
            [typeof(IEntityMgr)] = GetPriority(),
            [typeof(IBehaviourTreeMgr)] = GetPriority(),
            [typeof(IUIMgr)] = GetPriority(),
            [typeof(IRedDotMgr)] = GetPriority(),
            [typeof(IInputMgr)] = GetPriority(),
            [typeof(ISoundMgr)] = GetPriority(),
            [typeof(ISceneMgr)] = GetPriority(),
            [typeof(ITimerMgr)] = GetPriority(),
            [typeof(IGameObjectPoolMgr)] = GetPriority(),
            [typeof(IResourcePoolMgr)] = GetPriority(),
            [typeof(ILocalizationMgr)] = GetPriority(),
            [typeof(IResourcesMgr)] = GetPriority(),
            [typeof(ICameraMgr)] = GetPriority(),
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