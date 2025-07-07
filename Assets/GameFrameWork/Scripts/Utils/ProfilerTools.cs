using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameFrameWork.Utils
{
    public class ProfilerTools
    {
        /// <summary>
        /// profilter记录的类型，c#层会把数字转成字符串输出
        /// tips:需要增加类型时直接加到最后面即可
        /// </summary>
        public enum ProfilerType
        {
            def = 0,
            lua_update,
            Lua_LateUpdate,
            Timer_Update,
            FrameTimer_Update,
            CoTimer_Update,
            lua_UILoad_refreshUI,
            lua_MainCityDraw,
            lua_MainCityDraw_getMousePos,
            lua_MainCityDraw_getTouchPos,
            lua_update_uiCentre,
            lua_update_uiLeft,
            lua_update_uiLeftBottom,
            lua_update_uiLeftTop,
            lua_update_uiRight,
            lua_update_uiRightBottom,
            lua_update_uiTop,
            lua_update_uiWorldBossPop,
            lua_update_uiWorldRelicPop,
            lua_update_uiWorldBuildPop,
            lua_update_uiResFieldAltarPop,
            lua_update_uiResFieldOrePop,
            lua_update_uiResFieldTemplePop,
            lua_update_uiResFieldVillagePop,
            lua_update_uiMainWorld,
            lua_update_worldBossCell,
            lua_update_worldBossUICell,
            lua_update_worldCityCell,
            lua_update_worldCityUICell,
            lua_update_worldRelicCell,
            lua_update_worldRelicUICell,
            lua_update_worldResFieldCell,
            lua_update_worldResFieldUICell,
            lua_update_worldWalkerCell,
            lua_FightMove_updateFun,
            lua_SocketMgr_initTcp,
            lua_SocketMgr_listenTcp,
            lua_SocketMgr_heartTimeFun,
            lua_update_worldMapPreview,
            lua_drag_cityBuildCell,
            lua_armyUnit_stand,
            lua_otherrole_move,
            lua_instantiate_other,
            lua_camera_follow,
            lua_camera_follow1,
            lua_camera_follow2,
            lua_camera_follow3,
        }

        static private Dictionary<int, string> _map;

        /// <summary>
        /// 从lua传过来的分析类型，映射成string
        /// </summary>
        /// <param name="type"></param>
        static public void BeginSample(int type)
        {
            if (_map == null)
            {
                _map = new Dictionary<int, string>();
                var enumArr = Enum.GetValues(typeof(ProfilerTools.ProfilerType));
                foreach (int myCode in enumArr)
                {
                    string strName = Enum.GetName(typeof(ProfilerTools.ProfilerType), myCode);//获取名称
                    _map.Add(myCode, strName);
                }
            }

            string profilerName;
            if (_map.TryGetValue(type, out profilerName))
            {
                Profiler.BeginSample(profilerName);
            }
        }

    }
}