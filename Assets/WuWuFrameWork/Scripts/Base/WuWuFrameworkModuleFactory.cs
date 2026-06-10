using System;
using System.Collections.Generic;
using WuWuFramework.Sound;
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
    public static class WuWuFrameworkModuleFactory
    {
        private static byte s_TempPriority = byte.MaxValue;

        private static readonly Dictionary<Type, WuWuFrameworkFunc<WuWuFrameworkModule>> s_ModuleFacotries = new()
        {
            [typeof(ICameraMgr)] = CreateModule<CameraMgr>,
            [typeof(IConfigDataMgr)] = CreateModule<ConfigDataMgr>,
            [typeof(IInputMgr)] = CreateModule<InputMgr>,
            [typeof(ITimerMgr)] = CreateModule<TimerMgr>,
            [typeof(ILocalizationMgr)] = CreateModule<LocalizationMgr>,
            [typeof(INetMgr)] = CreateModule<NetMgr>,
            [typeof(IVersionMgr)] = CreateModule<VersionMgr>,
            [typeof(IDownloadMgr)] = CreateModule<DownloadMgr>,
            [typeof(IWebRequestMgr)] = CreateModule<WebRequestMgr>,
            [typeof(IGameEntityMgr)] = CreateModule<GameEntityMgr>,
            [typeof(IRedDotMgr)] = CreateModule<RedDotMgr>,
            [typeof(IUIMgr)] = CreateModule<UIMgr>,
            [typeof(ISceneMgr)] = CreateModule<SceneMgr>,
            [typeof(ISoundMgr)] = CreateModule<SoundMgr>,
            [typeof(IGameObjectPoolMgr)] = CreateModule<GameObjectPoolMgr>,
            [typeof(IResourcePoolMgr)] = CreateModule<ResourcePoolMgr>,
            [typeof(IResourcesMgr)] = CreateModule<ResourcesMgr>,
            [typeof(IBehaviourTreeMgr)] = CreateModule<BehaviourTreeMgr>,
            [typeof(IFsmMgr)] = CreateModule<FsmMgr>,
            [typeof(IEventMgr)] = CreateModule<EventMgr>,
        };

        private static readonly Dictionary<Type, byte> s_ModulePriorities = new()
        {
            [typeof(CameraMgr)] = GetTempPriority(),
            [typeof(ConfigDataMgr)] = GetTempPriority(),
            [typeof(InputMgr)] = GetTempPriority(),
            [typeof(TimerMgr)] = GetTempPriority(),
            [typeof(LocalizationMgr)] = GetTempPriority(),
            [typeof(NetMgr)] = GetTempPriority(),
            [typeof(VersionMgr)] = GetTempPriority(),
            [typeof(DownloadMgr)] = GetTempPriority(),
            [typeof(WebRequestMgr)] = GetTempPriority(),
            [typeof(GameEntityMgr)] = GetTempPriority(),
            [typeof(RedDotMgr)] = GetTempPriority(),
            [typeof(UIMgr)] = GetTempPriority(),
            [typeof(SceneMgr)] = GetTempPriority(),
            [typeof(SoundMgr)] = GetTempPriority(),
            [typeof(GameObjectPoolMgr)] = GetTempPriority(),
            [typeof(ResourcePoolMgr)] = GetTempPriority(),
            [typeof(ResourcesMgr)] = GetTempPriority(),
            [typeof(BehaviourTreeMgr)] = GetTempPriority(),
            [typeof(FsmMgr)] = GetTempPriority(),
            [typeof(EventMgr)] = GetTempPriority(),
        };

        public static byte GetPriority(Type moduleType)
        {
            if (s_ModulePriorities.TryGetValue(moduleType, out byte tempPriority))
            {
                return tempPriority;
            }

            throw new WuWuFrameworkException("模块不存在");
        }

        public static T GetModule<T>() where T : class
        {
            if (s_ModuleFacotries.TryGetValue(typeof(T), out WuWuFrameworkFunc<WuWuFrameworkModule> createFunc))
            {
                try
                {
                    return createFunc() as T;
                }
                catch (Exception ex)
                {
                    throw new WuWuFrameworkException(ex.Message);
                }
            }

            throw new WuWuFrameworkException("模块不存在");
        }

        private static byte GetTempPriority()
        {
            return s_TempPriority--;
        }

        private static T CreateModule<T>() where T : WuWuFrameworkModule, new()
        {
            return new T();
        }
    }
}