using WuWuFramework;
using WuWuFramework.Event;
using WuWuFramework.Utils;
using UnityEngine;

public class EffectMgr : Singleton<EffectMgr>
{
    public DBEffect PlayDBEffect(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, WuWuFrameworkAction playEndCallback = null)
    {
        return PlayEffect<DBEffect>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public DBEffect PlayDBEffect(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease, float playTime = -1, float speed = 1f, WuWuFrameworkAction playEndCallback = null)
    {
        return PlayEffect<DBEffect>(effectName, parent, pos, angles, isAutoPlay, isAutoRelease, playTime, speed, playEndCallback);
    }

    public BaseEffect PlayEffect(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, WuWuFrameworkAction playEndCallback = null)
    {
        return PlayEffect<BaseEffect>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public BaseEffect PlayEffect(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease = true, float playTime = -1, float speed = 1f, WuWuFrameworkAction playEndCallback = null)
    {
        return PlayEffect<BaseEffect>(effectName, parent, pos, angles, isAutoPlay, isAutoRelease, playTime, speed, playEndCallback);
    }

    public T PlayEffect<T>(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, WuWuFrameworkAction playEndCallback = null) where T : BaseEffect, new()
    {
        return PlayEffect<T>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public T PlayEffect<T>(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease, float playTime, float speed, WuWuFrameworkAction playEndCallback) where T : BaseEffect, new()
    {
        T effect = GameEntry.entityMgr.GetEntity<T>(effectName, parent);
        effect.transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(angles));
        effect.playTime = playTime;
        effect.speed = speed;
        effect.onPlayEndEvent = playEndCallback;
        effect.isAutoRelease = isAutoRelease;
        effect.SetAsset(PathUtil.FormatPath(AssetPathDefine.EffectPath, effectName));
        effect.SetLayer(LayerName.Unit);
        effect.SetObjectType(ObjectType.CantBreakItem);

        if (isAutoPlay)
        {
            effect.Play();
        }

        return effect;
    }

    public override void Shutdown()
    {

    }
}