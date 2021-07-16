using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utility;
using UnityEngine;

public class EffectMgr : BaseMgr<EffectMgr>
{
    public BaseEffect PlayEffect(string effectName, Vector3 pos, float playTime, GameFrameWorkAction playEndCallback = null)
    {
        return PlayEffect(effectName, null, pos, Vector3.zero, true, true, playTime, playEndCallback);
    }

    public BaseEffect PlayEffect(string effectName, Transform parent, Vector3 pos, Vector3 angles, bool isAutoPlay, bool isAutoRelease = true, float playTime = -1, GameFrameWorkAction playEndCallback = null)
    {
        BaseEffect effect = EntityMgr.Ins.GetEntity<BaseEffect>(effectName, parent);
        effect.transform.localPosition = pos;
        effect.transform.localRotation = Quaternion.Euler(angles);
        effect.PlayTime = playTime;
        effect.PlayEndCallback = playEndCallback;
        effect.IsAutoRelease = isAutoRelease;
        effect.SetRes(PathUtil.FormatPath(ResDefine.EFFECT_PATH, effectName));

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