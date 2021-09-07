using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utility;
using UnityEngine;

public class EffectMgr : BaseMgr<EffectMgr>
{
    public DBEffect PlayDBEffect(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, GameFrameWorkAction playEndCallback = null)
    {
        return PlayEffect<DBEffect>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public DBEffect PlayDBEffect(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease, float playTime = -1, float speed = 1f, GameFrameWorkAction playEndCallback = null)
    {
        return PlayEffect<DBEffect>(effectName, parent, pos, angles, isAutoPlay, isAutoRelease, playTime, speed, playEndCallback);
    }

    public BaseEffect PlayEffect(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, GameFrameWorkAction playEndCallback = null)
    {
        return PlayEffect<BaseEffect>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public BaseEffect PlayEffect(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease = true, float playTime = -1, float speed = 1f, GameFrameWorkAction playEndCallback = null)
    {
        return PlayEffect<BaseEffect>(effectName, parent, pos, angles, isAutoPlay, isAutoRelease, playTime, speed, playEndCallback);
    }

    public T PlayEffect<T>(string effectName, Vector3 pos, float playTime = -1, float speed = 1f, GameFrameWorkAction playEndCallback = null) where T : BaseEffect
    {
        return PlayEffect<T>(effectName, null, pos, Vector3.zero, true, true, playTime, speed, playEndCallback);
    }

    public T PlayEffect<T>(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease, float playTime, float speed, GameFrameWorkAction playEndCallback) where T : BaseEffect
    {
        T effect = EntityMgr.Ins.GetEntity<T>(effectName, parent);
        effect.transform.localPosition = pos;
        effect.transform.localRotation = Quaternion.Euler(angles);
        effect.PlayTime = playTime;
        effect.Speed = speed;
        effect.PlayEndCallback = playEndCallback;
        effect.IsAutoRelease = isAutoRelease;
        effect.SetRes(PathUtil.FormatPath(ResDefine.EFFECT_PATH, effectName));
        effect.SetLayer(LayerName.Unit);

        if (isAutoPlay)
        {
            effect.Play();
        }

        return effect;
    }

    public void PutEffect(BaseEffect effect)
    {
        effect.Release();
        EntityMgr.Ins.PutEntity(effect);
    }
}